namespace Attendances.Application.Commons.Exceptions;

public class AuthException(string message) : ProcessException(message)
{
    public new static void ThrowIf(Func<bool> predicate, string message)
    {
        if (predicate.Invoke()) throw new AuthException(message);
    }
}