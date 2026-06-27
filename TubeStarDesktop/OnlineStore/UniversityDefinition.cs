using System;
using System.Collections.Generic;

namespace TubeStar
{
    [Serializable]
    public class UniversityDefinition
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double EnrollmentFee { get; set; }
        public double DailyTuition { get; set; }
        public double HoursSpeedBonus { get; set; } // e.g., 0.20 means 20% fewer hours required to complete courses
        public double SkillGainBonusMultiplier { get; set; } // e.g., 1.0 means standard, 2.0 means double
        public string Description { get; set; }
        public List<string> AllowedStudies { get; set; } // List of Study class names allowed (empty/null means all allowed)

        public UniversityDefinition()
        {
            AllowedStudies = new List<string>();
        }

        public UniversityDefinition(string id, string name, double enrollmentFee, double dailyTuition, double hoursSpeedBonus, double skillGainBonusMultiplier, string desc, List<string> allowedStudies = null)
        {
            Id = id;
            Name = name;
            EnrollmentFee = enrollmentFee;
            DailyTuition = dailyTuition;
            HoursSpeedBonus = hoursSpeedBonus;
            SkillGainBonusMultiplier = skillGainBonusMultiplier;
            Description = desc;
            AllowedStudies = allowedStudies ?? new List<string>();
        }
    }
}
