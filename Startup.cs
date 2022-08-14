using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using _4AutoMarket.Services;
using _4AutoMarket.Classes;

using System.Reflection;
using Microsoft.Extensions.Localization;

[assembly: ResourceLocation("Resources")]
[assembly: RootNamespace("_4AutoMarket")]

namespace _4AutoMarket
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IHostingEnvironment ev)
        {
            var conf = new ConfigurationBuilder().SetBasePath(ev.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            if (ev.IsDevelopment()) conf.AddJsonFile($"appsettings.Development.json", optional: true);
            Configuration = conf.Build();
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddMvcLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();

            services.AddDbContext<Models.UserDBContext>(optins => optins.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<Models.User, IdentityRole>(options =>
            {
                //Registration Option
                options.Password.RequireNonAlphanumeric = false;//Password Options
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                //Login Options
                options.Lockout.DefaultLockoutTimeSpan = new TimeSpan(0, 15, 0);//15Minutes
            })
                .AddEntityFrameworkStores<Models.UserDBContext>()
                .AddDefaultTokenProviders();




            services.AddDistributedMemoryCache();
            services.AddSession();

            services.AddAuthentication().AddGoogle(options => {
                options.ClientId = "ClientId";
                options.ClientSecret = "ClientSecret";
                options.CallbackPath = "/Account/GoogleResponse/";
            }).AddFacebook(options => {
                options.ClientId = "ClientId";
                options.ClientSecret = "ClientSecret";
                options.CallbackPath = "/Account/FacebookResponse/";
            });
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en"),
                    new CultureInfo("ru"),
                    new CultureInfo("hy")
                };

                options.DefaultRequestCulture = new RequestCulture("ru");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
            //services.AddMemoryCache();
            services.AddTransient<IMainMenu, TwoLevelMenu>();

            services.AddSignalR(option => {
                option.ClientTimeoutInterval = TimeSpan.FromHours(1);
                option.HandshakeTimeout = TimeSpan.FromHours(1);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //env.EnvironmentName = "Production";
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/ErrorPage");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseRequestLocalization();
            app.UseStaticFiles();
            //app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseSession();
            app.UseSignalR(routes =>
            {
                routes.MapHub<ChatHub>("/chatHub");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

        }
    }
}
