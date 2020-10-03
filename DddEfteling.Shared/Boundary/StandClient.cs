using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DddEfteling.Shared.Boundary
{
    public class StandClient : RestClient, IStandClient
    {
        public StandClient(IConfiguration Configuration)
        {
            this.setBaseUri(Configuration["StandUrl"]);
        }

        public async Task<List<StandDto>> GetStandsAsync()
        {
            string url = "/api/v1/stands";
            return await JsonSerializer.DeserializeAsync<List<StandDto>>(await GetResource(url));
        }
    }

    public interface IStandClient
    {
        public Task<List<StandDto>> GetStandsAsync();
    }
}
