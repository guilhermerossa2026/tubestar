using System;

namespace TubeStar
{
    public class StudioLighting : StoreItem
    {
        public override string Name
        {
            get { return "Studio Lighting"; }
        }

        public override string Description
        {
            get { return "Professional softboxes that dramatically improve your video visual quality (+25 Shooting quality)."; }
        }

        public override string ImageName
        {
            get { return "Lighting"; }
        }

        public override int Cost
        {
            get { return 2500; }
        }

        public override int SkillModifier
        {
            get { return 25; }
        }

        public override SkillModifierType SkillModifierType
        {
            get { return TubeStar.SkillModifierType.Shooting; }
        }
    }
}
