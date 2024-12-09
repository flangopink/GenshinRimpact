using RimWorld;
using System.Collections.Generic;
using Verse;

namespace GenshinRimpact
{
    public class CompProperties_AbilityShootMaxRange : CompProperties_AbilityEffect
    {
        public ThingDef projectileDef;
        public bool preventFriendlyFire = true;
        public CompProperties_AbilityShootMaxRange() => compClass = typeof(CompAbilityShootMaxRange);

        public override IEnumerable<string> ConfigErrors(AbilityDef parentDef)
        {
            if (projectileDef == null) yield return parentDef + " does not have projectileDef assigned in CompProperties_AbilityShootMaxRange";
        }
    }

    [HotSwap.HotSwappable]
    public class CompAbilityShootMaxRange : CompAbilityEffect
    {
        public new CompProperties_AbilityShootMaxRange Props => (CompProperties_AbilityShootMaxRange)props;
        private Pawn Caster => parent.pawn;
        private float Range => parent.verb.verbProps.range;
        public ThingDef Projectile => Props.projectileDef; // removed unnecessary checks

        private static List<IntVec3> tmpCells = [];

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            bool isCasted = TryCastShotASMR(target);
            Utils.LogMessage("asmred");
            if (isCasted)
            {
                Caster.records.Increment(RecordDefOf.ShotsFired);
                parent.StartCooldown(parent.def.cooldownTicksRange.RandomInRange);
            }
        }

        private bool TryCastShotASMR(LocalTargetInfo currentTarget) // lol, asmr
        {
            Utils.LogMessage("trying to asmr");
            if (currentTarget.HasThing && currentTarget.Thing.Map != Caster.Map)
                return false;

            if (Projectile == null) 
                return false;

            bool flag = parent.verb.TryFindShootLineFromTo(Caster.Position, currentTarget, out ShootLine resultingLine);

            if (!flag) 
                return false;

            Projectile proj = (Projectile)GenSpawn.Spawn(Projectile, resultingLine.Source, Caster.Map);

            IntVec3 maxRangeCell = Utils.RedirectIntVec3ToMaxRange(Caster.Position, resultingLine.Dest, Caster.Map, Range);

            proj.Launch(Caster, Caster.DrawPos, maxRangeCell, currentTarget, ProjectileHitFlags.None, Props.preventFriendlyFire);
            if (proj.GetComp<CompDamageArea>() is CompDamageArea cda)
            {
                cda.abilityDef = parent.def;
            }
            return true;
        }

        public override void DrawEffectPreview(LocalTargetInfo target)
        {
            if (target.IsValid)
            {
                GenDraw.DrawTargetHighlight(target);
                //DrawHighlightFieldRadiusAroundTarget(target);
                GenDraw.DrawFieldEdges(Utils.AffectedLineCells(ref tmpCells, Caster.Position, Utils.RedirectIntVec3ToMaxRange(Caster.Position, target.Cell, Caster.Map, Range), Caster.MapHeld, Range, parent.verb, true)); ;
            }
        }
    }
}
