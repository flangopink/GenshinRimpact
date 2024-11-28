using RimWorld;
using System.Collections.Generic;
using System;
using Verse;
using System.Linq;

namespace GenshinRimpact
{
    public class CompProperties_AbilityKnockback : CompProperties_AbilityEffect
    {
        public float distance = 3f;
        public bool pull = false;

        public CompProperties_AbilityKnockback() => compClass = typeof(CompAbilityKnockback);
    }

    [HotSwap.HotSwappable]
    public class CompAbilityKnockback : CompAbilityEffect
    {
        public new CompProperties_AbilityKnockback Props => (CompProperties_AbilityKnockback)props;

        private Pawn Caster => parent.pawn;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);

            var thing = target.Thing;

            float distanceDiff = ((Caster.Position.DistanceTo(thing.Position) < Props.distance) ? Caster.Position.DistanceTo(thing.Position) : Props.distance);
            bool Validator(IntVec3 x)
            {
                if (x.DistanceTo(thing.Position) < Props.distance)
                {
                    return false;
                }
                if (!x.Walkable(thing.Map) || !GenSight.LineOfSight(thing.Position, x, thing.Map))
                {
                    return false;
                }
                float distCasterToThing = Caster.Position.DistanceTo(thing.Position);
                float distCasterToCell = Caster.Position.DistanceTo(x);
                float distThingToCell = thing.Position.DistanceTo(x);
                if (distCasterToThing > distCasterToCell)
                {
                    return false;
                }
                if (distCasterToCell > distThingToCell + (distanceDiff - 1f))
                {
                    return true;
                }
                return (Caster.Position == thing.Position);
            }
            IEnumerable<IntVec3> source = from x in GenRadial.RadialCellsAround(thing.Position, Props.distance + 1f, useCenter: true)
                                          where Validator(x)
                                          select x;
            if (source.Any())
            {
                IntVec3 position = source.RandomElement();
                thing.Position = position;
                if (thing is Pawn pawn)
                {
                    pawn.pather.StopDead();
                    pawn.jobs.StopAll();
                }
                /*if (extension.fleckOnDamage != null)
                {
                    Thing thing2 = (extension.fleckOnInstigator ? attacker : thing);
                    FleckMaker.Static(thing2.Position, thing2.Map, extension.fleckOnDamage, extension.fleckRadius);
                }*/
            }
        }
    }
}
