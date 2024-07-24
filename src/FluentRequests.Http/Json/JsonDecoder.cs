using System.Net.Mime;
using System.Text.Json;

namespace FluentRequests.Http.Json;

public class JsonDecoder<T> : IHttpContentDecoder<T>
{
    private readonly JsonSerializerOptions _options;
    
    public JsonDecoder(JsonSerializerOptions? options = null)
    {
        _options = options ?? new JsonSerializerOptions(JsonSerializerDefaults.Web);
    }
    
    public virtual string MediaType => MediaTypeNames.Application.Json;

    public async ValueTask<T?> DecodeContentAsync(HttpContent httpContent, CancellationToken cancellationToken)
    {
        return await JsonSerializer.DeserializeAsync<T>(
            await httpContent.ReadAsStreamAsync(cancellationToken),
            _options,
            cancellationToken);
    }
}