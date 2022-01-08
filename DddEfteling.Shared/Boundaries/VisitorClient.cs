using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace DddEfteling.Shared.Boundaries
{
    public class VisitorClient : IVisitorClient
    {
        private readonly HttpClient client;

        public VisitorClient(HttpClient client)
        {
            this.client = client;
        }

        public List<VisitorDto> GetVisitors()
        {
            var url = "/api/v1/visitors";
            var targetUri = new Uri(client.BaseAddress, url);
            var request = new HttpRequestMessage(HttpMethod.Get, targetUri.AbsoluteUri);

            var streamTask = client.SendAsync(request).Result;

            return streamTask.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<List<VisitorDto>>(streamTask.Content.ReadAsStringAsync().Result)
                : new List<VisitorDto>();

        }

        public VisitorDto GetVisitor(Guid guid)
        {
            var url = $"/api/v1/visitors/{guid}";
            var targetUri = new Uri(client.BaseAddress, url);

            var request = new HttpRequestMessage(HttpMethod.Get, targetUri.AbsoluteUri);

            var streamTask = client.SendAsync(request).Result;

            return streamTask.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<VisitorDto>(streamTask.Content.ReadAsStringAsync().Result)
                : null;
        }
    }

    public interface IVisitorClient
    {
        public List<VisitorDto> GetVisitors();

        public VisitorDto GetVisitor(Guid guid);

    }
}
