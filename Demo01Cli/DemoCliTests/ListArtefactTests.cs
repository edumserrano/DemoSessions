using CliFx.Infrastructure;
using Demo01Cli;
using Demo01Cli.Models;
using Demo01Cli.Services;
using DotNet.Sdk.Extensions.Testing.HttpMocking.HttpMessageHandlers;
using DotNet.Sdk.Extensions.Testing.HttpMocking.HttpMessageHandlers.ResponseMocking;
using Shouldly;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace DemoCliTests
{
    public class ListArtefactTests
    {
        [Fact]
        public async Task GetListArtifacts_ReturnsData()
        {
            var listArtifactResponse = new ListResponseModel()
            {
                total_count = 1,
                artifacts = new Artifact[]
            {
                new()
                {
                    name = "some-artifact-name",
                    id = 0
                }
            }
            };
            //
            var expected = JsonSerializer.Serialize(listArtifactResponse);


            // prepare the http mocks
            var httpResponseMessageMock = new HttpResponseMessageMockBuilder()            
                .RespondWith(httpRequestMessage =>
                {
                    var data = JsonSerializer.Serialize(listArtifactResponse);
                    var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.Created);
                    httpResponseMessage.Content = new StringContent(data);
                    return httpResponseMessage;
                })
                .Build();

            // add the mocks to the http handler
            var handler = new TestHttpMessageHandler();
            handler.MockHttpResponse(httpResponseMessageMock);

            // instantiate the http client with the test handler
            var httpClient = new HttpClient(handler);
            var listService = new ListService(httpClient);

            var listCommand = new ListArtifactsCommand(listService)
            {
                Repo = "edumserrano/share-jobs-data",
                RunId = "3185445164",
                Token = "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"
            };

            var testConsole = new FakeInMemoryConsole();

            await listCommand.ExecuteAsync(testConsole);

            var output = testConsole.ReadOutputString();

            output.ShouldContain(expected);
        }
    }
}