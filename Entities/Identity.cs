using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;
using JWTCommonLibForDotNetCore.Helpers.Hashers;

namespace JWTCommonLibForDotNetCore.Entities
{
    [Table("Identities")]
    public class Identity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Part1 { get; set; }
        [Required]
        public string Part2 { get; set; }
        [Required]
        public string Part3 { get; set; }
        [Required]
        public bool NeedsUpgrade { get; set; }
        public List<IdentityRole> Roles { get; set; }


        [NotMapped]
        public virtual string Token { get; set; }
        public Identity()
        {
            Roles = new List<IdentityRole>();
        }

        internal bool CheckPassword(IPasswordHasher passwordHasher, string password)
        {
            var check = passwordHasher.Check(new string[] { Part1, Part2, Part3 }, password);
            NeedsUpgrade = check.NeedsUpgrade;
            return check.Verified;
        }
    }

    [Table("Roles")]
    public class Role
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public List<IdentityRole> Identities { get; set; }

        public Role()
        {
            Identities = new List<IdentityRole>();
        }
    }


    public class IdentityRole
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Guid IdentityId { get; set; }
        public Identity Identity { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}