using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace FluentRequests.Http.Xml;

public class XmlContent : HttpContent
{
    private readonly Encoding _encoding;
    private readonly Type _inputType;
    private readonly object? _value;

    private XmlContent(object? value, Type inputType, MediaTypeHeaderValue? mediaType = null)
    {
        _value = value == null || inputType.IsInstanceOfType(value)
            ? value
            : throw new ArgumentException("The value is not of the correct type.", nameof(value));
        _inputType = inputType ?? throw new ArgumentNullException(nameof(inputType));
        _encoding = mediaType?.CharSet is { } charset ? Encoding.GetEncoding(charset) : Encoding.UTF8;
        Headers.ContentType = mediaType ?? new MediaTypeHeaderValue(MediaTypeNames.Application.Xml)
        {
            CharSet = _encoding.WebName
        };
    }

    protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
    {
        var xmlSerializer = new XmlSerializer(_inputType);
        using var xmlWriter = XmlWriter.Create(
            stream,
            new XmlWriterSettings
            {
                Encoding = _encoding
            });

        xmlSerializer.Serialize(xmlWriter, _value);

        return Task.CompletedTask;
    }

    protected override bool TryComputeLength(out long length)
    {
        // Length cannot be computed in advance, so return false
        length = 0L;
        return false;
    }

    public static XmlContent Create<T>(
        T value,
        MediaTypeHeaderValue? mediaType = null)
    {
        return Create(value, value?.GetType() ?? typeof(T), mediaType);
    }

    public static XmlContent Create(
        object? value,
        Type inputType,
        MediaTypeHeaderValue? mediaType = null)
    {
        return new XmlContent(value, inputType, mediaType);
    }
}