using DddEfteling.Shared.Boundaries;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Xunit;

namespace DddEfteling.ParkTests.Shared.Boundaries
{
    public class RideClientTest
    {

        [Fact]
        public void GetRides_NoRides_ExpectEmptyList()
        {

            HttpClient httpClient = HttpClientMockHelper.GetMockedHttpClient("[]");

            var rideClient = new RideClient(httpClient);

            var result = rideClient.GetRides();

            Assert.Empty(result);
        }

        [Fact]
        public void GetRides_TwoRides_ExpectTwoRides()
        {

            var tales = new List<RideDto>() { { new RideDto() }, { new RideDto() } };

            HttpClient httpClient = HttpClientMockHelper.GetMockedHttpClient(JsonConvert.SerializeObject(tales));
            var rideClient = new RideClient(httpClient);

            var result = rideClient.GetRides();

            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetRandomRide_NoRide_ExpectNull()
        {

            HttpClient httpClient = HttpClientMockHelper.GetMockedHttpClient(JsonConvert.SerializeObject(""), System.Net.HttpStatusCode.NotFound);
            var rideClient = new RideClient(httpClient);

            var result = rideClient.GetRandomRide();

            Assert.Null(result);
        }

        [Fact]
        public void GetRandomRide_ExistingRide_ExpectRide()
        {

            HttpClient httpClient = HttpClientMockHelper.GetMockedHttpClient(JsonConvert.SerializeObject(new RideDto()));
            var rideClient = new RideClient(httpClient);

            var result = rideClient.GetRandomRide();

            Assert.NotNull(result);
        }


        [Fact]
        public void GetNearestRide_NoRide_ExpectNull()
        {

            HttpClient httpClient = HttpClientMockHelper.GetMockedHttpClient(JsonConvert.SerializeObject(""), System.Net.HttpStatusCode.NotFound);
            var rideClient = new RideClient(httpClient);

            var result = rideClient.GetNearestRide(Guid.NewGuid(), new List<Guid>());

            Assert.Null(result);
        }

        [Fact]
        public void GetNearestRide_ExistingRide_ExpectRide()
        {

            HttpClient httpClient = HttpClientMockHelper.GetMockedHttpClient(JsonConvert.SerializeObject(new RideDto()));
            var rideClient = new RideClient(httpClient);

            var result = rideClient.GetNearestRide(Guid.NewGuid(), new List<Guid>());

            Assert.NotNull(result);
        }
    }
}
