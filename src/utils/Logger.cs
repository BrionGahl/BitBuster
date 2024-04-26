using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace BitBuster.utils;

public static class Logger
{
    public static readonly Serilog.Core.Logger Log = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo.Console(theme: AnsiConsoleTheme.Literate, outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u4}] - {Message:lj}{NewLine}{Exception}")
        .CreateLogger();
}