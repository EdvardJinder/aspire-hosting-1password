//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;


//namespace Aspire.Hosting.OnePassword;
//#pragma warning disable ASPIREINTERACTION001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
//internal sealed class DevTunnelLoginManager(
//    IOnePasswordClient onePasswordClient,
//    IInteractionService interactionService,
//    IConfiguration configuration,
//    ILogger<DevTunnelLoginManager> logger) : CoalescingAsyncOperation
//{
//    private readonly IOnePasswordClient _onePasswordClient = onePasswordClient;
//    private readonly IInteractionService _interactionService = interactionService;
//    private readonly IConfiguration _configuration = configuration;
//    private readonly ILogger<DevTunnelLoginManager> _logger = logger;

//    public Task EnsureUserLoggedInAsync(CancellationToken cancellationToken = default) => RunAsync(cancellationToken);

//    protected override async Task ExecuteCoreAsync(CancellationToken cancellationToken)
//    {
//        while (true)
//        {
//            _logger.LogDebug("Checking 1Password login status");
//            var loginStatus = await _onePasswordClient.GetUserSignInStatus(null, _logger, cancellationToken).ConfigureAwait(false);
//            if (loginStatus.IsLoggedIn)
//            {
//                _logger.LogDebug("User already logged in to 1Password service as {User}", loginStatus.Email);
//                // Already logged in
//                break;
//            }
//            else
//            {
//                if (_interactionService.IsAvailable)
//                {
                   
//                    loginStatus = await _onePasswordClient.GetUserSignInStatus(null, _logger, cancellationToken).ConfigureAwait(false);
//                }
//                if (!loginStatus.IsLoggedIn)
//                {
//                    // Trigger the login flow
//                    _logger.LogInformation("Initiating 1Password login");
//                    loginStatus = await _onePasswordClient.UserSignInAsync(null, _logger, cancellationToken).ConfigureAwait(false);

//                    if (loginStatus.IsLoggedIn)
//                    {
//                        // Successfully logged in
//                        _logger.LogInformation("User logged in to 1Password service as {Username} with {Provider}", loginStatus.Username, loginStatus.Provider);
//                        break;
//                    }
//                }
//                else
//                {
//                    // Logged in from another window while we were prompting
//                    _logger.LogDebug("User already logged in to 1Password service as {Username} with {Provider}", loginStatus.Username, loginStatus.Provider);
//                    break;
//                }

//                _logger.LogDebug("User login to 1Password service failed, retrying login prompt");
//            }
//        }
//    }
//}
//#pragma warning restore ASPIREINTERACTION001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.