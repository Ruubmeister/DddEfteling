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
            var url = "/api/v1/stands";
            var targetUri = new Uri(client.BaseAddress, url);
            var request = new HttpRequestMessage(HttpMethod.Get, targetUri.AbsoluteUri);

            var streamTask = client.SendAsync(request).Result;
            return streamTask.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<List<StandDto>>(streamTask.Content.ReadAsStringAsync().Result)
                : new List<StandDto>();
        }

        public StandDto GetStand(Guid guid)
        {
            string url = $"/api/v1/stands/{guid}";
            Uri targetUri = new Uri(client.BaseAddress, url);
            var request = new HttpRequestMessage(HttpMethod.Get, targetUri.AbsoluteUri);

            var streamTask = client.SendAsync(request).Result;
            return streamTask.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<StandDto>(streamTask.Content.ReadAsStringAsync().Result)
                : null;
        }

        public StandDto GetRandomStand()
        {
            var url = "/api/v1/stands/random";
            var targetUri = new Uri(client.BaseAddress, url);

            var request = new HttpRequestMessage(HttpMethod.Get, targetUri.AbsoluteUri);

            var streamTask = client.SendAsync(request).Result;

            return streamTask.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<StandDto>(streamTask.Content.ReadAsStringAsync().Result)
                : null;
        }

        public StandDto GetNewStandLocation(Guid guid, List<Guid> excludedGuid)
        {
            var url = $"/api/v1/stands/{guid}/new-location?exclude={String.Join(",", excludedGuid.ToArray())}";

            var targetUri = new Uri(client.BaseAddress, url);

            var request = new HttpRequestMessage(HttpMethod.Get, targetUri.AbsoluteUri);

            var streamTask = client.SendAsync(request).Result;

            return streamTask.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<StandDto>(streamTask.Content.ReadAsStringAsync().Result)
                : null;
        }

        public string OrderDinner(Guid guid, List<string> products)
        {
            var url = $"/api/v1/stands/{guid}/order";
            var targetUri = new Uri(client.BaseAddress, url);
            var request = new HttpRequestMessage(HttpMethod.Post, targetUri.AbsoluteUri);
            var postBody = JsonConvert.SerializeObject(products);
            request.Content = new StringContent(postBody, Encoding.UTF8, "application/json");

            var streamTask = client.SendAsync(request).Result;
            return streamTask.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<string>(streamTask.Content.ReadAsStringAsync().Result)
                : null;
        }
        
        public DinnerDto GetOrder(string ticket)
        {
            var url = $"/api/v1/stands/order/{ticket}";
            var targetUri = new Uri(client.BaseAddress, url);
            var request = new HttpRequestMessage(HttpMethod.Get, targetUri.AbsoluteUri);

            var streamTask = client.SendAsync(request).Result;
            return streamTask.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<DinnerDto>(streamTask.Content.ReadAsStringAsync().Result)
                : null;
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
