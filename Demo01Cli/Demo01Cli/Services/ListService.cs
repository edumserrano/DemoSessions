using CliFx.Infrastructure;
using Demo01Cli.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Demo01Cli.Services
{
    public class ListService
    {
        private readonly HttpClient _httpClient;

        public ListService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ListResponseModel?> ListArtifactByRunId(string runId, string token, string repo)
        {
            //      curl \
            //-H "Accept: application/vnd.github+json" \
            //-H "Authorization: Bearer <YOUR-TOKEN>" \
            //https://api.github.com/repos/OWNER/REPO/actions/artifacts

            //https://github.com/edumserrano/share-jobs-data/actions/runs/3185445164

            _httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Demo");

            var result = await _httpClient.GetFromJsonAsync<ListResponseModel>($"https://api.github.com/repos/{repo}/actions/runs/{runId}/artifacts");
            return result;
        }
    }
}
