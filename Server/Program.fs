open System
open System.IO
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.SignalR
open Microsoft.Extensions.DependencyInjection

type Message =
    { userName: string
      content: string
      time: string }

type MyHub() =
    inherit Hub()

    member __.PostMessage(message: Message) : unit =
        __.Clients.All.SendAsync("receiveMessage", message)
        |> ignore

let configureApp (app: IApplicationBuilder) =
    app
        .UseDefaultFiles()
        .UseStaticFiles()
        .UseRouting()
        .UseEndpoints(fun endpoints -> endpoints.MapHub<MyHub>("/myhub") |> ignore)
    |> ignore

let configureServices (services: IServiceCollection) : unit = services.AddSignalR() |> ignore

[<EntryPoint>]
let main _ =
    let publicPath = Path.GetFullPath "./public"

    WebHostBuilder()
        .UseKestrel()
        .UseWebRoot(publicPath)
        .UseContentRoot(publicPath)
        .Configure(Action<IApplicationBuilder> configureApp)
        .ConfigureServices(configureServices)
        .Build()
        .Run()

    0 // return an integer exit code
