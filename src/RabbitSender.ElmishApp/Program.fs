module RabbitSender.ElmishApp.Program


open System.IO
open Serilog
open Serilog.Extensions.Logging
open Elmish.WPF
open RabbitSender.ElmishApp
open RabbitSender.ElmishApp.Models
open RabbitSender.ElmishApp.MainModel

open Microsoft.Extensions.Logging

let main (window, errorQueue, settingsManager) =
    let logger =
        LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Override("Elmish.WPF.Update", Events.LogEventLevel.Verbose)
            .MinimumLevel.Override("Elmish.WPF.Bindings", Events.LogEventLevel.Verbose)
            .MinimumLevel.Override("Elmish.WPF.Performance", Events.LogEventLevel.Verbose)
            .MinimumLevel.Override("RabbitSender.ElmishApp", Events.LogEventLevel.Verbose)
            .WriteTo.Debug(outputTemplate="[{Timestamp:HH:mm:ss:fff} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.Console(outputTemplate="[{Timestamp:HH:mm:ss:fff} {Level:u3}] {Message:lj}{NewLine}{Exception}")
#else
            .MinimumLevel.Override("Elmish.WPF.Update", Events.LogEventLevel.Error)
            .MinimumLevel.Override("Elmish.WPF.Bindings", Events.LogEventLevel.Error)
            .MinimumLevel.Override("Elmish.WPF.Performance", Events.LogEventLevel.Error)
            .MinimumLevel.Override("LogParser.App", Events.LogEventLevel.Error)
            .WriteTo.Seq("http://localhost:5341")
#endif
            .CreateLogger()

    let loggerFactory = new SerilogLoggerFactory(logger)
    //let store = Infrastruture.FsStatementInMemoryStore.store

    let mainModelLogger : ILogger = loggerFactory.CreateLogger("LogParser.App.MainModel.MainModel")

    mainModelLogger.LogDebug("debug")
    mainModelLogger.LogInformation("info")

    WpfProgram.mkProgram (MainModel.init errorQueue settingsManager) Program.update MainModel.Bindings.bindings
    |> WpfProgram.withLogger loggerFactory
    |> WpfProgram.startElmishLoop window
