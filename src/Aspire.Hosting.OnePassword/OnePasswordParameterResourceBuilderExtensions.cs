#pragma warning disable ASPIREINTERACTION001

using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Eventing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.Loader;


namespace Aspire.Hosting.OnePassword;

public static class OnePasswordParameterResourceBuilderExtensions
{
    public static IResourceBuilder<T> WithEnvironment<T>(
        this IResourceBuilder<T> builder,
        string name,
        IResourceBuilder<OnePasswordFieldResource> parameter)
        where T : IResourceWithEnvironment
    {

        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(parameter);

        return builder.WithEnvironment(context =>
        {
            context.EnvironmentVariables[name] = parameter.Resource;
        });
    }
    public static IResourceBuilder<OnePasswordFieldResource> AddOnePasswordField(
       this IDistributedApplicationBuilder builder,
       string vaultId,
       string itemId,
       string field,
       string? accountId = null
       )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(vaultId);
        ArgumentNullException.ThrowIfNull(itemId);
        ArgumentNullException.ThrowIfNull(field);

        var name = $"onepassword-{vaultId}-{itemId}-{field}"
            .ToLowerInvariant()
            .Replace(" ", "", StringComparison.InvariantCulture)
            .Replace("_", "", StringComparison.InvariantCulture)
            .Replace(".", "", StringComparison.InvariantCulture);

        // Add services
        builder.Services.TryAddSingleton<OnePasswordCliInstallationManager>();
        builder.Services.TryAddSingleton<IOnePasswordClient, OnePasswordCliClient>();

        var onePasswordResource = new OnePasswordFieldResource(name, accountId, vaultId, itemId, field);

        var rb = builder.AddResource(onePasswordResource)
            .OnInitializeResource(static async (onePasswordResource, e, ct) =>
            {
                var logger = e.Services.GetRequiredService<ResourceLoggerService>().GetLogger(onePasswordResource);
                var eventing = e.Services.GetRequiredService<IDistributedApplicationEventing>();
                var notificationService = e.Services.GetRequiredService<ResourceNotificationService>();
                var onePasswordCliInstallationManager = e.Services.GetRequiredService<OnePasswordCliInstallationManager>();
                var onePasswordClient = e.Services.GetRequiredService<IOnePasswordClient>();
                // Ensure CLI is available
                await onePasswordCliInstallationManager.EnsureInstalledAsync(ct).ConfigureAwait(false);

                try
                {
                    var assembly = Assembly.GetEntryAssembly();

                    if (!SecretsStore.TryGetUserSecret(assembly, onePasswordResource.ConfigurationKey, out string field))
                    {

                        logger.LogInformation("Getting secret value from 1Password: account='{AccountId}' vault='{VaultId}' item='{ItemId}' field='{Field}'",
                            onePasswordResource.AccountId,
                            onePasswordResource.VaultId,
                            onePasswordResource.ItemId,
                            onePasswordResource.Field
                            );

                        field = await onePasswordClient.GetFieldAsync(onePasswordResource.AccountId, onePasswordResource.VaultId, onePasswordResource.ItemId, onePasswordResource.Field, logger, ct).ConfigureAwait(false);

                        if (SecretsStore.TrySetUserSecret(assembly, onePasswordResource.ConfigurationKey, field))
                        {
                            logger.LogWarning("Could not set field in user secrets. Ensure the assembly has a UserSecretsIdAttribute.");
                        }

                    }

                    if (field is null)
                    {
                        throw new DistributedApplicationException($"The field '{onePasswordResource.Field}' was not found in the item '{onePasswordResource.ItemId}' in vault '{onePasswordResource.VaultId}' in account '{onePasswordResource.AccountId}'.");
                    }

                    onePasswordResource.Value = field;

                    await notificationService.PublishUpdateAsync(onePasswordResource, s =>
                    {
                        return s with
                        {
                            Properties = s.Properties.SetResourceProperty(KnownProperties.OnePasswordField.Value, field),
                            State = KnownResourceStates.Running
                        };
                    })
                    .ConfigureAwait(false);

                    onePasswordResource.WaitForValueTcs?.TrySetResult(field);

                    logger.LogInformation("Successfully retrieved {Field} value from 1Password", field);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to get field value from 1Password");
                    throw;
                }
            });

        return rb;
    }
}
