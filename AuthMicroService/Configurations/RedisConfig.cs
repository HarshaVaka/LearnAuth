using StackExchange.Redis;

namespace AuthMicroService.Configurations
{
    public static class RedisConfig
    {
        public static IServiceCollection AddRedisConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConnectionString = configuration.GetConnectionString("Redis") ?? "localhost:6379";

            // Create singleton for Redis
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                return ConnectionMultiplexer.Connect(redisConnectionString);
            });

            return services;
        }
    }

}
