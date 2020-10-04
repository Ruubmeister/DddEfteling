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

        public List<EmployeeDto> GetEmployeesAsync()
        {
            string url = "/api/v1/employees";
            return JsonSerializer.Deserialize<List<EmployeeDto>>(GetResource(url));
        }
    }

    public interface IEmployeeClient
    {
        public List<EmployeeDto> GetEmployeesAsync();
    }
}
