using DddEfteling.Shared.Boundaries;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Xunit;

namespace DddEfteling.ParkTests.Shared.Boundaries
{
    public class VisitorClientTest
    {

        [Fact]
        public void GetVisitors_NoVisitors_ExpectEmptyList()
        {

            HttpClient httpClient = HttpClientMockHelper.GetMockedHttpClient("[]");

            var visitorClient = new VisitorClient(httpClient);

            var result = visitorClient.GetVisitors();

            Assert.Empty(result);
        }

        [Fact]
        public void GetVisitors_TwoVisitors_ExpectTwoVisitors()
        {

            var tales = new List<VisitorDto>() { { new VisitorDto() }, { new VisitorDto() } };

            HttpClient httpClient = HttpClientMockHelper.GetMockedHttpClient(JsonConvert.SerializeObject(tales));
            var visitorClient = new VisitorClient(httpClient);

            var result = visitorClient.GetVisitors();

            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetVisitor_NoVisitor_ExpectNull()
        {

            HttpClient httpClient = HttpClientMockHelper.GetMockedHttpClient(JsonConvert.SerializeObject(""), System.Net.HttpStatusCode.NotFound);
            var visitorClient = new VisitorClient(httpClient);

            var result = visitorClient.GetVisitor(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public void GetVisitor_ExistingVisitor_ExpectVisitor()
        {

            HttpClient httpClient = HttpClientMockHelper.GetMockedHttpClient(JsonConvert.SerializeObject(new VisitorDto()));
            var visitorClient = new VisitorClient(httpClient);

            var result = visitorClient.GetVisitor(Guid.NewGuid());

            Assert.NotNull(result);
        }
    }
}
