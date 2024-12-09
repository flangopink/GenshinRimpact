using RimWorld;
using UnityEngine;
using Verse;

namespace GenshinRimpact
{
    public class HediffCompProperties_MoteOverlay : HediffCompProperties
    {
        public ThingDef moteDef;
        public Vector3 offset = Vector3.zero;
        public bool removeHediffOnDowned;
        public bool rotateTowardsDir;

        public HediffCompProperties_MoteOverlay() => compClass = typeof(HediffComp_MoteOverlay);
    }

    [HotSwap.HotSwappable]
    public class HediffComp_MoteOverlay : HediffComp
    {
        public HediffCompProperties_MoteOverlay Props => (HediffCompProperties_MoteOverlay)props;
        private Mote mote;
        private Vector3 prevPos;
        public override bool CompShouldRemove => Props.removeHediffOnDowned && parent.pawn.Downed;

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (mote == null || mote.Destroyed)
                mote = MoteMaker.MakeAttachedOverlay(parent.pawn, Props.moteDef, Props.offset);
            mote.Maintain();
            if (Props.rotateTowardsDir) 
            {
                var pos = parent.pawn.Position.ToVector3();
                if (prevPos != pos)
                {
                    mote.exactRotation = (pos - prevPos).ToAngleFlat();
                }
                prevPos = pos;
            }
        }

        public override void CompPostPostRemoved() => mote?.Destroy();

        public override void CompExposeData()
        {
            Scribe_Values.Look(ref prevPos, "prevPos");
        }
    }
}
