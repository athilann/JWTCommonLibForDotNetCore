namespace JWTCommonLibForDotNetCore.Entities
{
    public class Identity
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }

        public Identity(){
            Role = "User";
        }
    }
}