using System;

namespace TubeStar
{
    [Serializable]
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public double Quality { get; set; }
        public double Novelty { get; set; }
        
        public int SalesYesterday { get; set; }
        public double RevenueYesterday { get; set; }

        public Product()
        {
        }

        public Product(string id, string name, double price, double quality)
        {
            Id = id;
            Name = name;
            Price = price;
            Quality = quality;
            Novelty = 100.0;
            SalesYesterday = 0;
            RevenueYesterday = 0.0;
        }
    }
}
