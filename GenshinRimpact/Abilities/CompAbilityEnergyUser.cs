using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Rimpact
{
    public class CompProperties_AbilityEnergyUser : CompProperties_AbilityEffect
    {
        public float energyUsage = 10f;
        public bool instaUse;
        public CompProperties_AbilityEnergyUser() => compClass = typeof(CompAbilityEnergyUser);
    }

    [HotSwap.HotSwappable]
    public class CompAbilityEnergyUser : CompAbilityEffect
    {
        public new CompProperties_AbilityEnergyUser Props => (CompProperties_AbilityEnergyUser)props;

        public MapComponent_EnergyPool pool;
        private int usageTickInterval = -1;
        private int sign;
        private int usageTicksLeft;
        private bool startedCasting;
        private bool alreadyUsed;

        public override bool CanCast => pool.energy - Props.energyUsage >= 0;
        public string EnergyTooltip => Props.energyUsage > 0 ? "GR_EnergyCost".Translate(Props.energyUsage.ToString().Colorize(ColoredText.TipSectionTitleColor)).Resolve()
                                                             : "GR_EnergyGain".Translate((-Props.energyUsage).ToString().Colorize(ColoredText.ExpectationsColor)).Resolve();

        public override void Initialize(AbilityCompProperties props)
        {
            base.Initialize(props);
            pool = parent.pawn.MapHeld.GetComponent<MapComponent_EnergyPool>();
            if (pool == null) Utils.LogError($"Error in {parent}: MapComponent_EnergyPool is null.");
            usageTickInterval = Props.instaUse ? 0 : Mathf.Max(1, (int)(parent.VerbProperties[0].warmupTime.SecondsToTicks() / Mathf.Abs(Props.energyUsage)));
            sign = Math.Sign(Props.energyUsage);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (pool == null || pool.dontShowGizmo) yield break;
            yield return pool.Gizmo;
            pool.dontShowGizmo = true;
        }

        public override void CompTick()
        {
            if (pool != null)
            {
                if (!startedCasting && parent.Casting)
                {
                    startedCasting = true;
                    usageTicksLeft = (int)Mathf.Abs(Props.energyUsage);
                }
                if (startedCasting && usageTicksLeft > 0)
                {
                    if (usageTickInterval == 0 && !alreadyUsed)
                    {
                        pool.UseEnergy(Props.energyUsage);
                        alreadyUsed = true;
                        startedCasting = false;
                        return;
                    }
                    else UsageTick();
                }
                alreadyUsed = false;
            }
        }

        private void UsageTick()
        {
            if (Find.TickManager.TicksGame % usageTickInterval == 0)
            {
                if (pool.energy == 0 && Props.energyUsage > 0)
                {
                    parent.pawn.stances.stunner.StunFor(Utils.settings.interruptedAbilityStunDuration, parent.pawn, false, true, false);
                    parent.pawn.jobs.StopAll();
                    Messages.Message("GR_NotEnoughEnergyAbilityInterrupted".Translate(parent.pawn.LabelCap), MessageTypeDefOf.NegativeEvent);
                    startedCasting = false;
                    return;
                }
                pool.UseEnergy(1 * sign);
                usageTicksLeft--;
                if (usageTicksLeft == 0) startedCasting = false;
            }
        }

        public override string ExtraLabelMouseAttachment(LocalTargetInfo target)
        {
            return EnergyTooltip;
        }

        public override string ExtraTooltipPart()
        {
            return EnergyTooltip;
        }

        /*public override void PostApplied(List<LocalTargetInfo> targets, Map map)
        {
            pool.UseEnergy(Props.energyUsage);
        }*/

        public override bool GizmoDisabled(out string reason)
        {
            if (!CanCast)
            {
                reason = "GR_NotEnoughEnergy".Translate();
                return true;
            }
            return base.GizmoDisabled(out reason);
        }

        public override bool AICanTargetNow(LocalTargetInfo target) => !parent.pawn.IsColonistPlayerControlled;

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref alreadyUsed, "alreadyUsed", false);
            Scribe_Values.Look(ref startedCasting, "alreadyUsed", false);
            Scribe_Values.Look(ref usageTicksLeft, "usageTicksLeft", 0);
        }
    }
}
