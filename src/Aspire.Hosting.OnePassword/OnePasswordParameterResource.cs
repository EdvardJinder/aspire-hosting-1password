using Aspire.Hosting.ApplicationModel;


namespace Aspire.Hosting.OnePassword;

public sealed class OnePasswordParameterResource(
    [ResourceName] string name,
     string accountId,
       string vaultId,
       string itemId,
       string field
    )
    : ParameterResource(name, (d) => d.GetDefaultValue(), secret: true)
{
    public string AccountId { get; } = accountId;
    public string VaultId { get; } = vaultId;
    public string ItemId { get; } = itemId;
    public string Field { get; } = field;
}

//public sealed class OnePasswordParameterResource(
//    string accountId,
//    string vaultId,
//    string itemId,
//    string field,
//    string command,
//    string workingDirectory
//    )
//    : ExecutableResource($"{accountId}-{vaultId}-{itemId}-{field}", command, workingDirectory)
//{

//    public string AccountId { get; } = accountId;
//    public string VaultId { get; } = vaultId;
//    public string ItemId { get; } = itemId;
//    public string Field { get; } = field;

//    public string Value { get; set; } = string.Empty;
//}
