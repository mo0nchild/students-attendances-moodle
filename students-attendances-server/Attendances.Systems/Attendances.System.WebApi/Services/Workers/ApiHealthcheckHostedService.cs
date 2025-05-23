using Attendances.Application.Commons.Exceptions;
using Attendances.Application.Fetching.Interfaces;

namespace Attendances.System.WebApi.Services.Workers;

public class ApiHealthcheckHostedService : BackgroundService
{
    private readonly IExternalHealthcheckService _healthcheckService;

    public ApiHealthcheckHostedService(IExternalHealthcheckService healthcheckService,
        ILogger<ApiHealthcheckHostedService> logger)
    {
        _healthcheckService = healthcheckService;
        Logger = logger;
    }
    private ILogger<ApiHealthcheckHostedService> Logger { get; }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = await _healthcheckService.IsApiAvailableAsync(stoppingToken);
                if (!result) Logger.LogInformation("Moodle API service is not available");
            }
            catch (ProcessException error)
            {
                Logger.LogError(error.Message);
            }
            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }
}