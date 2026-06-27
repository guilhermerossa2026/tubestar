using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Xml.Serialization;

namespace TubeStar
{
    public enum SkillModifierType
    {
        Shooting,
        PostProduction,
        VideoAttribute,
        ViewQuality,
        None,
    }

    [XmlInclude(typeof(StudyAudienceAnalysisI))]
    [XmlInclude(typeof(StudyAudienceAnalysisII))]
    [XmlInclude(typeof(StudyPostProductionI))]
    [XmlInclude(typeof(StudyPostProductionII))]
    [XmlInclude(typeof(StudyPostProductionIII))]
    [XmlInclude(typeof(StudyProductionI))]
    [XmlInclude(typeof(StudyProductionII))]
    [XmlInclude(typeof(StudyProductionIII))]
    [XmlInclude(typeof(StudyQualityAnalysis))]
    public abstract class Study : Task
    {
        public abstract Study Prerequisite { get; }
        public abstract int Cost { get; }

        public abstract int SkillModifier { get; }
        public abstract SkillModifierType SkillModifierType { get; }

        public abstract int BaseHoursToComplete { get; }

        public override int HoursToComplete
        {
            get
            {
                double speedBonus = 0.0;
                if (Player.Current != null && !string.IsNullOrEmpty(Player.Current.EnrolledUniversityId))
                {
                    var uni = UniversityCatalog.GetUniversityById(Player.Current.EnrolledUniversityId);
                    if (uni != null)
                    {
                        speedBonus = uni.HoursSpeedBonus;
                    }
                }
                return (int)Math.Max(1, Math.Round(BaseHoursToComplete * (1.0 - speedBonus)));
            }
        }

        public override TaskType TaskType
        {
            get { return TaskType.Study; }
        }

        public override Color Color
        {
            get { return Colors.DodgerBlue; }
        }
    }
}