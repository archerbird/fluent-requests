namespace FluentRequests.Http;

public static class Extensions
{
    public static HttpRequestBuilder BuildRequest(this HttpClient client)
    {
        ArgumentNullException.ThrowIfNull(client, nameof(client));
        return new HttpRequestBuilder(client);
    }
    
    public static HttpRequestBuilder BuildRequest(this IHttpClientFactory clientFactory, string? clientName = null)
    {
        ArgumentNullException.ThrowIfNull(clientFactory, nameof(clientFactory));
        ArgumentNullException.ThrowIfNull(clientName, nameof(clientName));
        return string.IsNullOrEmpty(clientName) ? new HttpRequestBuilder(clientFactory.CreateClient()) : new HttpRequestBuilder(clientFactory.CreateClient(clientName));
    }
}