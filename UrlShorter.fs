module UrlShorter

open System
open Giraffe
open StackExchange.Redis
open HashIdentity

[<CLIMutable>]
type UrlShorterType = {
    url: string
}


[<CLIMutable>]
type UrlResult = {
    hashid: string
}

[<CLIMutable>]
type UrlResult2 = {
    urlShorter: string
}


[<CLIMutable>]
type HashIdParam = {
    HashId: string
}