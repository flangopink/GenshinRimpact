using RimWorld;
using System.Collections.Generic;
using Verse;

namespace GenshinRimpact
{
    public class CompProperties_AbilityHideGizmo : CompProperties_AbilityEffect
    {
        public HediffDef hediff;
        public bool inverse;
        //public AbilityDef ability;

        public CompProperties_AbilityHideGizmo() => compClass = typeof(CompAbilityHideGizmo);
    }

    [HotSwap.HotSwappable]
    public class CompAbilityHideGizmo : CompAbilityEffect
    {
        public new CompProperties_AbilityHideGizmo Props => (CompProperties_AbilityHideGizmo)props;

        public override bool ShouldHideGizmo => Props.inverse ? LinkedHediff == null : LinkedHediff != null;

        private List<Hediff> PawnHediffs => parent.pawn.health.hediffSet.hediffs;
        private Hediff LinkedHediff
        {
            get
            {
                if (Props.hediff != null)
                {
                    for (int i = 0; i < PawnHediffs.Count; i++)
                    {
                        if (PawnHediffs[i].def == Props.hediff)
                        {
                            return PawnHediffs[i];
                        }
                    }
                }
                return null;
            }
        }
    }
}
