using Verse;

namespace Rimpact
{
    public class AoEKnockbackParameters
    {
        public bool showLandingCells;
        public bool isPull = false;
        public float distance = 3f;
        public float landingRadius = 0.2f;
        public ThingDef flyerDef;
        public EffecterDef flyerEffecter;
    }
}
