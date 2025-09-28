using Microsoft.Extensions.DependencyInjection;

namespace OnePassword.Cli.Sdk.Tests;


public class GetItemTests
{

    [Fact]
    public async Task ShouldRetrieveField()
    {
        var accountId = Environment.GetEnvironmentVariable("OP_ACCOUNT_ID") ?? throw new InvalidOperationException("OP_ACCOUNT_ID environment variable is not set.");
        var vaultId = Environment.GetEnvironmentVariable("OP_VAULT_ID") ?? throw new InvalidOperationException("OP_VAULT_ID environment variable is not set.");
        var itemId = Environment.GetEnvironmentVariable("OP_ITEM_ID") ?? throw new InvalidOperationException("OP_ITEM_ID environment variable is not set.");
        var fieldName = Environment.GetEnvironmentVariable("OP_FIELD") ?? throw new InvalidOperationException("OP_FIELD environment variable is not set.");
        var expectedFieldValue = Environment.GetEnvironmentVariable("OP_FIELD_VALUE") ?? throw new InvalidOperationException("OP_FIELD_VALUE environment variable is not set.");

        var serviceCollection = new ServiceCollection();
        serviceCollection.TryAdd1PasswordCliServices();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var client = serviceProvider.GetRequiredService<IOnePasswordClient>();

        var value = await client.GetFieldAsync(accountId, vaultId, itemId, fieldName, null, default);

        Assert.Equal(expectedFieldValue, value);
    }
}
