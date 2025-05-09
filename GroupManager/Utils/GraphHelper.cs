using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Identity.Client;
using Microsoft.Kiota.Abstractions.Authentication;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GroupManager.Utils
{
    public class GraphHelper
    {
        private static GraphServiceClient _graphClient;

        // Initialize the GraphServiceClient
        public static async Task InitializeGraphClientAsync()
        {
            var accessToken = await GetGraphAccessToken();
            var tokenProvider = new TokenProvider(accessToken);
            var authProvider = new BaseBearerTokenAuthenticationProvider(tokenProvider);

            _graphClient = new GraphServiceClient(authProvider);
        }

        // Fetch users in the tenant
        public static async Task<List<User>> GetUsersAsync()
        {
            if (_graphClient == null)
            {
                await InitializeGraphClientAsync();
            }

            var result = await _graphClient.Users.GetAsync(requestConfiguration =>
            {
                requestConfiguration.QueryParameters.Select = new string[] { "Id", "DisplayName", "JobTitle", "Mail" };
            });

            return result.Value;
        }

        // Get the access token for Microsoft Graph
        private static async Task<string> GetGraphAccessToken()
        {
            IConfidentialClientApplication confidentialClient = MsalAppBuilder.BuildConfidentialClientApplication();
            var userAccount = await confidentialClient.GetAccountAsync(ClaimsPrincipal.Current.GetMsalAccountId());

            var result = await confidentialClient.AcquireTokenSilent(new string[] { "user.readbasic.all" }, userAccount).ExecuteAsync();

            return result.AccessToken;
        }
    }
}