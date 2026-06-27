using System;
using System.Collections.Generic;
using System.Linq;

namespace TubeStar
{
    public class Channel : UniqueObject
    {
        public string Name { get; set; }
        public bool IsRivalChannel { get; set; }
        public int MinimumSubsribers { get; set; }

        private AdvertisingStrategy _advertising;
        public AdvertisingStrategy Advertising
        {
            get { return _advertising; }
            set
            {
                _advertising = value;
                _incomePerView = null;
            }
        }

        private double? _incomePerView;
        public double IncomePerView
        {
            get
            {
                if (_incomePerView == null)
                {
                    _incomePerView = Advertising.GetAttribute<AdvertistingIncomeAttribute>().IncomePerView;
                }
                return _incomePerView.Value;
            }
        }

        public HashSet<Video> Videos { get; set; }
        public int Subscribers { get; set; }

        public double Income { get; set; }

        public bool IsSuspended { get; set; }

        // Staff Properties
        public bool HiredEditor { get; set; }
        public bool HiredManager { get; set; }
        public double EditorBaseSalary { get; set; }
        public double ManagerBaseSalary { get; set; }
        public int EditorXP { get; set; }

        public int EditorLevel
        {
            get { return 1 + EditorXP / 100; }
        }

        public double EditorCurrentSalary
        {
            get { return EditorBaseSalary * (1 + (EditorLevel - 1) * 0.15); }
        }

        public double ManagerCurrentSalary
        {
            get { return ManagerBaseSalary; }
        }

        //Stats
        public List<double> IncomeOverTime { get; set; }
        public List<double> ExpensesOverTime { get; set; }
        public List<double> SubscribersOverTime { get; set; }

        public Channel()
            : base()
        {
            Videos = new HashSet<Video>();
            Advertising = AdvertisingStrategy.Normal;
            IncomeOverTime = new List<double>();
            ExpensesOverTime = new List<double>();
            SubscribersOverTime = new List<double>();

            HiredEditor = false;
            HiredManager = false;
            EditorBaseSalary = 200.0;
            ManagerBaseSalary = 2000.0;
            EditorXP = 0;
        }

        private static Channel _unreleasedVideos;
        public static Channel UnreleasedVideos
        {
            get
            {
                if (_unreleasedVideos == null)
                    _unreleasedVideos = new Channel() { Name = EnglishStrings.UnreleasedVideos.Translate() };
                return _unreleasedVideos;
            }
            set { _unreleasedVideos = value; }
        }
    }
}