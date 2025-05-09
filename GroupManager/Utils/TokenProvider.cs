using Microsoft.Kiota.Abstractions.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Web;

namespace GroupManager.Utils
{
    public class TokenProvider : IAccessTokenProvider
    {
        private readonly string _accessToken;
        public TokenProvider(string accessToken)
        {
            this._accessToken = accessToken;
        }

        public AllowedHostsValidator AllowedHostsValidator { get; }

        public Task<string> GetAuthorizationTokenAsync(Uri uri, Dictionary<string, object> additionalAuthenticationContext = default, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(this._accessToken);
        }

    }
}