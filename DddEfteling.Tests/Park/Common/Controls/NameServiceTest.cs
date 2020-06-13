using DddEfteling.Park.Common.Control;
using System;
using Xunit;

namespace DddEfteling.Tests.Park.Common.Controls
{
    public class NameServiceTest
    {
        [Fact]
        public void RandomFirstName_getName_expectNonEmptyString()
        {
            NameService nameService = new NameService();
            String name = nameService.RandomFirstName();

            Assert.NotEmpty(name);
            Assert.IsType<string>(name);
        }

        [Fact]
        public void RandomLastName_getName_expectNonEmptyString()
        {
            NameService nameService = new NameService();
            String name = nameService.RandomLastName();

            Assert.NotEmpty(name);
            Assert.IsType<string>(name);
        }
    }
}
