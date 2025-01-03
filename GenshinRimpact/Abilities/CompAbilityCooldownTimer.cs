using Verse;
using RimWorld;
using System.Collections.Generic;

namespace Rimpact
{
    public class CompProperties_AbilityCooldownTimer : CompProperties_AbilityEffect
    {
        public int startAfterUses;
        public int timerTicks = 120; // 2 secs
        public int cooldownTicks = 900; // 15 secs

        public CompProperties_AbilityCooldownTimer() => compClass = typeof(CompAbilityCooldownTimer);
    }

    [HotSwap.HotSwappable]
    public class CompAbilityCooldownTimer : CompAbilityEffect   // Multiple charges with long cooldown (Diluc)
    {
        public new CompProperties_AbilityCooldownTimer Props => (CompProperties_AbilityCooldownTimer)props;

        private int timerLeft;
        private bool timerActive;
        private bool ShouldSetOnLongCooldown => timerLeft <= 0 && timerActive;

        public float TimerSeconds => timerLeft.TicksToSeconds();

        public override string ExtraTooltipPart()
        {
            return timerActive ? "AbilityCooldownTimerActive".Translate(TimerSeconds.ToString("0.0"), "LetterSecond".Translate()).Resolve()
                               : (Props.startAfterUses == 1 ? "AbilityCooldownTimerAfterUse".Translate(Props.cooldownTicks.TicksToSeconds(), Props.timerTicks.TicksToSeconds(), "LetterSecond".Translate()).Resolve()
                                                            : "AbilityCooldownTimerAfterUses".Translate(Props.cooldownTicks.TicksToSeconds(), Props.timerTicks.TicksToSeconds(), "LetterSecond".Translate(), Props.startAfterUses).Resolve());
        }

        public override void PostApplied(List<LocalTargetInfo> targets, Map map)
        {
            base.PostApplied(targets, map);
            //Utils.LogMessage(parent.RemainingCharges + " ... " + (parent.maxCharges - Props.startAfterUses));
            if (parent.RemainingCharges == parent.maxCharges - Props.startAfterUses)
            {
                timerActive = true;
                timerLeft = Props.timerTicks;
            }
            else if (parent.RemainingCharges == parent.maxCharges) // it goes like 2 1 3, not 2 1 0
            {
                timerLeft = 0;
            }
           //Utils.LogMessage(!ShouldSetOnLongCooldown + " " + (parent.RemainingCharges != parent.maxCharges));
            if (!ShouldSetOnLongCooldown && parent.RemainingCharges != parent.maxCharges) StartCooldownNoRecharge(parent.def.cooldownTicksRange.RandomInRange);
        }

        public void StartCooldownNoRecharge(int ticks)
        {
            PrivateFields.Ability_inCooldown.SetValue(parent, true);
            PrivateFields.Ability_cooldownEndTick.SetValue(parent, GenTicks.TicksGame + ticks);
            PrivateFields.Ability_cooldownDuration.SetValue(parent, ticks);
        }

        public override void CompTick()
        {
            base.CompTick();
            if (ShouldSetOnLongCooldown)
            {
                parent.StartCooldown(Props.cooldownTicks);
                timerActive = false;
            }
            if (!timerActive) return;
            timerLeft--;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref timerLeft, "timerLeft");
            Scribe_Values.Look(ref timerActive, "timerActive");
        }
        public override bool AICanTargetNow(LocalTargetInfo target) => !parent.pawn.IsColonistPlayerControlled;
    }
}
