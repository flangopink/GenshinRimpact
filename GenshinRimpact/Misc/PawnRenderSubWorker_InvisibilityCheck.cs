using System.Collections.Generic;
using Verse;

namespace GenshinRimpact
{
    public class PawnRenderSubWorker_InvisibilityCheck : PawnRenderSubWorker
    {
        public override bool CanDrawNowSub(PawnRenderNode node, PawnDrawParms parms)
        {
            bool hasInvisibility = false;
            List<Hediff> hediffs = parms.pawn.health.hediffSet.hediffs;
            for (int i = 0; i < hediffs.Count; i++)
            {
                if (hediffs[i].def.HasModExtension<ModExt_TurnInvisible>())
                    hasInvisibility = true;
            }
            return !hasInvisibility;
        }
    }
}
