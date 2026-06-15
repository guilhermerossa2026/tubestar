using System;

namespace TubeStar
{
    public class Microphone : StoreItem
    {
        public override string Name
        {
            get { return "Condenser Microphone"; }
        }

        public override string Description
        {
            get { return "Improves the audio quality of your videos (+15 Shooting quality)."; }
        }

        public override string ImageName
        {
            get { return "Microphone"; }
        }

        public override int Cost
        {
            get { return 1200; }
        }

        public override int SkillModifier
        {
            get { return 15; }
        }

        public override SkillModifierType SkillModifierType
        {
            get { return TubeStar.SkillModifierType.Shooting; }
        }
    }
}
