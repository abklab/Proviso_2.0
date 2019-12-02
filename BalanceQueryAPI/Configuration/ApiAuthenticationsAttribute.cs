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
            IEnumerable<string> requestHeaderKey;
            IEnumerable<string> requestHeaderClient;

            try
            {
                var checkHeaderKey = request.Headers.TryGetValues("CLIENT_ACCESS_APIKEY", out requestHeaderKey);
                var checkclientName = request.Headers.TryGetValues("API_CLIENT", out requestHeaderClient);

                if (checkHeaderKey && checkclientName)
                {
                    string apikey = requestHeaderKey.FirstOrDefault();
                    string clientname = requestHeaderClient.FirstOrDefault();
                    var isActiveClient = services.AuthenticateKey(apikey, clientname);
                    if (isActiveClient)
                        isValidKey = true;

                }
                if (!isValidKey)
                {
                    return request.CreateResponse(System.Net.HttpStatusCode.Forbidden, "Something went wrong or Unauthorized Access. Contacting the Systems Admin.");
                }

                var response = await base.SendAsync(request, cancellationToken);
                return response;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return request.CreateResponse(System.Net.HttpStatusCode.BadRequest, "Bad Request");
            }
        }
    }
}