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
using Microsoft.AspNetCore.Identity;
using JWTCommonLibForDotNetCore.Helpers.Hashers;

namespace JWTCommonLibForDotNetCore.Services
{
    public interface IIdentityService
    {
        Identity Authenticate(string username, string password);
        void RevokeToken(string token);
    }

    public class IdentityService : IIdentityService
    {

        private readonly IDataRepository<Identity> _dataRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly JWTSettings _jwtSettings;

        public IdentityService(IOptions<JWTSettings> jwtSettings, IDataRepository<Identity> dataRepository, IPasswordHasher passwordHasher)
        {
            _jwtSettings = jwtSettings.Value;
            _dataRepository = dataRepository;
            _passwordHasher = passwordHasher;
        }

        public Identity Authenticate(string username, string password)
        {
            var identity = _dataRepository.Get(x => x.Username == username, x => new Identity
            {
                Id = x.Id,
                Username = x.Username,
                Part1 = x.Part1,
                Part2 = x.Part2,
                Part3 = x.Part3
            }).FirstOrDefault();

            // return null if user not found
            if (identity == null) return null;
            if (!identity.CheckPassword(_passwordHasher, password)) return null;
            
            identity = _dataRepository.Get(identity.Id);


            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, identity.Id.ToString()),
                    new Claim(ClaimTypes.Name, identity.Username),
                }),
                Expires = DateTime.UtcNow.AddSeconds(int.Parse(_jwtSettings.TokenExpiredTimeInSecounds)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            foreach (var role in identity.Roles)
            {
                tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, role.Role.Name));
            }

            var token = tokenHandler.CreateToken(tokenDescriptor);

            identity.Token = tokenHandler.WriteToken(token);

            return identity;
        }

        public void RevokeToken(string token)
        {
            RedisAccess.Instance.AddToken(token);
        }


    }
}