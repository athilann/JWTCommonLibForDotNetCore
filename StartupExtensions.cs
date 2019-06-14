using System;
using System.Text;
using JWTCommonLibForDotNetCore.Helpers;
using JWTCommonLibForDotNetCore.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using JWTCommonLibForDotNetCore.Controllers;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System.Reflection;

namespace JWTCommonLibForDotNetCore
{
    public static class StartupExtensions
    {
        public static void AddJwt(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddMvc().AddApplicationPart(Assembly.Load(new AssemblyName("JWTCommonLibForDotNetCore"))); ;

            var appSettingsSection = Configuration.GetSection("JWTSettings");
            services.Configure<JWTSettings>(appSettingsSection);

            var appSettings = appSettingsSection.Get<JWTSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

        }

    }
}
