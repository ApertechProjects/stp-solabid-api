using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SolaBid.Business.ActionFilters;
using SolaBid.Business.Models;
using SolaBid.Domain.Models.AppDbContext;
using SolaBid.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using SolaBid.Domain.Models;

namespace SolaBid.Business.Middlewares
{
    public static class AppMidleware
    {
        public static void ConfigureMyServices(this IServiceCollection services, IConfiguration Configuration)
        {
            var connectionString = Statics.ConnectionString();
            services.AddDbContext<SBDbContext>(options => options.UseSqlServer(connectionString));
            services.AddIdentity<AppUser, AppRole>()
                .AddEntityFrameworkStores<SBDbContext>()
                .AddDefaultTokenProviders();
            //services.ConfigureApplicationCookie(options => options.ExpireTimeSpan = TimeSpan.FromMinutes(440));
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;

                //options.Lockout.AllowedForNewUsers = true;
                //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(20);
                //options.Lockout.MaxFailedAccessAttempts = 5;
            });
            services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
            services.AddScoped<ValidateActionFilterWithApiResultModel>();
        }
    }
}