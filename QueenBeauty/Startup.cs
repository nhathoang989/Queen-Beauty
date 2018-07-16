﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.WebEncoders;
using Swastika.Cms.Lib.Models.Cms;
using Swastika.Cms.Lib.Services;
using Swastika.Identity.Services;
using System.Text.Encodings.Web;
using System.Text.Unicode;


namespace QueenBeauty
{
    public partial class Startup
    {
        public const string CONST_ROUTE_DEFAULT_CULTURE = "vi-vn";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            ConfigIdentity(services, Configuration, Swastika.Cms.Lib.SWCmsConstants.CONST_DEFAULT_CONNECTION); //Cms Config
            ConfigCookieAuth(services, Configuration);
            ConfigJWTToken(services, Configuration);

            //services.AddDbContext<SiocCmsContext>();
            //When View Page Source That changes only the HTML encoder, leaving the JavaScript and URL encoders with their (ASCII) defaults.
            services.Configure<WebEncoderOptions>(options => options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All));
            services.Configure<FormOptions>(options => options.MultipartBodyLengthLimit = 100000000);

            // add application services.
            services.AddTransient<IEmailSender, AuthEmailMessageSender>();
            services.AddTransient<ISmsSender, AuthSmsMessageSender>();

            services.AddSingleton<GlobalConfigurationService>();
            GlobalConfigurationService.Instance.RefreshAll();
            //services.AddSingleton<GlobalLanguageService>();

            services.AddMvc(options =>
            {
                options.CacheProfiles.Add("Default",
                    new CacheProfile()
                    {
                        Duration = 60
                    });
                options.CacheProfiles.Add("Never",
                    new CacheProfile()
                    {
                        Location = ResponseCacheLocation.None,
                        NoStore = true
                    });
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseDeveloperExceptionPage();
                //app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areaRoute",
                    template: "{culture=" + CONST_ROUTE_DEFAULT_CULTURE + "}/{area:exists}/{controller=Portal}/{action=Index}");
                routes.MapRoute(
                    name: "Page",
                    template: "{culture=" + CONST_ROUTE_DEFAULT_CULTURE + "}/{seoName}");
                routes.MapRoute(
                    name: "File",
                    template: "{culture=" + CONST_ROUTE_DEFAULT_CULTURE + "}/Portal/File");
                routes.MapRoute(
                    name: "Article",
                    template: "{culture=" + CONST_ROUTE_DEFAULT_CULTURE + "}/article/{seoName}");
                routes.MapRoute(
                    name: "Product",
                    template: "{culture=" + CONST_ROUTE_DEFAULT_CULTURE + "}/product/{seoName}");
            });
        }
    }
}
