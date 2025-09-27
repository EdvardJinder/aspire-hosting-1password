using System;

namespace Aspire.Hosting.OnePassword;

internal sealed record UserSignInStatus(string Status, string AccountId, string Email, string UserId)
{
    public bool IsLoggedIn => string.Equals(Status, "Logged in", StringComparison.OrdinalIgnoreCase);
}
