﻿using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NuGet.Insights
{
    public class LoggingHandler : DelegatingHandler
    {
        private readonly ILogger<LoggingHandler> _logger;

        public LoggingHandler(ILogger<LoggingHandler> logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "  {Method} {RequestUri}",
                request.Method,
                request.RequestUri);
            var stopwatch = Stopwatch.StartNew();
            var response = await base.SendAsync(request, cancellationToken);
            _logger.LogInformation(
                "  {StatusCode} {RequestUri} {ElapsedMs}ms",
                response.StatusCode,
                response.RequestMessage.RequestUri,
                stopwatch.Elapsed.TotalMilliseconds);
            return response;
        }
    }
}
