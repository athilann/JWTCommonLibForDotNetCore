using StackExchange.Redis;



namespace JWTCommonLibForDotNetCore.Services
{
    public class RedisAccess
    {
        private static RedisAccess _instance;
        private RedisAccess() { }
        public static RedisAccess Instance { get { return _instance; } }


        private ConnectionMultiplexer _redis;
        public static void Startup(string connectionString)
        {
            _instance = new RedisAccess();
            _instance._redis = ConnectionMultiplexer.Connect(connectionString);
        }


        public bool TokenExists(string token){
            return _redis.GetDatabase().KeyExists(token);
        }

        public void AddToken(string token){
            _redis.GetDatabase().SetAdd(token,token);
        }

    }

}