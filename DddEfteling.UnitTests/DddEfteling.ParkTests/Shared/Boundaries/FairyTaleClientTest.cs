using DddEfteling.Shared.Boundaries;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Xunit;

namespace DddEfteling.ParkTests.Shared.Boundaries
{
    public class FairyTaleClientTest
    {

        [Fact]
        public void GetFairyTales_NoFairyTales_ExpectEmptyList()
        {

            HttpClient httpClient = HttpClientMockHelper.GetMockedHttpClient("[]");

            var fairyTaleClient = new FairyTaleClient(httpClient);

            var result = fairyTaleClient.GetFairyTales();

            Assert.Empty(result);
        }

        [Fact]
        public void GetFairyTales_TwoFairyTales_ExpectTwoFairyTales()
        {

            var tales = new List<FairyTaleDto>() { { new FairyTaleDto() }, { new FairyTaleDto() } };

            HttpClient httpClient = HttpClientMockHelper.GetMockedHttpClient(JsonConvert.SerializeObject(tales));
            var fairyTaleClient = new FairyTaleClient(httpClient);

            var result = fairyTaleClient.GetFairyTales();

            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetRandomFairyTale_NoFairyTale_ExpectNull()
        {

            HttpClient httpClient = HttpClientMockHelper.GetMockedHttpClient(JsonConvert.SerializeObject(""), System.Net.HttpStatusCode.NotFound);
            var fairyTaleClient = new FairyTaleClient(httpClient);

            var result = fairyTaleClient.GetRandomFairyTale();

            Assert.Null(result);
        }

        [Fact]
        public void GetRandomFairyTale_ExistingFairyTale_ExpectFairyTale()
        {

            HttpClient httpClient = HttpClientMockHelper.GetMockedHttpClient(JsonConvert.SerializeObject(new FairyTaleDto()));
            var fairyTaleClient = new FairyTaleClient(httpClient);

            var result = fairyTaleClient.GetRandomFairyTale();

            Assert.NotNull(result);
        }


        [Fact]
        public void GetNearestFairyTale_NoFairyTale_ExpectNull()
        {

            HttpClient httpClient = HttpClientMockHelper.GetMockedHttpClient(JsonConvert.SerializeObject(""), System.Net.HttpStatusCode.NotFound);
            var fairyTaleClient = new FairyTaleClient(httpClient);

            var result = fairyTaleClient.GetNewFairyTaleLocation(Guid.NewGuid(), new List<Guid>());

            Assert.Null(result);
        }

        [Fact]
        public void GetNearestFairyTale_ExistingFairyTale_ExpectFairyTale()
        {

            HttpClient httpClient = HttpClientMockHelper.GetMockedHttpClient(JsonConvert.SerializeObject(new FairyTaleDto()));
            var fairyTaleClient = new FairyTaleClient(httpClient);

            var result = fairyTaleClient.GetNewFairyTaleLocation(Guid.NewGuid(), new List<Guid>());

            Assert.NotNull(result);
        }
    }
}
