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
using System.Threading.Tasks;
using System.Diagnostics;
using JWTCommonLibForDotNetCore.Database;
using Microsoft.EntityFrameworkCore;
using JWTCommonLibForDotNetCore.Entities;

namespace JWTCommonLibForDotNetCore
{
    public static class StartupExtensions
    {
        private static bool UseRedis = false;
        public static void AddJwt(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IDataRepository<Identity>, IdentityManager>();
            services.AddDbContext<IdentityContext>(opts => opts.UseSqlServer(Configuration["ConnectionString:IdentityDB"]));
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
                x.Events = new JwtBearerEvents
                {
                    OnTokenValidated = OnTokenValidated
                };
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

            if (appSettings.RedisConnectionString != null && appSettings.RedisConnectionString != string.Empty)
            {
                RedisAccess.Startup(appSettings.RedisConnectionString);
                UseRedis = true;
            }

        }

        private static Task OnTokenValidated(TokenValidatedContext context)
        {
            if (UseRedis && RedisAccess.Instance.TokenExists(context.Request.Headers["Authorization"].ToString()))
            {
                context.Fail("invalid_token");
            }
            return Task.FromResult(0);
        }

    }
}
