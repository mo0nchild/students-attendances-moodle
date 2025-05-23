using Attendances.Application.Fetching.Infrastructures.Interfaces;
using Attendances.RestWrapper.MoodleApi.Infrastructures;
using Attendances.RestWrapper.MoodleApi.Models;
using Attendances.RestWrapper.MoodleApi.Models.Commons;
using Microsoft.Extensions.Logging;

namespace Attendances.RestWrapper.MoodleApi.Services;

internal class MoodleHealthcheckService : IHealthcheckExternal
{
    private readonly IMoodleClient _moodleClient;

    public MoodleHealthcheckService(IMoodleClient moodleClient, ILogger<MoodleHealthcheckService> logger)
    {
        Logger = logger;
        _moodleClient = moodleClient;
    }
    private ILogger<MoodleHealthcheckService> Logger { get; }
    
    public async Task<bool> IsHealthy()
    {
        const string functionName = "local_myplugin_get_current_time";
        try
        {
            await _moodleClient.SendRequestAsync<MoodleCurrentTime>(functionName, new ());
            return true;
        }
        catch (MoodleException error)
        {
            Logger.LogError(error, $" Moodle not health: {error.Message}");
            return false;
        }
        catch (Exception ex) when (ex is not MoodleException)
        {
            Logger.LogError(ex, "Unexpected error during Moodle health check");
            return false;
        }
    }
}