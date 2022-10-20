using CliFx.Infrastructure;
using Demo01Cli;
using DotNet.Sdk.Extensions.Testing.HttpMocking.HttpMessageHandlers;
using DotNet.Sdk.Extensions.Testing.HttpMocking.HttpMessageHandlers.ResponseMocking;
using Shouldly;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DemoCliTests
{
    public class ListArtefactTests
    {
        [Fact]
        public async Task Test1()
        {
            // prepare the http mocks
            var httpResponseMessageMock = new HttpResponseMessageMockBuilder()            
                .RespondWith(httpRequestMessage =>
                {
                    var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.Created);
                    httpResponseMessage.Content = new StringContent("set-data");
                    return httpResponseMessage;
                })
                .Build();

            // add the mocks to the http handler
            var handler = new TestHttpMessageHandler();
            handler.MockHttpResponse(httpResponseMessageMock);

            // instantiate the http client with the test handler
            var httpClient = new HttpClient(handler);

            var listCommand = new ListArtifactsCommand(httpClient)
            {
                Repo = "edumserrano/share-jobs-data",
                RunId = "3185445164",
                Token = "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"
            };

            var testConsole = new FakeInMemoryConsole();

            await listCommand.ExecuteAsync(testConsole);

            var output = testConsole.ReadOutputString();

            output.ShouldContain("set-data");
        }
    }
}