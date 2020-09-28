using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DddEfteling.Shared.Boundary
{
    public class EmployeeClient : RestClient
    {
        public async Task<List<EmployeeDto>> GetEmployeesAsync()
        {
            string url = "/api/v1/employees";
            return await JsonSerializer.DeserializeAsync<List<EmployeeDto>>(await GetResource(url));
        }
    }
}
