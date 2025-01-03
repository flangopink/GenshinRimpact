using RimWorld;
using Verse;

namespace Rimpact
{
    public class CompProperties_ExplodeOnDestroyed : CompProperties
    {
        public bool onCommandOnly;
        public float explosionRadius = 2.9f;
        public int damageAmount = -1;
        public DamageDef explosionDamageDef;
        public CompProperties_ExplodeOnDestroyed() => compClass = typeof(CompExplodeOnDestroyed);
    }

    public class CompExplodeOnDestroyed : ThingComp
    {
        public CompProperties_ExplodeOnDestroyed Props => (CompProperties_ExplodeOnDestroyed)props;
        public bool shouldExplode;
        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            if (!Props.onCommandOnly || (Props.onCommandOnly && shouldExplode))
                GenExplosion.DoExplosion(parent.PositionHeld, previousMap, Props.explosionRadius, Props.explosionDamageDef ?? DamageDefOf.Bomb, parent, Props.damageAmount);
        }
        public override void PostExposeData()
        {
            Scribe_Values.Look(ref shouldExplode, "shouldExplode");
        }
    }
}
