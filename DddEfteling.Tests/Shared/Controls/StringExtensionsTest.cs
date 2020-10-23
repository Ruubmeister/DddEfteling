using DddEfteling.Common.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DddEfteling.Tests.Common.Control
{
    public class StringExtensionsTest
    {

        [Fact]
        public void FirstCharToUpper_GiveStrings_ExpectUppercaseFirst()
        {
            string string1 = "give a string";
            string string2 = "Another string";
            string string3 = "2 see what happens now";
            string string4 = "";

            Assert.Equal("Give a string", StringExtensions.FirstCharToUpper(string1));
            Assert.Equal("Another string", StringExtensions.FirstCharToUpper(string2));
            Assert.Equal("2 see what happens now", StringExtensions.FirstCharToUpper(string3));
            Assert.Throws<ArgumentException>(() => StringExtensions.FirstCharToUpper(string4));
            Assert.Throws<ArgumentNullException>(() => StringExtensions.FirstCharToUpper(null));
        }
    }
}
