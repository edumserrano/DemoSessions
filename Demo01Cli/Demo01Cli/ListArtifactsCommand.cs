using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace Demo01Cli;

[Command("list")]
public class ListArtifactsCommand : ICommand
{
    private HttpClient _httpClient;

    public ListArtifactsCommand(HttpClient client)
    {
        _httpClient = client;
    }
    public ListArtifactsCommand(): this (new HttpClient())
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
        //      curl \
        //-H "Accept: application/vnd.github+json" \
        //-H "Authorization: Bearer <YOUR-TOKEN>" \
        //https://api.github.com/repos/OWNER/REPO/actions/artifacts

        //https://github.com/edumserrano/share-jobs-data/actions/runs/3185445164

        _httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Demo");

        var result = await _httpClient.GetStringAsync($"https://api.github.com/repos/{Repo}/actions/runs/{RunId}/artifacts");
        console.Output.WriteLine(result);
    }
}