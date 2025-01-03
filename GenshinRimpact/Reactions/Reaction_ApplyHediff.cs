using System.Collections.Generic;
using Verse;

namespace Rimpact
{
    public class Reaction_ApplyHediff : ElementalReaction
    {
        public override void ApplyReaction(Thing thing, Thing instigator = null)
        {
            bool affectsTarget = Def.targetHediffDef != null;
            bool affectsCaster = Def.casterHediffDef != null;

            if (Def.effectRadius > 0)
            {
                int cellNum = GenRadial.NumCellsInRadius(Def.effectRadius);
                for (int i = 0; i < cellNum; i++)
                {
                    if (affectsTarget) 
                    {
                        var cell = thing.Position + GenRadial.RadialPattern[i];
                        var pawns = GetPawnsAt(cell, thing.Map); 
                        for (int j = 0; j < pawns.Count; j++)
                        {
                            ApplyHediff(pawns[i], Def.targetHediffDef);
                        }
                    }
                    if (affectsCaster) 
                    {
                        var cell = instigator.Position + GenRadial.RadialPattern[i];
                        var pawns = GetPawnsAt(cell, thing.Map); 
                        for (int j = 0; j < pawns.Count; j++)
                        {
                            ApplyHediff(pawns[i], Def.casterHediffDef);
                        }
                    }
                }
            }
            if (affectsTarget) ApplyHediff(thing, Def.targetHediffDef);
            if (affectsCaster) ApplyHediff(instigator, Def.casterHediffDef);
        }

        private void ApplyHediff(Thing thing, HediffDef hediffDef)
        {
            if (thing is Pawn pawn && !pawn.Dead)
            {
                pawn.health?.AddHediff(HediffMaker.MakeHediff(hediffDef, pawn));
            }
        }

        private List<Pawn> GetPawnsAt(IntVec3 cell, Map map)
        {
            if (!cell.InBounds(map)) return null;
            List<Pawn> pawnList = [];
            List<Thing> thingList = cell.GetThingList(map);
            for (int i = 0; i < thingList.Count; i++)
            {
                if (thingList[i] is Pawn pawn) pawnList.Add(pawn);
            }
            return pawnList;
        }
    }
}
