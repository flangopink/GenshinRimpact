using RimWorld;
using System.Collections.Generic;
using Verse;

namespace GenshinRimpact
{
    public class CompProperties_AbilityCancelEffect : CompProperties_AbilityEffect
    {
        public HediffDef hediff;
        //public AbilityDef ability;

        public CompProperties_AbilityCancelEffect() => compClass = typeof(CompAbilityCancelEffect);
    }

    [HotSwap.HotSwappable]
    public class CompAbilityCancelEffect : CompAbilityEffect
    {
        public new CompProperties_AbilityCancelEffect Props => (CompProperties_AbilityCancelEffect)props;

        private List<Hediff> PawnHediffs => parent.pawn.health.hediffSet.hediffs;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            {
                if (Props.hediff != null)
                {
                    for (int i = 0; i < PawnHediffs.Count; i++)
                    {
                        if (PawnHediffs[i].def == Props.hediff)
                        {
                            parent.pawn.health.RemoveHediff(PawnHediffs[i]);
                        }
                    }
                }
            }
        }
    }
}
