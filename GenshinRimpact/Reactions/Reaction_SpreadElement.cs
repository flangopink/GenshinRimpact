using System.Collections.Generic;
using Verse;

namespace Rimpact
{
    public class Reaction_SpreadElement : ElementalReaction
    {
        public override void ApplyReaction(Thing thing, Thing instigator = null)
        {
            if (thing.Spawned)
            {
                var comp = thing.TryGetComp<CompElementalHandler>();
                var otherElement = comp?.currentElement;
                List<Thing> things = Utils.GetThingsInRange(thing.PositionHeld, thing.MapHeld, Def.effectRadius, Def.requireLOS, Def.affectedThingCategories);
                foreach (Thing t in things)
                {
                    if (t == thing || t == instigator) continue;
                    if (Def.targetHediffDef != null && t is Pawn pawn) pawn.health?.AddHediff(HediffMaker.MakeHediff(Def.targetHediffDef, pawn));
                    comp?.ApplyElement(otherElement);
                }
            }
        }
    }
}
