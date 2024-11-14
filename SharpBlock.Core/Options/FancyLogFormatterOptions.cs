using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging.Console;

namespace SharpBlock.Core.Options;

public class FancyLogFormatterOptions : ConsoleFormatterOptions
{
    [JsonInclude] public string TimestampColor { get; set; } = "\x1b[36m"; // Cyan
    [JsonInclude] public string CategoryColor { get; set; } = "\x1b[90m"; // Dark Gray
    [JsonInclude] public string PropertyColor { get; set; } = "\x1b[36m"; // Cyan
    [JsonInclude] public string MessageColor { get; set; } = "\x1b[37m"; // White
    [JsonInclude] public string ExceptionColor { get; set; } = "\x1b[31m"; // Red
    [JsonInclude] public string ResetColor { get; set; } = "\x1b[0m";
    [JsonInclude] public bool IncludeTimestamp { get; set; }

    [JsonInclude] public bool UseColors { get; set; } = true;
}