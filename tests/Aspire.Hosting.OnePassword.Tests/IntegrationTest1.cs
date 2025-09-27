
namespace Aspire.Hosting.OnePassword.Tests.Tests;

public class OnePasswordResourceBuilderExtensionsTests
{
    [Fact]
    public async Task Test()
    {
        var builder = DistributedApplication.CreateBuilder();

        var param = builder.AddParameterFrom1Password(
            "test-param",
            "H5GH4RDY6BFHVDJ2JO32OOWEOI",
            "pqiwxlghzsqf4teetuv22cdxiu",
            "b6adzymfiba56kuqwddyu5wska",
            "username"
            );

        var projectA = builder.AddProject<ProjectA>("project-a")
            .WithEnvironment("OnePasswordUsername", param);

        using var app = builder.Build();

        var values = await projectA.Resource.GetArgumentValuesAsync();

        //var expectedKey = $"OnePasswordUsername";
        //Assert.Contains(expectedKey, values.Keys);

        //var expectedValue = "edvard@jinder.se";
        //Assert.Equal(expectedValue, values[expectedKey]);
    }

    private sealed class ProjectA : IProjectMetadata
    {
        public string ProjectPath => "projectA";

        public LaunchSettings LaunchSettings { get; } = new();
    }
}



