using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Rimpact
{
    public class CompProperties_AbilityHideGizmo : CompProperties_AbilityEffect
    {
        public AbilityDef otherAbility;
        public HediffDef hediff;
        public bool inverse;

        public CompProperties_AbilityHideGizmo() => compClass = typeof(CompAbilityHideGizmo);
    }

    [HotSwap.HotSwappable]
    public class CompAbilityHideGizmo : CompAbilityEffect
    {
        public new CompProperties_AbilityHideGizmo Props => (CompProperties_AbilityHideGizmo)props;

        public override bool ShouldHideGizmo 
        {
            get 
            {
                //if (Props.otherAbility != null) return Props.inverse ? LinkedAbility.GizmosVisible() : !LinkedAbility.GizmosVisible();
                if (Props.hediff != null) return Props.inverse ? LinkedHediff == null : LinkedHediff != null;
                return false;
            }
        }

        private List<Hediff> PawnHediffs => parent.pawn.health.hediffSet.hediffs;
        private Hediff LinkedHediff
        {
            get
            {
                for (int i = 0; i < PawnHediffs.Count; i++)
                {
                    if (PawnHediffs[i].def == Props.hediff)
                    {
                        return PawnHediffs[i];
                    }
                }
                return null;
            }
        }
        //private Ability LinkedAbility => parent.pawn.abilities.GetAbility(Props.otherAbility);

        public override bool AICanTargetNow(LocalTargetInfo target) => !parent.pawn.IsColonistPlayerControlled;
    }
}
