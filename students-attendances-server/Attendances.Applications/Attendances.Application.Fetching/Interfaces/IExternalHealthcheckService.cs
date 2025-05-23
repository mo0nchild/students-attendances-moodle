namespace Attendances.Application.Fetching.Interfaces;

public interface IExternalHealthcheckService
{
    Task<bool> IsApiAvailableAsync(CancellationToken cancellationToken = default);
    Task SetRequestToCheck();
    ExternalApiStatus GetCurrentStatus();
}

public record ExternalApiStatus(
    bool IsAvailable,
    DateTime LastCheckTime,
    int TotalChecks,
    int FailedChecks);