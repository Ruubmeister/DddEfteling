using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DddEfteling.Shared.Boundary
{
    public class RideClient: RestClient, IRideClient
    {
        public RideClient(IConfiguration Configuration)
        {
            this.setBaseUri(Configuration["RideUrl"]);
        }

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

    public interface IRideClient
    {
        public Task<List<RideDto>> GetRidesAsync();

        public Task<RideDto> GetRandomRideAsync();

        public Task<RideDto> GetNearestRide(Guid guid, List<Guid> excludedGuid);

    }
}
