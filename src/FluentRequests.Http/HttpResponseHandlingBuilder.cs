namespace FluentRequests.Http;

public class HttpResponseHandlingBuilder<T1> : HttpRequestBuilder<HttpResponseHandlingBuilder<T1>>
{
    private Dictionary<string, Func<HttpContent, CancellationToken, ValueTask<T1?>>> _decoders = [];
    
    internal HttpResponseHandlingBuilder(HttpClient client) : base(client)
    {
    }
    
    public HttpResponseHandlingBuilder<T1> WithDecoder(string media, Func<HttpContent, CancellationToken, ValueTask<T1?>> decoder)
    {
        ArgumentNullException.ThrowIfNull(media, nameof(media));
        ArgumentExceptionExtensions.ThrowIfNullOrWhiteSpace(media);
        ArgumentNullException.ThrowIfNull(decoder, nameof(decoder));
        
        if (!_decoders.TryAdd(media, decoder))
        {
            _decoders[media] = decoder;
        }; 
        _request.Headers.Accept.ParseAdd(media);
        return this;
    }
    
    public HttpResponseHandlingBuilder<T1> WithDecoder(IHttpContentDecoder<T1> decoder)
    {
        ArgumentNullException.ThrowIfNull(decoder, nameof(decoder));
        
        WithDecoder(decoder.MediaType, decoder.DecodeContentAsync);
        return this;
    }
    
    public HttpResponseHandlingBuilder<T1> WithDecoders(params IHttpContentDecoder<T1>[] decoders)
    {
        ArgumentNullException.ThrowIfNull(decoders, nameof(decoders));
        
        foreach(var decoder in decoders)
        {
            WithDecoder(decoder);
        }
        return this;
    }
    
    /// <summary>
    /// Builds and sends the request and deserializes the response body to <typeparamref name="TApiModel" />.
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken" />.</param>
    /// <typeparam name="T1">An object to which the response can be deserialized.</typeparam>
    /// <returns><see cref="T1" /> The response body, deserialized from json.</returns>
    /// <exception cref="InvalidBuilderStateException">
    ///     Thrown when the builder has not been fully configured before executing
    ///     the call.
    /// </exception>
    public new async Task<T1?> SendAsync(CancellationToken cancellationToken = default)
    {
        var response = await InternalSendAsync(cancellationToken);
        var mediaType = response.Content.Headers.ContentType?.MediaType
            ?? response.RequestMessage!.Headers.Accept.First().MediaType!;
        
        if (!_decoders.TryGetValue(
                mediaType,
                out var decoder))
        {
            var exception = new FormatException($"No decoder found for the response content type: '{mediaType}'");
            exception.Data.Add("HttpResponseMessage", response);
            throw exception;
        }
            
        var content = await decoder.Invoke(response.Content, cancellationToken);
        return content;
    }
}


