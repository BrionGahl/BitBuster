using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace BitBuster.utils;

public static class Logger
{
    public static readonly Serilog.Core.Logger Log = new LoggerConfiguration()
        .MinimumLevel.Information()
        .WriteTo.Console(theme: AnsiConsoleTheme.Literate, applyThemeToRedirectedOutput: true, outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u4}] - {Message:lj}{NewLine}{Exception}")
        // .WriteTo.Godot(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u4}] - {Message:lj}{NewLine}{Exception}")
        .CreateLogger();
}