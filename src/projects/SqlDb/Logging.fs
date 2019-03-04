module SqlDb.Logging

open Serilog
open Serilog.Core

type LoggingContexts =
| DbDropCommand
| DbEnsureCommand
| DbUpCommand

module LoggingContext =
    let asString (ctx: LoggingContexts) =
        ctx.ToString()

let logger = (
    Log.Logger <- (new LoggerConfiguration())
        .MinimumLevel
        .Information()
        .WriteTo.Console(outputTemplate="{Timestamp:HH:mm:ss} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
        .Enrich.FromLogContext()
        .CreateLogger() :> ILogger

    Log.Logger
)

let loggerFor (context:LoggingContexts) =
    logger.ForContext(Constants.SourceContextPropertyName, LoggingContext.asString context)

