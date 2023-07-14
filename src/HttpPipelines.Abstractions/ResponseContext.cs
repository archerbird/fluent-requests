using System.Net.Http;
namespace HttpPipelines.Abstractions;

public class ResponseContext
{
    public ResponseContext(HttpResponseMessage httpResponseMessage)
    {
        HttpResponseMessage = httpResponseMessage;
    }

    public HttpResponseMessage HttpResponseMessage { get; init; }

    public object? Response { get; set; }
}