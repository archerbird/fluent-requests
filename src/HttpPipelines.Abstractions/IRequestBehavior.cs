using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HttpPipelines.Abstractions;

public interface IRequestBehavior
{
    Task<bool> Apply(HttpRequestMessage request, CancellationToken cancellationToken);
}