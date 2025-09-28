using Aspire.Hosting.OnePassword;

var builder = DistributedApplication.CreateBuilder(args);

string? accountId = builder.Configuration["1PASSWORD__ACCOUNT__ID"];

var username = builder.AddOnePasswordField(
    "username",
    "Developer",
    "Aspire - OnePassword - Test",
    "username",
    accountId: accountId
    );

var credential = builder.AddOnePasswordField(
    "credential",
    "Developer",
    "Aspire - OnePassword - Test",
    "credential",
    accountId: accountId
    );


builder.AddProject<Projects.Aspire_Hosting_OnePassword_Tests_WebApp>("tests-webapp")
    .WithEnvironment("username", username)
    .WithEnvironment("credential", credential);


builder.Build().Run();
