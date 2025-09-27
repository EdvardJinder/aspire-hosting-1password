using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;


namespace Aspire.Hosting.OnePassword;

internal sealed class OnePasswordCliClient : IOnePasswordClient
{
    private readonly OnePasswordCli _cli = new(OnePasswordCli.GetCliPath());
    public async Task<string> GetFieldAsync(string? accountId, string vaultId, string itemId, string field, ILogger? logger = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(vaultId);
        ArgumentNullException.ThrowIfNull(itemId);
        ArgumentNullException.ThrowIfNull(field);

        var (value, exitCode, error) = await CallCliAsync(
          (ow, ew, logger, ct) => _cli.GetFieldValue(accountId, vaultId, itemId, field, ow, ew, logger, ct),
          logger, cancellationToken).ConfigureAwait(false);

        return value ?? throw new DistributedApplicationException($"Failed to get field '{field}' from item '{itemId}' in vault '{vaultId}' (account '{accountId}'). Exit code {exitCode}: {error}");
    }

    private async Task<(string? Result, int ExitCode, string? Error)> CallCliAsync(Func<TextWriter, TextWriter, ILogger?, CancellationToken, Task<int>> cliCall, ILogger? logger = default, CancellationToken cancellationToken = default)
    {
        return await CallCliAsync(cliCall, propertyName: null, logger, cancellationToken).ConfigureAwait(false);
    }
    private async Task<(string? Result, int ExitCode, string? Error)> CallCliAsync(Func<TextWriter, TextWriter, ILogger?, CancellationToken, Task<int>> cliCall, string? propertyName, ILogger? logger = default, CancellationToken cancellationToken = default)
    {
        // PERF: Could pool these writers
        using var stdout = new StringWriter();
        using var stderr = new StringWriter();

        var exitCode = await cliCall(stdout, stderr, logger, cancellationToken).ConfigureAwait(false);

        if (exitCode != 0)
        {
            var error = stderr.ToString().Trim();
            logger?.LogError("CLI call returned non-zero exit code '{ExitCode}'. stderr output:\n{Error}", exitCode, error);
            return (default, exitCode, error);
        }

        var output = stdout.ToString().Trim();
        logger?.LogTrace("CLI call output:\n{Output}", output);

        if (cancellationToken.IsCancellationRequested)
        {
            logger?.LogDebug("Operation was cancelled.");
            return (default, exitCode, "Operation was cancelled.");
        }

        if (string.IsNullOrEmpty(output))
        {
            logger?.LogError("CLI call returned empty output with exit code '{ExitCode}'.", exitCode);
            return (default, exitCode, "CLI call returned empty output.");
        }

        return (output, exitCode, default);
    }

}
