using System.Net.Mime;
using System.Xml.Serialization;

namespace FluentRequests.Http.Xml;

public class XmlDecoder<T> : IHttpContentDecoder<T>
{
    private readonly XmlSerializer _serializer = new XmlSerializer(typeof(T));
    public virtual string MediaType => MediaTypeNames.Application.Xml;

    public ValueTask<T?> DecodeContentAsync(HttpContent httpContent, CancellationToken cancellationToken)
        => ValueTask.FromResult((T?)_serializer.Deserialize(httpContent.ReadAsStream(cancellationToken)));
}