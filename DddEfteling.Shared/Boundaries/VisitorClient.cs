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
            string url = "/api/v1/visitors";
            Uri targetUri = new Uri(client.BaseAddress, url);
            var request = new HttpRequestMessage(HttpMethod.Get, targetUri.AbsoluteUri);

            var streamTask = client.SendAsync(request).Result;

            if (streamTask.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<List<VisitorDto>>(streamTask.Content.ReadAsStringAsync().Result);
            }

            return new List<VisitorDto>();

        }

        public VisitorDto GetVisitor(Guid guid)
        {
            string url = $"/api/v1/visitors/{guid}";
            Uri targetUri = new Uri(client.BaseAddress, url);

            var request = new HttpRequestMessage(HttpMethod.Get, targetUri.AbsoluteUri);

            var streamTask = client.SendAsync(request).Result;

            if (streamTask.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<VisitorDto>(streamTask.Content.ReadAsStringAsync().Result);
            }

            return null;
        }
    }

    public interface IVisitorClient
    {
        public List<VisitorDto> GetVisitors();

        public VisitorDto GetVisitor(Guid guid);

    }
}
