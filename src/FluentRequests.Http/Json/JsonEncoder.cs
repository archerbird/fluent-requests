using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text.Json;

namespace FluentRequests.Http.Json;

public class JsonEncoder<T> : IHttpContentEncoder<T>
{
    private readonly JsonSerializerOptions _options;
    
    public JsonEncoder(JsonSerializerOptions? options = null)
    {
        _options = options ?? new JsonSerializerOptions(JsonSerializerDefaults.Web);
    }

    public virtual string MediaType => MediaTypeNames.Application.Json;

    public HttpContent EncodeContent(T content) => JsonContent.Create(content, typeof(T), new MediaTypeHeaderValue(MediaType), _options);
}