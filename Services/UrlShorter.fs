module UrlShorter

open Hash
open StackExchange.Redis
open Microsoft.AspNetCore.Http
open Giraffe

[<CLIMutable>]
type UrlShorterType = {
    url: string
}

type UrlResult = {
    hashid: string
}

type UrlService(db: IDatabase) =
    member this.SaveToRedis (newUrl: UrlShorterType) =
        let id = generateNewHash()
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

let saveUrlHttpHandler (serivceTree: UrLServiceTree) =
        fun(next: HttpFunc) (ctx: HttpContext) ->
            task {
                let! url = ctx.BindJsonAsync<UrlShorterType>()
                let result = serivceTree.getUrlService().SaveToRedis(url)
                let result = {
                    urlShorter = $"http://localhost:5218/go/{result.hashid}"
                }
                return! json (result) next ctx
            }

let handleGetRoute (hashIdParam: HashIdParam) : HttpHandler =
        let value =
            let strValue = "aaa"//db.StringGet(hashIdParam.HashId)
            (string) strValue 
        fun (next: HttpFunc) (ctx : HttpContext) -> 
            ctx.Response.Redirect(value)
            next ctx