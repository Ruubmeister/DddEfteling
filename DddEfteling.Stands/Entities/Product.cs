namespace DddEfteling.Stands.Entities
{
    public class Product
    {
        public string Name { get; }
        public float Price { get; }

        public ProductType Type { get; }

        public Product(string name, float price, ProductType type)
        {
            Name = name;
            Price = price;
            Type = type;
        }
    }
}
