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
    public class EmployeeClient : IEmployeeClient
    {
        private HttpClient client;
        public EmployeeClient(HttpClient client)
        {
            this.client = client;
        }

        public List<EmployeeDto> GetEmployeesAsync()
        {
            string url = "/api/v1/employees";
            Uri targetUri = new Uri(client.BaseAddress, url);

            var streamTask = client.GetStringAsync(targetUri.AbsoluteUri);
            return JsonConvert.DeserializeObject<List<EmployeeDto>>(streamTask.Result);
        }
    }

    public interface IEmployeeClient
    {
        public List<EmployeeDto> GetEmployeesAsync();
    }
}
