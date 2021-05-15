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
            string url = "/api/v1/rides";
            Uri targetUri = new Uri(client.BaseAddress, url);
            var request = new HttpRequestMessage(HttpMethod.Get, targetUri.AbsoluteUri);

            var streamTask = client.SendAsync(request).Result;

            if (streamTask.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<List<RideDto>>(streamTask.Content.ReadAsStringAsync().Result);
            }

            return new List<RideDto>();
        }

        public RideDto GetRandomRide()
        {
            string url = "/api/v1/rides/random";
            Uri targetUri = new Uri(client.BaseAddress, url);

            var request = new HttpRequestMessage(HttpMethod.Get, targetUri.AbsoluteUri);

            var streamTask = client.SendAsync(request).Result;

            if (streamTask.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<RideDto>(streamTask.Content.ReadAsStringAsync().Result);
            }

            return null;
        }

        public RideDto GetNextLocation(Guid guid, List<Guid> excludedGuid)
        {
            string url = $"/api/v1/rides/{guid}/new-location?exclude={String.Join(",", excludedGuid.ToArray())}";

            Uri targetUri = new Uri(client.BaseAddress, url);

            var request = new HttpRequestMessage(HttpMethod.Get, targetUri.AbsoluteUri);

            var streamTask = client.SendAsync(request).Result;

            if (streamTask.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<RideDto>(streamTask.Content.ReadAsStringAsync().Result);
            }

            return null;
        }
    }

    public interface IRideClient
    {
        public List<RideDto> GetRides();

        public RideDto GetRandomRide();

        public RideDto GetNextLocation(Guid guid, List<Guid> excludedGuid);

    }
}
