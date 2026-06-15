using System;

namespace TubeStar
{
    public class VehicleItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; } // "Carro", "Avião", "Lancha", "Jetski"
        public int Cost { get; set; }
        public int DailyTax { get; set; }

        public VehicleItem() { }

        public VehicleItem(string id, string name, string category, int cost, int dailyTax)
        {
            Id = id;
            Name = name;
            Category = category;
            Cost = cost;
            DailyTax = dailyTax;
        }
    }
}
