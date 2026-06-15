using System;

namespace TubeStar
{
    public class CaptureCard : StoreItem
    {
        public override string Name
        {
            get { return "4K Capture Card"; }
        }

        public override string Description
        {
            get { return "High-end video capture card to record gameplay in 4K resolution (+12 Post-Production quality)."; }
        }

        public override string ImageName
        {
            get { return "CaptureCard"; }
        }

        public override int Cost
        {
            get { return 1800; }
        }

        public override int SkillModifier
        {
            get { return 12; }
        }

        public override SkillModifierType SkillModifierType
        {
            get { return TubeStar.SkillModifierType.PostProduction; }
        }
    }
}
