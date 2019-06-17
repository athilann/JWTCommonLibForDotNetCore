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

namespace JWTCommonLibForDotNetCore.Services
{
    public interface IIdentityService
    {
        Identity Authenticate(string username, string password);
        IEnumerable<Identity> GetAll();
        IEnumerable<Identity> GetUsers();
    }

    public class IdentityService : IIdentityService
    {
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        private List<Identity> _identities = new List<Identity>
        {
            new Identity { Id = 1, Username = "test",  Password = "test", Role= "Admin" },
            new Identity { Id = 2, Username = "test2", Password = "test" },
            new Identity { Id = 3, Username = "test3", Password = "test" },
            new Identity { Id = 4, Username = "test4", Password = "test", Role= "Admin" },
            new Identity { Id = 5, Username = "test5", Password = "test" },
            new Identity { Id = 6, Username = "test6", Password = "test" }
        };

        private readonly JWTSettings _jwtSettings;

        public IdentityService(IOptions<JWTSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public Identity Authenticate(string username, string password)
        {
            var identity = _identities.SingleOrDefault(x => x.Username == username && x.Password == password);

            // return null if user not found
            if (identity == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, identity.Id.ToString()),
                    new Claim(ClaimTypes.Role, identity.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            identity.Token = tokenHandler.WriteToken(token);

            // remove password before returning
            identity.Password = null;

            return identity;
        }

        public IEnumerable<Identity> GetAll()
        {
            // return users without passwords
            return _identities.Select(x =>
            {
                x.Password = null;
                return x;
            });
        }
        public IEnumerable<Identity> GetUsers()
        {
            // return users without passwords
            return _identities.Where(s => s.Role == "User").ToList().Select(x =>
            {
                x.Password = null;
                return x;
            });
        }
    }
}