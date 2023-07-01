open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.Hosting

open Giraffe
open UrlShorter
open StackExchange.Redis
open Microsoft.AspNetCore.Http

module Program =
    let redisConfiguration = ConfigurationOptions()
    redisConfiguration.EndPoints.Add("localhost", 6379)

    let redisConnection = ConnectionMultiplexer.Connect(redisConfiguration)
    let db = redisConnection.GetDatabase()

                   

    

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
            let strValue = db.StringGet(hashIdParam.HashId)
            (string) strValue 
        fun (next: HttpFunc) (ctx : HttpContext) -> 
            ctx.Response.Redirect(value)
            next ctx


    let webApp =

        let urlDb = new UrlService()

        let serviceUrlTree = {
            getUrlService = fun () -> urlDb
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
