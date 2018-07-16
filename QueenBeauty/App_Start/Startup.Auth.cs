﻿// Licensed to the Swastika I/O Foundation under one or more agreements.
// The Swastika I/O Foundation licenses this file to you under the GNU General Public License v3.0 license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Swastika.Cms.Lib;
using Swastika.Cms.Lib.Models.Account;
using Swastika.Identity.Data;
using Swastika.Identity.Models;
using System;
using System.Text;
using System.Threading.Tasks;

namespace QueenBeauty
{
    public partial class Startup
    {
        protected void ConfigIdentity(IServiceCollection services, IConfiguration Configuration, string connectionName)
        {
            services.AddDbContext<ApplicationDbContext>();

            PasswordOptions pOpt = new PasswordOptions()
            {
                RequireDigit = false,
                RequiredLength = 6,
                RequireLowercase = false,
                RequireNonAlphanumeric = false,
                RequireUppercase = false
            };

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password = pOpt;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddUserManager<UserManager<ApplicationUser>>()
                ;
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AddEditUser", policy =>
                {
                    policy.RequireClaim("Add User");
                    policy.RequireClaim("Edit User");
                });
                options.AddPolicy("DeleteUser", policy => policy.RequireClaim("Delete User"));
            })
             ;
        }

        protected void ConfigJWTToken(IServiceCollection services, IConfiguration Configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters =
                             new TokenValidationParameters
                             {
                                 ValidateIssuer = false,
                                 ValidateAudience = false,
                                 ValidateLifetime = true,
                                 ValidateIssuerSigningKey = true,

                                 ValidIssuer = SWCmsConstants.JwtSettings.ISSUER,
                                 ValidAudience = SWCmsConstants.JwtSettings.AUDIENCE,
                                 IssuerSigningKey = JwtSecurityKey.Create(SWCmsConstants.JwtSettings.SECRET_KEY)
                             };
                        options.Events = new JwtBearerEvents
                        {
                            OnAuthenticationFailed = context =>
                            {
                                Console.WriteLine("OnAuthenticationFailed: " + context.Exception.Message);
                                return Task.CompletedTask;
                            },
                            OnTokenValidated = context =>
                            {
                                Console.WriteLine("OnTokenValidated: " + context.SecurityToken);
                                return Task.CompletedTask;
                            }
                        };
                    });
        }

        protected void ConfigCookieAuth(IServiceCollection services, IConfiguration Configuration)
        {
            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.Cookie.Expiration = TimeSpan.FromDays(150);
                options.LoginPath = "/" + CONST_ROUTE_DEFAULT_CULTURE + "/Portal/Auth/Login"; // If the LoginPath is not set here, ASP.NET Core will default to /Account/Login
                options.LogoutPath = "/" + CONST_ROUTE_DEFAULT_CULTURE + "/Portal/Auth/Logout"; // If the LogoutPath is not set here, ASP.NET Core will default to /Account/Logout
                options.AccessDeniedPath = "/"; // If the AccessDeniedPath is not set here, ASP.NET Core will default to /Account/AccessDenied
                options.SlidingExpiration = true;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(
                options =>
                {
                    // Cookie settings
                    options.Cookie.HttpOnly = true;
                    options.Cookie.Expiration = TimeSpan.FromDays(150);
                    options.LoginPath = "/" + CONST_ROUTE_DEFAULT_CULTURE + "/Portal/Auth/Login"; // If the LoginPath is not set here, ASP.NET Core will default to /Account/Login
                    options.LogoutPath = "/" + CONST_ROUTE_DEFAULT_CULTURE + "/Portal/Auth/Logout"; // If the LogoutPath is not set here, ASP.NET Core will default to /Account/Logout
                    options.AccessDeniedPath = "/"; // If the AccessDeniedPath is not set here, ASP.NET Core will default to /Account/AccessDenied
                    options.SlidingExpiration = true;
                });
        }

        protected static class JwtSecurityKey
        {
            public static SymmetricSecurityKey Create(string secret)
            {
                return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
            }
        }
    }
}
