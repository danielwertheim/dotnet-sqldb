module SqlDb.Logging

open Serilog
open Serilog.Core
open Serilog.Events

type LoggingContexts =
| DbDropCommand
| DbEnsureCommand
| DbUpCommand

module LoggingContext =
    let asString (ctx: LoggingContexts) =
        ctx.ToString()

let private logLevelSwitch = new LoggingLevelSwitch(LogEventLevel.Information)

let overrideLogLevelWith minimumLevel =
    logLevelSwitch.MinimumLevel <- minimumLevel

let logger =
    Log.Logger <- (new LoggerConfiguration())
        .MinimumLevel.ControlledBy(logLevelSwitch)
        .WriteTo.Console(outputTemplate="{Timestamp:HH:mm:ss} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
        .Enrich.FromLogContext()
        .CreateLogger() :> ILogger

    Log.Logger

let loggerFor (context:LoggingContexts) =
    logger.ForContext(Constants.SourceContextPropertyName, LoggingContext.asString context)

