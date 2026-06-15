using System;

namespace TubeStar
{
    public class RealEstateItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; } // "Apartamento", "Casa", "Mansão", "Ilha", "Sala Comercial"
        public int Cost { get; set; }
        public int DailyTax { get; set; }
        public int DailyRent { get; set; }

        public RealEstateItem() { }

        public RealEstateItem(string id, string name, string category, int cost, int dailyTax, int dailyRent)
        {
            Id = id;
            Name = name;
            Category = category;
            Cost = cost;
            DailyTax = dailyTax;
            DailyRent = dailyRent;
        }
    }
}
