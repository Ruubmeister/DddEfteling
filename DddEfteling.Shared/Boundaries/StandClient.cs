using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace DddEfteling.Shared.Boundaries
{
    public class StandClient : IStandClient
    {
        private readonly HttpClient client;

        public StandClient(HttpClient client)
        {
            this.client = client;
        }

        public List<StandDto> GetStands()
        {
            string url = "/api/v1/stands";
            Uri targetUri = new Uri(client.BaseAddress, url);
            var request = new HttpRequestMessage(HttpMethod.Get, targetUri.AbsoluteUri);

            var streamTask = client.SendAsync(request).Result;
            if (streamTask.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<List<StandDto>>(streamTask.Content.ReadAsStringAsync().Result);
            }

            return new List<StandDto>();
        }

        public StandDto GetStand(Guid guid)
        {
            string url = $"/api/v1/stands/{guid}";
            Uri targetUri = new Uri(client.BaseAddress, url);
            var request = new HttpRequestMessage(HttpMethod.Get, targetUri.AbsoluteUri);

            var streamTask = client.SendAsync(request).Result;
            if (streamTask.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<StandDto>(streamTask.Content.ReadAsStringAsync().Result);
            }

            return null;
        }

        public StandDto GetRandomStand()
        {
            string url = "/api/v1/stands/random";
            Uri targetUri = new Uri(client.BaseAddress, url);

            var request = new HttpRequestMessage(HttpMethod.Get, targetUri.AbsoluteUri);

            var streamTask = client.SendAsync(request).Result;

            if (streamTask.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<StandDto>(streamTask.Content.ReadAsStringAsync().Result);
            }

            return null;
        }

        public StandDto GetNewStandLocation(Guid guid, List<Guid> excludedGuid)
        {
            string url = $"/api/v1/stands/{guid}/new-location?exclude={String.Join(",", excludedGuid.ToArray())}";

            Uri targetUri = new Uri(client.BaseAddress, url);

            var request = new HttpRequestMessage(HttpMethod.Get, targetUri.AbsoluteUri);

            var streamTask = client.SendAsync(request).Result;

            if (streamTask.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<StandDto>(streamTask.Content.ReadAsStringAsync().Result);
            }

            return null;
        }

        public string OrderDinner(Guid guid, List<string> products)
        {
            string url = $"/api/v1/stands/{guid}/order";
            Uri targetUri = new Uri(client.BaseAddress, url);
            var request = new HttpRequestMessage(HttpMethod.Post, targetUri.AbsoluteUri);
            var postBody = JsonConvert.SerializeObject(products);
            request.Content = new StringContent(postBody, Encoding.UTF8, "application/json");

            var streamTask = client.SendAsync(request).Result;
            if (streamTask.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<string>(streamTask.Content.ReadAsStringAsync().Result);
            }

            return null;
        }
        public DinnerDto GetOrder(string ticket)
        {
            string url = $"/api/v1/stands/order/{ticket}";
            Uri targetUri = new Uri(client.BaseAddress, url);
            var request = new HttpRequestMessage(HttpMethod.Get, targetUri.AbsoluteUri);

            var streamTask = client.SendAsync(request).Result;
            if (streamTask.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<DinnerDto>(streamTask.Content.ReadAsStringAsync().Result);
            }

            return null;
        }
    }

    public interface IStandClient
    {
        public List<StandDto> GetStands();

        public StandDto GetStand(Guid guid);

        public string OrderDinner(Guid guid, List<string> products);

        public StandDto GetRandomStand();

        public StandDto GetNewStandLocation(Guid guid, List<Guid> excludedGuid);

        public DinnerDto GetOrder(string ticket);

    }
}
