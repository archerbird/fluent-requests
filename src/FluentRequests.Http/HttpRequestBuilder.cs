namespace FluentRequests.Http;

public class HttpRequestBuilder : HttpRequestBuilder<HttpRequestBuilder>
{
    public HttpRequestBuilder(HttpClient client) : base(client)
    {
    }
}

/// <summary>
/// A builder class for creating and sending <see cref="HttpRequestMessage" />s.
/// </summary>
public class HttpRequestBuilder<TRequestBuilder> where TRequestBuilder : HttpRequestBuilder<TRequestBuilder>
{
    protected private HttpRequestMessage _request;
    private readonly HttpClient _client;

    private IHttpContentBuilder? _bodyBuilder;
    private HttpCompletionOption _completionOption = HttpCompletionOption.ResponseContentRead;
    private Func<HttpRequestMessage, Task>? _requestHandler;
    private Func<HttpResponseMessage, Task>? _responseHandler;


    protected HttpRequestBuilder(HttpClient client)
    {
        ArgumentNullException.ThrowIfNull(client, nameof(client));
        _client = client;
        _request = new HttpRequestMessage();
    }


    public HttpContentBuilder<TBody> WithBody<TBody>(TBody body)
    {
        ArgumentNullException.ThrowIfNull(body, nameof(body));
        if (_bodyBuilder is HttpContentBuilder<TBody> builder)
        {
            return builder.WithBody(body);
        }

        var bodyBuilder = new HttpContentBuilder<TBody>(_client);
       _bodyBuilder = bodyBuilder;
       
        return bodyBuilder.TransferState(this).WithBody(body);
    }

    public HttpResponseHandlingBuilder<TResponse> WithAutoDecoding<TResponse>(IHttpContentDecoder<TResponse> decoder)
    {
        ArgumentNullException.ThrowIfNull(decoder, nameof(decoder));
        return new HttpResponseHandlingBuilder<TResponse>(_client)
            .TransferState(this)
            .WithDecoder(decoder);
    }

    public HttpResponseHandlingBuilder<TResponse> WithAutoDecoding<TResponse>(params IHttpContentDecoder<TResponse>[] decoders)
    {
        ArgumentNullException.ThrowIfNull(decoders, nameof(decoders));
        return new HttpResponseHandlingBuilder<TResponse>(_client).WithDecoders(decoders);
    }

    /// <summary>
    /// Sets the <see cref="HttpMethod" /> for the request.
    /// </summary>
    /// <param name="method"></param>
    /// <returns>The current instance of the <see cref="HttpRequestBuilder" />.</returns>
    public TRequestBuilder WithMethod(HttpMethod method)
    {
        _request.Method = method;
        return (TRequestBuilder)this;
    }

    /// <summary>
    /// Sets the resource Uri for the request.
    /// </summary>
    /// <param name="uri"><see cref="Uri" />.</param>
    /// <returns>The current instance of the <see cref="HttpRequestBuilder" />.</returns>
    public TRequestBuilder WithUri(Uri uri)
    {
        _request.RequestUri = uri;
        return (TRequestBuilder)this;
    }

    /// <summary>
    /// Sets the resource Uri for the request.
    /// </summary>
    /// <param name="uri">A string representation of the URI.</param>
    /// <returns>The current instance of the <see cref="HttpRequestBuilder" />.</returns>
    public TRequestBuilder WithUri(string uri)
    {
        ArgumentNullException.ThrowIfNull(uri, nameof(uri));
        return WithUri(new Uri(uri));
    }

    /// <summary>
    /// Adds or updates a header on the request.
    /// <remarks>This method can be called multiple times to add additional headers.</remarks>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns>The current instance of the <see cref="HttpRequestBuilder" />.</returns>
    public TRequestBuilder IncludeHeader(string key, string value)
    {
        ArgumentNullException.ThrowIfNull(key, nameof(key));
        ArgumentNullException.ThrowIfNull(value, nameof(value));
        _request.Headers.Add(key, value);
        return (TRequestBuilder)this;
    }

    /// <summary>
    /// Configures the completion option used when sending the request.
    /// <remarks>By default, this is <see cref="HttpCompletionOption.ResponseContentRead"/>,
    /// which is the default used by the <see cref="HttpClient"/>.</remarks>
    /// </summary>
    /// <param name="completionOption">The <see cref="HttpCompletionOption"/>.</param>
    /// <returns>The current instance <see cref="HttpRequestBuilder"/>.</returns>
    public TRequestBuilder WithCompletionOption(HttpCompletionOption completionOption)
    {
        _completionOption = completionOption;
        return (TRequestBuilder)this;
    }

    /// <summary>
    /// Sets an async action to be executed on the response, before it is deserialized. You can use this to handle the response in a custom way, such as logging or error handling.
    /// If this is not set, the default behavior is to throw an <see cref="HttpRequestException"/> if the response is not successful.
    /// </summary>
    /// <param name="responseHandler"></param>
    /// <returns>The current instance of the <see cref="HttpRequestBuilder" />.</returns>
    public TRequestBuilder WithResponseHandler(Func<HttpResponseMessage, Task> responseHandler)
    {
        ArgumentNullException.ThrowIfNull(responseHandler, nameof(responseHandler));
        _responseHandler = responseHandler;
        return (TRequestBuilder)this;
    }

    public TRequestBuilder WithRequestHandler(Func<HttpRequestMessage, Task> requestHandler)
    {
        ArgumentNullException.ThrowIfNull(requestHandler, nameof(requestHandler));
        _requestHandler = requestHandler;
        return (TRequestBuilder)this;
    }


    /// <summary>
    /// Builds and sends the request.
    /// <remarks>Use this method when the response body is not needed.</remarks>
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken" />.</param>
    /// <exception cref="InvalidBuilderStateException">
    ///     Thrown when the builder has not been fully configured before executing
    ///     the call.
    /// </exception>
    public async Task<HttpResponseMessage> SendAsync(CancellationToken cancellationToken = default)
    {
        return await InternalSendAsync(cancellationToken);
    }

    protected async Task<HttpResponseMessage> InternalSendAsync(CancellationToken cancellationToken = default)
    {
        ValidateBuilderState();
        cancellationToken.ThrowIfCancellationRequested();

        _bodyBuilder?.SetHttpContent(_request);

        if (_requestHandler is not null)
        {
            await _requestHandler.Invoke(_request);
        }

        var response = await _client
            .SendAsync(_request, _completionOption, cancellationToken);

        if (_responseHandler != null)
        {
            await _responseHandler.Invoke(response);
        }

        return response;
    }

    private void ValidateBuilderState()
    {
        if (_request.Method is null) throw new InvalidBuilderStateException(nameof(WithMethod));

        if (_request.RequestUri is null) throw new InvalidBuilderStateException(nameof(WithUri));
    }

    private TRequestBuilder TransferState<T>(HttpRequestBuilder<T> builder) where T : HttpRequestBuilder<T>
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));
        _request = builder._request;
        _bodyBuilder = builder._bodyBuilder;
        _requestHandler = builder._requestHandler;
        _responseHandler = builder._responseHandler;
        return (TRequestBuilder)this;
    }
}