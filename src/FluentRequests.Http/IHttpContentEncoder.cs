namespace FluentRequests.Http;

public interface IHttpContentEncoder<in T>
{
    public string MediaType { get; }
    public HttpContent EncodeContent(T content);
}