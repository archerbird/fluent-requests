using System.Threading;
using System.Threading.Tasks;

namespace HttpPipelines.Abstractions;
public interface IRequestPipeline
{
    Task<object?> Send();
    Task<object?> Send(CancellationToken cancellationToken);
}