using Verse;
using RimWorld;
using System.Collections.Generic;

namespace GenshinRimpact
{
    public class CompProperties_AbilityCooldownTimer : CompProperties_AbilityEffect
    {
        public int startAfterUses = 1;
        public int timerTicks = 1800; // 30 secs

        public CompProperties_AbilityCooldownTimer() => compClass = typeof(CompAbilityCooldownTimer);
    }

    [HotSwap.HotSwappable]
    public class CompAbilityCooldownTimer : CompAbilityEffect
    {
        public new CompProperties_AbilityCooldownTimer Props => (CompProperties_AbilityCooldownTimer)props;

        private int timerLeft;
        private bool timerActive;

        public float TimerSeconds => timerLeft.TicksToSeconds();

        public override string ExtraTooltipPart()
        {
            return timerActive ? "AbilityCooldownTimerActive".Translate(TimerSeconds.ToString("0.0"), "LetterSecond".Translate()) 
                               : (Props.startAfterUses == 1 ? "AbilityCooldownTimerAfterUse".Translate(Props.timerTicks.TicksToSeconds(), "LetterSecond".Translate())
                                                            : "AbilityCooldownTimerAfterUses".Translate(Props.timerTicks.TicksToSeconds(), "LetterSecond".Translate(), Props.startAfterUses));
        }

        public override void PostApplied(List<LocalTargetInfo> targets, Map map)
        {
            base.PostApplied(targets, map);
            Log.Message(parent.RemainingCharges);
            if (parent.RemainingCharges == parent.maxCharges - Props.startAfterUses)
            {
                timerActive = true;
                timerLeft = Props.timerTicks;
            }
            else if (parent.RemainingCharges == parent.maxCharges)
            {
                timerActive = false;
                timerLeft = 0;
            }
        }

        public override void CompTick()
        {
            base.CompTick();
            if (!timerActive) return;
            
            if (timerLeft <= 0)
            {
                parent.StartCooldown(parent.def.cooldownTicksRange.RandomInRange);
                timerActive = false;
            }
            timerLeft--;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref timerLeft, "timerLeft");
            Scribe_Values.Look(ref timerActive, "timerActive");
        }
    }
}
