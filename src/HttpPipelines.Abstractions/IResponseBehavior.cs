using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HttpPipelines.Abstractions;

public interface IResponseBehavior
{
    Task<bool> Apply(ResponseContext response, CancellationToken cancellationToken);
}