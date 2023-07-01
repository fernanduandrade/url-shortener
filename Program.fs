open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.Hosting

open Giraffe
open UrlShorter
open Redis
open StackExchange.Redis

module Program =

    let webApp =

        let db = configureRedis()
        let urlDb = new UrlService(db)

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
        let db = configureRedis()
        services.AddSingleton<IDatabase>(db) |> ignore
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
