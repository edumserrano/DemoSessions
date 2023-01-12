using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CliFx.Infrastructure;
using Demo01Cli;
using Demo01Cli.Models;
using DotNet.Sdk.Extensions.Testing.HttpMocking.HttpMessageHandlers.ResponseMocking;
using DotNet.Sdk.Extensions.Testing.HttpMocking.HttpMessageHandlers;
using Shouldly;
using Xunit;

namespace DemoCliTests;

public class GetArtefactTests
{

    [Fact]
    public async Task GetArtifactByName_DoesNotExist_ReturnsEmpty()
    {
        var artifactId = new Random().Next();
        var runId = "some-run-id";
        var artifactName = "some-non-existant-artifact-name";
        var listArtifactResponse = new ListResponseModel()
        {
            total_count = 1,
            artifacts = new Artifact[]
            {
                new()
                {
                    name = "some-artifact-name",
                    id = artifactId
                }
            }
        };

        var listArtifactsHttpResponseMock = new HttpResponseMessageMockBuilder()
            .Where(httpRequestMessage => httpRequestMessage.RequestUri.ToString() == $"https://api.github.com/repos/test_repo/actions/runs/{runId}/artifacts")
            .RespondWith(httpRequestMessage =>
            {
                var data = JsonSerializer.Serialize(listArtifactResponse);
                var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                responseMessage.Content = new StringContent(data);
                return responseMessage;
            })
            .Build();

        // add the mocks to the http handler
        var handler = new TestHttpMessageHandler();
        handler.MockHttpResponse(listArtifactsHttpResponseMock);

        var httpClient = new HttpClient(handler);
        var sut = new GetArtifactCommand(httpClient)
        {
            Repo = "test_repo",
            ArtifactName = artifactName,
            RunId = runId,
            Token = ""
        };

        var console = new FakeInMemoryConsole();
        await sut.ExecuteAsync(console);

        var output = console.ReadOutputString();
        output.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetArtifactByName_ReturnsData()
    {
        var artifactId = new Random().Next();
        var runId = "some-run-id";
        var artifactName = "some-artifact-name";
        var getArtifactResponse = new GetResponseModel()
        {
            id = artifactId
        };

        var listArtifactResponse = new ListResponseModel()
        {
            total_count = 1,
            artifacts = new Artifact[]
            {
                new()
                {
                    name = artifactName,
                    id = artifactId
                }
            }
        };

        // prepare the http mocks
        var getArtifactHttpResponseMock = new HttpResponseMessageMockBuilder()
            .Where(httpRequestMessage =>
            {
                return httpRequestMessage.RequestUri.ToString()
                    .Equals($"https://api.github.com/repos/test_repo/actions/artifacts/{artifactId}");
            })
            .RespondWith(httpRequestMessage =>
            {
                var data = JsonSerializer.Serialize(getArtifactResponse);

                var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                httpResponseMessage.Content = new StringContent(data);
                return httpResponseMessage;
            })
            .Build();

        var listArtifactsHttpResponseMock = new HttpResponseMessageMockBuilder()
            .Where(httpRequestMessage => httpRequestMessage.RequestUri.ToString() == $"https://api.github.com/repos/test_repo/actions/runs/{runId}/artifacts")
            .RespondWith(httpRequestMessage =>
            {
                var data = JsonSerializer.Serialize(listArtifactResponse);
                var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                responseMessage.Content = new StringContent(data);
                return responseMessage;
            })
            .Build();

        // add the mocks to the http handler
        var handler = new TestHttpMessageHandler();
        handler.MockHttpResponse(getArtifactHttpResponseMock);
        handler.MockHttpResponse(listArtifactsHttpResponseMock);

        var httpClient = new HttpClient(handler);

        var sut = new GetArtifactCommand(httpClient)
        {
            Repo = "test_repo",
            ArtifactName = artifactName,
            RunId = runId,
            Token = ""
        };

        var console = new FakeInMemoryConsole();
        await sut.ExecuteAsync(console);

        var output = console.ReadOutputString();
        var expected = JsonSerializer.Serialize(getArtifactResponse);
        output.ShouldBeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetArtifactById_ReturnsData()
    {
        var id = new Random().Next();
        var response = new GetResponseModel()
        {
            id = id
        };

        // prepare the http mocks
        var httpResponseMessageMock = new HttpResponseMessageMockBuilder()
            .Where(httpRequestMessage =>
            {
                return httpRequestMessage.RequestUri.ToString()
                    .Equals($"https://api.github.com/repos/test_repo/actions/artifacts/{id}");
            })
            .RespondWith(httpRequestMessage =>
            {
                var data = JsonSerializer.Serialize(response);

                var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                httpResponseMessage.Content = new StringContent(data);
                return httpResponseMessage;
            })
            .Build();

        // add the mocks to the http handler
        var handler = new TestHttpMessageHandler();
        handler.MockHttpResponse(httpResponseMessageMock);

        var httpClient = new HttpClient(handler);

        var sut = new GetArtifactCommand(httpClient)
        {
            Repo = "test_repo",
            ArtifactId = id.ToString(),
            Token = ""
        };

        var console = new FakeInMemoryConsole();
        await sut.ExecuteAsync(console);

        var output = console.ReadOutputString();

        var expected = JsonSerializer.Serialize(response);
        output.ShouldBeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetArtifactById_IncludeTokenOnRequest()
    {
        var id = new Random().Next();
        
        string authTokenReceived = "";

        // prepare the http mocks
        var httpResponseMessageMock = new HttpResponseMessageMockBuilder()
            .RespondWith(httpRequestMessage =>
            {
                authTokenReceived = httpRequestMessage.Headers.Authorization.ToString();

                var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                httpResponseMessage.Content = new StringContent("Horse");
                return httpResponseMessage;
            })
            .Build();

        // add the mocks to the http handler
        var handler = new TestHttpMessageHandler();
        handler.MockHttpResponse(httpResponseMessageMock);

        var httpClient = new HttpClient(handler);

        var sut = new GetArtifactCommand(httpClient)
        {
            Repo = "test_repo",
            ArtifactId = id.ToString(),
            Token = "MY_TOKEN"
        };

        var console = new FakeInMemoryConsole();
        await sut.ExecuteAsync(console);

        authTokenReceived.ShouldBe("Bearer MY_TOKEN");
    }
}