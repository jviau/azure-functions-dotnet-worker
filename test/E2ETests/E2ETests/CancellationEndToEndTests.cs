// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.Azure.Functions.Tests.E2ETests
{
    [Collection(Constants.FunctionAppCollectionName)]
    public class CancellationEndToEndTests
    {
        // TODO: add `functionTimeout` to host.json to force timeout
        private readonly FunctionAppFixture _fixture;

        public CancellationEndToEndTests(FunctionAppFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task HttpTrigger_HandlesCancellationToken_SuccessfullyCancelsInvocation()
        {
            var expectedStatusCode = HttpStatusCode.ServiceUnavailable;
            var expectedMessage = "Invocation cancelled";

            var response = await HttpHelpers.InvokeHttpTrigger("HandlesCancellationToken");

            var actualMessage = await response.Content.ReadAsStringAsync();

            Assert.Equal(expectedStatusCode, response.StatusCode);

            if (!string.IsNullOrEmpty(expectedMessage))
            {
                Assert.False(string.IsNullOrEmpty(actualMessage));
                Assert.Contains(expectedMessage, actualMessage);
            }
        }

        [Fact]
        public async Task HttpTrigger_DoesNotHandleCancellationToken_Throws_AndThenWhat()
        {
            var expectedStatusCode = HttpStatusCode.InternalServerError;

            var response = await HttpHelpers.InvokeHttpTrigger("DoesNotHandleCancellationToken");
            var actualMessage = await response.Content.ReadAsStringAsync();

            Assert.Equal(expectedStatusCode, response.StatusCode);
        }
    }
}
