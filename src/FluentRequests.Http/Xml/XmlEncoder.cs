using System.Net.Http.Headers;
using System.Net.Mime;

namespace FluentRequests.Http.Xml;

public class XmlEncoder<T> : IHttpContentEncoder<T>
{

    public virtual string MediaType => MediaTypeNames.Application.Xml;

    public HttpContent EncodeContent(T content)
        => XmlContent.Create(content, new MediaTypeHeaderValue(MediaType));
}