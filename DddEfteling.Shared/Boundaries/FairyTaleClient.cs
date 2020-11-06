using Microsoft.AspNetCore.WebUtilities;
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
            string url = "/api/v1/fairy-tales";
            Uri targetUri = new Uri(client.BaseAddress, url);

            var streamTask = client.GetStringAsync(targetUri.AbsoluteUri);

            if (streamTask.IsCompletedSuccessfully)
            {
                return JsonConvert.DeserializeObject<List<FairyTaleDto>>(streamTask.Result);
            }

            return new List<FairyTaleDto>();
        }

        public FairyTaleDto GetRandomFairyTale()
        {
            string url = "/api/v1/fairy-tales/random";
            Uri targetUri = new Uri(client.BaseAddress, url);

            var streamTask = client.GetStringAsync(targetUri.AbsoluteUri);

            if (streamTask.IsCompletedSuccessfully)
            {
                return JsonConvert.DeserializeObject<FairyTaleDto>(streamTask.Result);
            }

            return null;
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

            if (streamTask.IsCompletedSuccessfully)
            {
                return JsonConvert.DeserializeObject<FairyTaleDto>(streamTask.Result);
            }

            return null;
        }
    }

    public interface IFairyTaleClient
    {
        public List<FairyTaleDto> GetFairyTales();

        public FairyTaleDto GetRandomFairyTale();

        public FairyTaleDto GetNearestFairyTale(Guid guid, List<Guid> excludedGuid);

    }
}
