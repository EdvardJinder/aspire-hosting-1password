using Aspire.Hosting.ApplicationModel;
using Microsoft.AspNetCore.DataProtection;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace Aspire.Hosting.OnePassword;

public sealed class OnePasswordFieldResource
    :  Resource, IManifestExpressionProvider, IValueProvider
{
    private readonly Lazy<string> _lazyValue;
    private readonly Func<ParameterDefault?, string> _valueGetter;
    private string? _configurationKey;
    public OnePasswordFieldResource(
        string name,
        string? accountId,
        string vaultId,
        string itemId,
        string field
        ) : base(name)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(vaultId);
        ArgumentNullException.ThrowIfNull(itemId);
        ArgumentNullException.ThrowIfNull(field);

        AccountId = accountId;
        VaultId = vaultId;
        ItemId = itemId;
        Field = field;

        _lazyValue = new Lazy<string>(() => Value);
    }


    public string? AccountId { get; }
    public string VaultId { get; }
    public string ItemId { get; } 
    public string Field { get; }
    public string Value { get; internal set; } 

    /// <summary>
    /// Gets the expression used in the manifest to reference the value of the parameter.
    /// </summary>
    public string ValueExpression => $"{{{Name}.value}}";

    internal string ConfigurationKey
    {
        get => _configurationKey ?? $"OnePassword:{Name}";
        set => _configurationKey = value;
    }

    internal string ValueInternal => _lazyValue.Value;
    /// <summary>
    /// A task completion source that can be used to wait for the value of the parameter to be set.
    /// </summary>
    internal TaskCompletionSource<string>? WaitForValueTcs { get; set; }

    /// <summary>
    /// Gets the value of the parameter asynchronously, waiting if necessary for the value to be set.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the value.</param>
    /// <returns>A task that represents the asynchronous operation, containing the value of the parameter.</returns>
    public async ValueTask<string?> GetValueAsync(CancellationToken cancellationToken)
    {
        if (WaitForValueTcs is not null)
        {
            // Wait for the value to be set if the task completion source is available.
            return await WaitForValueTcs.Task.WaitAsync(cancellationToken).ConfigureAwait(false);
        }

        // In publish mode, there's no WaitForValueTcs set.
        return ValueInternal;
    }

}