using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DddEfteling.Shared.Boundary
{
    public class EmployeeClient : RestClient, IEmployeeClient
    {

        public EmployeeClient(IConfiguration Configuration)
        {
            this.setBaseUri(Configuration["ParkUrl"]);
        }

        public async Task<List<EmployeeDto>> GetEmployeesAsync()
        {
            string url = "/api/v1/employees";
            return await JsonSerializer.DeserializeAsync<List<EmployeeDto>>(await GetResource(url));
        }
    }

    public interface IEmployeeClient
    {
        public Task<List<EmployeeDto>> GetEmployeesAsync();
    }
}
