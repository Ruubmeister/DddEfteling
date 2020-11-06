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

            var streamTask = client.GetStringAsync(targetUri.AbsoluteUri);
            return JsonConvert.DeserializeObject<List<StandDto>>(streamTask.Result);
        }
    }

    public interface IStandClient
    {
        public List<StandDto> GetStands();
    }
}
