using Back.Domain.Services.Abstractions;

namespace Back.Services
{
    public sealed class LoggerService(ILogger<LoggerService> logger) : ILoggerService
    {
        private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public void LogError(Exception ex, string? message = null)
        {
            _logger.LogError(ex, message ?? "[ERR]: An error occurred");
        }

        public void LogInformation(string? message = null)
        {
            _logger.LogInformation(message ?? "[INFO]: Information");
        }

        public void LogServiceInformation(string moduleName, string serviceName, string methodName)
        {
            string formattedModuleName = char.ToUpper(moduleName[0]) + moduleName.Substring(1);
            _logger.LogInformation($"[INFO]: {formattedModuleName}Module called {serviceName}.{methodName}");
        }

        public void LogStartService(string moduleName, string serviceName, string methodName)
        {
            string formattedModuleName = char.ToUpper(moduleName[0]) + moduleName.Substring(1);
            _logger.LogInformation($"[START]: {formattedModuleName}Module.{serviceName}.{methodName} - Endpoint called");
        }

        public void LogEndService(string moduleName, string serviceName, string methodName, long elapsedMilliseconds)
        {
            string formattedModuleName = char.ToUpper(moduleName[0]) + moduleName.Substring(1);
            _logger.LogInformation($"[END]: {formattedModuleName}Module.{serviceName}.{methodName} - Completed in {elapsedMilliseconds}ms");
        }
    }
}
