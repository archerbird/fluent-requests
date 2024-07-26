namespace FluentRequests.Http;

public class HttpDiscriminatedResponseHandlingBuilder<T1, T2> : HttpResponseHandlingBuilder<HttpDiscriminatedResponseHandlingBuilder<T1, T2>, T1>
{
    private readonly Dictionary<string, Func<HttpContent, CancellationToken, ValueTask<T2?>>> _alternateDecoders = [];
    
    internal HttpDiscriminatedResponseHandlingBuilder(HttpClient client) : base(client)
    {
    }
    
    public HttpDiscriminatedResponseHandlingBuilder<T1, T2> UsingDecoder(IHttpContentDecoder<T2> decoder)
    {
        ArgumentNullException.ThrowIfNull(decoder, nameof(decoder));
        
        if (!_alternateDecoders.TryAdd(decoder.MediaType, decoder.DecodeContentAsync))
        {
            _alternateDecoders[decoder.MediaType] = decoder.DecodeContentAsync;
        }
        return this;
    }
    
    public HttpDiscriminatedResponseHandlingBuilder<T1, T2> UsingDecoder(string media, Func<HttpContent, CancellationToken, ValueTask<T2?>> decoder)
    {
        ArgumentNullException.ThrowIfNull(media, nameof(media));
        ArgumentExceptionExtensions.ThrowIfNullOrWhiteSpace(media);
        ArgumentNullException.ThrowIfNull(decoder, nameof(decoder));
        
        if (!_alternateDecoders.TryAdd(media, decoder))
        {
            _alternateDecoders[media] = decoder;
        }
        _request.Headers.Accept.ParseAdd(media);
        return this;
    }
    
    public new async Task<object?> SendAsync(CancellationToken cancellationToken = default)
    {
        var response = await InternalSendAsync(cancellationToken);
        var mediaType = response.Content.Headers.ContentType?.MediaType
            ?? response.RequestMessage!.Headers.Accept.First().MediaType!;

        if (_decoders.TryGetValue(mediaType, out var decoder))
        {
            return await decoder(response.Content, cancellationToken);
        }

        if (_alternateDecoders.TryGetValue(mediaType, out var failureDecoder))
        {
            return await failureDecoder(response.Content, cancellationToken);
        }
        
        throw new InvalidOperationException($"No decoder found for media type '{mediaType}'.");
    }
}