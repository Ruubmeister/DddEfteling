using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DddEfteling.Shared.Boundary
{
    class StandClient : RestClient
    {
        public async Task<List<StandDto>> GetStandsAsync()
        {
            string url = "/api/v1/stands";
            return await JsonSerializer.DeserializeAsync<List<StandDto>>(await GetResource(url));
        }
    }
}
