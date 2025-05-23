namespace Attendances.RestWrapper.MoodleApi.Infrastructures;

public interface IMoodleClient
{
    Task<T> SendRequestAsync<T>(string function, Dictionary<string, object> parameters);
}