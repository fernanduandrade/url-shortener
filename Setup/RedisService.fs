module Redis

open StackExchange.Redis

let configureRedis () =
    let redisConfiguration = ConfigurationOptions()
    redisConfiguration.EndPoints.Add("localhost", 6379)
    
    let redisConnection = ConnectionMultiplexer.Connect(redisConfiguration)
    let db = redisConnection.GetDatabase()
    db