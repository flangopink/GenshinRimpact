using RimWorld;
using System.Collections.Generic;
using Verse;

namespace GenshinRimpact
{
    public class Verb_AbilityShootMaxRange : Verb_AbilityShoot
    {
        public override ThingDef Projectile => verbProps.defaultProjectile; // removed unnecessary checks

        private static List<IntVec3> tmpCells = [];

        protected override bool TryCastShot()
        {
            bool isCasted = TryCastShotASMR();
            if (isCasted && CasterIsPawn)
            {
                CasterPawn.records.Increment(RecordDefOf.ShotsFired);
            }
            if (isCasted)
            {
                Ability.StartCooldown(Ability.def.cooldownTicksRange.RandomInRange);
            }
            return isCasted;
        }

        private bool TryCastShotASMR() // lol, asmr
        {
            if (currentTarget.HasThing && currentTarget.Thing.Map != caster.Map)
                return false;
            
            if (Projectile == null) 
                return false;

            bool flag = TryFindShootLineFromTo(caster.Position, currentTarget, out ShootLine resultingLine);

            if (verbProps.stopBurstWithoutLos && !flag) 
                return false;

            lastShotTick = Find.TickManager.TicksGame;
            Projectile proj = (Projectile)GenSpawn.Spawn(Projectile, resultingLine.Source, caster.Map);

            IntVec3 maxRangeCell = Utils.MaxRangeIntVec3(resultingLine.Dest - caster.Position, verbProps.range);

            proj.Launch(caster, caster.DrawPos, maxRangeCell, currentTarget, ProjectileHitFlags.None, preventFriendlyFire);
            return true;
        }

        public override void DrawHighlight(LocalTargetInfo target)
        {
            base.DrawHighlight(target);

            GenDraw.DrawFieldEdges(Utils.AffectedLineCells(ref tmpCells, caster.Position, Utils.MaxRangeIntVec3(target.Cell - caster.Position, verbProps.range), caster.MapHeld, verbProps.range, this, true));
        }
    }
}
