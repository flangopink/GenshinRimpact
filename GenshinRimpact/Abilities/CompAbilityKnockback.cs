using RimWorld;
using System.Collections.Generic;
using Verse;
using System.Linq;
using UnityEngine;

namespace GenshinRimpact
{
    public class CompProperties_AbilityKnockback : CompProperties_AbilityEffect
    {
        /*public float knockbackDistance = 3f;
        public float knockbackRadius = 0.2f;
        public bool doFlyer = false;
        public bool pull = false;
        public ThingDef flyerDef;
        public EffecterDef flyerEffecter;

        public CompProperties_AbilityKnockback() => compClass = typeof(CompAbilityKnockback);*/
    }

    //[HotSwap.HotSwappable]
    public class CompAbilityKnockback : CompAbilityEffect
    {
        /*public new CompProperties_AbilityKnockback Props => (CompProperties_AbilityKnockback)props;
        private Pawn Caster => parent.pawn;
        //public List<IntVec3> tmpCells = [];
        public List<IntVec3> tmpKnockbackCells = [];

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);

            //var affectedThings = parent.CompOfType<CompAbilityAoE>().tmpCells ? target.Thing;

            if (target.Thing is not Pawn p) return;

            var cells = GetKnockbackCells(p, Props.pull);
            if (cells.Any())
            {
                IntVec3 targetPos = cells.RandomElement();

                if (Props.doFlyer)
                {
                    bool isSelected = Find.Selector.IsSelected(p);
                    Map map = p.Map;

                    PawnFlyer pawnFlyer = PawnFlyer.MakeFlyer(Props.flyerDef ?? ThingDefOf.PawnFlyer, p, targetPos, Props.flyerEffecter, null, false, null, null, target);
                    if (pawnFlyer != null)
                    {
                        FleckMaker.ThrowDustPuff(targetPos.ToVector3Shifted() + Gen.RandomHorizontalVector(0.5f), map, 2f);
                        GenSpawn.Spawn(pawnFlyer, targetPos, map);
                        if (isSelected)
                        {
                            Find.Selector.Select(p, playSound: false, forceDesignatorDeselect: false);
                        }
                    }
                }
                else
                {
                    p.Position = targetPos;
                    p.pather.StopDead();
                    p.jobs.StopAll();
                }
            }
        }

        public override void DrawEffectPreview(LocalTargetInfo target)
        {
            if (target.Thing is Pawn p)
                GenDraw.DrawFieldEdges(GetKnockbackCells(p, Props.pull), Color.cyan);
        }

        private List<IntVec3> GetKnockbackCells(Thing thing, bool isPull)
        {
            tmpKnockbackCells.Clear();
            if (Caster.Position == thing.Position) return tmpKnockbackCells;

            IntVec3 centerCell = IntVec3.Invalid;
            Map map = thing.Map;

            var direction = (isPull ? (Caster.Position - thing.Position) : (thing.Position - Caster.Position)).ToVector3().normalized;
            IntVec3 newTargetPos = thing.Position + (direction * Props.knockbackDistance).ToIntVec3();

            if (newTargetPos.InBounds(map) && newTargetPos.Walkable(map) && GenSight.LineOfSight(thing.Position, newTargetPos, map))
            {
                centerCell = newTargetPos;
            }
            else
            {
                // LOS-check
                for (float i = Props.knockbackDistance; i > 0; i--)
                {
                    IntVec3 potentialTargetPos = thing.Position + (direction * i).ToIntVec3();
                    if (potentialTargetPos.InBounds(map) && potentialTargetPos.Walkable(map) && GenSight.LineOfSight(thing.Position, potentialTargetPos, map))
                    {
                        centerCell = potentialTargetPos;
                        break;
                    }
                }

            }
            if (centerCell.IsValid)
            {
                var cells = GenRadial.RadialCellsAround(newTargetPos, Props.knockbackRadius, true);
                foreach (var c in cells)
                {
                    if (c.InBounds(map) && c.Walkable(map) && GenSight.LineOfSight(thing.Position, c, map))
                        tmpKnockbackCells.Add(c);
                }
            }
            else
            {
                centerCell = thing.Position;
                tmpKnockbackCells.Add(centerCell);
            }
            return tmpKnockbackCells;
        }*/
    }
}
