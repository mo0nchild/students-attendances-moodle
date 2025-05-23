using Attendances.Application.Fetching.Infrastructures.Interfaces;
using Attendances.Application.Fetching.Interfaces;
using Microsoft.Extensions.Logging;

namespace Attendances.Application.Fetching.Services;

internal class ExternalHealthcheckService : IExternalHealthcheckService
{
    private readonly IHealthcheckExternal _healthcheckExternal;
    private readonly SemaphoreSlim _checkLock = new(1, 1);
    
    private bool _healthcheckRequired = false;
    private ExternalApiStatus _status = new(
        IsAvailable: true,
        LastCheckTime: DateTime.MinValue,
        TotalChecks: 0,
        FailedChecks: 0);
    
    public ExternalHealthcheckService(IHealthcheckExternal healthcheckExternal, 
        ILogger<ExternalHealthcheckService> logger)
    {
        Logger = logger;
        _healthcheckExternal = healthcheckExternal;
    }
    private ILogger<ExternalHealthcheckService> Logger { get; }
    
    public async Task<bool> IsApiAvailableAsync(CancellationToken cancellationToken = default)
    {
        if (!_healthcheckRequired) return true;
        await _checkLock.WaitAsync(cancellationToken);
        try
        {
            var isAvailable = await _healthcheckExternal.IsHealthy();
            _status = _status with
            {
                TotalChecks = _status.TotalChecks + 1,
                FailedChecks = isAvailable ? _status.FailedChecks : _status.FailedChecks + 1,
                IsAvailable = isAvailable,
                LastCheckTime = DateTime.UtcNow,
            };
            if (isAvailable) _healthcheckRequired = false;
            return isAvailable;
        }
        finally { _checkLock.Release(); }
    }

    public Task SetRequestToCheck()
    {
        _healthcheckRequired = true;
        _status = _status with { IsAvailable = false };
        
        Logger.LogInformation("External API health check requested");
        return Task.CompletedTask;
    }

    public ExternalApiStatus GetCurrentStatus() => _status;
}