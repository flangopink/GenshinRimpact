using Verse;

namespace Rimpact
{
    public class HediffCompProperties_EnergyCostMultiplier : HediffCompProperties
    {
        public float mult = 1f;
        public HediffCompProperties_EnergyCostMultiplier() => compClass = typeof(HediffComp_EnergyCostMultiplier);
    }

    public class HediffComp_EnergyCostMultiplier : HediffComp
    {
        public HediffCompProperties_EnergyCostMultiplier Props => (HediffCompProperties_EnergyCostMultiplier)props;
        public MapComponent_EnergyPool Pool => Pawn.MapHeld.GetComponent<MapComponent_EnergyPool>();
        public override string CompTipStringExtra => "GR_EnergyCostMultiplier".Translate() + ": x" + Props.mult.ToStringPercent();
        public override void CompPostPostAdd(DamageInfo? dinfo) => Pool?.PawnEnergyCostMultipliers.SetOrAdd(Pawn, Props.mult);
        public override void CompPostPostRemoved() => Pool?.PawnEnergyCostMultipliers.Remove(Pawn);
    }
}
