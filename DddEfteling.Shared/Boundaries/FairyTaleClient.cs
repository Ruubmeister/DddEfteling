using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace DddEfteling.Shared.Boundaries
{
    public class FairyTaleClient : IFairyTaleClient
    {
        private readonly HttpClient client;

        public FairyTaleClient(HttpClient client)
        {
            this.client = client;
        }

        public List<FairyTaleDto> GetFairyTales()
        {
            var url = "/api/v1/fairy-tales";
            var targetUri = new Uri(client.BaseAddress, url);

            var request = new HttpRequestMessage(HttpMethod.Get, targetUri.AbsoluteUri);

            var streamTask = client.SendAsync(request).Result;

            return streamTask.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<List<FairyTaleDto>>(streamTask.Content.ReadAsStringAsync().Result)
                : new List<FairyTaleDto>();
        }

        public FairyTaleDto GetRandomFairyTale()
        {
            var url = "/api/v1/fairy-tales/random";
            var targetUri = new Uri(client.BaseAddress, url);

            var request = new HttpRequestMessage(HttpMethod.Get, targetUri.AbsoluteUri);

            var streamTask = client.SendAsync(request).Result;

            return streamTask.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<FairyTaleDto>(streamTask.Content.ReadAsStringAsync().Result)
                : null;
        }

        public FairyTaleDto GetNewFairyTaleLocation(Guid guid, List<Guid> excludedGuid)
        {
            var url = $"/api/v1/fairy-tales/{guid}/new-location?exclude={String.Join(",", excludedGuid.ToArray())}";

            var targetUri = new Uri(client.BaseAddress, url);

            var request = new HttpRequestMessage(HttpMethod.Get, targetUri.AbsoluteUri);

            var streamTask = client.SendAsync(request).Result;
            
            return streamTask.IsSuccessStatusCode
                    ? JsonConvert.DeserializeObject<FairyTaleDto>(streamTask.Content.ReadAsStringAsync().Result)
                    : null;
        }
    }

    public interface IFairyTaleClient
    {
        public List<FairyTaleDto> GetFairyTales();

        public FairyTaleDto GetRandomFairyTale();

        public FairyTaleDto GetNewFairyTaleLocation(Guid guid, List<Guid> excludedGuid);

    }
}
