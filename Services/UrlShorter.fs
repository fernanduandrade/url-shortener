module UrlShorter

open Hash

[<CLIMutable>]
type UrlShorterType = {
    url: string
}

type UrlResult = {
    hashid: string
}

type UrlService() =
    member this.SaveToRedis (newUrl: UrlShorterType) =
        let id = Hash.GenerateNewHash()
        //db.StringSet(id, newUrl.url) |> ignore
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