

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DddEfteling.Shared.Boundary
{
    public class RideClient: IRideClient
    {
        private HttpClient client;

        public RideClient(HttpClient client)
        {
            this.client = client;
        }

        public List<RideDto> GetRidesAsync()
        {
            string url = "/api/v1/rides";
            Uri targetUri = new Uri(client.BaseAddress, url);

            var streamTask = client.GetStringAsync(targetUri.AbsoluteUri);
            return JsonConvert.DeserializeObject<List<RideDto>>(streamTask.Result);
        }

        public RideDto GetRandomRideAsync()
        {
            string url = "/api/v1/rides/random";
            Uri targetUri = new Uri(client.BaseAddress, url);

            var streamTask = client.GetStringAsync(targetUri.AbsoluteUri);
            return JsonConvert.DeserializeObject<RideDto>(streamTask.Result);
        }

        public RideDto GetNearestRide(Guid guid, List<Guid> excludedGuid)
        {
            string url = $"/api/v1/rides/{guid}/nearest";

            var urlParams = new Dictionary<string, string>
            {
                {"exclude", String.Join(",", excludedGuid.ToArray())}
            };

            Uri targetUri = new Uri(client.BaseAddress, url);

            var streamTask = client.GetStringAsync(QueryHelpers.AddQueryString(targetUri.AbsoluteUri, urlParams));


            return JsonConvert.DeserializeObject<RideDto>(streamTask.Result);
        }
    }

    public interface IRideClient
    {
        public List<RideDto> GetRidesAsync();

        public RideDto GetRandomRideAsync();

        public RideDto GetNearestRide(Guid guid, List<Guid> excludedGuid);

    }
}
