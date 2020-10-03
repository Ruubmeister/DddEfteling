using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DddEfteling.Shared.Boundary
{
    public class FairyTaleClient : RestClient, IFairyTaleClient
    {
        public FairyTaleClient(IConfiguration Configuration)
        {
            this.setBaseUri(Configuration["FairyTaleUrl"]);
        }

        public async Task<List<FairyTaleDto>> GetFairyTalesAsync()
        {
            string url = "/api/v1/fairy-tales";
            return await JsonSerializer.DeserializeAsync<List<FairyTaleDto>>(await GetResource(url));
        }

        public async Task<FairyTaleDto> GetRandomFairyTaleAsync()
        {
            string url = "/api/v1/fairy-tales/random";
            return await JsonSerializer.DeserializeAsync<FairyTaleDto>(await GetResource(url));
        }

        public async Task<FairyTaleDto> GetNearestFairyTale(Guid guid, List<Guid> excludedGuid)
        {
            string url = $"/api/v1/fairy-tales/{guid}/nearest";

            var urlParams = new Dictionary<string, string>
            {
                {"exclude", String.Join(",", excludedGuid.ToArray())}
            };

            return await JsonSerializer.DeserializeAsync<FairyTaleDto>(await GetResource(url, urlParams));
        }
    }

    public interface IFairyTaleClient
    {
        public Task<List<FairyTaleDto>> GetFairyTalesAsync();

        public Task<FairyTaleDto> GetRandomFairyTaleAsync();

        public Task<FairyTaleDto> GetNearestFairyTale(Guid guid, List<Guid> excludedGuid);

    }
}
