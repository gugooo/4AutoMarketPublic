using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using _4AutoMarket.Models;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using _4AutoMarket.ViewModels.Admin;
using _4AutoMarket.ViewModels;
using Microsoft.AspNetCore.SignalR;
using _4AutoMarket.Classes;


namespace _4AutoMarket.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserDBContext DBContext;
        private readonly IHubContext<ChatHub> hubContext;
        private readonly int OrderHistoryItems = 20;
        public AdminController(UserManager<User> _userManager, SignInManager<User> _signInManager, RoleManager<IdentityRole> _roleManager, UserDBContext context, IHubContext<ChatHub> _hubContext)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            roleManager = _roleManager;
            DBContext = context;
            hubContext = _hubContext;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            ViewBag.Index = "ASelect";
            ViewBag.users = await DBContext.Users.CountAsync();
            ViewBag.neworders = await DBContext.Orders.Where(_ => _.Status == Order.StatusType.Pending.ToString()).CountAsync();
            ViewBag.livechat = UserHandler.Users.SelectMany(_ => _.Value.messages).Where(_ => _.IsNew).Count();
            ViewBag.messages = await DBContext.Users.Where(_ => _.UserSend != null && _.UserSend.Count() > 0 && _.UserSend.Any(m => m.IsNew)).SelectMany(_ => _.UserSend).Where(_ => _.IsNew).CountAsync();
            ViewBag.reqCell = await DBContext.RequestCells.Where(_ => _.isNew).CountAsync();
            ViewBag.reqEmail = await DBContext.RequestEmails.Where(_ => _.isNew).CountAsync();
            ViewBag.subEmail = await DBContext.SubscribeEmails.Where(_ => _.isNew).CountAsync();
            ViewBag.newReviews = await DBContext.Reviews.Where(_ => _.Status == null).CountAsync();
            var gs = await DBContext.GlobalSetings.FirstOrDefaultAsync();
            ViewBag.orderSum = gs?.ShippingOrderSum;
            ViewBag.whenMore = gs?.ShippingWhenMore;
            ViewBag.whenLess = gs?.ShippingWhenLess;
            return View();
        }
        public async Task<JsonResult> changeShipping(decimal orderSum, decimal whenMore, decimal whenLess)
        {
            if (orderSum < 0 || whenMore < 0 || whenLess < 0) return new JsonResult(new { err = "Values must be >= 0" });
            var gs = await DBContext.GlobalSetings.FirstOrDefaultAsync();
            if (gs == null) DBContext.GlobalSetings.Add(new GlobalSetings() { ShippingOrderSum = orderSum, ShippingWhenMore = whenMore, ShippingWhenLess = whenLess });
            bool isNew = false;
            if (gs.ShippingOrderSum != orderSum) { gs.ShippingOrderSum = orderSum; isNew = true; }
            if (gs.ShippingWhenMore != whenMore) { gs.ShippingWhenMore = whenMore; isNew = true; }
            if (gs.ShippingWhenLess != whenLess) { gs.ShippingWhenLess = whenLess; isNew = true; }
            if (isNew && await DBContext.SaveChangesAsync() > 0)
            {
                return new JsonResult(new { res = true });
            }
            return new JsonResult(new { err = "Not changed." });
        }
        #region Catalog
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Catalogs()
        {
            string Culture = Request.HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture.Name;
            ViewBag.Culture = Culture;
            var TempCatalog = await DBContext.Catalogs.Where(_ => _.FatherCatalog == null).ToArrayAsync();
            ViewBag.Enumerable = TempCatalog?.OrderBy(_ => _._Index);
            ViewBag.Catalogs = "ASelect";
            ViewBag.Atributes = await DBContext.Atributes.ToListAsync();
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Catalogs(string NewName, string CultureName, string CurrentID, string Metod)
        {
            try
            {
                string Culture = Request.HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture.Name;
                switch (Metod)
                {
                    case "Add":
                        if (!string.IsNullOrEmpty(NewName))
                        {
                            var TempCulture = string.IsNullOrEmpty(CultureName) ? null : new CultureData() { Culture = Culture, Text = CultureName };
                            var TempCatalog = new Catalog() { Name = NewName, IsActive = true };
                            if (TempCulture != null) TempCatalog.CultureName.Add(TempCulture);
                            if (string.IsNullOrEmpty(CurrentID))
                            {
                                DBContext.Catalogs.Add(TempCatalog);
                                await DBContext.SaveChangesAsync();
                            }
                            else
                            {
                                var TempFatherCatalog = await DBContext.Catalogs.FirstOrDefaultAsync(_ => _.Id == CurrentID);
                                if (TempFatherCatalog != null)
                                {
                                    TempFatherCatalog.ChaildCatalogs.Add(TempCatalog);
                                    await DBContext.SaveChangesAsync();
                                }
                            }
                        }
                        break;
                    case "Delete":
                        if (!string.IsNullOrEmpty(CurrentID))
                        {
                            var TempCatalog = await DBContext.Catalogs.FirstOrDefaultAsync(_ => _.Id == CurrentID);
                            if (TempCatalog != null)
                            {
                                IEnumerable<Catalog> CatalogTree(Catalog c)
                                {
                                    List<Catalog> TC = new List<Catalog>() { c };
                                    foreach (var el in c.ChaildCatalogs)
                                    {
                                        if (el.ChaildCatalogs.Count > 0) TC.AddRange(CatalogTree(el));
                                        else TC.Add(el);
                                    }
                                    return TC;
                                }

                                var catalogs = CatalogTree(TempCatalog);
                                var products = catalogs.Where(_ => _.Products.Count > 0).SelectMany(_ => _.Products);
                                var cultures = catalogs.Where(_ => _.CultureName.Count > 0).SelectMany(_ => _.CultureName);
                                if (products.Count() > 0)
                                {
                                    var ProductTypes = products.SelectMany(_ => _.ProductTypes);
                                    var ProductIMGs = ProductTypes.SelectMany(_ => _.Images);
                                    if (ProductIMGs.Count() > 0) DBContext.RemoveRange(ProductIMGs);
                                    if (ProductTypes.Count() > 0) DBContext.RemoveRange(ProductTypes);
                                    DBContext.RemoveRange(products);
                                }
                                if (cultures.Count() > 0) DBContext.RemoveRange(cultures);

                                DBContext.Catalogs.RemoveRange(catalogs);
                                await DBContext.SaveChangesAsync();
                            }
                        }
                        break;
                    case "Change":
                        if (!string.IsNullOrEmpty(CurrentID) && !string.IsNullOrEmpty(NewName))
                        {
                            var TempCatalog = await DBContext.Catalogs.FirstOrDefaultAsync(_ => _.Id == CurrentID);
                            if (TempCatalog != null)
                            {
                                TempCatalog.Name = NewName;

                                var TempCulture = TempCatalog.CultureName.FirstOrDefault(_ => _.Culture == Culture);
                                if (string.IsNullOrEmpty(CultureName))
                                {
                                    if (TempCulture != null) TempCulture.Text = "";
                                }
                                else
                                {
                                    if (TempCulture != null) TempCulture.Text = CultureName;
                                    else TempCatalog.CultureName.Add(new CultureData() { Culture = Culture, Text = CultureName });
                                }
                            }
                            await DBContext.SaveChangesAsync();
                        }
                        break;
                    case "View":
                        if (!string.IsNullOrEmpty(CurrentID))
                        {
                            var TempCatalog = await DBContext.Catalogs.FirstOrDefaultAsync(_ => _.Id == CurrentID);
                            if (TempCatalog != null)
                            {
                                TempCatalog.IsActive = !TempCatalog.IsActive;
                                await DBContext.SaveChangesAsync();
                            }
                        }
                        break;
                }
                return RedirectToAction("Catalogs");
            }
            catch (Exception)
            {
                return RedirectToAction("Catalogs");
            }

        }
        #endregion
        #region Product
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<JsonResult> Products(string CatalogID, int Index)
        {
            try
            {
                string Culture = Request.HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture.Name;
                if (string.IsNullOrEmpty(CatalogID)) return new JsonResult(new { error = "CatalogID is empty." }); ;
                var TempCatalog = await DBContext.Catalogs.FirstOrDefaultAsync(_ => _.Id == CatalogID);
                if (TempCatalog != null)
                {
                    if (TempCatalog.Products.Count > 0)
                    {
                        var resp = new JsonResult(new
                        {
                            res = TempCatalog.Products.Skip(Index * 50).Take(50)?.OrderBy(_ => _._Index).Select(_ => new
                            {
                                id = _.Id,
                                isActive = _.IsActive,
                                title = CultureData.GetDefoultName(_.ProductTypes.FirstOrDefault(pt => pt.IsMine == true)?.CultureTitle, Culture),
                                imgId = _.ProductTypes.FirstOrDefault(pt => pt.IsMine == true)?.Images?.FirstOrDefault()?.Id ?? ""
                            }).ToArray()
                        });
                        return resp;
                    }
                    return new JsonResult(new { });
                }
                return new JsonResult(new { error = "Incorrect CatalogID" });
            }
            catch (Exception e)
            {
                return new JsonResult(new { error = e.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<JsonResult> GetMineProductType(string ProductId)
        {
            if (string.IsNullOrEmpty(ProductId)) return new JsonResult(new { error = "ProductID is empty. Please reload page and try again." });
            var TempProduct = await DBContext.Products.FirstOrDefaultAsync(_ => _.Id == ProductId);
            if (TempProduct == null) return new JsonResult(new { error = "Incorrect ProductID. Please reload page and try again." });
            var TempProductType = TempProduct?.ProductTypes.FirstOrDefault(_ => _.IsMine);
            if (TempProductType == null) return new JsonResult(new { error = "Error Product type not found. Please reload page and try again." });

            return new JsonResult(new { data = GetResponse(TempProduct, TempProductType) });

        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<JsonResult> ChangeProductView([FromBody]IEnumerable<string> ProductIds)
        {
            try
            {
                if (ProductIds == null && ProductIds.Count() == 0) return new JsonResult(new { error = "ProductIDs is empty. Please reload page and try again." });
                var TempProducts = await DBContext.Products.Where(_ => ProductIds.Any(id => id == _.Id)).ToListAsync();
                if (TempProducts == null || TempProducts.Count == 0 || TempProducts.Count != ProductIds.Count()) return new JsonResult(new { error = "Incorrect Product ID. Please reload page and try again." });
                TempProducts.ForEach(_ => _.IsActive = !_.IsActive);
                int status = await DBContext.SaveChangesAsync();
                return new JsonResult(new { data = status });
            }
            catch (Exception e)
            {
                return new JsonResult(new { error = e.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<JsonResult> DeleteProducts([FromBody]IEnumerable<string> ProductIds)
        {
            try
            {
                if (ProductIds == null && ProductIds.Count() == 0) return new JsonResult(new { error = "ProductIDs is empty. Please reload page and try again." });
                var TempProducts = await DBContext.Products.Where(_ => ProductIds.Any(id => id == _.Id)).ToListAsync();
                if (TempProducts == null || TempProducts.Count == 0 || TempProducts.Count != ProductIds.Count()) return new JsonResult(new { error = "Incorrect Product ID. Please reload page and try again." });

                foreach (var product in TempProducts)
                {
                    if (product.ProductTypes != null && product.ProductTypes.Count > 0)
                    {
                        foreach (var prType in product.ProductTypes)
                        {
                            var AllCultureData = prType.CultureBrand.Union(prType.CultureDescription).Union(prType.CultureTitle);
                            if (AllCultureData != null && AllCultureData.Count() > 0) DBContext.CultureDatas.RemoveRange(AllCultureData);//Delete Cultures

                            if (prType.Images != null && prType.Images.Count > 0) DBContext.ProductIMGs.RemoveRange(prType.Images);//Delete Imgs

                            if (prType.ProductAtributes != null && prType.ProductAtributes.Count > 0)//Delete Atributes
                            {
                                var AllAributeValues = prType.ProductAtributes.SelectMany(_ => _.AtributeValues);
                                DBContext.ProductAtributeValues.RemoveRange(AllAributeValues);
                                DBContext.ProductAtributes.RemoveRange(prType.ProductAtributes);
                            }
                        }
                        DBContext.ProductTypes.RemoveRange(product.ProductTypes);
                    }
                }
                DBContext.Products.RemoveRange(TempProducts);
                int status = await DBContext.SaveChangesAsync();
                return new JsonResult(new { data = status });
            }
            catch (Exception e)
            {
                return new JsonResult(new { error = e.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<JsonResult> AddProuct(string ProductData, [FromForm]IFormFileCollection NewIMGs)
        {
            try
            {
                var NewProduct = JObject.Parse(ProductData).ToObject<ProductAjax>();
                if (NewProduct == null || string.IsNullOrEmpty(NewProduct.CatalogId)) return new JsonResult(new { error = "Incorrect data. Please reload page and try again." });
                if (string.IsNullOrEmpty(NewProduct.Title)) return new JsonResult(new { error = "Title is rquered." });
                decimal Price = 0;
                decimal Sale = 0;
                if (!string.IsNullOrEmpty(NewProduct.Price) && !decimal.TryParse(NewProduct.Price, out Price)) return new JsonResult(new { error = "Incorrect Price Value. Please enter correct value." });
                if (!string.IsNullOrEmpty(NewProduct.Sale) && !decimal.TryParse(NewProduct.Sale, out Sale)) return new JsonResult(new { error = "Incorrect Sale Value. Please enter correct value." });

                string Culture = Request.HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture.Name;

                Product _pr = null;
                ProductType _prType = null;

                if (string.IsNullOrEmpty(NewProduct.ProductId) && string.IsNullOrEmpty(NewProduct.ProductTypeId))//Add New Product and New ProductType
                {
                    var TempCatalog = await DBContext.Catalogs.FirstOrDefaultAsync(_ => _.Id == NewProduct.CatalogId);
                    if (TempCatalog == null) return new JsonResult(new { error = "Incorrect CatalogID. Please reload page and try again." });

                    var TempProduct = new Product() { IsActive = true };


                    var TempProductType = new ProductType() { IsActive = NewProduct.IsActive, IsMine = true, Price = Price, Sale = Sale };
                    TempProductType.CultureTitle.Add(new CultureData() { Text = NewProduct.Title, Culture = Culture });
                    TempProductType.CultureDescription.Add(new CultureData() { Text = NewProduct.Description, Culture = Culture });
                    TempProductType.CultureBrand.Add(new CultureData() { Text = NewProduct.Brand, Culture = Culture });
                    TempProductType.SerchKeys = NewProduct.SerchKeys;

                    if (!string.IsNullOrEmpty(NewProduct.CurrentAtributeValue))//Add Current AtributeValue
                    {
                        var TempCurrentLink = await DBContext.AtributeValues.FirstOrDefaultAsync(_ => _.Id == NewProduct.CurrentAtributeValue);
                        if (TempCurrentLink == null) return new JsonResult(new { error = "Incorrect Current Atribute Value ID. Please reload page and try again." });
                        TempProductType.LinkAtributeValue = TempCurrentLink;
                    }

                    if (!string.IsNullOrEmpty(NewProduct.GroupAtribute))//Add Group Atribute
                    {
                        var TempGroupAtribute = await DBContext.Atributes.FirstOrDefaultAsync(_ => _.Id == NewProduct.GroupAtribute);
                        if (TempGroupAtribute == null) return new JsonResult(new { error = "Incorrect Link Atribute ID. Please reload page and try again." });
                        TempProduct.LinkAtribute = TempGroupAtribute;
                    }

                    if (NewProduct.AtributeLinks.Count() > 1)//Add Any ProductTypes
                    {
                        IEnumerable<string> TempGroupAtribueVsID = NewProduct.AtributeLinks.Except(new[] { NewProduct.CurrentAtributeValue });
                        var TempGroupAtribues = await DBContext.AtributeValues.Where(_ => TempGroupAtribueVsID.Any(id => id == _.Id)).ToListAsync();
                        if (TempGroupAtribues.Count != TempGroupAtribueVsID.Count()) return new JsonResult(new { error = "Incorrect Links Atribute Value IDs. Please reload page and try again." });
                        foreach (var atr in TempGroupAtribues)
                        {
                            TempProduct.ProductTypes.Add(new ProductType() { LinkAtributeValue = atr, IsActive = false });
                        }
                    }
                    if (!string.IsNullOrEmpty(NewProduct.FirstAtributeId))
                    {
                        var TempFirstAtr = await DBContext.Atributes.FirstOrDefaultAsync(_ => _.Id == NewProduct.FirstAtributeId);
                        if (TempFirstAtr == null) return new JsonResult(new { error = "Incorrect First Atribute ID. Please reload page and try again." });
                        TempProductType.FirstAtribute = TempFirstAtr;
                    }

                    List<ProductAtributes> productAtributes = null;
                    if (NewProduct.ProductAtributes != null && NewProduct.ProductAtributes.Count() > 0)
                    {
                        if (NewProduct.ProductAtributes.Any(_ => _.AtributeValuesID == null || _.AtributeValuesID.Count() == 0)) return new JsonResult(new { error = "Incorrect AtributeValues. Please reload page and try again." });
                        if (NewProduct.ProductAtributes.Any(_ => _.Quantity < 0)) return new JsonResult(new { error = "Incorrect AtributeValues Quantity (0=>). Please correct it." });
                        productAtributes = new List<ProductAtributes>();
                        foreach (var atrVal in NewProduct.ProductAtributes)
                        {
                            var aValues = await DBContext.AtributeValues.Where(_ => atrVal.AtributeValuesID.Any(vId => vId == _.Id.ToString())).Select(_ => new ProductAtributeValue() { AtributeValue = _ }).ToListAsync();
                            if (aValues.Count != atrVal.AtributeValuesID.Count() || aValues.Any(_ => _ == null)) return new JsonResult(new { error = "Incorrect AtributeValueID. Please reload page and try again." });
                            productAtributes.Add(new ProductAtributes() { AtributeValues = aValues, ProductQuantity = atrVal.Quantity });
                        }
                    }
                    if (productAtributes != null && productAtributes.Count > 0)
                    {
                        //productAtributes.ForEach(_ => TempProductType.ProductAtributes.Add(_));
                        TempProductType.ProductAtributes = productAtributes;
                    }

                    if (NewIMGs != null && NewIMGs.Count > 0)
                    {
                        foreach (var img in NewIMGs)
                        {
                            using (MemoryStream mr = new MemoryStream())
                            {
                                img.CopyTo(mr);
                                TempProductType.Images.Add(new ProductIMG() { IMG = mr.ToArray() });
                            }
                        }

                    }

                    TempProduct.ProductTypes.Add(TempProductType);
                    TempCatalog.Products.Add(TempProduct);

                    _pr = TempProduct;
                    _prType = TempProduct.ProductTypes.First(_ => _.IsMine);

                }
                else if (!string.IsNullOrEmpty(NewProduct.ProductId) && !string.IsNullOrEmpty(NewProduct.ProductTypeId))//change Product and Product Type
                {
                    var TempCatalog = await DBContext.Catalogs.FirstOrDefaultAsync(_ => _.Id == NewProduct.CatalogId);
                    if (TempCatalog == null) new JsonResult(new { error = "Incorrect Catalog ID. Please reload page and try again." });
                    _pr = TempCatalog.Products.FirstOrDefault(_ => _.Id == NewProduct.ProductId);
                    if (_pr == null) new JsonResult(new { error = "Incorrect Product ID. Please reload page and try again." });
                    _prType = _pr.ProductTypes.FirstOrDefault(_ => _.Id == NewProduct.ProductTypeId);
                    if (_prType == null) new JsonResult(new { error = "Incorrect Product Type ID. Please reload page and try again." });

                    if (NewProduct.Title != CultureData.GetDefoultName(_prType.CultureTitle, Culture)) _prType.CultureTitle = CultureData.SetOrChangeCultureName(_prType.CultureTitle, NewProduct.Title, Culture);
                    if (NewProduct.Brand != CultureData.GetDefoultName(_prType.CultureBrand, Culture)) _prType.CultureBrand = CultureData.SetOrChangeCultureName(_prType.CultureBrand, NewProduct.Brand, Culture);
                    if (NewProduct.Description != CultureData.GetDefoultName(_prType.CultureDescription, Culture)) _prType.CultureDescription = CultureData.SetOrChangeCultureName(_prType.CultureDescription, NewProduct.Description, Culture);
                    if (NewProduct.SerchKeys != _prType.SerchKeys) _prType.SerchKeys = NewProduct.SerchKeys;
                    _prType.IsActive = NewProduct.IsActive;
                    if (NewProduct.IsMine)
                    {
                        _pr.ProductTypes.First(_ => _.IsMine);
                        _prType.IsMine = NewProduct.IsMine;
                    }

                    _prType.Price = Price;
                    _prType.Sale = Sale;

                    if (NewProduct.RemoveIMGsId != null && NewProduct.RemoveIMGsId.Count() > 0)//Remove Images
                    {
                        var TempRempvedImgs = DBContext.ProductIMGs.Where(_ => NewProduct.RemoveIMGsId.Any(id => id == _.Id));
                        if (TempRempvedImgs == null || TempRempvedImgs.Count() != NewProduct.RemoveIMGsId.Count()) new JsonResult(new { error = "Incorrect Remove Imgs ID. Please reload page and try again." });
                        DBContext.ProductIMGs.RemoveRange(TempRempvedImgs);
                    }

                    if (NewIMGs != null && NewIMGs.Count > 0)//Add New Images
                    {
                        foreach (var img in NewIMGs)
                        {
                            using (MemoryStream mr = new MemoryStream())
                            {
                                img.CopyTo(mr);
                                _prType.Images.Add(new ProductIMG() { IMG = mr.ToArray() });
                            }
                        }
                    }

                    if (NewProduct.ProductAtributes != null && NewProduct.ProductAtributes.Count() > 0)//Change Product Atributes
                    {
                        if (NewProduct.ProductAtributes.Any(_ => _.AtributeValuesID == null || _.AtributeValuesID.Count() == 0)) return new JsonResult(new { error = "Incorrect AtributeValues. Please reload page and try again." });
                        if (NewProduct.ProductAtributes.Any(_ => _.Quantity < 0)) return new JsonResult(new { error = "Incorrect AtributeValues Quantity (0=>). Please correct it." });
                        var TempProductAtr = new List<ProductAtributes>();
                        foreach (var atrVal in NewProduct.ProductAtributes)
                        {
                            var aValues = await DBContext.AtributeValues.Where(_ => atrVal.AtributeValuesID.Any(vId => vId == _.Id.ToString())).Select(_ => new ProductAtributeValue() { AtributeValue = _ }).ToListAsync();
                            if (aValues.Count != atrVal.AtributeValuesID.Count() || aValues.Any(_ => _ == null)) return new JsonResult(new { error = "Incorrect AtributeValueID. Please reload page and try again." });
                            TempProductAtr.Add(new ProductAtributes() { AtributeValues = aValues, ProductQuantity = atrVal.Quantity });
                        }
                        //Remove Old Atributes
                        if (_prType.ProductAtributes != null && _prType.ProductAtributes.Count > 0)
                        {
                            DBContext.ProductAtributeValues.RemoveRange(_prType.ProductAtributes.SelectMany(_ => _.AtributeValues.Select(av => av)));
                            //_prType.ProductAtributes.ToList().ForEach(_ => _prType.ProductAtributes.Remove(_));
                            _prType.ProductAtributes.ToList().ForEach(_ => DBContext.ProductAtributes.Remove(_));
                        }
                        TempProductAtr.ForEach(_ => _prType.ProductAtributes.Add(_));
                    }

                    if (!string.IsNullOrEmpty(NewProduct.CurrentAtributeValue) && _prType?.LinkAtributeValue?.Id != NewProduct.CurrentAtributeValue)//Change Current Atribute
                    {
                        var TempCurrentLink = await DBContext.AtributeValues.FirstOrDefaultAsync(_ => _.Id == NewProduct.CurrentAtributeValue);
                        if (TempCurrentLink == null) return new JsonResult(new { error = "Incorrect Current Atribute Value ID. Please reload page and try again." });
                        _prType.LinkAtributeValue = TempCurrentLink;
                    }

                    if (!string.IsNullOrEmpty(NewProduct.FirstAtributeId) && _prType?.FirstAtribute?.Id != NewProduct.FirstAtributeId)//Change First Atribute
                    {
                        var TempFirstAtr = await DBContext.Atributes.FirstOrDefaultAsync(_ => _.Id == NewProduct.FirstAtributeId);
                        if (TempFirstAtr == null) return new JsonResult(new { error = "Incorrect First Atribute ID. Please reload page and try again." });
                        _prType.FirstAtribute = TempFirstAtr;
                    }

                    if (!string.IsNullOrEmpty(NewProduct.GroupAtribute) && _pr?.LinkAtribute?.Id != NewProduct.GroupAtribute)
                    {
                        var TempGroupAtribute = await DBContext.Atributes.FirstOrDefaultAsync(_ => _.Id == NewProduct.GroupAtribute);
                        if (TempGroupAtribute == null) return new JsonResult(new { error = "Incorrect Link Atribute ID. Please reload page and try again." });
                        _pr.LinkAtribute = TempGroupAtribute;
                    }

                    var proTypeLinks = _pr?.ProductTypes?.Where(_ => _?.LinkAtributeValue != null).Select(_ => _.LinkAtributeValue.Id).ToList();
                    if (proTypeLinks != null && NewProduct.AtributeLinks != null && NewProduct.AtributeLinks.Count() > proTypeLinks.Count)
                    {
                        var AddedLinks = NewProduct.AtributeLinks.Except(proTypeLinks);
                        foreach (var prt in AddedLinks)
                        {
                            var atrVal = await DBContext.AtributeValues.FirstOrDefaultAsync(_ => _.Id == prt);
                            if (atrVal == null) return new JsonResult(new { error = "Incorrect Atribut. Please reload page and try again." });
                            _pr.ProductTypes.Add(new ProductType() { LinkAtributeValue = atrVal });
                        }
                    }

                }
                else return new JsonResult(new { error = "Incorrect data. Please reload page and try again." });
                int res = await DBContext.SaveChangesAsync();
                return new JsonResult(new { message = res.ToString(), resp = (res > 0 ? GetResponse(_pr, _prType) : null) });
            }
            catch (Exception e)
            {
                return new JsonResult(new { error = e.Message });
            }
        }

        private dynamic GetResponse(Product product, ProductType productType)
        {
            string Culture = Request.HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture.Name;
            var SelectedOptions = productType?.ProductAtributes?.Select(_ => _.AtributeValues)
                        .Select(_ => _.Where(av => av.AtributeValue.FK_Atribute.Id != productType?.FirstAtribute?.Id).Select(av => new { atrId = av.AtributeValue.FK_Atribute.Id, atrValId = av.AtributeValue.Id })).ToList();
            for (int i = 0; i < SelectedOptions.Count; i++)
            {
                for (int j = i + 1; j < SelectedOptions.Count; j++)
                {
                    if (SelectedOptions[i].Except(SelectedOptions[j]).Count() == 0)
                    {
                        SelectedOptions.RemoveAt(j);
                        j--;
                    }
                }
            }


            return new
            {

                productId = product?.Id,
                productTypeId = productType?.Id,
                imgIds = productType?.Images?.OrderBy(_ => _._Index).Select(_ => _.Id).ToList(),
                title = CultureData.GetDefoultName(productType?.CultureTitle, Culture),
                brand = CultureData.GetDefoultName(productType?.CultureBrand, Culture),
                description = CultureData.GetDefoultName(productType?.CultureDescription, Culture),
                serchKeys = productType?.SerchKeys,
                price = productType.Price,
                sale = productType.Sale,
                isActive = productType.IsActive,
                isMain = productType.IsMine,

                groupAtributeId = product?.LinkAtribute?.Id,
                currentAtrValId = productType?.LinkAtributeValue?.Id,
                groupAtrValIds = product?.ProductTypes?.OrderBy(_ => _._Index).Select(_ => new
                {
                    atrValId = _?.LinkAtributeValue?.Id,
                    atrId = _?.LinkAtributeValue?.FK_Atribute?.Id,
                    name = CultureData.GetDefoultName(_?.LinkAtributeValue?.CultureName, Culture, _?.LinkAtributeValue?.Value)
                }).ToList(),

                //tableHeader
                firstAtrId = new { id = productType?.FirstAtribute?.Id, name = productType?.FirstAtribute?.Name },
                lastAtrIds = productType?.ProductAtributes?.FirstOrDefault()?.AtributeValues?.OrderBy(_ => _._Index)
                 .Select(_ => _.AtributeValue.FK_Atribute).Where(_ => _.Id != productType?.FirstAtribute?.Id)
                 .Select(_ => new { id = _.Id, name = _.Name }).ToList(),

                //tableBody
                selectedOptions = SelectedOptions,
                inputsVal = productType?.ProductAtributes?.Select(_ => new { qty = _?.ProductQuantity, atrValIds = _?.AtributeValues?.OrderBy(av => av._Index).Select(i => i.AtributeValue.Id) }).ToList()

            };
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<JsonResult> DeleteProduct(string ProductId, string ProductTypeId)
        {
            try
            {
                if (string.IsNullOrEmpty(ProductId) || string.IsNullOrEmpty(ProductTypeId)) return new JsonResult(new { error = "Incorrect request data. Please reload page and try again." });

                var TempProduct = await DBContext.Products.FirstOrDefaultAsync(_ => _.Id == ProductId);
                if (TempProduct == null) return new JsonResult(new { error = "Incorrect Product ID. Please reload page and try again." });
                if (TempProduct.ProductTypes == null || TempProduct.ProductTypes.Count == 0)
                {
                    DBContext.Products.Remove(TempProduct);
                    return new JsonResult(new { error = "Error data. Product Type is empty." });
                }
                var TempProductType = TempProduct.ProductTypes.FirstOrDefault(_ => _.Id == ProductTypeId);
                if (TempProductType == null) return new JsonResult(new { error = "Incorrect Product Type ID. Please reload page and try again." });
                if (TempProduct.ProductTypes.Count > 1 && TempProductType.IsMine)
                {
                    var tp = TempProduct?.ProductTypes?.FirstOrDefault(_ => !_.IsMine);
                    if (tp != null) tp.IsMine = true;

                }
                var AllCultureData = TempProductType.CultureBrand.Union(TempProductType.CultureDescription).Union(TempProductType.CultureTitle);
                if (AllCultureData != null && AllCultureData.Count() > 0) DBContext.CultureDatas.RemoveRange(AllCultureData);

                if (TempProductType.Images != null && TempProductType.Images.Count > 0) DBContext.ProductIMGs.RemoveRange(TempProductType.Images);
                if (TempProductType.ProductAtributes != null && TempProductType.ProductAtributes.Count > 0)
                {
                    var AllAributeValues = TempProductType.ProductAtributes.SelectMany(_ => _.AtributeValues);
                    DBContext.ProductAtributeValues.RemoveRange(AllAributeValues);
                    DBContext.ProductAtributes.RemoveRange(TempProductType.ProductAtributes);
                }

                //TempProduct.ProductTypes.Remove(TempProductType);
                DBContext.ProductTypes.Remove(TempProductType);
                if (TempProduct.ProductTypes.Count == 0)
                {
                    DBContext.Products.Remove(TempProduct);
                    int st = await DBContext.SaveChangesAsync();
                    return new JsonResult(new { message = st });
                }

                int Status = await DBContext.SaveChangesAsync();

                var resp = TempProduct?.ProductTypes?.FirstOrDefault(_ => _.IsMine);
                return new JsonResult(new { message = Status, data = resp });
            }
            catch (Exception e)
            {
                return new JsonResult(new { error = e.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<JsonResult> ChangeProductType(string ProductId, string AtributeTypeId)
        {
            try
            {
                if (string.IsNullOrEmpty(ProductId) || string.IsNullOrEmpty(AtributeTypeId)) return new JsonResult(new { error = "Incorrect request data. Please reload page and try again." });
                var TempProduct = await DBContext.Products.FirstOrDefaultAsync(_ => _.Id == ProductId);
                if (TempProduct == null) return new JsonResult(new { error = "Incorrect Product ID. Please reload page and try again." });
                var TempProductType = TempProduct.ProductTypes.FirstOrDefault(_ => _?.LinkAtributeValue?.Id == AtributeTypeId);
                if (TempProductType == null) return new JsonResult(new { error = "Atribute type link not found. Please SAVE and try again." });
                return new JsonResult(new { data = GetResponse(TempProduct, TempProductType) });
            }
            catch (Exception e)
            {
                return new JsonResult(new { error = e.Message });
            }
        }
        #endregion
        #region Atribute
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Atribute()
        {
            var TempAtributes = await DBContext.Atributes.ToListAsync();
            ViewBag.AtributesList = TempAtributes?.OrderBy(_ => _._Index);
            ViewBag.Culture = Request.HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture.Name;
            ViewBag.Atributes = "ASelect";
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Atribute(string NewName, string CultureName, string CurrentID, string Metod, bool IsAtribute)
        {
            try
            {
                string Culture = Request.HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture.Name;
                switch (Metod)
                {
                    case "Add":
                        if (!string.IsNullOrEmpty(NewName))
                        {
                            if (string.IsNullOrEmpty(CurrentID) && IsAtribute) //Add Atribute
                            {
                                var TempCulture = string.IsNullOrEmpty(CultureName) ? null : new CultureData() { Culture = Culture, Text = CultureName };
                                var TempAtribute = new Atribute() { Name = NewName, IsActive = true };
                                if (TempCulture != null) TempAtribute.CultureName.Add(TempCulture);
                                DBContext.Atributes.Add(TempAtribute);
                            }
                            else if (!string.IsNullOrEmpty(CurrentID) && IsAtribute) // Add AtributeValue
                            {
                                var TempAtribute = await DBContext.Atributes.FirstOrDefaultAsync(_ => _.Id.ToString() == CurrentID);
                                if (TempAtribute != null)
                                {
                                    var TempCulture = string.IsNullOrEmpty(CultureName) ? null : new CultureData() { Culture = Culture, Text = CultureName };
                                    var TempAtributeValue = new AtributeValue() { Value = NewName, };
                                    if (TempCulture != null) TempAtributeValue.CultureName.Add(TempCulture);
                                    TempAtribute.Values.Add(TempAtributeValue);
                                }
                            }
                        }
                        break;
                    case "Delete":
                        if (!string.IsNullOrEmpty(CurrentID))
                        {
                            if (IsAtribute)//Delete Atribute
                            {
                                var TempAtribute = await DBContext.Atributes.FirstOrDefaultAsync(_ => _.Id.ToString() == CurrentID);
                                if (TempAtribute != null)
                                {
                                    var TempAtributeValues = TempAtribute.Values;
                                    if (TempAtributeValues != null && TempAtributeValues.Count > 0)
                                    {
                                        var TempAtributeValueCultures = TempAtributeValues.SelectMany(_ => _.CultureName);
                                        if (TempAtributeValueCultures != null && TempAtributeValueCultures.Count() > 0) DBContext.RemoveRange(TempAtributeValueCultures);
                                        DBContext.AtributeValues.RemoveRange(TempAtributeValues);
                                    }
                                    if (TempAtribute.CultureName != null && TempAtribute.CultureName.Count > 0) DBContext.RemoveRange(TempAtribute.CultureName);
                                    DBContext.Remove(TempAtribute);
                                }
                            }
                            else//Delete AtributeValue
                            {
                                var TempAtributeValue = await DBContext.AtributeValues.FirstOrDefaultAsync(_ => _.Id.ToString() == CurrentID);
                                if (TempAtributeValue != null)
                                {
                                    if (TempAtributeValue.CultureName != null && TempAtributeValue.CultureName.Count > 0) DBContext.RemoveRange(TempAtributeValue.CultureName);
                                    DBContext.AtributeValues.Remove(TempAtributeValue);
                                }
                            }
                        }
                        break;
                    case "Change":
                        if (!string.IsNullOrEmpty(CurrentID) && !string.IsNullOrEmpty(NewName))
                        {
                            if (IsAtribute)//Change Atribute
                            {
                                var TempAtribute = await DBContext.Atributes.FirstOrDefaultAsync(_ => _.Id.ToString() == CurrentID);
                                if (TempAtribute != null)
                                {
                                    TempAtribute.Name = NewName;

                                    var TempCulture = TempAtribute.CultureName.FirstOrDefault(_ => _.Culture == Culture);
                                    if (string.IsNullOrEmpty(CultureName))
                                    {
                                        if (TempCulture != null) TempCulture.Text = "";
                                    }
                                    else
                                    {
                                        if (TempCulture != null) TempCulture.Text = CultureName;
                                        else TempAtribute.CultureName.Add(new CultureData() { Culture = Culture, Text = CultureName });
                                    }
                                }
                            }
                            else//Change AtributeValue
                            {
                                var TempAtributeValue = await DBContext.AtributeValues.FirstOrDefaultAsync(_ => _.Id.ToString() == CurrentID);
                                if (TempAtributeValue != null)
                                {
                                    TempAtributeValue.Value = NewName;

                                    var TempCulture = TempAtributeValue.CultureName.FirstOrDefault(_ => _.Culture == Culture);
                                    if (string.IsNullOrEmpty(CultureName))
                                    {
                                        if (TempCulture != null) TempCulture.Text = "";
                                    }
                                    else
                                    {
                                        if (TempCulture != null) TempCulture.Text = CultureName;
                                        else TempAtributeValue.CultureName.Add(new CultureData() { Culture = Culture, Text = CultureName });
                                    }
                                }
                            }
                        }
                        break;
                    case "View":
                        if (!string.IsNullOrEmpty(CurrentID) && IsAtribute)
                        {
                            var TempAtribute = await DBContext.Atributes.FirstOrDefaultAsync(_ => _.Id.ToString() == CurrentID);
                            if (TempAtribute != null)
                            {
                                TempAtribute.IsActive = !TempAtribute.IsActive;
                            }
                        }
                        break;
                }
                await DBContext.SaveChangesAsync();
                return RedirectToAction("Atribute");
            }
            catch (Exception)
            {
                return RedirectToAction("Atribute");
            }

        }
        #endregion
        #region HomePage
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> HomePage()
        {
            var resp = new HomePageSectionsView(true);
            try
            {
                string Culture = Request.HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture.Name;

                var HP_Sections = await DBContext.HomePageSections.ToListAsync();
                if (HP_Sections != null && HP_Sections.Count > 0)
                {
                    resp = new HomePageSectionsView()
                    {
                        Sections = HP_Sections.OrderBy(_ => _._Index).Select(_ => new SectionView()
                        {
                            Id = _.Id,
                            IsActive = _.IsActive,
                            IsFixedData = _.IsFixedData,
                            Position = _.Position,
                            SectionName = _.SectionName,
                            SectionDatas = _.HomePageSectionDatas.OrderBy(sd => sd._Index).Select(sd => new SectionDataView()
                            {
                                Id = sd.Id,
                                CatalogId = sd.Catalog?.Id ?? "",
                                Description = CultureData.GetDefoultName(sd.Description, Culture),
                                Title = CultureData.GetDefoultName(sd.Title, Culture),
                                TextIsWhith = sd.TextIsWhith,
                                ImgID = (sd.Img == null || sd.Img.Count() == 0) ? "" : sd.Id
                            })
                        })
                    };
                }
                var TempCatalos = DBContext.Catalogs.Where(_ => _.FatherCatalog == null).OrderBy(_ => _._Index);

                ViewBag.CatalogList = GetCatalog(TempCatalos, "");
                ViewBag.HomePage = "ASelect";
                return View(resp);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(resp);
            }

        }

        private IDictionary<string, string> GetCatalog(IEnumerable<Catalog> Catalogs, string Position)
        {
            Dictionary<string, string> Temp = new Dictionary<string, string>();
            for (int i = 0; i < Catalogs.Count(); i++)
            {
                Temp.Add(Catalogs.ElementAt(i).Id, Position + Catalogs.ElementAt(i).Name);
                if (Catalogs.ElementAt(i).ChaildCatalogs != null && Catalogs.ElementAt(i).ChaildCatalogs.Count() > 0)
                {
                    foreach (var n in GetCatalog(Catalogs.ElementAt(i).ChaildCatalogs.OrderBy(_ => _._Index), Position + "&#8195;"))
                    {
                        Temp.Add(n.Key, n.Value);
                    }
                }
            }
            return Temp;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> HomePage(HomePageSectionsView Req)
        {
            string Culture = Request.HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture.Name;
            if (Req != null && Req.Sections != null && Req.Sections.Count() == 7)
            {
                for (int i = 0; i < 7; i++)
                {
                    var ReqSectionView = Req.Sections.ElementAt(i);
                    if (string.IsNullOrEmpty(ReqSectionView.Id)) return RedirectToAction("HomePage");
                    if (ReqSectionView.Id == "first")//Add New Section
                    {
                        if (ReqSectionView.SectionDatas == null || ReqSectionView.SectionDatas.Count() == 0) return RedirectToAction("HomePage");
                        var TempHP_Section = new HomePageSection() { IsActive = ReqSectionView.IsActive, IsFixedData = ReqSectionView.IsFixedData, Position = ReqSectionView.Position, SectionName = ReqSectionView.SectionName };
                        for (int j = 0; j < ReqSectionView.SectionDatas.Count(); j++)
                        {
                            var TempReq = ReqSectionView.SectionDatas.ElementAt(j);
                            var TempCatalog = await DBContext.Catalogs.FirstOrDefaultAsync(_ => _.Id == TempReq.CatalogId);
                            if (TempCatalog == null) return RedirectToAction("HomePage");
                            byte[] TempImg = null;
                            if (Req.GetImgs(i) != null && Req.GetImgs(i).Count() != 0)
                            {
                                for (int k = 0; k < Req.GetImgs(i).Count(); k++)
                                {
                                    if (Req.ChangedImgs != null && Req.ChangedImgs[i] != null && Req.ChangedImgs[i][k] == j)
                                    {
                                        using (MemoryStream mr = new MemoryStream())
                                        {
                                            Req.GetImgs(i).ElementAt(k).CopyTo(mr);
                                            TempImg = mr.ToArray();
                                        }
                                    }
                                }
                            }

                            var TempSectionData = new HomePageSectionData()
                            {
                                Catalog = TempCatalog,
                                Description = new CultureData[] { new CultureData() { Text = TempReq.Description, Culture = Culture } },
                                Title = new CultureData[] { new CultureData() { Text = TempReq.Title, Culture = Culture } },
                                TextIsWhith = TempReq.TextIsWhith,
                                Img = TempImg
                            };
                            TempHP_Section.HomePageSectionDatas.Add(TempSectionData);
                        }
                        DBContext.HomePageSections.Add(TempHP_Section);
                        continue;
                    }

                    var TempSection = await DBContext.HomePageSections.FirstOrDefaultAsync(_ => _.Id == ReqSectionView.Id);
                    if (TempSection == null) return RedirectToAction("HomePage");

                    if (TempSection.Position != ReqSectionView.Position) TempSection.Position = ReqSectionView.Position;//Change Section Position
                    if (TempSection.IsActive != ReqSectionView.IsActive) TempSection.IsActive = ReqSectionView.IsActive;

                    var ReqSectionDatasView = ReqSectionView.SectionDatas;
                    if (ReqSectionDatasView == null) return RedirectToAction("HomePage");

                    for (int j = 0; j < ReqSectionDatasView.Count(); j++)
                    {
                        var ReqSDViews = ReqSectionDatasView.ElementAt(j);
                        if (ReqSDViews.IsDelete && (i == 0 || i == 2) && ReqSectionView.SectionDatas.Count() > 1 && ReqSDViews.Id != "new")//Delete Section Data
                        {
                            var TempDeleteSD = TempSection.HomePageSectionDatas.FirstOrDefault(_ => _.Id == ReqSDViews.Id);
                            if (TempDeleteSD == null) return RedirectToAction("HomePage");
                            if (TempDeleteSD.Title != null || TempDeleteSD.Title.Count() > 0) DBContext.CultureDatas.RemoveRange(TempDeleteSD.Title);
                            if (TempDeleteSD.Description != null || TempDeleteSD.Description.Count() > 0) DBContext.CultureDatas.RemoveRange(TempDeleteSD.Description);
                            //TempSection.HomePageSectionDatas.Remove(TempDeleteSD);
                            DBContext.HomePageSectionDatas.Remove(TempDeleteSD);
                            await DBContext.SaveChangesAsync();
                            return RedirectToAction("HomePage");
                        }
                        if ((i == 0 || i == 2) && ReqSDViews.Id == "new")//Add New
                        {
                            var TempCatalog = await DBContext.Catalogs.FirstOrDefaultAsync(_ => _.Id == ReqSDViews.CatalogId);
                            if (TempCatalog == null) return RedirectToAction("HomePage");
                            byte[] TempImg = null;
                            if (Req.GetImgs(i) != null && Req.GetImgs(i).Count() > 0 && Req.ChangedImgs[i] != null && Req.ChangedImgs[i].Count() > 0)
                            {
                                for (int k = 0; k < Req.ChangedImgs[i].Count(); k++)
                                {
                                    if (Req.ChangedImgs[i][k] == j)
                                    {
                                        using (MemoryStream mr = new MemoryStream())
                                        {
                                            Req.GetImgs(i).ElementAt(k).CopyTo(mr);
                                            TempImg = mr.ToArray();
                                        }
                                        break;
                                    }
                                }
                            }
                            HomePageSectionData NewData = new HomePageSectionData()
                            {
                                Catalog = TempCatalog,
                                TextIsWhith = ReqSDViews.TextIsWhith,
                                Title = new CultureData[] { new CultureData() { Text = ReqSDViews.Title, Culture = Culture } },
                                Description = new CultureData[] { new CultureData() { Text = ReqSDViews.Description, Culture = Culture } },
                                Img = TempImg
                            };
                            TempSection.HomePageSectionDatas.Add(NewData);
                        }
                        else
                        {
                            var TempSD = TempSection.HomePageSectionDatas.FirstOrDefault(_ => _.Id == ReqSDViews.Id);
                            if (TempSD == null) return RedirectToAction("HomePage");

                            if (TempSD.TextIsWhith != ReqSDViews.TextIsWhith) TempSD.TextIsWhith = ReqSDViews.TextIsWhith;
                            if (!string.IsNullOrEmpty(ReqSDViews.CatalogId))
                            {
                                if (TempSD.Catalog == null || TempSD.Catalog.Id != ReqSDViews.CatalogId)
                                {
                                    var TempCatalog = await DBContext.Catalogs.FirstOrDefaultAsync(_ => _.Id == ReqSDViews.CatalogId);
                                    if (TempCatalog == null) return RedirectToAction("HomePage");
                                    TempSD.Catalog = TempCatalog;
                                }
                            }
                            if (CultureData.GetDefoultName(TempSD.Title, Culture) != ReqSDViews.Title) TempSD.Title = CultureData.SetOrChangeCultureName(TempSD.Title, ReqSDViews.Title, Culture);
                            if (CultureData.GetDefoultName(TempSD.Description, Culture) != ReqSDViews.Description) TempSD.Description = CultureData.SetOrChangeCultureName(TempSD.Description, ReqSDViews.Description, Culture);
                            byte[] TempImg = null;
                            if (Req.GetImgs(i) != null && Req.GetImgs(i).Count() > 0 && Req.ChangedImgs[i] != null && Req.ChangedImgs[i].Count() > 0)
                            {
                                for (int k = 0; k < Req.ChangedImgs[i].Count(); k++)
                                {
                                    if (Req.ChangedImgs[i][k] == j)
                                    {
                                        using (MemoryStream mr = new MemoryStream())
                                        {
                                            Req.GetImgs(i).ElementAt(k).CopyTo(mr);
                                            TempImg = mr.ToArray();
                                        }
                                        break;
                                    }
                                }
                            }
                            if (TempImg != null) { TempSD.Img = TempImg; TempSD.Added = DateTime.Now; }
                        }
                    }

                }
                await DBContext.SaveChangesAsync();
            }
            return RedirectToAction("HomePage");
        }
        #endregion
        #region Review
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Review(ReviewSerch serch)
        {
            try
            {
                var reviews = await DBContext.Reviews.Where(_ => _.Status == serch.status && (serch.lastDays == 0 || _.Date > DateTime.Now.AddDays(-serch.lastDays)) && (string.IsNullOrEmpty(serch.userId) || _.FK_User.NormalizedEmail == serch.userId.ToUpper())).ToListAsync();

                var m = reviews.Select(_ => new ReviewViewModel() { Id = _.Id, Description = _.Description, Rating = _.Rating, Status = _.Status, UserId = _.FK_User.Email }).ToList();
                ViewBag.Reviews = "ASelect";
                var gs = await DBContext.GlobalSetings.FirstOrDefaultAsync();
                ViewBag.setAll = false;
                if (gs != null && gs.EnableAllReviews) ViewBag.setAll = true;
                ViewBag.Serch = serch;
                return View(m);
            }
            catch (Exception)
            {
                return View();
            }

        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Review(IEnumerable<ReviewViewModel> req, bool setAll)
        {
            try
            {
                var gs = await DBContext.GlobalSetings.FirstOrDefaultAsync();
                if (gs != null && gs.EnableAllReviews != setAll)
                {
                    gs.EnableAllReviews = setAll;
                    await DBContext.SaveChangesAsync();
                }
                else
                {
                    DBContext.GlobalSetings.Add(new GlobalSetings() { EnableAllReviews = setAll });
                    await DBContext.SaveChangesAsync();
                }
                if (req != null && req.Count() > 0)
                {
                    var TempRev = await DBContext.Reviews.Where(_ => req.Any(r => r.Id == _.Id)).ToListAsync();
                    if (TempRev != null && TempRev.Count() > 0 && TempRev.Count() == req.Count())
                    {
                        foreach (var r in TempRev)
                        {
                            var tr = req.FirstOrDefault(_ => _.Id == r.Id);
                            if (tr != null)
                            {
                                r.Status = tr.Status;
                            }
                        }
                        await DBContext.SaveChangesAsync();
                    }
                }
                ViewBag.Reviews = "ASelect";
                return RedirectToAction("Review");
            }
            catch (Exception)
            {
                return RedirectToAction("Review");
            }

        }
        #endregion
        #region Users
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var users = await DBContext.Users.Where(_ => _.Registred > DateTime.Now.AddDays(-7)).ToArrayAsync();
            ViewBag.Email = "";
            ViewBag.From = DateTime.Now.AddDays(-7);
            ViewBag.To = DateTime.Now.AddDays(1);
            ViewBag.Users = "ASelect";
            return View(users);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Users(string Email, DateTime From, DateTime To)
        {
            ViewBag.Email = Email;
            ViewBag.From = From;
            ViewBag.To = To;
            ViewBag.Users = "ASelect";
            List<User> users = new List<User>();
            if (string.IsNullOrEmpty(Email))
            {
                users = await DBContext.Users.Where(_ => _.Registred > From && _.Registred < To).OrderBy(_ => _._Index).ToListAsync();
            }
            else
            {
                var u = await DBContext.Users.FirstOrDefaultAsync(_ => _.NormalizedEmail == Email.ToUpper());
                if (u != null) users.Add(u);
            }
            return View(users);
        }
        #endregion
        #region PromoCode
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Promo()
        {
            ViewBag.Promo = "ASelect";
            ViewBag.PromoCodes = await DBContext.PromoCodes.ToListAsync();
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Promo(PromoCodeViewModel model)
        {
            try
            {
                ViewBag.PromoCodes = await DBContext.PromoCodes.ToListAsync();
                if (ModelState.IsValid)
                {
                    if (model.Expired < DateTime.Now)
                    {
                        ModelState.AddModelError("Expired", "Incorrect date.");
                        return View();
                    }
                    if (DBContext.PromoCodes.Any(_ => _.Name.ToUpper() == model.Name))
                    {
                        ModelState.AddModelError("Name", "This code NAME is already added.");
                        return View();
                    }
                    DBContext.PromoCodes.Add(new PromoCode() { Created = DateTime.Now, Expired = model.Expired, Name = model.Name, SalePercent = model.Parcent, isActive = true });
                    await DBContext.SaveChangesAsync();
                    return RedirectToAction("Promo", "Admin");
                }
                return View();
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                return View();
            }

        }
        [Authorize(Roles = "Admin")]
        public async Task changePromoStatus(int id)
        {
            var promo = await DBContext.PromoCodes.FirstOrDefaultAsync(_ => _.id == id);
            if (promo != null)
            {
                promo.isActive = !promo.isActive;
                await DBContext.SaveChangesAsync();
            }
        }
        [Authorize(Roles = "Admin")]
        public async Task deletePromo(int id)
        {
            var promo = await DBContext.PromoCodes.FirstOrDefaultAsync(_ => _.id == id);
            if (promo != null)
            {
                DBContext.PromoCodes.Remove(promo);
                await DBContext.SaveChangesAsync();
            }
        }
        #endregion
        #region OrderHistory
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> OrderHistory(OrderFilters filters)
        {
            OrderFilters F = filters;
            if (F.from == DateTime.MinValue)
            {
                F = new OrderFilters() { from = DateTime.Now.Date, to = DateTime.Now.Date.AddDays(1), status = Order.StatusType.Pending };
            }
            else
            {
                F.email = F.email?.Trim().ToLower();
                F.orderNumber = F.orderNumber?.Trim();
                F.phone = F.phone?.Trim();
            }
            ViewBag.Filtr = F;
            ViewBag.Orders = await DBContext.Orders
                .Where(_ => _.Status == F.status.ToString() && _.Created > F.from && _.Created < F.to
                && (string.IsNullOrEmpty(F.orderNumber) || _.OrderNumber.ToString() == F.orderNumber)
                && (string.IsNullOrEmpty(F.email) || (_.Email != null && _.Email.ToLower() == F.email))
                && (string.IsNullOrEmpty(F.phone) || _.Phone.Contains(F.phone)))
                .Take(OrderHistoryItems).ToListAsync();
            ViewBag.OrderHistory = "ASelect";
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> OrderHistoryI(OrderFilters filters, int Index)
        {
            OrderFilters F = filters;
            var Orders = await DBContext.Orders.Where(_ => _.Status == F.status.ToString() && _.Created > F.from && _.Created < F.to
                && (string.IsNullOrEmpty(F.orderNumber) || _.OrderNumber.ToString() == F.orderNumber)
                && (string.IsNullOrEmpty(F.email) || (_.Email != null && _.Email.ToLower() == F.email))
                && (string.IsNullOrEmpty(F.phone) || _.Phone.Contains(F.phone)))
                .Skip(Index * OrderHistoryItems).Take(OrderHistoryItems).ToListAsync();
            return View(Orders);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task changeOrderStatus(string id, Order.StatusType status)
        {
            if (string.IsNullOrEmpty(id)) return;
            var order = await DBContext.Orders.FirstOrDefaultAsync(_ => _.Id == id);
            if (order == null) return;
            order.Status = status.ToString();
            switch (status)
            {
                case Order.StatusType.Pending:
                    if (order.Status == Order.StatusType.Refunded.ToString() || order.Status == Order.StatusType.Canceled.ToString()) order.ProductsInfo.ToList().ForEach(_ => _.ProductAtributes.ProductQuantity -= _.Quantity);
                    break;
                case Order.StatusType.Shipping:
                    if (order.Status == Order.StatusType.Refunded.ToString() || order.Status == Order.StatusType.Canceled.ToString()) order.ProductsInfo.ToList().ForEach(_ => _.ProductAtributes.ProductQuantity -= _.Quantity);
                    break;
                case Order.StatusType.Delivered:
                    if (order.Status == Order.StatusType.Refunded.ToString() || order.Status == Order.StatusType.Canceled.ToString()) order.ProductsInfo.ToList().ForEach(_ => _.ProductAtributes.ProductQuantity -= _.Quantity);
                    break;
                case Order.StatusType.Refunded:
                    if (order.Status != Order.StatusType.Canceled.ToString()) order.ProductsInfo.ToList().ForEach(_ => _.ProductAtributes.ProductQuantity += _.Quantity);
                    break;
                case Order.StatusType.Canceled:
                    if (order.Status != Order.StatusType.Refunded.ToString()) order.ProductsInfo.ToList().ForEach(_ => _.ProductAtributes.ProductQuantity += _.Quantity);
                    break;
            }
            await DBContext.SaveChangesAsync();
        }
        #endregion
        #region LiveChat
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> liveChat(string connectionId)
        {
            var gs = await DBContext.GlobalSetings.FirstOrDefaultAsync();
            if (gs != null)
            {
                ViewBag.AutoResponse = gs.LiveChat_AutoResponse;
                ViewBag.OperatorName = gs.LiveChat_OperatorName;
            }
            ViewBag.liveChat = "ASelect";
            UserChat userChat = null;
            if (!string.IsNullOrEmpty(connectionId)) UserHandler.Users.TryGetValue(connectionId, out userChat);
            if (userChat != null && userChat.messages != null && userChat.messages.Count > 0) userChat.messages.ForEach(_ => _.IsNew = false);
            ViewBag.userChat = userChat;
            ViewBag.ConnId = connectionId;
            UserHandler.CurrentUserConnectionId = connectionId;
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task SendMessage(string connectionId, string message, string operatorName)
        {
            if (!UserHandler.Users.Any(_ => _.Key == connectionId))
            {
                return;
            }
            UserHandler.Users[connectionId].messages.Add(new ChatMessage() { IsNew = false, IsOperator = true, Message = message, Sended = DateTime.Now });
            if (string.IsNullOrEmpty(UserHandler.Users[connectionId].OperatorName)) UserHandler.Users[connectionId].OperatorName = string.IsNullOrEmpty(operatorName) ? "Operator" : operatorName;
            await hubContext.Clients.Client(connectionId).SendAsync("newMessage", operatorName, message);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> liveChatSetings(string autoResponse, string operatorName)
        {
            var globalSetings = await DBContext.GlobalSetings.FirstOrDefaultAsync();
            if (globalSetings == null)
            {
                DBContext.GlobalSetings.Add(new GlobalSetings() { LiveChat_AutoResponse = autoResponse, LiveChat_OperatorName = operatorName });
                await DBContext.SaveChangesAsync();
                return RedirectToAction("liveChat");
            }
            if (globalSetings.LiveChat_AutoResponse != autoResponse) globalSetings.LiveChat_AutoResponse = autoResponse;
            if (globalSetings.LiveChat_OperatorName != operatorName) globalSetings.LiveChat_OperatorName = operatorName;
            await DBContext.SaveChangesAsync();
            return RedirectToAction("liveChat");
        }
        #endregion
        #region Messages
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Messages(AdminMessagesFiltr filtr)
        {
            AdminMessagesFiltr f = filtr;
            if (f.from == DateTime.MinValue) f = new AdminMessagesFiltr() { from = DateTime.Now.Date, to = DateTime.Now.Date.AddDays(1), OnlyNew = true };
            var users = await DBContext.Users.Where(_ => (string.IsNullOrEmpty(f.Email) || _.NormalizedEmail == f.Email.ToUpper())
                && ((_.UserSend.Count() > 0 && _.UserSend.Any(m => m.SendedDate > f.from && m.SendedDate < f.to)) || (_.AdminSend.Count() > 0 && _.AdminSend.Any(m => m.SendedDate > f.from && m.SendedDate < f.to)))
              && (!f.OnlyNew || (_.UserSend.Count() > 0 && _.UserSend.Any(s => s.IsNew)))).ToListAsync();
            ViewBag.Users = users;
            ViewBag.Messages = "ASelect";
            ViewBag.filtr = f;
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<JsonResult> getMessages(string email)
        {
            if (string.IsNullOrEmpty(email)) return new JsonResult(new { error = "Empty Email." });
            var user = await DBContext.Users.FirstOrDefaultAsync(_ => _.NormalizedEmail == email.ToUpper());
            if (user == null) return new JsonResult(new { error = "Incorrect Email." });
            if (user.UserSend.Where(_ => _.IsNew).Count() > 0)
            {
                user.UserSend.Where(_ => _.IsNew).ToList().ForEach(_ => _.IsNew = false);
                await DBContext.SaveChangesAsync();
            }
            var mess = user.UserSend.Concat(user.AdminSend).OrderBy(_ => _.Id).ToArray();
            return new JsonResult(new { res = mess.Select(_ => new { title = _.Title, mess = _.Text, send = _.SendedDate.ToString("dd/MM/yyyy HH:mm"), is_user = (_.FK_UserSend != null ? true : false) }) });
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<JsonResult> sendtMessages(string title, string mess, string email)
        {
            if (string.IsNullOrEmpty(mess)) return new JsonResult(new { error = "Message is empty." });
            if (string.IsNullOrEmpty(email)) return new JsonResult(new { error = "Title is empty." });
            if (string.IsNullOrEmpty(email)) return new JsonResult(new { error = "Email is empty." });
            var user = await DBContext.Users.FirstOrDefaultAsync(_ => _.NormalizedEmail == email.ToUpper());
            if (user == null) return new JsonResult(new { error = "Incorrect Email." });
            user.AdminSend.Add(new Message() { SendedDate = DateTime.Now, Text = mess, Title = title, IsNew = true, FK_AdminSend = await userManager.GetUserAsync(User) });
            int status = await DBContext.SaveChangesAsync();
            return new JsonResult(new { res = status });
        }
        #endregion
        #region Serch History
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> serchHistory(DateTime from, DateTime to)
        {
            ViewBag.serchHistory = "ASelect";
            if (from == DateTime.MinValue)
            {
                from = DateTime.Now.Date;
                to = DateTime.Now.Date.AddDays(1);
            }
            ViewBag.from = from;
            ViewBag.to = to;
            var sh = await DBContext.SerchHistories.Where(_ => _.Created > from && _.Created < to).ToListAsync();
            ViewBag.SH = sh;
            ViewBag.SHGroup = sh.GroupBy(_ => _.Key).Select(_ => new KeyValuePair<string, int>(_.Key, _.Count()));
            return View();
        }
        #endregion
        #region Mailing
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Mailing()
        {
            ViewBag.Mailing = "ASelect";
            ViewBag.uCount = await DBContext.Users.CountAsync();
            ViewBag.sCount = await DBContext.SubscribeEmails.CountAsync();
            ViewBag.nsCount = await DBContext.SubscribeEmails.Where(_ => _.isNew).CountAsync();
            ViewBag.mh = await DBContext.MailingHistories.Where(_ => _.Created > DateTime.Now.Date.AddMonths(-1)).ToListAsync();
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Mailing(MailingViewModel req)
        {
            ViewBag.Mailing = "ASelect";
            try
            {
                ViewBag.m = req;
                ViewBag.uCount = await DBContext.Users.CountAsync();
                ViewBag.sCount = await DBContext.SubscribeEmails.CountAsync();
                ViewBag.nsCount = await DBContext.SubscribeEmails.Where(_ => _.isNew).CountAsync();
                ViewBag.mh = await DBContext.MailingHistories.Where(_ => _.Created > DateTime.Now.Date.AddMonths(-1)).ToListAsync();
                List<string> emails = new List<string>();
                if (req.RegistredUsers)
                {
                    var temp = await DBContext.Users.Select(_ => _.Email).ToListAsync();
                    if (temp != null && temp.Count > 0) emails.AddRange(temp);
                }
                if (req.SubscribedUsers)
                {
                    var temp = await DBContext.SubscribeEmails.Select(_ => _.Email).ToListAsync();
                    if (temp != null && temp.Count > 0) emails.AddRange(temp);
                }
                if (req.NewSubscribedUsers && !req.SubscribedUsers)
                {
                    var temp = await DBContext.SubscribeEmails.Where(_ => _.isNew).Select(_ => _.Email).ToListAsync();
                    if (temp != null && temp.Count > 0) emails.AddRange(temp);
                }
                if (!string.IsNullOrEmpty(req.OtherUsers))
                {
                    var temp = req.OtherUsers.Trim().Split(' ', ',', '\n').Where(_ => !string.IsNullOrEmpty(_)).Select(_ => _.Trim());
                    if (temp != null && temp.Count() > 0) emails.AddRange(temp);
                }
                if (emails.Count > 0)
                {
                    bool res = await EmailService.SendEmailAsync(null, req.Title, req.Message, emails);
                    if (res)
                    {
                        if (req.NewSubscribedUsers)
                        {
                            var temp = await DBContext.SubscribeEmails.Where(_ => _.isNew).ToListAsync();
                            if (temp != null && temp.Count > 0)
                            {
                                temp.Take((int)ViewBag.nsCount).ToList().ForEach(_ => _.isNew = false);
                            }
                        }
                        DBContext.MailingHistories.Add(new MailingHistory() { Created = DateTime.Now, Title = req.Title, Message = req.Message, MessageCount = emails.Count, to = string.Join(' ', emails) });
                        await DBContext.SaveChangesAsync();
                        return RedirectToAction("Mailing");
                    }
                    ViewBag.err = "Error.";
                }
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.err = ex.Message;
                return View();
            }
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> getEmails(int ise)
        {
            if (ise == 1) ViewBag.ems = await DBContext.Users.Select(_ => _.Email).ToListAsync();
            if (ise == 2) ViewBag.ems = await DBContext.SubscribeEmails.Select(_ => _.Email).ToListAsync();
            if (ise == 3) ViewBag.ems = await DBContext.SubscribeEmails.Where(_ => _.isNew).Select(_ => _.Email).ToListAsync();
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<JsonResult> getMailingHistory(int interval)
        {
            if (interval < 0) return new JsonResult(new { });
            var historyList = await DBContext.MailingHistories.Where(_ => interval == 0 || _.Created > DateTime.Now.Date.AddMonths(-interval)).ToListAsync();
            if (historyList != null && historyList.Count() > 0)
            {
                historyList.Reverse();
                return new JsonResult(historyList.Select(_ => new { title = _.Title, count = _.MessageCount, date = _.Created.ToShortDateString(), to = _.to, mess = _.Message }));
            }
            return new JsonResult(new { });
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RequestCallAndEmail(int? indexCell = null, bool? isNewCell = null, int? indexEmail = null, bool? isNewEmail = null)
        {
            ViewBag.ReqCallAndEmail = "ASelect";
            if (indexCell == null) indexCell = 1;
            if (indexEmail == null) indexEmail = 1;
            if (isNewCell == null) isNewCell = true;
            if (isNewEmail == null) isNewEmail = true;
            ViewBag.ic = indexCell;
            ViewBag.ie = indexEmail;
            ViewBag.isnc = isNewCell;
            ViewBag.isne = isNewEmail;

            var tc = await DBContext.RequestCells.Where(_ => (!(bool)isNewCell || _.isNew == isNewCell) && (indexCell == 0 || _.Created > DateTime.Now.Date.AddMonths((int)-indexCell))).ToListAsync();
            tc?.Reverse();
            ViewBag.cells = tc;
            var te = await DBContext.RequestEmails.Where(_ => (!(bool)isNewEmail || _.isNew == isNewEmail) && (indexEmail == 0 || _.Created > DateTime.Now.Date.AddMonths((int)-indexEmail))).ToListAsync();
            te?.Reverse();
            ViewBag.emails = te;

            return View();
        }
        [HttpPost]
        public async Task viewRequestCellOrEmail(bool isCell, int id)
        {
            if (isCell)
            {
                var temp = await DBContext.RequestCells.FirstOrDefaultAsync(_ => _.Id == id);
                if (temp != null)
                {
                    temp.isNew = false;
                    await DBContext.SaveChangesAsync();
                }
            }
            else
            {
                var temp = await DBContext.RequestEmails.FirstOrDefaultAsync(_ => _.Id == id);
                if (temp != null)
                {
                    temp.isNew = false;
                    await DBContext.SaveChangesAsync();
                }
            }
        }
        #endregion
    }
}