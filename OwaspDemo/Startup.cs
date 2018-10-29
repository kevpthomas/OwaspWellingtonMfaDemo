using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MultiFactorAuthentication;
using MultiFactorAuthentication.Abstractions;
using OwaspDemo.Abstractions;
using OwaspDemo.Data;
using OwaspDemo.Identity;
using OwaspDemo.Models;
using ScottBrady91.AspNetCore.Identity;

namespace OwaspDemo
{
    public class Startup
    {
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

            services.AddAuthentication(options =>
            {
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(options => { options.LoginPath = "/Login"; });

            services.AddMemoryCache();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AuthorizeFolder("/");
                    options.Conventions.AllowAnonymousToPage("/EnableAuthenticator");
                    options.Conventions.AllowAnonymousToPage("/Login");
                    options.Conventions.AllowAnonymousToPage("/LoginWith2fa");
                    options.Conventions.AllowAnonymousToPage("/Register");
                });;

            services.AddSingleton<ISecretKeyProvider, Base32SecretKeyProvider>();
            services.AddSingleton<ITotpTokenBuilder, TotpTokenBuilder>();

            services.AddScoped<ICacheContext, CacheContext>();
            services.AddScoped<IUserManager, UserManager>();
            services.AddScoped<IMemoryContext, MemoryContext>();
            services.AddScoped<ISignInManager, SignInManager>();
            services.AddScoped<IUserAuthenticatorKeyStore, UserAuthenticatorKeyStore>();
            services.AddScoped<ISecretKeyEncrypter, RijndaelCbcSecretKeyEncrypter>();
            services.AddSingleton<IMemoryDatabase, MemoryDatabase>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IDateTime, DateTimeAdapter>();
            services.AddScoped<IPasswordHasher<UserData>, BCryptPasswordHasher<UserData>>();

            ConfigureTotpServices(services);

            // AddAsync CookieTempDataProvider after AddMvc and include ViewFeatures.
            // using Microsoft.AspNetCore.Mvc.ViewFeatures;
            services.AddSingleton<ITempDataProvider, CacheTempDataProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();
            app.UseMvc();
        }

        private void ConfigureTotpServices(IServiceCollection services)
        {
            var totpType = Configuration["ioc:totpType"];
            switch (totpType)
            {
                case "Manual":
                    services.AddScoped<ITotpTokenProvider, ManualTotpTokenProvider>();
                    services.AddScoped<ITotpTokenValidator, ManualTotpTokenValidator>();
                    services.AddScoped<IUserTwoFactorTokenProvider, ManualUserTwoFactorTokenProvider>();
                    break;
                case "OtpSharp":
                    services.AddScoped<ITotpTokenProvider, OtpSharpTotpTokenProvider>();
                    services.AddScoped<ITotpTokenValidator, OtpSharpTotpTokenValidator>();
                    services.AddScoped<IUserTwoFactorTokenProvider, OtpSharpUserTwoFactorTokenProvider>();
                    break;
                default:
                    throw new InvalidOperationException($"Invalid or missing ioc:totpType configuration value: '{totpType}'");
            }
        }
    }
}
