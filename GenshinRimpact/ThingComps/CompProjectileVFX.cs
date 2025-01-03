using Verse;

namespace Rimpact
{
    public class CompProperties_ProjectileVFX : CompProperties
    {
        public bool rotating;
        public bool counterClockwise;
        public int intervalTicks = 60;
        public float rotationSpeed = 10f;
        public bool stopRotatingOnImpact;
        public EffecterDef effecter;
        public bool effecterAttached;

        public CompProperties_ProjectileVFX()
        {
            compClass = typeof(CompProjectileVFX);
        }
    }

    public class CompProjectileVFX : ThingComp
    {
        public CompProperties_ProjectileVFX Props => (CompProperties_ProjectileVFX)props;
        private Effecter attachedEffecter;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (parent.Map != null && Props.effecterAttached && attachedEffecter == null)
                attachedEffecter = Props.effecter?.SpawnAttached(parent, parent.Map);
        }

        public override void CompTick()
        {
            base.CompTick();
            if (!Props.effecterAttached && parent.Map != null && parent.IsHashIntervalTick(Props.intervalTicks))
            {
                Props.effecter?.Spawn(parent.PositionHeld, parent.MapHeld).Cleanup();
            }
        }
    }
}
