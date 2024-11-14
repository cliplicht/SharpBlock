using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging.Abstractions;
using SharpBlock.Core.Options;

namespace SharpBlock.Core.Utils;

public class FancyLogFormatter : ConsoleFormatter, IDisposable
{
    private readonly IDisposable _optionsReloadToken;
    private FancyLogFormatterOptions _formatterOptions;

    public FancyLogFormatter(IOptionsMonitor<FancyLogFormatterOptions> options)
        : base("fancy")
    {
        _formatterOptions = options.CurrentValue;
        _optionsReloadToken = options.OnChange(ReloadLoggerOptions);
    }

    private void ReloadLoggerOptions(FancyLogFormatterOptions options)
    {
        _formatterOptions = options;
    }

    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
    {
        // Fetch log level color and uppercase the level name
        var logLevelColor = GetLogLevelColor(logEntry.LogLevel);
        var logLevel = logEntry.LogLevel.ToString().ToUpper();

        // Structure the output to include [SharpBlock] and consistent formatting
        textWriter.Write($"[SharpBlock] {logLevelColor}[{logLevel}]{_formatterOptions.ResetColor} ");
        textWriter.Write($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] : ");

        // Print the message
        textWriter.Write($"{_formatterOptions.MessageColor}{logEntry.Formatter(logEntry.State, logEntry.Exception)}{_formatterOptions.ResetColor}");

        // Append any additional structured data if available
        if (logEntry.State is IEnumerable<KeyValuePair<string, object>> properties)
        {
            foreach (var property in properties)
            {
                if (property.Key != "{OriginalFormat}" && property.Value is not null)
                {
                    textWriter.Write($" | {_formatterOptions.PropertyColor}{property.Key}{_formatterOptions.ResetColor}={property.Value}");
                }
            }
        }

        // Print exception details if available
        if (logEntry.Exception != null)
        {
            textWriter.WriteLine($"{_formatterOptions.ExceptionColor}{logEntry.Exception}{_formatterOptions.ResetColor}");
        }
        else
        {
            textWriter.WriteLine();
        }
    }

    private string GetLogLevelColor(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Information => "\x1b[32m", // Green
            LogLevel.Warning => "\x1b[33m", // Yellow
            LogLevel.Error => "\x1b[31m", // Red
            LogLevel.Critical => "\x1b[35m", // Magenta
            LogLevel.Debug => "\x1b[34m", // Blue
            _ => "\x1b[37m" // White
        };
    }

    public void Dispose() => _optionsReloadToken.Dispose();
}