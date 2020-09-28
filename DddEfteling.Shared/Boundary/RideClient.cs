using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DddEfteling.Shared.Boundary
{
    public class RideClient: RestClient
    {
        public async Task<List<RideDto>> GetRidesAsync()
        {
            string url = "/api/v1/rides";
            return await JsonSerializer.DeserializeAsync<List<RideDto>>(await GetResource(url));
        }

        public async Task<RideDto> GetRandomRideAsync()
        {
            string url = "/api/v1/rides/random";
            return await JsonSerializer.DeserializeAsync<RideDto>(await GetResource(url));
        }

        public async Task<RideDto> GetNearestRide(Guid guid, List<Guid> excludedGuid)
        {
            string url = $"/api/v1/rides/{guid}/nearest";

            var urlParams = new Dictionary<string, string>
            {
                {"exclude", String.Join(",", excludedGuid.ToArray())}
            };

            return await JsonSerializer.DeserializeAsync<RideDto>(await GetResource(url, urlParams));
        }
    }
}
