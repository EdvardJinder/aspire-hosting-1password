using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Eventing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;


namespace Aspire.Hosting.OnePassword;

public static class OnePasswordParameterResourceBuilderExtensions
{
    public static IResourceBuilder<OnePasswordParameterResource> AddParameterFrom1Password(
       this IDistributedApplicationBuilder builder,
       [ResourceName] string name,
       string accountId,
       string vaultId,
       string itemId,
       string field
       )
    {

        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(name);

        // Add services
        //builder.Services.TryAddSingleton<OnePasswordCliInstallationManager>();
        //builder.Services.TryAddSingleton<OnePasswordLoginManager>();
        builder.Services.TryAddSingleton<LoggedOutNotificationManager>();
        builder.Services.TryAddSingleton<IOnePasswordClient, OnePasswordCliClient>();

        var workingDirectory = builder.AppHostDirectory;

        string? value = null;
        var onePasswordResource = new OnePasswordParameterResource(name, accountId, vaultId, itemId, field);

        var rb = builder.AddResource(onePasswordResource)
            .OnBeforeResourceStarted(static async (onePasswordResource, e, ct) =>
            {
                var logger = e.Services.GetRequiredService<ResourceLoggerService>().GetLogger(onePasswordResource);
                var eventing = e.Services.GetRequiredService<IDistributedApplicationEventing>();
                //var onePasswordCliInstallationManager = e.Services.GetRequiredService<OnePasswordCliInstallationManager>();
                //var onePasswordEnvironmentManager = e.Services.GetRequiredService<OnePasswordLoginManager>();
                var onePasswordClient = e.Services.GetRequiredService<IOnePasswordClient>();

                //// Ensure CLI is available
                //await onePasswordCliInstallationManager.EnsureInstalledAsync(ct).ConfigureAwait(false);

                //// Login to the 1Password service if needed
                //logger.LogInformation("Ensuring user is logged in to 1Password service");
                //await onePasswordEnvironmentManager.EnsureUserLoggedInAsync(ct).ConfigureAwait(false);

                try
                {
                    logger.LogInformation("Getting secret value from 1Password: account='{AccountId}' vault='{VaultId}' item='{ItemId}' field='{Field}'", 
                        onePasswordResource.AccountId,
                        onePasswordResource.VaultId,
                        onePasswordResource.ItemId,
                        onePasswordResource.Field
                        );

                    // https://start.1password.com/open/i?a=H5GH4RDY6BFHVDJ2JO32OOWEOI&v=pqiwxlghzsqf4teetuv22cdxiu&i=b6adzymfiba56kuqwddyu5wska&h=my.1password.eu
                    string? field = await onePasswordClient.GetFieldAsync(onePasswordResource.AccountId, onePasswordResource.VaultId, onePasswordResource.ItemId, onePasswordResource.Field, ct).ConfigureAwait(false);

                    if (field is null)
                    {
                        throw new DistributedApplicationException($"The field '{onePasswordResource.Field}' was not found in the item '{onePasswordResource.ItemId}' in vault '{onePasswordResource.VaultId}' in account '{onePasswordResource.AccountId}'.");
                    }

                    logger.LogInformation("Successfully retrieved {Secret} value from 1Password", field);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to get secret value from 1Password");
                    throw;
                }
            });

        return rb;
    }
}
