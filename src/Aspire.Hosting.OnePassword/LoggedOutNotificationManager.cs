using System.Threading;
using System.Threading.Tasks;

namespace Aspire.Hosting.OnePassword;
#pragma warning disable ASPIREINTERACTION001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
internal sealed class LoggedOutNotificationManager(IInteractionService interactionService) : CoalescingAsyncOperation
{
    public Task NotifyUserLoggedOutAsync(CancellationToken cancellationToken = default) => RunAsync(cancellationToken);

    protected override async Task ExecuteCoreAsync(CancellationToken cancellationToken)
    {
        if (interactionService.IsAvailable)
        {
            _ = await interactionService.PromptNotificationAsync(
                "1Password",
                "1Password authentication has expired. Restart to re-authenticate.",
                new NotificationInteractionOptions() { Intent = MessageIntent.Warning },
                cancellationToken).ConfigureAwait(false);
        }
    }
}
#pragma warning restore ASPIREINTERACTION001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
