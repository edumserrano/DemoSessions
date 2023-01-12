using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Demo01Cli.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Demo01Cli;

[Command("list")]
public class ListArtifactsCommand : ICommand
{
    private ListService _listService;

    public ListArtifactsCommand(ListService listService)
    {
        _listService = listService;
    }
    public ListArtifactsCommand(): this (new ListService(new HttpClient()))
    {

    }

    [CommandOption(
        "run-id",
        IsRequired = true,
        Description = "The run id to get the artefacts from")]
    public string RunId { get; init; }

    [CommandOption(
        "repo",
        IsRequired = true,
        Description = "The repository id to get the artefacts from")]
    public string Repo { get; init; }

    [CommandOption(
        "token",
        IsRequired = true,
        Description = "The user token for the repository")]
    public string Token { get; init; } 

    public async ValueTask ExecuteAsync(IConsole console)
    {
        var result = await _listService.ListArtifactByRunId(RunId, Token, Repo);
        var data = JsonSerializer.Serialize(result);
        console.Output.WriteLine(data);
    }
}