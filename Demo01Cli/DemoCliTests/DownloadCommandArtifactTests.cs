using CliFx.Infrastructure;
using Demo01Cli;
using DotNet.Sdk.Extensions.Testing.HttpMocking.HttpMessageHandlers;
using DotNet.Sdk.Extensions.Testing.HttpMocking.HttpMessageHandlers.ResponseMocking;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DemoCliTests
{
    public class DownloadCommandArtifactTests
    {
        [Fact]
        public async Task Test1()
        {
            // prepare the http mocks
            var httpResponseMessageMock = new HttpResponseMessageMockBuilder()
                .RespondWith(httpRequestMessage =>
                {
                    var zipFileStream = File.OpenRead("TestData/from-set-with-github-step-json-output.zip");
                    var streamContent = new StreamContent(zipFileStream);
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Zip);

                    var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                    httpResponseMessage.Content = streamContent;
                    return httpResponseMessage;
                })
                .Build();

            // add the mocks to the http handler
            var handler = new TestHttpMessageHandler();
            handler.MockHttpResponse(httpResponseMessageMock);

            // instantiate the http client with the test handler
            var httpClient = new HttpClient(handler);

            var downloadCommand = new DownloadArtifactCommand(httpClient)
            {
                Repo = "edumserrano/share-jobs-data",
                ArtefactId = "386521405",
                Token = "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"
            };

            var testConsole = new FakeInMemoryConsole();

            await downloadCommand.ExecuteAsync(testConsole);

            var output = testConsole.ReadOutputString();
            output.ShouldContain("George Washington");
            output.ShouldContain("400 Mockingbird Lane");
        }


        [Fact]
        public async Task Test2()
        {
            // prepare the http mocks
            var httpResponseMessageMock = new HttpResponseMessageMockBuilder()
                .RespondWith(httpRequestMessage =>
                {
                    var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.Gone);
                    return httpResponseMessage;
                })
                .Build();

            // add the mocks to the http handler
            var handler = new TestHttpMessageHandler();
            handler.MockHttpResponse(httpResponseMessageMock);

            // instantiate the http client with the test handler
            var httpClient = new HttpClient(handler);

            const string artefactId = "blah";
            var downloadCommand = new DownloadArtifactCommand(httpClient)
            {
                Repo = "edumserrano/share-jobs-data",
                ArtefactId = artefactId,
                Token = "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"
            };

            var testConsole = new FakeInMemoryConsole();
            await downloadCommand.ExecuteAsync(testConsole);

            var output = testConsole.ReadOutputString();
            output.ShouldBe($"Oh dear, user, the thing with ID {artefactId} you have searched for could not be found.");
        }
    }
}
