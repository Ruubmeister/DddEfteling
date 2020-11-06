

using Microsoft.AspNetCore.WebUtilities;
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

            var streamTask = client.GetStringAsync(targetUri.AbsoluteUri);

            if (streamTask.IsCompletedSuccessfully)
            {
                return JsonConvert.DeserializeObject<List<RideDto>>(streamTask.Result);
            }

            return new List<RideDto>();
        }

        public RideDto GetRandomRide()
        {
            string url = "/api/v1/rides/random";
            Uri targetUri = new Uri(client.BaseAddress, url);

            var streamTask = client.GetStringAsync(targetUri.AbsoluteUri);

            if (streamTask.IsCompletedSuccessfully)
            {
                return JsonConvert.DeserializeObject<RideDto>(streamTask.Result);
            }

            return null;
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


            if (streamTask.IsCompletedSuccessfully)
            {
                return JsonConvert.DeserializeObject<RideDto>(streamTask.Result);
            }

            return null;
        }
    }

    public interface IRideClient
    {
        public List<RideDto> GetRides();

        public RideDto GetRandomRide();

        public RideDto GetNearestRide(Guid guid, List<Guid> excludedGuid);

    }
}
