using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Demo01Cli
{
    [Command("download")]
    public class DownloadArtifactCommand : ICommand
    {
        private HttpClient _httpClient;

        public DownloadArtifactCommand(HttpClient client)
        {
            _httpClient = client;
        }
        public DownloadArtifactCommand() : this(new HttpClient())
        {

        }

        [CommandOption(
            "artefact-id",
            IsRequired = true,
            Description = "The artefact id to get the artefacts from")]
        public string ArtefactId { get; init; }

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

            //          curl \
            //-H "Accept: application/vnd.github+json" \
            //-H "Authorization: Bearer <YOUR-TOKEN>" \
            //https://api.github.com/repos/OWNER/REPO/actions/artifacts/ARTIFACT_ID/ARCHIVE_FORMAT

            _httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Demo");

            var result = await _httpClient.GetAsync($"https://api.github.com/repos/{Repo}/actions/artifacts/{ArtefactId}/zip");
            var responseStream = await result.Content.ReadAsStreamAsync();
            if(!result.IsSuccessStatusCode && responseStream.Length == 0)
            {
                console.Output.Write($"Oh dear, user, the thing with ID {ArtefactId} you have searched for could not be found.");
                //Brodie broke the code!!!!
                return;
            }

            var zipArchive = new ZipArchive(responseStream, ZipArchiveMode.Read);
            using (zipArchive)
            {
                var artifactFileAsZip = zipArchive.Entries.First();
                await using var artifactAsStream = artifactFileAsZip.Open();
                using var streamReader = new StreamReader(artifactAsStream, Encoding.UTF8);
                var artifactFileContent = await streamReader.ReadToEndAsync();
                console.Output.WriteLine(artifactFileContent);
            }
        }
    }
}
