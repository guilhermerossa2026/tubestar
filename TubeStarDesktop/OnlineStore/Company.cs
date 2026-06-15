using System;

namespace TubeStar
{
    [Serializable]
    public class Company
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Niche { get; set; } // "Alimentos", "Merch", "Gamer", "Brinquedos"
        public double Balance { get; set; }
        
        public double BrandAwareness { get; set; } // 0 to 100
        public int InfrastructureLevel { get; set; } // 1 to 5
        public string MarketingCampaign { get; set; } // "Nenhuma", "Baixa", "Média", "Agressiva"
        public string PricingPolicy { get; set; } // "Popular", "Mercado", "Premium"
        
        public bool HiredCEO { get; set; }
        public bool HiredMarketingDirector { get; set; }
        public bool HiredSalesManager { get; set; }
        public bool HiredRDEngineer { get; set; }

        public double YesterdayRevenue { get; set; }
        public double YesterdayCosts { get; set; }
        public int YesterdaySales { get; set; }
        public string MarketTrend { get; set; }
        
        public int ActiveProductsCount { get; set; }
        public double ProductNoveltyFactor { get; set; } // 0 to 100

        private System.Collections.Generic.List<Product> _products;
        public System.Collections.Generic.List<Product> Products
        {
            get { return _products ?? (_products = new System.Collections.Generic.List<Product>()); }
            set { _products = value; }
        }

        public Company()
        {
            // Parameterless constructor for XML serialization
        }

        public Company(string id, string name, string niche, double balance)
        {
            Id = id;
            Name = name;
            Niche = niche;
            Balance = balance;
            BrandAwareness = 10.0; // Start with 10% awareness
            InfrastructureLevel = 1;
            MarketingCampaign = "Nenhuma";
            PricingPolicy = "Mercado";
            HiredCEO = false;
            HiredMarketingDirector = false;
            HiredSalesManager = false;
            HiredRDEngineer = false;
            ActiveProductsCount = 1;
            ProductNoveltyFactor = 100.0;
            MarketTrend = "Estável";
            YesterdayRevenue = 0;
            YesterdayCosts = 0;
            YesterdaySales = 0;

            Products = new System.Collections.Generic.List<Product>();
            double standardPrice = 5.0;
            if (niche == "Alimentos") standardPrice = 5.0;
            else if (niche == "Merch") standardPrice = 25.0;
            else if (niche == "Gamer") standardPrice = 120.0;
            else if (niche == "Brinquedos") standardPrice = 40.0;
            Products.Add(new Product(Guid.NewGuid().ToString(), "Produto Base " + name, standardPrice, 50.0));
        }
    }
}
