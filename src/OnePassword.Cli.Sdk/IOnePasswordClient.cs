using Microsoft.Extensions.Logging;

namespace OnePassword.Cli.Sdk;

public interface IOnePasswordClient
{
    /// <summary>
    /// Asynchronously retrieves the value of a specified field from an item in a vault for the given account.
    /// </summary>
    /// <param name="accountId">The identifier of the account containing the vault. If null, the default account is used.</param>
    /// <param name="vaultId">The identifier of the vault that contains the item.</param>
    /// <param name="itemId">The identifier of the item from which to retrieve the field value.</param>
    /// <param name="field">The name of the field to retrieve from the specified item.</param>
    /// <param name="logger">An optional logger used to record diagnostic information during the operation. If null, no logging is performed.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the value of the specified field, or
    /// null if the field does not exist.</returns>
    Task<string> GetFieldAsync(string? accountId, string vaultId, string itemId, string field, ILogger? logger = null, CancellationToken cancellationToken = default);


    static string GetCliPath() => "op";
}
