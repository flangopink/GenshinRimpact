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
        private Map map;
        private int usageTickInterval = -1;
        private int sign;
        private int usageTicksLeft;
        private bool startedCasting;
        private bool alreadyUsed;

        private bool HasPawn => parent.pawn != null;
        public float EnergyUsage
        {
            get
            {
                if (HasPawn && Props.energyUsage > 0 && pool != null && pool.PawnEnergyCostMultipliers.TryGetValue(parent.pawn, out float mult))
                {
                    return Props.energyUsage * mult;
                }
                return Props.energyUsage;
            }
        }
        public bool HasEnoughEnergy => pool.energy - EnergyUsage >= 0;
        //public override bool CanCast => pool.energy - EnergyUsage >= 0;
        public string EnergyTooltip => EnergyUsage > 0 ? "GR_EnergyCost".Translate(EnergyUsage.ToString().Colorize(ColoredText.TipSectionTitleColor)).Resolve()
                                                       : "GR_EnergyGain".Translate((-EnergyUsage).ToString().Colorize(ColoredText.ExpectationsColor)).Resolve();
        
        public override void Initialize(AbilityCompProperties props)
        {
            base.Initialize(props);
            if (map == null) UpdateMapPool();
            else pool = map.GetComponent<MapComponent_EnergyPool>();
            if (pool == null) Utils.LogError($"Error in {parent}: MapComponent_EnergyPool is null.");
            usageTickInterval = Props.instaUse ? 0 : Mathf.Max(1, (int)(parent.VerbProperties[0].warmupTime.SecondsToTicks() / Mathf.Abs(EnergyUsage)));
            sign = Math.Sign(EnergyUsage);
        }

        private void UpdateMapPool() 
        {
            map = parent.pawn == null ? Find.CurrentMap : parent.pawn.MapHeld;
            pool = map.GetComponent<MapComponent_EnergyPool>();
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (pool == null || pool.dontShowGizmo) yield break;
            yield return pool.Gizmo;
            pool.dontShowGizmo = true;
        }

        public override void CompTick()
        {
            if (Find.TickManager.TicksGame % 300 == 0)
            {
                if (pool == null || pool.map != Find.CurrentMap)
                {
                    UpdateMapPool();
                }
            }
            if (pool != null)
            {
                if (!startedCasting && parent.Casting)
                {
                    startedCasting = true;
                    usageTicksLeft = (int)Mathf.Abs(EnergyUsage);
                }
                if (startedCasting && usageTicksLeft > 0)
                {
                    if (usageTickInterval == 0 && !alreadyUsed)
                    {
                        pool.UseEnergy(EnergyUsage);
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
                if (pool.energy == 0 && EnergyUsage > 0)
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

        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            if(!HasEnoughEnergy)
            {
                Messages.Message("GR_TargetingCancelledNotEnoughEnergy".Translate(parent.pawn.LabelCap), MessageTypeDefOf.RejectInput, false);
                Find.Targeter.StopTargeting();
                return false;
            }
            return base.Valid(target, throwMessages);
        }

        /*public override void PostApplied(List<LocalTargetInfo> targets, Map map)
        {
        }*/

        public override bool GizmoDisabled(out string reason)
        {
            if (!HasEnoughEnergy)
            {
                reason = "GR_NotEnoughEnergy".Translate();
                return true;
            }
            return base.GizmoDisabled(out reason);
        }

        public override bool AICanTargetNow(LocalTargetInfo target) => !parent.pawn.IsColonistPlayerControlled && HasEnoughEnergy;

        public override void PostExposeData()
        {
            Scribe_References.Look(ref map, "map");
            Scribe_Values.Look(ref startedCasting, "alreadyUsed", false);
            Scribe_Values.Look(ref startedCasting, "alreadyUsed", false);
            Scribe_Values.Look(ref usageTicksLeft, "usageTicksLeft", 0);
        }
    }
}
