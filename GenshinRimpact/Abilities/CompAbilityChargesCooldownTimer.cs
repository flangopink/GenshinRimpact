using Verse;
using RimWorld;
using System.Collections.Generic;

namespace Rimpact
{
    public class CompProperties_AbilityChargesCooldownTimer : CompProperties_AbilityEffect
    {
        public int startAfterUses;
        public int timerTicks = 120; // 2 secs
        public int longCooldownTicks = 900; // 15 secs
        public int maxCharges = 3;

        public CompProperties_AbilityChargesCooldownTimer() => compClass = typeof(CompAbilityChargesCooldownTimer);
    }

    [HotSwap.HotSwappable]
    public class CompAbilityChargesCooldownTimer : CompAbilityEffect   // Multiple charges with long cooldown (Diluc)
    {
        public new CompProperties_AbilityChargesCooldownTimer Props => (CompProperties_AbilityChargesCooldownTimer)props;

        public int charges = -1;
        private int timerLeft;
        private bool timerActive;
        private bool ShouldSetOnLongCooldown => timerLeft <= 0 && timerActive;

        public float TimerSeconds => timerLeft.TicksToSeconds();
        public override bool GizmoDisabled(out string reason)
        {
            if (charges <= 0)
            {
                reason = "AbilityNoCharges".Translate();
                return true;
            }
            else return base.GizmoDisabled(out reason);
        }

        public override void Initialize(AbilityCompProperties props)
        {
            base.Initialize(props);
            charges = Props.maxCharges;
        }

        public override string ExtraTooltipPart()
        {
            return timerActive ? "AbilityCooldownTimerActive".Translate(TimerSeconds.ToString("0.0"), "LetterSecond".Translate()).Resolve()
                               : (Props.startAfterUses == 1 ? "AbilityCooldownTimerAfterUse".Translate(Props.longCooldownTicks.TicksToSeconds(), Props.timerTicks.TicksToSeconds(), "LetterSecond".Translate()).Resolve()
                                                            : "AbilityCooldownTimerAfterUses".Translate(Props.longCooldownTicks.TicksToSeconds(), Props.timerTicks.TicksToSeconds(), "LetterSecond".Translate(), Props.startAfterUses).Resolve());
        }

        public override void PostApplied(List<LocalTargetInfo> targets, Map map)
        {
            base.PostApplied(targets, map);
            charges--;
            //Utils.LogMessage(charges + " ... " + (Props.maxCharges - Props.startAfterUses));
            if (charges == Props.maxCharges - Props.startAfterUses)
            {
                timerActive = true;
                timerLeft = Props.timerTicks;
            }
            else if (charges == 0)
            {
                timerLeft = 0;
            }
            //Utils.LogMessage(!ShouldSetOnLongCooldown + " " + (charges != Props.maxCharges));
            if (!ShouldSetOnLongCooldown && charges != Props.maxCharges) 
                parent.StartCooldown(parent.def.cooldownTicksRange.RandomInRange); //StartCooldownNoRecharge(parent.def.cooldownTicksRange.RandomInRange);
        }

        /*public void StartCooldownNoRecharge(int ticks)
        {
            PrivateFields.Ability_inCooldown.SetValue(parent, true);
            PrivateFields.Ability_cooldownEndTick.SetValue(parent, GenTicks.TicksGame + ticks);
            PrivateFields.Ability_cooldownDuration.SetValue(parent, ticks);
        }*/

        public override void CompTick()
        {
            if (!timerActive) return;
            if (ShouldSetOnLongCooldown)
            {
                parent.StartCooldown(Props.longCooldownTicks);
                timerActive = false;
                charges = Props.maxCharges;
            }
            timerLeft--;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref timerLeft, "timerLeft");
            Scribe_Values.Look(ref timerActive, "timerActive");
            Scribe_Values.Look(ref charges, "charges");
        }
        public override bool AICanTargetNow(LocalTargetInfo target) => !parent.pawn.IsColonistPlayerControlled && charges > 0;
    }
}
