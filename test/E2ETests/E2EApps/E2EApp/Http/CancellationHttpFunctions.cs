// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.Functions.Worker.E2EApp
{
    public static class CancellationHttpFunctions
    {
        [Function(nameof(HandlesCancellationToken))]
        public static async Task<HttpResponseData> HandlesCancellationToken(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequestData req,
            FunctionContext context,
            CancellationToken cancellationToken)
        {
            var logger = context.GetLogger(nameof(HandlesCancellationToken));
            logger.LogInformation(".NET Worker HTTP trigger function processed a request");

            try
            {
                var response = req.CreateResponse(HttpStatusCode.OK);
                response.WriteString($"Hello world!");

                await Task.Delay(6000, cancellationToken);

                return response;
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("Function invocation cancelled");

                var response = req.CreateResponse(HttpStatusCode.ServiceUnavailable);
                response.WriteString("Invocation cancelled");

                return response;
            }
        }

        [Function(nameof(DoesNotHandleCancellationToken))]
        public static async Task<HttpResponseData> DoesNotHandleCancellationToken(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequestData req,
            FunctionContext context,
            CancellationToken cancellationToken)
        {
            var logger = context.GetLogger(nameof(DoesNotHandleCancellationToken));
            logger.LogInformation(".NET Worker HTTP trigger function processed a request");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.WriteString($"Hello world!");

            await Task.Delay(6000, cancellationToken);

            return response;
        }
    }
}
