using DddEfteling.Shared.Boundaries;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using Xunit;

namespace DddEfteling.ParkTests.Shared.Boundaries
{
    public class StandClientTest
    {
        [Fact]
        public void GetStands_NoStands_ExpectEmptyList()
        {

            HttpClient httpClient = HttpClientMockHelper.GetMockedHttpClient("[]");

            var standClient = new StandClient(httpClient);

            var result = standClient.GetStands();

            Assert.Empty(result);
        }

        [Fact]
        public void GetStands_TwoStands_ExpectTwoStands()
        {

            var tales = new List<StandDto>() { { new StandDto() }, { new StandDto() } };

            HttpClient httpClient = HttpClientMockHelper.GetMockedHttpClient(JsonConvert.SerializeObject(tales));
            var standClient = new StandClient(httpClient);

            var result = standClient.GetStands();

            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count);
        }
    }
}
