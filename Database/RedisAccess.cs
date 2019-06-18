using StackExchange.Redis;



namespace JWTCommonLibForDotNetCore.Database
{
    public class RedisAccess
    {
        private static RedisAccess _instance;
        private RedisAccess() { }
        public static RedisAccess Instance { get { return _instance; } }


        private ConnectionMultiplexer _redis;
        private int _tokenExpiredTimeInSecounds; 
        public static void Startup(string connectionString, string tokenExpiredTimeInSecounds)
        {
            _instance = new RedisAccess();
            _instance._redis = ConnectionMultiplexer.Connect(connectionString);
            _instance._tokenExpiredTimeInSecounds = int.Parse(tokenExpiredTimeInSecounds);
        }


        public bool TokenExists(string token){
            return _redis.GetDatabase().KeyExists(token);
        }

        public void AddToken(string token){
            _redis.GetDatabase().StringSet(token,token,new System.TimeSpan(0,0,_tokenExpiredTimeInSecounds));
        }

    }

}