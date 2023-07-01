module Redis

open StackExchange.Redis

type RedisRepository() =
    let redisConnection = ConfigurationOptions()
    do redisConnection.EndPoints.Add("localhost", 6379)
        
    let redis = ConnectionMultiplexer.Connect(redisConnection)
    let redisDb = redis.GetDatabase()

    member _.Create (hashKey: string, value: string): unit =
        redisDb.StringSet(hashKey, value) |> ignore

    member _.GetValueByKey (key: string) =
        let value = redisDb.StringGet(key)
        (string) value