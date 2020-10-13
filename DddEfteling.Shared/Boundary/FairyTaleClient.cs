using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace DddEfteling.Shared.Boundary
{
    public class FairyTaleClient : IFairyTaleClient
    {
        private HttpClient client;

        public FairyTaleClient(HttpClient client)
        {
            this.client = client;
        }

        public List<FairyTaleDto> GetFairyTalesAsync()
        {
            string url = "/api/v1/fairy-tales";
            Uri targetUri = new Uri(client.BaseAddress, url);

            var streamTask = client.GetStringAsync(targetUri.AbsoluteUri);
            return JsonConvert.DeserializeObject<List<FairyTaleDto>>(streamTask.Result);
        }

        public FairyTaleDto GetRandomFairyTaleAsync()
        {
            string url = "/api/v1/fairy-tales/random";
            Uri targetUri = new Uri(client.BaseAddress, url);

            var streamTask = client.GetStringAsync(targetUri.AbsoluteUri);
            return JsonConvert.DeserializeObject<FairyTaleDto>(streamTask.Result);
        }

        public FairyTaleDto GetNearestFairyTale(Guid guid, List<Guid> excludedGuid)
        {
            string url = $"/api/v1/fairy-tales/{guid}/nearest";

            var urlParams = new Dictionary<string, string>
            {
                {"exclude", String.Join(",", excludedGuid.ToArray())}
            };

            Uri targetUri = new Uri(client.BaseAddress, url);

            var streamTask = client.GetStringAsync(QueryHelpers.AddQueryString(targetUri.AbsoluteUri, urlParams));

            return JsonConvert.DeserializeObject<FairyTaleDto>(streamTask.Result);

        }
    }

    public interface IFairyTaleClient
    {
        public List<FairyTaleDto> GetFairyTalesAsync();

        public FairyTaleDto GetRandomFairyTaleAsync();

        public FairyTaleDto GetNearestFairyTale(Guid guid, List<Guid> excludedGuid);

    }
}
