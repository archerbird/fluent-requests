using System.Net.Http.Json;
using FluentRequests.Http;
using FluentRequests.Http.Json;
using FluentRequests.Http.Xml;

Console.WriteLine("Hello, World!");

// new HttpRequestBuilder()
//     .WithMethod(HttpMethod.Get)
//     .WithUri("https://jsonplaceholder.typicode.com/posts/1")
//     .When(response => response.StatusCode == HttpStatusCode.OK)
//         .Then(async response => Console.WriteLine(await response.Content.ReadAsStringAsync().Result))
//     .Otherwise(async response => Console.WriteLine($"Error: {response.StatusCode}"))
//     .SendAsync();

var client = new HttpClient();

var builder = client.BuildRequest()
    .WithMethod(HttpMethod.Post)
    .WithBody(new RequestBody("Test", 69))
    .WithUri("http://www.example.com")
    .WithContentEncoder(new XmlEncoder<RequestBody>())
    .WithCompletionOption(HttpCompletionOption.ResponseHeadersRead)
    .WithMethod(HttpMethod.Get)
    .WithContentEncoder(x => JsonContent.Create(x));
    
var builder2 = builder.WithAutoDecoding(new JsonDecoder<Response>());

var result = await builder.SendAsync();
    public record RequestBody(string Title, int UserId);
    public record Response(int Id, int UserId);