using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace AWebService
{
    public class CustomLogger : ILogger
    {
        protected readonly CustomLoggerProvider _customLoggerProvider;

        public CustomLogger([NotNull] CustomLoggerProvider customLoggerProvider)
        {
            _customLoggerProvider = customLoggerProvider;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var fullFilePath = _customLoggerProvider.Options.FolderPath + "/" + _customLoggerProvider.Options.FilePath?.Replace("{date}", DateTimeOffset.UtcNow.ToString("yyyyMMdd"));
            var logRecord = string.Format("{0} [{1}] {2} {3}", "[" + DateTimeOffset.UtcNow.ToString("yyyy-MM-dd HH:mm:ss+00:00") + "]", logLevel.ToString(), formatter(state, exception), exception != null ? exception.StackTrace : "");

            using (var streamWriter = new StreamWriter(fullFilePath, true))
            {
                streamWriter.WriteLine(logRecord);
            }
        }
    }

    [ProviderAlias("CustomLogger")]
    public class CustomLoggerProvider : ILoggerProvider
    {
        public readonly CustomLoggerOptions? Options;

        public CustomLoggerProvider(IOptions<CustomLoggerOptions> _options)
        {
            Options = _options.Value;

            if (!Directory.Exists(Options.FolderPath))
            {
                Directory.CreateDirectory(Options.FolderPath ?? "");
            }
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new CustomLogger(this);
        }

        public void Dispose()
        {
        }
    }

    public class CustomLoggerOptions
    {
        public virtual string? FilePath { get; set; }

        public virtual string? FolderPath { get; set; }
    }

    public static class CustomLoggerExtensions
    {
        public static ILoggingBuilder AddCustomLogger(this ILoggingBuilder builder, Action<CustomLoggerOptions> configure)
        {
            builder.Services.AddSingleton<ILoggerProvider, CustomLoggerProvider>();
            builder.Services.Configure(configure);
            return builder;
        }
    }
}
