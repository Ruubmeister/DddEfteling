using DddEfteling.Shared.Controls;
using Xunit;

namespace DddEfteling.SharedTests.Controls
{
    public class EnumExtensionsTest
    {
        [Fact]
        public void GetValues_GivenEnum_ExpectsEnums()
        {
            int count = 1;
            foreach (TestEnum testEnum in EnumExtensions.GetValues<TestEnum>())
            {
                switch (count)
                {
                    case 1:
                        Assert.Equal(TestEnum.TEST1, testEnum);
                        break;
                    case 2:
                        Assert.Equal(TestEnum.TEST2, testEnum);
                        break;
                    case 3:
                        Assert.Equal(TestEnum.TEST3, testEnum);
                        break;
                }
                count++;
            }
        }
    }
    public enum TestEnum
    {
        TEST1, TEST2, TEST3
    }
}
