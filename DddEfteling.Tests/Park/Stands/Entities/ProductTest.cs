using DddEfteling.Park.Stands.Entities;
using Xunit;

namespace DddEfteling.Tests.Park.Stands.Entities
{
    public class ProductTest
    {
        [Fact]
        public void Construct_CreateProduct_ExpectProduct()
        {
            Product product = new Product("Fanta", 1.55F, ProductType.Drink);

            Assert.Equal("Fanta", product.Name);
            Assert.Equal(1.55F, product.Price);
            Assert.Equal(ProductType.Drink, product.Type);
        }
    }
}
