

using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DddEfteling.Shared.Boundary
{
    public class RideClient: RestClient, IRideClient
    {
        public RideClient(IConfiguration Configuration)
        {
            this.setBaseUri(Configuration["RideUrl"]);
        }

        public List<RideDto> GetRidesAsync()
        {
            string url = "/api/v1/rides";
            return JsonConvert.DeserializeObject<List<RideDto>>(GetResource(url));
        }

        public RideDto GetRandomRideAsync()
        {
            string url = "/api/v1/rides/random";
            string resource = GetResource(url);
            return JsonConvert.DeserializeObject<RideDto>(GetResource(url));
        }

        public RideDto GetNearestRide(Guid guid, List<Guid> excludedGuid)
        {
            string url = $"/api/v1/rides/{guid}/nearest";

            var urlParams = new Dictionary<string, string>
            {
                {"exclude", String.Join(",", excludedGuid.ToArray())}
            };

            return JsonConvert.DeserializeObject<RideDto>(GetResource(url, urlParams));
        }
    }

    public interface IRideClient
    {
        public List<RideDto> GetRidesAsync();

        public RideDto GetRandomRideAsync();

        public RideDto GetNearestRide(Guid guid, List<Guid> excludedGuid);

    }
}
