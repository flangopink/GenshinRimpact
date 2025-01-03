using RimWorld;
using Verse;

namespace Rimpact
{
    public class Command_AbilityCooldownTimer(Ability ability, Pawn pawn) : Command_Ability(ability, pawn)
    {
        private CompAbilityCooldownTimer comp;
        private CompAbilityCooldownTimer Comp
        {
            get
            {
                if (comp == null)
                {
                    foreach (CompAbilityEffect c in ability.EffectComps)
                    {
                        if (c is CompAbilityCooldownTimer timerComp)
                        {
                            comp = timerComp;
                        }
                    }
                }
                return comp;
            }
        }
        private string TimerSecondsString => Comp?.TimerSeconds.ToString("0.0") + "LetterSecond".Translate();

        public override string TopRightLabel => ability.GizmoExtraLabel + (Comp != null ? ("\n" + TimerSecondsString) : "");

    }
}
