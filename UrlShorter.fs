module UrlShorter


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