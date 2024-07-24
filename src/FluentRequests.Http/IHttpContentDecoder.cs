namespace FluentRequests.Http;

public interface IHttpContentDecoder<T>
{
    public string MediaType { get; }
    public ValueTask<T?> DecodeContentAsync(HttpContent httpContent, CancellationToken cancellationToken);
}