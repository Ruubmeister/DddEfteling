using DddEfteling.Shared.Boundaries;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using Xunit;

namespace DddEfteling.ParkTests.Shared.Boundaries
{
    public class EmployeeClientTest
    {

        [Fact]
        public void GetEmployees_NoEmployees_ExpectEmptyList()
        {

            HttpClient httpClient = HttpClientMockHelper.GetMockedHttpClient("[]");

            var employeeClient = new EmployeeClient(httpClient);

            var result = employeeClient.GetEmployees();

            Assert.Empty(result);
        }

        [Fact]
        public void GetEmployees_TwoEmployees_ExpectTwoEmployees()
        {

            var employees = new List<EmployeeDto>() { { new EmployeeDto() }, { new EmployeeDto() } };

            HttpClient httpClient = HttpClientMockHelper.GetMockedHttpClient(JsonConvert.SerializeObject(employees));
            var employeeClient = new EmployeeClient(httpClient);

            var result = employeeClient.GetEmployees();

            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count);
        }
    }
}
