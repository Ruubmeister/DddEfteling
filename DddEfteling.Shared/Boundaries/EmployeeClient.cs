using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace DddEfteling.Shared.Boundaries
{
    public class EmployeeClient : IEmployeeClient
    {
        private readonly HttpClient client;
        public EmployeeClient(HttpClient client)
        {
            this.client = client;
        }

        public List<EmployeeDto> GetEmployees()
        {
            var url = "/api/v1/employees";
            var targetUri = new Uri(client.BaseAddress, url);
            var request = new HttpRequestMessage(HttpMethod.Get, targetUri.AbsoluteUri);

            var streamTask = client.SendAsync(request).Result;

            return streamTask.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<List<EmployeeDto>>(streamTask.Content.ReadAsStringAsync().Result)
                : new List<EmployeeDto>();
        }
    }

    public interface IEmployeeClient
    {
        public List<EmployeeDto> GetEmployees();
    }
}
