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

            var streamTask = client.GetStringAsync(targetUri.AbsoluteUri);
            return JsonConvert.DeserializeObject<List<EmployeeDto>>(streamTask.Result);
        }
    }

    public interface IEmployeeClient
    {
        public List<EmployeeDto> GetEmployees();
    }
}
