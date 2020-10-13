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
    public class VisitorClient: IVisitorClient
    {
        private HttpClient client;

        public VisitorClient(HttpClient client)
        {
            this.client = client;
        }

        public List<VisitorDto> GetVisitors()
        {
            string url = "/api/v1/visitors";
            Uri targetUri = new Uri(client.BaseAddress, url);

            var streamTask = client.GetStringAsync(targetUri.AbsoluteUri);
            return JsonConvert.DeserializeObject<List<VisitorDto>>(streamTask.Result);
        }

        public VisitorDto GetVisitor(Guid guid)
        {
            string url = $"/api/v1/visitors/{guid}";
            Uri targetUri = new Uri(client.BaseAddress, url);

            var streamTask = client.GetStringAsync(targetUri.AbsoluteUri);
            return JsonConvert.DeserializeObject<VisitorDto>(streamTask.Result);
        }
    }

    public interface IVisitorClient
    {
        public List<VisitorDto> GetVisitors();

        public VisitorDto GetVisitor(Guid guid);

    }
}
