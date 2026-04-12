namespace Back.Domain.Services.Abstractions;

public interface ILoggerService
{
    void LogError(Exception ex, string? message = null);
    void LogInformation(string message);
    void LogServiceInformation(string moduleName, string serviceName, string methodName);
    void LogStartService(string moduleName, string serviceName, string methodName);
    void LogEndService(string moduleName, string serviceName, string methodName, long elapsedMilliseconds);
}