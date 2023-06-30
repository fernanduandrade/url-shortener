
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.Hosting

open Giraffe
open Blog
open UrlShorter
open StackExchange.Redis
open Microsoft.AspNetCore.Http
open Hashids

module Program =
    let redisConfiguration = ConfigurationOptions()
    redisConfiguration.EndPoints.Add("localhost", 6379)

    let redisConnection = ConnectionMultiplexer.Connect(redisConfiguration)
    let db = redisConnection.GetDatabase()
    let value = db.StringGet "fernando"

    type UrlDb() =
        member this.SaveToRedis (newUrl: UrlShorterType) =
            let config =
                HashidConfiguration.create 
                    { HashidConfiguration.defaultOptions with Salt = System.Guid.NewGuid().ToString() }
            
            let encode = Hashid.encode64 config
            let id = encode [| 73L; 88L |]
            let randomStr = 
                let chars = "ABCDEFGHIJKLMNOPQRSTUVWUXYZ0123456789"
                let charsLen = chars.Length
                let random = System.Random()

                fun len -> 
                    let randomChars = [|for i in 0..len -> chars.[random.Next(charsLen)]|]
                    new System.String(randomChars)

            let a = randomStr
            printf "teste %A" a
            db.StringSet(id, newUrl.url) |> ignore
            let result = {
                hashid = id
            }
            result

        member this.RedirectToSite () =
            fun (next: HttpFunc) (ctx: HttpContext) ->
                let urlFromRedis = db.StringGet("baZhqO")
                (string)urlFromRedis
                

    type UrLServiceTree = {
        getUrlDb: unit -> UrlDb
    }

    let saveUrlHttpHandler (serivceTree: UrLServiceTree) =
        fun(next: HttpFunc) (ctx: HttpContext) ->
            task {
                let! url = ctx.BindJsonAsync<UrlShorterType>()
                let result = serivceTree.getUrlDb().SaveToRedis(url)
                let test = {
                    urlShorter = $"http://localhost:5218/go/{result.hashid}"
                }
                return! json (test) next ctx
            }

    let redirectUrlHttpHandler (serivceTree: UrLServiceTree) =
        fun(next: HttpFunc) (ctx: HttpContext) ->
            task {
                let hashId = ctx.Request.RouteValues["hashId"]
                let urlFromRedis = db.StringGet("baZhqO")
                ctx.Response.Redirect((string)urlFromRedis)
                return! next ctx
            }

    let handleGetRoute (hashIdParam: HashIdParam) : HttpHandler =
        let value =
            let strValue = db.StringGet(hashIdParam.HashId)
            (string) strValue 
        fun (next: HttpFunc) (ctx : HttpContext) -> 
            ctx.Response.Redirect(value)
            next ctx


    let webApp = 
        let blogDb = new BlogDb()

        let serviceTree = {
            getBlogDb = fun () -> blogDb
        }

        let urlDb = new UrlDb()

        let serviceUrlTree = {
            getUrlDb = fun () -> urlDb
        }
        
        choose[
            route "/" >=>  text "iamanapi"
            routeBind<HashIdParam> "/go/{hashId}" handleGetRoute
            
            subRoute "/posts"
                (
                    choose [
                        route "/save" 
                            >=> POST
                            >=> warbler (fun _->
                                (saveUrlHttpHandler serviceUrlTree))
                        route "" >=> GET >=> warbler (fun _ -> 
                            (getPostsHttpHandler serviceTree))
                        route "/create"
                            >=> POST
                            >=> warbler (fun _ -> 
                                (createPostHttpHandler serviceTree))
                    ]
                )
        ]

    let configureApp (app: IApplicationBuilder) =
        app.UseGiraffe (webApp)

    let configureServices (services: IServiceCollection) =
        services.AddGiraffe() |> ignore
    let exitCode = 0

    [<EntryPoint>]
    let main _ =
        Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(
            fun webHostBuilder ->
                webHostBuilder
                    .Configure(configureApp)
                    .ConfigureServices(configureServices)
                    |> ignore)
            .Build()
            .Run()

        exitCode
