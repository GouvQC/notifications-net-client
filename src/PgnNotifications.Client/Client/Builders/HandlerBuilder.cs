using System;
using System.Net.Http;
using System.Net;
using PgnNotifications.Client.Handlers;

namespace PgnNotifications.Client.Builders
{
    /// <summary>
    /// Fournit une composition contrôlée des handlers HTTP utilisés par <see cref="NotificationClientBuilder"/> 
    /// (par ex. gestion du retry, du proxy et du logging).
    /// Cette classe empêche l’injection de handlers arbitraires afin de préserver la sécurité de la chaîne HTTP 
    /// — notamment en évitant tout accès direct aux en-têtes sensibles comme <c>Authorization</c> ou au corps des requêtes.
    /// </summary>
    public class HandlerBuilder
    {
        private HttpMessageHandler _handler;
        private bool _hasTestHandler = false;

        private HandlerBuilder()
        {
            _handler = new HttpClientHandler();
        }

        private HandlerBuilder(HttpMessageHandler handler)
        {
            _handler = handler;
            _hasTestHandler = true;
        }

        public static HandlerBuilder Create()
        {
            return new HandlerBuilder();
        }

        internal static HandlerBuilder Create(HttpMessageHandler handler)
        {
            return new HandlerBuilder(handler);
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
