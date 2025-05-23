using Attendances.Application.Authorization.Infrastructures.Interface;
using Attendances.Application.Authorization.Infrastructures.Models;
using Attendances.Application.Authorization.Models;
using Attendances.Application.Commons.Exceptions;
using Attendances.RestWrapper.MoodleApi.Infrastructures;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Attendances.RestWrapper.MoodleApi.Services;

internal class MoodleAuthorizationService : IAuthorizationExternal
{
    private readonly IMoodleClient _moodleClient;

    public MoodleAuthorizationService(IMoodleClient moodleClient, ILogger<MoodleAuthorizationService> logger)
    {
        Logger = logger;
        _moodleClient = moodleClient;
    }
    private ILogger<MoodleAuthorizationService> Logger { get; }
    
    public async Task<TeacherExternalInfo?> GetAccountInfoAsync(CredentialsModel credentials)
    {
        const string functionName = "local_myplugin_get_teacher_info";
        try
        {
            var response = await _moodleClient.SendRequestAsync<TeacherExternalInfo>(functionName, new()
            {
                { "username", credentials.Username },
                { "password", credentials.Password }
            });
            return response;
        }
        catch (MoodleException error)
        {
            Logger.LogError(error, $"Failed to authorization from Moodle: {error.Message}");
            return null;
        }
    }
}