using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PgnNotifications.Client.Handlers
{
    /// <summary>
    /// Handler de logging contrôlé pour la chaîne de handlers du HandlerBuilder.
    /// </summary>
    public class LoggingHandler : DelegatingHandler
    {
        private readonly Action<string> _logAction;

        public LoggingHandler(HttpMessageHandler innerHandler, Action<string> logAction = null) : base(innerHandler)
        {
            _logAction = logAction ?? Console.WriteLine;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _logAction($"[LOG] Request: {request.Method} {request.RequestUri}");
            var response = await base.SendAsync(request, cancellationToken);
            _logAction($"[LOG] Response: {(int)response.StatusCode} {response.ReasonPhrase}");
            return response;
        }
    }
}
