using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Providers.Entities;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace Ndot.MessageHandlers
{
    public class BasicAuthMessageHandler : DelegatingHandler
    {
        private const string BasicAuthResponseHeader = "WWW-Authenticate";
        private const string BasicAuthResponseHeaderValue = "Basic";


        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            AuthenticateUserIfPossible(request);

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized && !response.Headers.Contains(BasicAuthResponseHeader))
                response.Headers.Add(BasicAuthResponseHeader, BasicAuthResponseHeaderValue);

            return response;
        }

        private void AuthenticateUserIfPossible(HttpRequestMessage request)
        {
            var authValue = request.Headers.Authorization;

            if (authValue == null || authValue.Scheme != "Basic" || String.IsNullOrWhiteSpace(authValue.Parameter))
                return;

            var credentials = Encoding.ASCII.GetString(Convert.FromBase64String(authValue.Parameter))
                .Split(new[] { ':' });
            if (credentials.Length != 2 || string.IsNullOrEmpty(credentials[0])
                || string.IsNullOrEmpty(credentials[1])) 
                return;

            var username = credentials[0];
            var password = credentials[1];

            if (password == (username + username))
            {
                Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(username), null);
            }
        }
        
    }
}