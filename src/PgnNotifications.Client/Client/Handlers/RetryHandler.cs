using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PgnNotifications.Client.Handlers
{
    /// <summary>
    /// Handler de retry simple pour la chaîne de handlers contrôlée par HandlerBuilder.
    /// </summary>
    internal class RetryHandler : DelegatingHandler
    {
        private readonly int _maxRetries;

        public RetryHandler(HttpMessageHandler innerHandler, int maxRetries) : base(innerHandler)
        {
            _maxRetries = maxRetries;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            int retry = 0;
            HttpResponseMessage response = null;
            while (retry < _maxRetries)
            {
                response = await base.SendAsync(request, cancellationToken);
                if (response.IsSuccessStatusCode)
                    return response;
                retry++;
            }
            return response;
        }
    }
}
