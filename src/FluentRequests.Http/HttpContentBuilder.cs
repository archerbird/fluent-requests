using FluentRequests.Http.Json;

namespace FluentRequests.Http;

internal interface IHttpContentBuilder
{
    internal void SetHttpContent(HttpRequestMessage request);
}

public class HttpContentBuilder<TBody> : HttpRequestBuilder<HttpContentBuilder<TBody>>, IHttpContentBuilder
{
    private TBody? _body;
    private Func<TBody, HttpContent> _encoder = new JsonEncoder<TBody>().EncodeContent;

    internal HttpContentBuilder(HttpClient client) : base(client)
    {
    }

    public HttpContentBuilder<TBody> WithBody(TBody body)
    {
        _body = body;
        return this;
    }

    public HttpContentBuilder<TBody> WithContentEncoder(IHttpContentEncoder<TBody> encoder)
    {
        ArgumentNullException.ThrowIfNull(encoder, nameof(encoder));
        _encoder = encoder.EncodeContent;
        return this;
    }

    public HttpContentBuilder<TBody> WithContentEncoder(Func<TBody, HttpContent> encoder)
    {
        ArgumentNullException.ThrowIfNull(encoder, nameof(encoder));
        _encoder = encoder;
        return this;
    }

    void IHttpContentBuilder.SetHttpContent(HttpRequestMessage request)
    {
        if (_body is null)
        {
            return;
        }
        
        request.Content = _encoder(_body);
    }
}
