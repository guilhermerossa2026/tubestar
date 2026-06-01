using System;
using System.Collections.Generic;
using System.IO;

namespace TubeStar
{
    public static class Settings
    {
        public static VideoCategory? LastCategory { get; set; }
        public static Channel LastChannel { get; set; }
        public static string PlayerName { get; set; }

        public static string GameJoltLogin { get; set; }
        public static string GameJoltToken { get; set; }

        public static bool ListView { get; set; }
        public static bool UseCreativeCommons { get; set; }

        public static string RivalsModPath { get; set; }

        private static string _customYouTubeApiKey = null;
        public static string CustomYouTubeApiKey
        {
            get
            {
                if (_customYouTubeApiKey == null)
                {
                    _customYouTubeApiKey = string.Empty;
                    try
                    {
                        string path = SaveLoadHelper.SaveDirectory + @"\custom_key.txt";
                        if (File.Exists(path))
                        {
                            _customYouTubeApiKey = File.ReadAllText(path).Trim();
                        }
                    }
                    catch { }
                }
                return _customYouTubeApiKey;
            }
            set
            {
                _customYouTubeApiKey = value;
                try
                {
                    string dir = SaveLoadHelper.SaveDirectory;
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    File.WriteAllText(dir + @"\custom_key.txt", value);
                }
                catch { }
            }
        }
    }
}