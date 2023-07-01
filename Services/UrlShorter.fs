module UrlShorter

open Hash
open StackExchange.Redis

[<CLIMutable>]
type UrlShorterType = {
    url: string
}

type UrlResult = {
    hashid: string
}

type UrlService(db: IDatabase) =
    member this.SaveToRedis (newUrl: UrlShorterType) =
        let id = generateNewHash
        db.StringSet(id, newUrl.url) |> ignore
        let result = {
            hashid = id
        }
        result 

type UrLServiceTree = {
    getUrlService: unit -> UrlService
}

[<CLIMutable>]
type HashIdParam = {
    HashId: string
}

type UrlShoterResponse = {
    urlShorter: string
}