using System;
using System.Net.Http;
using System.Net;
using PgnNotifications.Client.Handlers;

namespace PgnNotifications.Client.Builders
{
    /// <summary>
    /// Permet la composition contrôlée de handlers HTTP (retry, logging, proxy) pour NotificationClientBuilder.
    /// Empêche l'injection de handlers arbitraires pour garantir la sécurité de la chaîne (ex: pas d'accès direct à Authorization/body).
    /// </summary>
    public class HandlerBuilder
    {
        private HttpMessageHandler _handler;
        private bool _hasTestHandler = false;

        public HandlerBuilder()
        {
            _handler = new HttpClientHandler();
        }

        public HandlerBuilder WithRetry(int maxRetries = 3)
        {
            EnsureNotTestHandler();
            _handler = new RetryHandler(_handler, maxRetries);
            return this;
        }

        public HandlerBuilder WithLogging(Action<string> logAction = null)
        {
            EnsureNotTestHandler();
            _handler = new LoggingHandler(_handler, logAction);
            return this;
        }

        public HandlerBuilder WithProxy(IWebProxy proxy)
        {
            EnsureNotTestHandler();
            if (_handler is HttpClientHandler httpClientHandler)
                httpClientHandler.Proxy = proxy;
            return this;
        }

        /// <summary>
        /// Pour les tests uniquement : injecte un handler custom (ex: mock). Interdit en usage normal.
        /// </summary>
        public HandlerBuilder WithTestHandler(HttpMessageHandler testHandler)
        {
            _handler = testHandler;
            _hasTestHandler = true;
            return this;
        }

        public HttpMessageHandler Build() => _handler;

        private void EnsureNotTestHandler()
        {
            if (_hasTestHandler)
                throw new InvalidOperationException("Impossible de composer d'autres handlers après WithTestHandler (usage réservé aux tests).");
        }
    }
}
