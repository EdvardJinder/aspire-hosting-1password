using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;


namespace Aspire.Hosting.OnePassword;

internal sealed class OnePasswordCliClient(
    IConfiguration configuration
    ) : IOnePasswordClient
{
    private readonly int _maxCliAttempts = configuration.GetValue<int?>("ASPIRE_ONEPASSWORD_CLI_MAX_ATTEMPTS") ?? 3;
    private readonly TimeSpan _cliRetryOnErrorDelay = TimeSpan.FromSeconds(2);
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web) { Converters = { new JsonStringEnumConverter() } };
    private readonly OnePasswordCli _cli = new(OnePasswordCli.GetCliPath());

    public Task<string> GetFieldAsync(string accountId, string vaultId, string itemId, string field, CancellationToken ct)
    {
        throw new System.NotImplementedException();
    }

    public async Task<UserSignInStatus> GetUserSignInStatus(string? accountId, ILogger? logger = null, CancellationToken cancellationToken = default)
    {
        var (login, exitCode, error) = await CallCliAsJsonAsync<UserSignInStatus>(
            (ow, ew, logger, ct) => _cli.GetUserSignInStatus(ow, ew, accountId, logger, ct),
            logger, cancellationToken).ConfigureAwait(false);
        return login ?? throw new DistributedApplicationException($"Failed to get user login status. Exit code {exitCode}: {error}");
    }
    public async Task<UserSignInStatus> UserSignInAsync(string? accountId, ILogger? logger = null, CancellationToken cancellationToken = default)
    {
        logger?.LogTrace("Logging in to 1Password service.");

        var exitCode =  await _cli.UserSignInAsync(accountId, logger, cancellationToken).ConfigureAwait(false);

        if (exitCode == 0)
        {
            // Login succeeded, get the login status
            return await GetUserSignInStatus(accountId, logger, cancellationToken).ConfigureAwait(false);
        }

        throw new DistributedApplicationException($"Failed to perform user login. Process finished with exit code: {exitCode}");
    }
    private async Task<(T? Result, int ExitCode, string? Error)> CallCliAsJsonAsync<T>(Func<TextWriter, TextWriter, ILogger?, CancellationToken, Task<int>> cliCall, ILogger? logger = default, CancellationToken cancellationToken = default)
    {
        return await CallCliAsJsonAsync<T>(cliCall, propertyName: null, logger, cancellationToken).ConfigureAwait(false);
    }
    private async Task<(T? Result, int ExitCode, string? Error)> CallCliAsJsonAsync<T>(Func<TextWriter, TextWriter, ILogger?, CancellationToken, Task<int>> cliCall, string? propertyName, ILogger? logger = default, CancellationToken cancellationToken = default)
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

        try
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                output = JsonDocument.Parse(output).RootElement.GetProperty(propertyName).GetRawText();
                logger?.LogTrace("Extracted JSON property '{PropertyName}':\n{Output}", propertyName, output);
            }
            var result = JsonSerializer.Deserialize<T>(output, _jsonOptions);
            logger?.LogTrace("JSON output successfully deserialized to '{TypeName}' instance", typeof(T).Name);
            return (result, 0, default);
        }
        catch (JsonException ex)
        {
            logger?.LogError(ex, "Failed to parse JSON output into type '{TypeName}':\n{Output}", typeof(T).Name, output);
            throw new DistributedApplicationException($"Failed to parse JSON output into type '{typeof(T).Name}':\n{output}", ex);
        }
    }

}
