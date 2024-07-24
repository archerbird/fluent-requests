using System.Net.Mime;

namespace FluentRequests.Http.Text;

public class TextEncoder<T> : IHttpContentEncoder<T>
{

    public string MediaType => MediaTypeNames.Text.Plain;

    public HttpContent EncodeContent(T content)
    {
        return new StringContent(content.ToString(), System.Text.Encoding.UTF8, MediaType);
    }
}