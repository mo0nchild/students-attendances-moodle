using Attendances.RestWrapper.MoodleApi.Models;
using Attendances.RestWrapper.MoodleApi.Models.Commons;
using Attendances.RestWrapper.MoodleApi.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Attendances.RestWrapper.MoodleApi.Infrastructures;

internal class MoodleClient : IMoodleClient
{
    private readonly MoodleRestSettings _moodleRestSettings;
    private readonly HttpClient _httpClient;

    public MoodleClient(IOptions<MoodleRestSettings> options, HttpClient httpClient,
        ILogger<MoodleClient> logger)
    {
        _moodleRestSettings = options.Value;
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromSeconds(_moodleRestSettings.TimeoutSeconds);
        Logger = logger;
    }
    private ILogger<MoodleClient> Logger { get; }
    
    public async Task<T> SendRequestAsync<T>(string function, Dictionary<string, object> parameters)
    {
        var requestParams = new Dictionary<string, string>()
        {
            {"wstoken", _moodleRestSettings.Token},
            {"wsfunction", function},
            {"moodlewsrestformat", "json"}
        };
        foreach (var param in parameters)
        {
            requestParams.Add(param.Key, param.Value?.ToString() ?? string.Empty);
        }
        var content = new FormUrlEncodedContent(requestParams);
        var requestUrl = $"{_moodleRestSettings.BaseUrl}/webservice/rest/server.php";
        try
        {
            var response = await _httpClient.PostAsync(requestUrl, content);
            if (!response.IsSuccessStatusCode)
            {
                Logger.LogError($"HTTP Error [{response.StatusCode}]: {await response.Content.ReadAsStringAsync()}");
                throw new MoodleException($"HTTP Error [{response.StatusCode}]");
            }
            return await HandleResponse<T>(response);
        }
        catch (HttpRequestException error)
        {
            Logger.LogError($"HTTP Error: {error.Message}");
            throw new MoodleException($"HTTP Error: {error.Message}", new MoodleErrorResponse()
            {
                ErrorCode = "notavailable",
            });
        }
    }

    protected virtual async Task<T> HandleResponse<T>(HttpResponseMessage response)
    {
        var jsonContent = await response.Content.ReadAsStringAsync();
        var jsonToken = JToken.Parse(jsonContent);
        try
        {
            if (jsonToken.Type == JTokenType.Object && jsonToken["errorcode"] != null)
            {
                var errorResponse = jsonToken.ToObject<MoodleErrorResponse>();
                Logger.LogError($"Moodle Api bad response: {errorResponse?.ErrorCode}, {errorResponse?.ErrorMessage}");
                throw new MoodleException("Moodle API bad response", errorResponse!);
            }
            return jsonToken.ToObject<T>()!;
        }
        catch (JsonSerializationException error)
        {
            Logger.LogError(error, "Failed to deserialize Moodle response to {Type}. JSON: {Json}", typeof(T), jsonToken.ToString());
            throw;
        }
    }
}