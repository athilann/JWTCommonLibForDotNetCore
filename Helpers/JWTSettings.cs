namespace JWTCommonLibForDotNetCore.Helpers
{
    public class JWTSettings
    {
        public string Secret { get; set; }
        public string RedisConnectionString { get; set; }
    }
}