namespace FluentRequests.Http;

/// <summary>
/// An exception that is thrown when a required builder member is not set.
/// </summary>
internal class InvalidBuilderStateException : Exception
{
    public InvalidBuilderStateException(string builderProperty)
        : base($"The builder method {builderProperty} must be called.")
    {
    }
}