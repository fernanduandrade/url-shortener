module Handlers

open Microsoft.AspNetCore.Http
open UrlShorter
open Giraffe
open Redis
open Hash
open Redis

let handleCreateHashUrl =
    fun(next: HttpFunc) (ctx: HttpContext) ->
        task {
            let! url = ctx.BindJsonAsync<UrlShorterType>()
            let hashId = generateNewHash()
            CreateCache (hashId, url.url)
            let result = {
                urlShorter = $"http://localhost:5218/go/{hashId}"
            }
            return! json (result) next ctx
        }

let handleRedirectToUrl (hashParam: string) =
        fun (next: HttpFunc) (ctx : HttpContext) ->
            let urlValue = GetValueByKey hashParam
            ctx.Response.Redirect((string)urlValue)
            next ctx