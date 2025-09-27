using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;


namespace Aspire.Hosting.OnePassword;

internal interface IOnePasswordClient
{
    Task<string?> GetFieldAsync(string accountId, string vaultId, string itemId, string field, CancellationToken ct);
    Task<UserSignInStatus> GetUserSignInStatus(string? accountId, ILogger? logger = default, CancellationToken cancellationToken = default);
    Task<UserSignInStatus> UserSignInAsync(string? accountId, ILogger? logger = default, CancellationToken cancellationToken = default);
}
