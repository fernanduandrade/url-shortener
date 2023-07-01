open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.Hosting

open Giraffe
open UrlShorter
open StackExchange.Redis
open Microsoft.AspNetCore.Http
open Hash

module Program =
    let redisConfiguration = ConfigurationOptions()
    redisConfiguration.EndPoints.Add("localhost", 6379)

    let redisConnection = ConnectionMultiplexer.Connect(redisConfiguration)
    let db = redisConnection.GetDatabase()

    type UrlDb() =
        member this.SaveToRedis (newUrl: UrlShorterType) =

            let id = Hash.GenerateNewHash()
            db.StringSet(id, newUrl.url) |> ignore
            let result = {
                hashid = id
            }
            result                

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

    let handleGetRoute (hashIdParam: HashIdParam) : HttpHandler =
        let value =
            let strValue = db.StringGet(hashIdParam.HashId)
            (string) strValue 
        fun (next: HttpFunc) (ctx : HttpContext) -> 
            ctx.Response.Redirect(value)
            next ctx


    let webApp =

        let urlDb = new UrlDb()

        let serviceUrlTree = {
            getUrlDb = fun () -> urlDb
        }
        
        choose[
            routeBind<HashIdParam> "/go/{hashId}" handleGetRoute
            route "/create" 
                >=> POST
                >=> warbler (fun _->
                    (saveUrlHttpHandler serviceUrlTree))
        ]

    let configureApp (app: IApplicationBuilder) =
        app.UseGiraffe (webApp)

    let configureServices (services: IServiceCollection) =
        services.AddSingleton
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
