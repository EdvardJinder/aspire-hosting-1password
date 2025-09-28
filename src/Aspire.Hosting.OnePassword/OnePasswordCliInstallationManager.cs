#pragma warning disable ASPIREINTERACTION001

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using OnePassword.Cli.Sdk;

namespace Aspire.Hosting.OnePassword;

internal sealed class OnePasswordCliInstallationManager : RequiredCommandValidator
{
    private readonly IOnePasswordClient _onePasswordClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger;
    private string? _resolvedCommandPath;

#pragma warning disable ASPIREINTERACTION001 // Interaction service is experimental.
    public OnePasswordCliInstallationManager(
        IOnePasswordClient onePasswordClient,
        IConfiguration configuration,
        IInteractionService interactionService,
        ILogger<OnePasswordCliInstallationManager> logger)
        : base(interactionService, logger)
#pragma warning restore ASPIREINTERACTION001
    {
        _onePasswordClient = onePasswordClient ?? throw new ArgumentNullException(nameof(onePasswordClient));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets the resolved full path to the devtunnel CLI after a successful validation, otherwise <c>null</c>.
    /// </summary>
    public string? ResolvedCommandPath => _resolvedCommandPath;

    /// <summary>
    /// Gets a value indicating whether the CLI was found (after calling <see cref="EnsureInstalledAsync"/>).
    /// </summary>
    public bool IsInstalled => _resolvedCommandPath is not null;

    /// <summary>
    /// Ensures the devtunnel CLI is installed/available. This method is safe for concurrent callers;
    /// only one validation will run at a time.
    /// </summary>
    /// <throws cref="DistributedApplicationException">Thrown if the devtunnel CLI is not found.</throws>
    public Task EnsureInstalledAsync(CancellationToken cancellationToken = default) => RunAsync(cancellationToken);

    protected override string GetCommandPath() => IOnePasswordClient.GetCliPath();

    protected internal override async Task<(bool IsValid, string? ValidationMessage)> OnResolvedAsync(string resolvedCommandPath, CancellationToken cancellationToken)
    {
        return (true, null);
    }

    protected override Task OnValidatedAsync(string resolvedCommandPath, CancellationToken cancellationToken)
    {
        _resolvedCommandPath = resolvedCommandPath;
        return Task.CompletedTask;
    }

    protected override string? GetHelpLink() => "https://developer.1password.com/docs/cli/get-started#step-1-install-1password-cli";
}
