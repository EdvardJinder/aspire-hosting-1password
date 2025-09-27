using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;


namespace Aspire.Hosting.OnePassword;

internal interface IOnePasswordClient
{
    Task<string> GetFieldAsync(string? accountId, string vaultId, string itemId, string field, ILogger? logger = null, CancellationToken cancellationToken = default);
}
