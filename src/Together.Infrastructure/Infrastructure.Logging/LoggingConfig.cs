namespace Infrastructure.Logging;

public sealed class LoggingConfig
{
    public string LogToFilePath { get; set; } = default!;

    public string LogToSeqUrl { get; set; } = default!;
}