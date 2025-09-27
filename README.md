# 1Password Hosting integration for .NET Aspire

Hosting integration for retrieving fields from 1Password with Aspire.

```csharp

var somefield = builder.AddOnePasswordField(
    "somefield", // For aspire
    "MyVault",
    "MyItem",
    "somefield"
    // Optional: accountId: string
    );

var someProject = builder.AddProject<Projects.SomeProject>("someproject")
    .WithEnvironment("SOME__ENVIRONMENT__VARIABLE__NAME", somefield);

```

The integration uses 1Password CLI to retrieve the field and then stores it in the AppHosts user-secrets for reuse.
