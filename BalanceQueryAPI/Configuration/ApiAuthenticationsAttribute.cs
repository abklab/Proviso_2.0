using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using BalanceQueryAPI.DAL;

namespace BalanceQueryAPI.Configuration
{
    public class ApiAuthenticationsAttribute : DelegatingHandler
    {
        private readonly DataAccessLayer services = new DataAccessLayer();

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //// Please Use the following if you want to enforce https/SSL in web apps;

            //if (request.RequestUri.Scheme != Uri.UriSchemeHttps)
            //{
            //    // Forbidden (or do a redirect)...
            //    return request.CreateResponse(System.Net.HttpStatusCode.ServiceUnavailable, "Service Unavailable or Resource has been moved.");
            //}

            bool isValidKey = false;
            IEnumerable<string> requestHeaders;

            var checkHeaderKey = request.Headers.TryGetValues("CLIENT_ACCESS_APIKEY", out requestHeaders);


            if (checkHeaderKey)
            {
                string apikey = requestHeaders.FirstOrDefault();
                var isActiveClient = services.AuthenticateKey(apikey);
                if (isActiveClient)
                    isValidKey = true;

            }
            if (!isValidKey)
            {
                return request.CreateResponse(System.Net.HttpStatusCode.Forbidden, "You are NOT authorized to access this resource.");
            }

            var response = await base.SendAsync(request, cancellationToken);
            return response;
        }
    }
}