﻿using System.Net;
using System.Net.Sockets;

using Microsoft.Extensions.Logging;

namespace Common.Logging;

public class LoggingDelegatingHandler(ILogger<LoggingDelegatingHandler> logger) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Sending request to {Url}", request.RequestUri);
            var response = await base.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("Received a success response from {Url}", response.RequestMessage?.RequestUri);
            }
            else
            {
                logger.LogWarning("Received a non-success status code {StatusCode} from {Url}",
                    (int)response.StatusCode, response.RequestMessage?.RequestUri);
            }

            return response;
        }
        catch (HttpRequestException ex) when (ex.InnerException is SocketException se &&
                                              se.SocketErrorCode == SocketError.ConnectionRefused)
        {

            var isDefaultPort = request.RequestUri?.IsDefaultPort ?? false;
            var hostWithPort = isDefaultPort
                    ? request.RequestUri?.DnsSafeHost
                    : $"{request.RequestUri?.DnsSafeHost}:{request.RequestUri?.Port}";

            logger.LogCritical(ex, "Unable to connect to {Host}. Please check the " +
                                   "configuration to ensure the correct URL for the service " +
                                   "has been configured", hostWithPort);
        }

        return new HttpResponseMessage(HttpStatusCode.BadGateway)
        {
            RequestMessage = request
        };
    }
}
