open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.Hosting
open Redis
open Giraffe

module Program =

    let webApp =
        choose [
            
        ]
        // choose[
        //     routeBind<UrlShortParam> "/go/{hashId}" handleGetRoute
        //     route "/create" 
        //         >=> POST
        //         >=> warbler (fun _->
        //             (saveUrlHttpHandler serviceUrlTree))
        // ]

    let configureApp (app: IApplicationBuilder) =
        app.UseGiraffe (webApp)

    let configureServices (services: IServiceCollection) =
        services.AddSingleton<RedisRepository>(RedisRepository()) |> ignore
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
