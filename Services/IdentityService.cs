using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using JWTCommonLibForDotNetCore.Entities;
using JWTCommonLibForDotNetCore.Helpers;
using JWTCommonLibForDotNetCore.Database;

namespace JWTCommonLibForDotNetCore.Services
{
    public interface IIdentityService
    {
        Identity Authenticate(string username, string password);
        void RevokeToken(string token);
    }

    public class IdentityService : IIdentityService
    {
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        private List<Identity> _identities = new List<Identity>
        {
            new Identity { Id = Guid.NewGuid(), Username = "test",  Password = "test", Roles = new List<Role>(){new Role(){Id= 1,Name = "Admin"}}},
            new Identity { Id = Guid.NewGuid(), Username = "test2", Password = "test", Roles = new List<Role>(){new Role(){Id= 3,Name = "User"}, new Role(){Id= 2,Name = "Bidder"}}},
            new Identity { Id = Guid.NewGuid(), Username = "test3", Password = "test", Roles = new List<Role>(){new Role(){Id= 1,Name = "Admin"}, new Role(){Id= 2,Name = "Bidder"}}},
            new Identity { Id = Guid.NewGuid(), Username = "test4", Password = "test", Roles = new List<Role>(){new Role(){Id= 1,Name = "Admin"}}},
            new Identity { Id = Guid.NewGuid(), Username = "test5", Password = "test" },
            new Identity { Id = Guid.NewGuid(), Username = "test6", Password = "test" }
        };

        private readonly IDataRepository<Identity> _dataRepository;
        private readonly JWTSettings _jwtSettings;

        public IdentityService(IOptions<JWTSettings> jwtSettings, IDataRepository<Identity> dataRepository)
        {
            _jwtSettings = jwtSettings.Value;
            _dataRepository = dataRepository;
        }

        public Identity Authenticate(string username, string password)
        {
            var identityData = _dataRepository.Get(x => x.Username == username && x.Password == password, x => new { x.Id, x.Username, x.Roles }).FirstOrDefault();

            // return null if user not found
            if (identityData == null)
                return null;

            var identity = new Identity() { Id = identityData.Id, Username = identityData.Username, Roles = identityData.Roles };

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, identity.Id.ToString()),
                    //new Claim(ClaimTypes.Role, identity.Role),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            foreach (var role in identity.Roles)
            {
                tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, role.Name));
            }

            var token = tokenHandler.CreateToken(tokenDescriptor);

            identity.Token = tokenHandler.WriteToken(token);

            // remove password before returning
            //identity.Password = null;

            return identity;
        }

        public void RevokeToken(string token)
        {
            RedisAccess.Instance.AddToken(token);
        }


    }
}