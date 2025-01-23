using RimWorld;
using Verse;

namespace Rimpact
{
    [HotSwap.HotSwappable]
    public class Command_AbilityCooldownTimer(Ability ability, Pawn pawn) : Command_Ability(ability, pawn)
    {
        private CompAbilityChargesCooldownTimer comp;
        private CompAbilityChargesCooldownTimer Comp
        {
            get
            {
                if (comp == null)
                {
                    foreach (CompAbilityEffect c in ability.EffectComps)
                    {
                        if (c is CompAbilityChargesCooldownTimer timerComp)
                        {
                            comp = timerComp;
                        }
                    }
                }
                return comp;
            }
        }
        private string TimerSecondsString => Comp?.TimerSeconds.ToString("0.0") + "LetterSecond".Translate();

        public override string TopRightLabel => ability.GizmoExtraLabel + (Comp != null ? ($"\n{TimerSecondsString} - {Comp.charges}/{Comp.Props.maxCharges}") : "");
    }
}
