using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TubeStar
{
    public class Player
    {
        private static Player _current;
        public static Player Current
        {
            get
            {
                if (_current == null)
                    _current = new Player();
                return _current;
            }
            set { _current = value; }
        }

        public event Action MoneyChanged;

        private double _money;
        public double Money 
        {
            get { return _money; }
            set
            {
                _money = value;
                if (MoneyChanged != null)
                {
                    MoneyChanged();
                }
            }
        }

        public bool QuitJob { get; set; }
        public bool HasPromotion { get; set; }
        public bool Overtime { get; set; }

        public List<Channel> Channels { get; set; }
        public List<Task> TasksInProgress { get; set; }
        public List<Video> Videos { get; set; }

        public int ShootingSkill { get; set; }
        public int PostProductionSkill { get; set; }
        public int VideoAttributePoints { get; set; }
        public bool CanViewQualityBeforeUpload { get; set; }

        public int Iterations { get; set; }

        public int CostOfLivingExtra { get; set; }

        public bool ChallengeMode { get; set; }
        public bool UltraMode { get; set; }
        public bool RobotRulers { get; set; }

        public double LoanPayOff { get; set; }

        // Youtuber Avatar Customization Properties
        public string YoutuberName { get; set; }
        public int YoutuberAvatarId { get; set; }
        public string YoutuberHairColor { get; set; }
        public string YoutuberOutfit { get; set; }
        public string YoutuberTattoos { get; set; }
        public string YoutuberAccessories { get; set; }

        // Stock Market (Bolsa de Valores) Properties
        public int SharesSTB { get; set; }
        public int SharesPEAR { get; set; }
        public int SharesRVG { get; set; }
        public int SharesGDR { get; set; }
        public int SharesWHP { get; set; }

        public double PricePaidSTB { get; set; }
        public double PricePaidPEAR { get; set; }
        public double PricePaidRVG { get; set; }
        public double PricePaidGDR { get; set; }
        public double PricePaidWHP { get; set; }

        public string SerializeStockPrices { get; set; }

        // Sponsorships remaining days (0 if not active)
        public int SponsorSTBDays { get; set; }
        public int SponsorPEARDays { get; set; }
        public int SponsorRVGDays { get; set; }
        public int SponsorGDRDays { get; set; }
        public int SponsorWHPDays { get; set; }

        // Active Trend remaining days (0 if not active)
        public int TrendSTBDays { get; set; }
        public int TrendPEARDays { get; set; }
        public int TrendRVGDays { get; set; }
        public int TrendGDRDays { get; set; }
        public int TrendWHPDays { get; set; }

        public List<string> OwnedRealEstate { get; set; }
        public List<string> OwnedVehicles { get; set; }
        public List<Company> OwnedCompanies { get; set; }

        public string CurrentJobId { get; set; }
        public double JobPerformance { get; set; }
        public string JobEffortLevel { get; set; }
        public double SalaryBonusMultiplier { get; set; }

        public Player()
        {
            Reset();
        }

        public void Reset()
        {
            RobotRulers = false;
            Iterations = -1;
            Money = 950;
            QuitJob = false;
            ShootingSkill = 30;
            PostProductionSkill = 20;
            VideoAttributePoints = 2;
            CanViewQualityBeforeUpload = false;
            CostOfLivingExtra = 0;
            Overtime = false;
            HasPromotion = false;
            LoanPayOff = 0;

            CurrentJobId = "panfletos";
            JobPerformance = 50.0;
            JobEffortLevel = "Normal";
            SalaryBonusMultiplier = 1.0;

            // Initialize Customization
            YoutuberName = "Gamer Pro";
            YoutuberAvatarId = 0;
            YoutuberHairColor = "#00FFFF"; // neon cyan
            YoutuberOutfit = "Camiseta Gamer";
            YoutuberTattoos = "Nenhuma";
            YoutuberAccessories = "Nenhum";

            // Reset Stock Shares and prices
            SharesSTB = 0;
            SharesPEAR = 0;
            SharesRVG = 0;
            SharesGDR = 0;
            SharesWHP = 0;
            PricePaidSTB = 0;
            PricePaidPEAR = 0;
            PricePaidRVG = 0;
            PricePaidGDR = 0;
            PricePaidWHP = 0;
            SerializeStockPrices = string.Empty;

            SponsorSTBDays = 0;
            SponsorPEARDays = 0;
            SponsorRVGDays = 0;
            SponsorGDRDays = 0;
            SponsorWHPDays = 0;

            TrendSTBDays = 0;
            TrendPEARDays = 0;
            TrendRVGDays = 0;
            TrendGDRDays = 0;
            TrendWHPDays = 0;

            OwnedRealEstate = new List<string>();
            OwnedVehicles = new List<string>();
            OwnedCompanies = new List<Company>();

            TasksInProgress = new List<Task>();
            Videos = new List<Video>();
            Channel.UnreleasedVideos.Name = EnglishStrings.UnreleasedVideos.Translate();
            Channels = new List<Channel>() { Channel.UnreleasedVideos }; //Default channel

            Studies.Current = null;
            StoreItems.Current = null;
            Rivals.Current = null;
        }
    }
}