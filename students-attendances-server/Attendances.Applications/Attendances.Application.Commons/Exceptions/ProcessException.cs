namespace Attendances.Application.Commons.Exceptions;

public class ProcessException(string message, string? type = null) : Exception(message)
{
    public string? Type { get; } = type;
    
    public static void ThrowIf(Func<bool> predicate, string message)
    {
        if (predicate.Invoke()) throw new ProcessException(message);
    }
}

