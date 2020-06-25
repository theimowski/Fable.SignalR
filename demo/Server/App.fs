namespace SignalRApp    

module App =
    open Fable.SignalR
    open Giraffe.ResponseWriters
    open Saturn
    open System

    [<EntryPoint>]
    let main args =
        try
            let app =
                application {
                    use_signalr (
                        configure_signalr {
                            endpoint Endpoints.Root
                            update SignalRHub.update
                            stream_from SignalRHub.Stream.sendToClient
                            stream_to SignalRHub.Stream.getFromClient
                        }
                    )
                    error_handler (fun e log -> text e.Message)
                    url (sprintf "http://0.0.0.0:%i/" <| Env.getPortsOrDefault 8085us)
                    use_router Router.appRouter
                    use_static (Env.clientPath args)
                    use_developer_exceptions
                }
            printfn "Working directory - %s" (System.IO.Directory.GetCurrentDirectory())
            run app
            0 // return an integer exit code
        with e ->
            let color = Console.ForegroundColor
            Console.ForegroundColor <- System.ConsoleColor.Red
            Console.WriteLine(e.Message)
            Console.ForegroundColor <- color
            1 // return an integer exit code