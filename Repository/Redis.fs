namespace Redis
open StackExchange.Redis

module Redis =
    let redisConnection = ConfigurationOptions()
    redisConnection.EndPoints.Add("localhost", 6379)
        
    let redis = ConnectionMultiplexer.Connect redisConnection

    let redisDb = redis.GetDatabase()

    let CreateCache (hashKey: string, value: string): unit =
        redisDb.StringSet(hashKey, value) |> ignore

    let GetValueByKey (key: string) =
        let value = redisDb.StringGet(key)
        (string) value
