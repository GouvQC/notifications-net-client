using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PgnNotifications.Client.Tests.Utils
{
    /// <summary>
    /// Handler factice pour simuler des timeouts ou des délais lors des tests.
    /// </summary>
    public class TimeoutHandler : HttpMessageHandler
    {
        private readonly int delayMilliseconds;

        public TimeoutHandler(int delayMilliseconds = 5000)
        {
            this.delayMilliseconds = delayMilliseconds;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await Task.Delay(delayMilliseconds, cancellationToken);

            return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("{}")
            };
        }
    }
}
