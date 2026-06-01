using System;
using System.Collections.Generic;
using System.IO;

namespace TubeStar
{
    public enum Trophy
    {
        Pupil = 1358,
        PropDepartment = 1363,
        Loser = 1366,
        PoopStar = 1359,
        AptPupil = 1361,
        RantMaster = 1364,
        WellHeeld = 1367,
        Bunsen = 1365,
        HookLineAndSinker = 1362,
        InternetFamous = 1360,
        DownTheTubes = 1922,
        Procrastinator = 1924,
        InItToWinIt = 1926,
        TopModel = 1923,
        GiveAndTake = 1925,
        RebelLeader = 1950,
        RobotMasters = 1949,
        Upgrade = 7610,
        King = 7611,
        CatInBin = 7612,
        OCD = 7613,
        OCD2 = 7614,
    }

    public static class TrophyManager
    {
        private static List<Trophy> _achievedTrophies;
        private static readonly string TrophiesFile = SaveLoadHelper.SaveDirectory + @"\trophies.xml";

        static TrophyManager()
        {
            _achievedTrophies = new List<Trophy>();
            LoadTrophies();
        }

        private static void LoadTrophies()
        {
            try
            {
                if (!Directory.Exists(SaveLoadHelper.SaveDirectory))
                {
                    Directory.CreateDirectory(SaveLoadHelper.SaveDirectory);
                }
                if (File.Exists(TrophiesFile))
                {
                    var xml = File.ReadAllText(TrophiesFile);
                    _achievedTrophies = SerializationHelpers.FromXml<List<Trophy>>(xml);
                }
            }
            catch
            {
                _achievedTrophies = new List<Trophy>();
            }
        }

        private static void SaveTrophies()
        {
            try
            {
                var xml = SerializationHelpers.ToXml(_achievedTrophies);
                File.WriteAllText(TrophiesFile, xml);
            }
            catch
            {
            }
        }

        public static bool HasTrophy(Trophy trophy)
        {
            return _achievedTrophies.Contains(trophy);
        }

        public static void UnlockTrophy(Trophy trophy)
        {
            if (HasTrophy(trophy))
                return;

            _achievedTrophies.Add(trophy);
            SaveTrophies();

            string trophyName = trophy.ToString();
            // Let's make names look nicer by splitting CamelCase
            try
            {
                trophyName = System.Text.RegularExpressions.Regex.Replace(trophy.ToString(), "([a-z])([A-Z])", "$1 $2");
            }
            catch { }

            CustomMessageBox.ShowDialog(
                "Conquista Desbloqueada! / Trophy Unlocked!", 
                "Você desbloqueou a conquista: " + trophyName, 
                MessagePicture.Happy
            );
        }
    }
}