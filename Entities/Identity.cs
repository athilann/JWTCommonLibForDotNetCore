using System.Collections.Generic;

namespace JWTCommonLibForDotNetCore.Entities
{
    public class Identity
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<Role> Roles { get; set; }
        public string Token { get; set; }

        public Identity(){
            Roles = new List<Role>();
        }
    }

    public class Role{
        public int Id {get;set;}
        public string Name{get;set;}
    }
}