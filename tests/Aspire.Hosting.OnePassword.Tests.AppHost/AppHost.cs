using Aspire.Hosting.OnePassword;

var builder = DistributedApplication.CreateBuilder(args);

var username = builder.AddOnePasswordField(
    "Developer",
    "Aspire - OnePassword - Test",
    "username"
    );

var credential = builder.AddOnePasswordField(
    "Developer",
    "Aspire - OnePassword - Test",
    "credential"
    );


builder.AddProject<Projects.Aspire_Hosting_OnePassword_Tests_WebApp>("tests-webapp")
    .WithEnvironment("username", username)
    .WithEnvironment("credential", credential);


builder.Build().Run();
