using JWTCommonLibForDotNetCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace JWTCommonLibForDotNetCore.Database
{
    public class IdentityContext : DbContext
    {
        public IdentityContext(DbContextOptions options): base(options)
        {
        }
 
        public DbSet<Identity> Identities { get; set; }
    }

}