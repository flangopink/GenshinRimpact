using Verse;

namespace Rimpact
{
    public class HediffCompProperties_EnergyRegenMultiplier : HediffCompProperties
    {
        public float multOffset = 1f;
        public HediffCompProperties_EnergyRegenMultiplier() => compClass = typeof(HediffComp_EnergyRegenMultiplier);
    }

    public class HediffComp_EnergyRegenMultiplier : HediffComp
    {
        public HediffCompProperties_EnergyRegenMultiplier Props => (HediffCompProperties_EnergyRegenMultiplier)props;
        public MapComponent_EnergyPool Pool => Pawn.MapHeld.GetComponent<MapComponent_EnergyPool>();
        public override string CompTipStringExtra => "GR_EnergyRegenRateDelay".Translate() + ": " + Props.multOffset.ToStringPercentSigned();
        public override void CompPostPostAdd(DamageInfo? dinfo) { if (Pool != null) Pool.regenRateMultiplier += Props.multOffset; }
        public override void CompPostPostRemoved() { if (Pool != null) Pool.regenRateMultiplier -= Props.multOffset; }
    }
}
