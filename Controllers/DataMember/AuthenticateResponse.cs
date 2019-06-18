using System.Collections.Generic;
using JWTCommonLibForDotNetCore.Entities;

namespace JWTCommonLibForDotNetCore.Controllers.DataMember
{
    public class AuthenticateResponse
    {
        public string Username { get; set; }
        public List<string> Roles { get; set; }
        public string Token { get; set; }

        public AuthenticateResponse(Identity identity)
        {
            Roles = new List<string>();
            Username = identity.Username;
            Token = identity.Token;
            foreach(var role in identity.Roles){
                Roles.Add(role.Role.Name);
            }
        }
    }
}