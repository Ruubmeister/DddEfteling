using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DddEfteling.Shared.Boundary
{
    public class StandClient : IStandClient
    {
        private HttpClient client;

        public StandClient(HttpClient client)
        {
            this.client = client;
        }

        public List<StandDto> GetStandsAsync()
        {
            string url = "/api/v1/stands";
            Uri targetUri = new Uri(client.BaseAddress, url);

            var streamTask = client.GetStringAsync(targetUri.AbsoluteUri);
            return JsonConvert.DeserializeObject<List<StandDto>>(streamTask.Result);
        }
    }

    public interface IStandClient
    {
        public List<StandDto> GetStandsAsync();
    }
}
