using Verse;

namespace GenshinRimpact
{
    public class CompProperties_ProjectileVFX : CompProperties
    {
        public bool rotating;
        public bool counterClockwise;
        public int intervalTicks = 60;
        public float rotationSpeed = 10f;
        public bool stopRotatingOnImpact;
        public EffecterDef effecter;

        public CompProperties_ProjectileVFX()
        {
            compClass = typeof(CompProjectileVFX);
        }
    }

    public class CompProjectileVFX : ThingComp
    {
        public CompProperties_ProjectileVFX Props => (CompProperties_ProjectileVFX)props;

        public override void CompTick()
        {
            base.CompTick();
            if (parent.Map != null && parent.IsHashIntervalTick(Props.intervalTicks))
            {
                Props.effecter?.Spawn(parent.PositionHeld, parent.MapHeld).Cleanup();
            }
        }
    }
}
