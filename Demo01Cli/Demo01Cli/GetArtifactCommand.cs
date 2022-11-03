using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace Demo01Cli;

[Command("get-artifact")]
public class GetArtifactCommand : ICommand
{
    private readonly HttpClient _httpClient;

    public GetArtifactCommand(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public GetArtifactCommand() : this(new HttpClient())
    {

    }

    public async ValueTask ExecuteAsync(IConsole console)
    {
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Demo");

        var url = $"https://api.github.com/repos/{Repo}/actions/artifacts/{ArtifactId}";
        var response = await _httpClient.GetStringAsync(url);

        await console.Output.WriteAsync(response);
    }

    [CommandOption(
        "repo",
        IsRequired = true,
        Description = "The repository id to get the artefacts from")]
    public string Repo { get; set; }
    [CommandOption(
        "artefact-id",
        IsRequired = true,
        Description = "The artefact id to get the artefacts from")]
    public string ArtifactId { get; set; }

    [CommandOption(
        "token",
        IsRequired = true,
        Description = "The user token for the repository")]
    public string Token { get; set; }
}