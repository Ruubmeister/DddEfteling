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
            string url = "/api/v1/employees";
            Uri targetUri = new Uri(client.BaseAddress, url);
            var request = new HttpRequestMessage(HttpMethod.Get, targetUri.AbsoluteUri);

            var streamTask = client.SendAsync(request).Result;

            if (streamTask.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<List<EmployeeDto>>(streamTask.Content.ReadAsStringAsync().Result);
            }

            return new List<EmployeeDto>();
        }
    }

    public interface IEmployeeClient
    {
        public List<EmployeeDto> GetEmployees();
    }
}
