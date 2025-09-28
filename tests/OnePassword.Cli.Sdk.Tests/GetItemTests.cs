using Microsoft.Extensions.DependencyInjection;

namespace OnePassword.Cli.Sdk.Tests;

public class GetItemTests
{

    [Theory]
    [InlineData("H5GH4RDY6BFHVDJ2JO32OOWEOI", "qwwh4velrmj6p4snz6je3bjpmy", "iglqlo2pjgsvroc3n6vpjwlpm4", "username", "someusername")]
    public async Task ShouldRetrieveField(
        string accountId,
        string vaultId,
        string itemId,
        string field,
        string expectedValue
        )
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.TryAdd1PasswordCliServices();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var client = serviceProvider.GetRequiredService<IOnePasswordClient>();

        var value = await client.GetFieldAsync(accountId, vaultId, itemId, field, null, default);

        Assert.Equal(expectedValue, value);
    }
}
