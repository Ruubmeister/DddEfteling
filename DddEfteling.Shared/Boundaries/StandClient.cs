using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;

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
    }

    public interface IStandClient
    {
        public List<StandDto> GetStands();
    }
}
