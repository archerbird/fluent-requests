using System.Runtime.CompilerServices;

namespace FluentRequests.Http;

public static class ArgumentExceptionExtensions
{
    public static void ThrowIfNullOrWhiteSpace(string? argument, [CallerArgumentExpression("argument")] string? paramName = null)
        {
            if (!string.IsNullOrWhiteSpace(argument))
            {
                return;
            }
            ArgumentNullException.ThrowIfNull(argument, paramName);
            throw new ArgumentException("The value cannot be an empty string or composed entirely of whitespace.", paramName);
        }
}