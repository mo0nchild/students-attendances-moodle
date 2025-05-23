using Attendances.Application.Commons.Exceptions;
using Attendances.RestWrapper.MoodleApi.Models;
using Attendances.RestWrapper.MoodleApi.Models.Commons;

namespace Attendances.RestWrapper.MoodleApi.Infrastructures;

public class MoodleException(string message, MoodleErrorResponse? errorResponse = null) 
    : ProcessException(message)
{
    public MoodleErrorResponse? ErrorResponse { get; } = errorResponse;

    public static void ThrowIf(Func<bool> predicate, string message, MoodleErrorResponse? errorResponse = null)
    {
        if (predicate.Invoke()) throw new MoodleException(message, errorResponse);
    }
}