using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace DddEfteling.Shared.Boundaries
{
    public class RideClient : IRideClient
    {
        private readonly HttpClient client;

        public RideClient(HttpClient client)
        {
            this.client = client;
        }

        public List<RideDto> GetRides()
        {
            var url = "/api/v1/rides";
            var targetUri = new Uri(client.BaseAddress, url);
            var request = new HttpRequestMessage(HttpMethod.Get, targetUri.AbsoluteUri);

            var streamTask = client.SendAsync(request).Result;

            return streamTask.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<List<RideDto>>(streamTask.Content.ReadAsStringAsync().Result)
                : new List<RideDto>();
        }

        public RideDto GetRandomRide()
        {
            var url = "/api/v1/rides/random";
            var targetUri = new Uri(client.BaseAddress, url);

            var request = new HttpRequestMessage(HttpMethod.Get, targetUri.AbsoluteUri);

            var streamTask = client.SendAsync(request).Result;

            return streamTask.IsSuccessStatusCode 
                ? JsonConvert.DeserializeObject<RideDto>(streamTask.Content.ReadAsStringAsync().Result)
                : null;
        }

        public RideDto GetNextLocation(Guid guid, List<Guid> excludedGuid)
        {
            var url = $"/api/v1/rides/{guid}/new-location?exclude={String.Join(",", excludedGuid.ToArray())}";

            var targetUri = new Uri(client.BaseAddress, url);

            var request = new HttpRequestMessage(HttpMethod.Get, targetUri.AbsoluteUri);

            var streamTask = client.SendAsync(request).Result;

            return streamTask.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<RideDto>(streamTask.Content.ReadAsStringAsync().Result)
                : null;
        }
    }

    public interface IRideClient
    {
        public List<RideDto> GetRides();

        public RideDto GetRandomRide();

        public RideDto GetNextLocation(Guid guid, List<Guid> excludedGuid);

    }
}
