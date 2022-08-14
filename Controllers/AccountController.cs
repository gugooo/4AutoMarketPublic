using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using _4AutoMarket.Models;
using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.Extensions.Localization;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;
using _4AutoMarket.Classes;
using Microsoft.AspNetCore.Localization;
using MimeKit;
using MimeKit.Utils;
using System.IO;

namespace _4AutoMarket.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly IStringLocalizer<AccountController> _Localizer;
        private readonly UserDBContext DBContext;
        private readonly int OrderHistoryProducts = 5;
        public AccountController(UserManager<User> _userManager, SignInManager<User> _signInManager, IStringLocalizer<AccountController> stringLocalizer, UserDBContext userDBContext)
        {
            this.userManager = _userManager;
            this.signInManager = _signInManager;
            this._Localizer = stringLocalizer;
            this.DBContext = userDBContext;
        }
        #region MyAccount
        #region MyAccount
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MyAccount()
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            var model = new ViewModels.MyAccount.PersonalInformationViewModel()
            {
                FName = user.FirstName,
                LName = user.LastName,
                Email = user.Email,
                Adress = user.Address,
                City = user.City,
                Country = user.Country,
                DateOfBirth = user.DateOfBirth,
                Phone = user.PhoneNumber,
                Postcode = user.PostCode
            };
            ViewBag.Account = "active";
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> MyAccount(ViewModels.MyAccount.PersonalInformationViewModel _Model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(HttpContext.User);
                if (!await userManager.CheckPasswordAsync(user, _Model.CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", "Incorrect password.");
                    ViewBag.ARead = "FALSE";
                    return View(_Model);
                }
                user.FirstName = _Model.FName;
                user.LastName = _Model.LName;
                user.DateOfBirth = _Model.DateOfBirth;
                user.Country = _Model.Country;
                user.City = _Model.City;
                user.Address = _Model.Adress;
                user.PostCode = _Model.Postcode;
                user.PhoneNumber = _Model.Phone;
                if (!string.IsNullOrEmpty(_Model.Password))
                {
                    var status = await userManager.ChangePasswordAsync(user, _Model.CurrentPassword, _Model.Password);
                    if (!status.Succeeded)
                    {
                        foreach (var err in status.Errors) ModelState.AddModelError("", err.Description);
                        ViewBag.ARead = "FALSE";
                        return View(_Model);
                    }
                }
                if (HttpContext.User.Identity.Name.ToUpper() != _Model.Email.ToUpper())
                {
                    user.Email = _Model.Email;
                    user.NormalizedEmail = _Model.Email.ToUpper();
                    user.UserName = _Model.Email;
                    user.NormalizedUserName = _Model.Email.ToUpper();
                    user.EmailConfirmed = false;
                    var res = await userManager.UpdateAsync(user);
                    if (!res.Succeeded)
                    {
                        foreach (var err in res.Errors) ModelState.AddModelError("", err.Description);
                        ViewBag.ARead = "FALSE";
                        return View(_Model);
                    }
                    await signInManager.SignOutAsync();
                    HttpContext.Session.Clear();
                    return RedirectToAction("Index", "Home");
                }
                {
                    var res = await userManager.UpdateAsync(user);
                    if (!res.Succeeded)
                    {
                        foreach (var err in res.Errors) ModelState.AddModelError("", err.Description);
                        ViewBag.ARead = "FALSE";
                        return View(_Model);
                    }
                }
            }

            ViewBag.Account = "active";
            return View(_Model);
        }

        [Authorize]
        public async Task<IActionResult> MyAccountChakEmail(string Email)
        {
            if (HttpContext.User.Identity.Name.ToUpper() == Email.ToUpper()) return Json(true);
            var user = await userManager.FindByEmailAsync(Email);
            if (user != null) return Json(false);
            return Json(true);

        }
        #endregion
        #region AddressBook
        [Authorize]
        public async Task<IActionResult> AddressBook(string SelectedAddressId, ViewModels.MyAccount.AddAddressBookViewModel NewAddress)
        {
            var user = await userManager.Users.Include(_ => _.AddressBook).Include(_ => _.SelectedAddress).FirstOrDefaultAsync(_ => _.NormalizedEmail == HttpContext.User.Identity.Name.ToUpper());
            var UserAddressBook = user.AddressBook;
            var _Model = new List<AddressBook>();
            _Model.Add(new Models.AddressBook()
            {
                FName = user.FirstName,
                LName = user.LastName,
                Address = user.Address,
                City = user.City,
                Country = user.Country,
                PostCode = user.PostCode,
                Id = 0
            });
            if (UserAddressBook != null && UserAddressBook.Count > 0) _Model.AddRange(UserAddressBook.Select(_ => new Models.AddressBook() { FName = _.FName, LName = _.LName, Country = _.Country, Address = _.Address, City = _.City, PostCode = _.PostCode, Id = _.Id }).ToArray().OrderBy(_ => _.Id));
            if (SelectedAddressId == null || SelectedAddressId == "0")
            {
                ViewBag.SAID = 0;
            }
            else
            {
                var NewSellectedAddress = user.AddressBook.FirstOrDefault(_ => _.Id.ToString() == SelectedAddressId);
                if (NewSellectedAddress != null)
                {
                    user.SelectedAddress = NewSellectedAddress;
                    var res = await userManager.UpdateAsync(user);
                    if (res.Succeeded) ViewBag.SAID = NewSellectedAddress.Id;
                }
                else
                {
                    ViewBag.SAID = 0;
                    user.SelectedAddress = null;
                    await userManager.UpdateAsync(user);
                }
            }
            ViewBag.Address = "active";
            ViewModels.MyAccount.AddAddressBookViewModel _NewAddres = null;
            if (NewAddress != null && !(NewAddress.Adress == null && NewAddress.City == null && NewAddress.Country == null && NewAddress.FName == null && NewAddress.LName == null && NewAddress.Postcode == null))
            {
                ViewBag.hasNewAddress = "true";
                _NewAddres = NewAddress;
            }
            return View(new AddressBookWrap() { Addresses = _Model, AddNewAddress = _NewAddres });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddNewAddress(AddressBookWrap AddressWrap)
        {
            ViewModels.MyAccount.AddAddressBookViewModel NewAddress = AddressWrap.AddNewAddress;
            if (!ModelState.IsValid)
            {
                return RedirectToAction("AddressBook", NewAddress);
            }
            var user = await userManager.Users.Include(_ => _.AddressBook).FirstOrDefaultAsync(_ => _.NormalizedEmail == HttpContext.User.Identity.Name.ToUpper());
            AddressBook _NewAddress = new AddressBook() { Address = NewAddress.Adress, City = NewAddress.City, Country = NewAddress.Country, FName = NewAddress.FName, LName = NewAddress.LName, PostCode = NewAddress.Postcode };
            user.AddressBook.Add(_NewAddress);
            var status = await userManager.UpdateAsync(user);
            if (!status.Succeeded)
            {
                foreach (var el in status.Errors) ModelState.AddModelError("", el.Description);
                return RedirectToAction("AddressBook", NewAddress);
            }
            return RedirectToAction("AddressBook");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DeleteAddress(string AddressId)
        {
            var user = await userManager.Users.Include(_ => _.SelectedAddress).Include(_ => _.AddressBook).FirstOrDefaultAsync(_ => _.NormalizedEmail == HttpContext.User.Identity.Name.ToUpper());
            if (AddressId == null || AddressId.Length == 0 || AddressId == "0" || user == null || user.AddressBook == null || user.AddressBook.Count == 0) return RedirectToAction("AddressBook");
            var remEl = user.AddressBook.FirstOrDefault(_ => _.Id.ToString() == AddressId);
            if (remEl != null)
            {
                if (user.SelectedAddress != null && user.SelectedAddress.Id == remEl.Id) user.SelectedAddress = null;
                user.AddressBook.Remove(remEl);
                await userManager.UpdateAsync(user);
            }
            return RedirectToAction("AddressBook");
        }
        #endregion

        [Authorize]
        public async Task<IActionResult> OrderHistory(int Interval)
        {
            ViewBag.Order = "active";
            var user = await userManager.GetUserAsync(User);
            var orders = user?.Orders?.Where(_ => Interval == 0 || _.Created > DateTime.Now.AddMonths(-Interval)).Take(OrderHistoryProducts).ToList();
            ViewBag.Orders = orders;
            return View();
        }
        [Authorize]
        public async Task<IActionResult> OrderHistoryI(int Interval, int index)
        {
            var user = await userManager.GetUserAsync(User);
            var orders = user?.Orders?.Where(_ => Interval == 0 || _.Created > DateTime.Now.AddMonths(-Interval))
                .Skip(index * OrderHistoryProducts)
                .Take(OrderHistoryProducts).ToList();
            return View(orders);
        }
        [Authorize]
        public async Task<IActionResult> Messages()
        {
            var user = await userManager.Users.FirstOrDefaultAsync(_ => _.NormalizedUserName == HttpContext.User.Identity.Name.ToUpper());
            #region Inbox
            var AdminShareMessages = user.AdminSharedMessage?.Select(_ => new MessageWrapper() { IsShare = true, Id = _.Id, IsNew = _.IsNew, Title = _.Message?.Title, Text = _.Message?.Text, Date = _.Message.SendedDate });
            var AdminSendedMessages = user.AdminSend?.Select(_ => new MessageWrapper() { IsShare = false, Id = _.Id, IsNew = _.IsNew, Title = _.Title, Text = _.Text, Date = _.SendedDate });
            var AdminSend = new List<MessageWrapper>(AdminShareMessages);
            AdminSend.AddRange(AdminSendedMessages);
            ViewBag.Inbox = AdminSend.OrderBy(_ => _.Date).ToArray().Reverse();
            #endregion
            ViewBag.Sent = user.UserSend?.Select(_ => new MessageWrapper() { Title = _.Title, Text = _.Text, Date = _.SendedDate }).OrderBy(_ => _.Date).ToArray().Reverse();

            ViewBag.Messages = "active";
            return await Task.Run(() => View());
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddNewMessage(string Title, string Text)
        {
            string _Title = Title == null ? "" : Title, _Text = Text == null ? "" : Text;
            _Title = _Title.Length > 50 ? _Title.Substring(0, 150) : _Title;
            _Text = _Text.Length > 500 ? _Text.Substring(0, 500) : _Text;
            var user = await userManager.Users.FirstOrDefaultAsync(_ => _.NormalizedUserName == HttpContext.User.Identity.Name.ToUpper());
            user.UserSend.Add(new Message() { IsNew = true, Title = _Title, Text = _Text, SendedDate = DateTime.Now });
            await userManager.UpdateAsync(user);
            return RedirectToAction("Messages");
        }

        [Authorize]
        [HttpPost]
        public async Task ReadMessages(int MessageId, bool IsShare)
        {
            if (MessageId <= 0) return;
            var user = await userManager.GetUserAsync(User);
            if (user == null) return;
            if (IsShare)
            {
                var tempMess = user.AdminSharedMessage.FirstOrDefault(_ => _.Id == MessageId && _.IsNew);
                if (tempMess != null) tempMess.IsNew = false;
            }
            else
            {
                var tempMess = user.AdminSend.FirstOrDefault(_ => _.Id == MessageId && _.IsNew);
                if (tempMess != null) tempMess.IsNew = false;
            }
            await DBContext.SaveChangesAsync();
            return;
        }
        #endregion

        #region Registration-Login-LogOut
        public async Task<IActionResult> LogOut()
        {
            await this.signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]



        public async Task<JsonResult> Login(ViewModels.LoginViewModel reg)
        {
            try
            {
                var resp = new List<string>();
                if (ModelState.IsValid)
                {
                    var SIResult = await this.signInManager.PasswordSignInAsync(reg.LoginEmail, reg.LoginPassword, reg.Remember, true);
                    if (SIResult.Succeeded)
                    {
                        var user = await userManager.FindByEmailAsync(reg.LoginEmail);
                        if (!user.EmailConfirmed)
                        {
                            await this.signInManager.SignOutAsync();
                            HttpContext.Session.Clear();
                            var callBack = Url.Action("SendEmailConfirmLink", "Account", new { Email = user.Email }, protocol: HttpContext.Request.Scheme);
                            resp.Add($"Pleas confirm Your Email. <a href='' onclick=\"$.get('{callBack}',function(){{alert('Email sent!');}});return false;\">Send again.</a>");
                        }
                        return new JsonResult(new { Errors = resp });
                    }
                    else if (SIResult.IsLockedOut)
                    {
                        resp.Add("Your account is blocked. Please try again later.");
                        return new JsonResult(new { Errors = resp });
                    }
                    resp.Add("Incorrect Email or Password.");
                }
                resp.AddRange(ModelState.Values.SelectMany(_ => _.Errors).Select(_ => _.ErrorMessage));
                return new JsonResult(new { Errors = resp });
            }
            catch (Exception)
            {
                return new JsonResult(new { Errors = new[] { "Error: Please reload page and try again." } });
            }
        }

        [HttpPost]
        public async Task<JsonResult> Registration(ViewModels.RegistrationViewModel reg)
        {
            try
            {
                var resp = new List<string>();
                if (ModelState.IsValid)
                {
                    var user = await userManager.FindByEmailAsync(reg.Email);
                    if (user == null)
                    {
                        if (!EmailService.isEmailAddres(reg.Email)) return new JsonResult(new { Errors = new[] { "Incorrect email address." } });
                        User newUser = new User
                        {
                            UserName = reg.Email,
                            FirstName = reg.FName,
                            LastName = reg.LName,
                            Email = reg.Email,
                            Registred = DateTime.Now
                        };
                        var CreatedStatus = await this.userManager.CreateAsync(newUser, reg.Password);
                        if (CreatedStatus.Succeeded)
                        {
                            await SendEmailConfirmLink(newUser.Email);
                        }
                        else
                        {
                            resp.AddRange(CreatedStatus.Errors.Select(_ => _.Description));
                        }
                        return new JsonResult(new { Errors = resp });
                    }
                    else
                    {
                        return new JsonResult(new { Errors = new[] { "Warning: This email is already registered." } });
                    }
                }
                resp.AddRange(ModelState.Values.SelectMany(_ => _.Errors).Select(_ => _.ErrorMessage));
                return new JsonResult(new { Errors = resp });
            }
            catch (Exception)
            {
                return new JsonResult(new { Errors = new[] { "Error: Please reload page and try again.\n" } });
            }
        }
        #endregion
        #region Email-Chek-Confirm-SendConfirmLink
        public async Task<IActionResult> ChakEmail(string Email)
        {
            var user = await userManager.FindByEmailAsync(Email);
            if (user != null) return Json(false);
            return Json(true);

        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }
            var result = await userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
                return RedirectToAction("Index", "Home");
            else
                return View("Error");
        }

        [HttpGet]
        public async Task SendEmailConfirmLink(string Email)
        {
            var user = await userManager.FindByEmailAsync(Email);
            if (user == null || user.EmailConfirmed) return;
            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Action("ConfirmEmail", "Account",
                new { userId = user.Id, code = code },
                protocol: HttpContext.Request.Scheme);

            await sentdAlcantaraMessage(user.Email, "Confirm your Email",
                $"THANKS FOR REGISTERING:</br>Please click here to authenticate Your email. </br><a href='{callbackUrl}'>It's my Email.</a>");
        }
        #endregion
        #region Password-Reset-Forgot

        [HttpGet]
        public IActionResult ResetPassword(string EmailId, string Code)
        {
            ViewBag.EmailId = EmailId;
            ViewBag.Code = Code;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ViewModels.ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await userManager.FindByIdAsync(model.EmailID);
            if (user == null)
            {
                ModelState.AddModelError("", "Incorrect Data.");
                return View();
            }
            var result = await userManager.ResetPasswordAsync(user, model.Code, model.ResetPassword);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Incorrect Data.");
                return View();
            }
            if (!await userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError("", "Please confirm your Email before sign in.");
                return View();
            }
            await signInManager.SignInAsync(user, false);
            return RedirectToAction("index", "Home");
        }

        [HttpPost]
        public async Task<JsonResult> ForgotPassword(ViewModels.ForgotPasswordViewModel model)
        {
            try
            {
                var resp = new List<string>();


                if (ModelState.IsValid)
                {
                    var user = await userManager.FindByEmailAsync(model.ForgotPasswordEmail);
                    if (user != null)
                    {
                        var code = await userManager.GeneratePasswordResetTokenAsync(user);
                        var callbackUrl = Url.Action("ResetPassword", "Account", new { EmailId = user.Id, Code = code }, protocol: HttpContext.Request.Scheme);
                        string message = $"Please click here to reset Your password.</br> <a href='{callbackUrl}'>Reset</a>";

                        bool status = await sentdAlcantaraMessage(model.ForgotPasswordEmail, "Reset Password", message);
                        if (status)
                        {
                            return new JsonResult(new { Errors = resp, massage = _Localizer["ForgotPassword_SuccessMassage"].Value });
                        }
                    }
                }
                resp.Add(_Localizer["ForgotPassword_InvalidEmailMassage"]);
                return new JsonResult(new { Errors = resp });
            }
            catch (Exception)
            {
                return new JsonResult(new { Errors = "Error Plase try again." });
            }
        }
        private async Task<bool> sentdAlcantaraMessage(string email, string subject, string message)
        {
            if (!EmailService.isEmailAddres(email)) return false;
            ViewBag.culture = Request.HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture.Name;
            ViewBag.model = await DBContext.Catalogs.Where(_ => _.FatherCatalogId == null).Take(5).ToListAsync();
            ViewBag.message = message;
            //-------------------BodyBuilder Imag-i hamar
            var builder = new BodyBuilder();
            var path = Directory.GetCurrentDirectory() + "\\wwwroot\\Imag\\Logo.png";
            var image = builder.LinkedResources.Add(path);
            image.ContentId = MimeUtils.GenerateMessageId();
            //-------------------
            ViewBag.imgId = image.ContentId;
            var view = this.PartialView("EmailRequest");
            builder.HtmlBody = view.ToHtml(HttpContext);
            return await Classes.EmailService.SendEmailAsync(email, subject, null, bodyBuilder: builder);
        }
        #endregion
        #region Google-Facebook
        public IActionResult GoogleLogin()
        {
            string redirectUrl = Url.Action("GoogleResponse", "Account");
            var prop = signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return new ChallengeResult("Google", prop);
        }
        public async Task<IActionResult> GoogleResponse()
        {
            var resp = base.Content("<script>window.close()</script>", "text/html");
            try
            {
                ExternalLoginInfo info = await signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return resp;
                }
                var res = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
                if (res.Succeeded)
                {
                    return resp;
                }
                else
                {
                    var getUser = await userManager.FindByEmailAsync(info.Principal.FindFirst(ClaimTypes.Email).Value);
                    if (getUser != null)
                    {
                        await signInManager.SignInAsync(getUser, false);
                        return resp;
                    }
                    Models.User newUser = new User()
                    {
                        Email = info.Principal.FindFirst(ClaimTypes.Email).Value,
                        UserName = info.Principal.FindFirst(ClaimTypes.Email).Value,
                        FirstName = info.Principal.FindFirst(ClaimTypes.GivenName).Value,
                        LastName = info.Principal.FindFirst(ClaimTypes.Surname).Value,
                        Registred = DateTime.Now
                    };
                    IdentityResult idRes = await userManager.CreateAsync(newUser);
                    if (idRes.Succeeded)
                    {
                        idRes = await userManager.AddLoginAsync(newUser, info);
                        if (idRes.Succeeded)
                        {
                            await signInManager.SignInAsync(newUser, false);
                            return resp;
                        }
                    }
                }
                return resp;
            }
            catch (Exception)
            {
                return resp;
            }
        }

        public IActionResult FacebookLogin()
        {
            string redirectUrl = Url.Action("FacebookResponse", "Account");
            var prop = signInManager.ConfigureExternalAuthenticationProperties("Facebook", redirectUrl);
            return new ChallengeResult("Facebook", prop);
        }
        public async Task<IActionResult> FacebookResponse()
        {
            var resp = base.Content("<script>window.close()</script>", "text/html");
            try
            {

                ExternalLoginInfo info = await signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return resp;
                }
                var res = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
                if (res.Succeeded)
                {
                    return resp;
                }
                else
                {
                    var getUser = await userManager.FindByEmailAsync(info.Principal.FindFirst(ClaimTypes.Email).Value);
                    if (getUser != null)
                    {
                        await signInManager.SignInAsync(getUser, false);
                        return resp;
                    }
                    Models.User newUser = new User()
                    {
                        Email = info.Principal.FindFirst(ClaimTypes.Email).Value,
                        UserName = info.Principal.FindFirst(ClaimTypes.Email).Value,
                        FirstName = info.Principal.FindFirst(ClaimTypes.GivenName).Value,
                        LastName = info.Principal.FindFirst(ClaimTypes.Surname).Value,
                        Registred = DateTime.Now
                    };
                    IdentityResult idRes = await userManager.CreateAsync(newUser);
                    if (idRes.Succeeded)
                    {
                        idRes = await userManager.AddLoginAsync(newUser, info);
                        if (idRes.Succeeded)
                        {
                            await signInManager.SignInAsync(newUser, false);
                            return resp;
                        }
                    }
                }
                return resp;
            }
            catch (Exception)
            {
                return resp;
            }
        }
        #endregion
    }
}