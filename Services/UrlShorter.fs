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

type UrlShortParam = {
    HashId: string
}

type UrlShoterResponse = {
    urlShorter: string
}