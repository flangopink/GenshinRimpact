using Verse;
using RimWorld;
using UnityEngine;

namespace GenshinRimpact
{
    [HotSwap.HotSwappable]
    public class BulletBezier : Bullet
    {
        ModExt_Bezier Ext => def.GetModExtension<ModExt_Bezier>();
        private bool initialized;
        private Vector3 nextPos;
        private Vector3 endVector;
        private Vector3 v1;
        private Vector3 v2;

        public override Quaternion ExactRotation => Quaternion.LookRotation((ExactPosition - nextPos).Yto0());

        public override void Tick()
        {
            base.Tick();
            if (!initialized)
            {
                endVector = Ext.v3isv0 ? origin : destination;
                nextPos = destination;
                float flipMult = Rand.Chance(Ext.flipChance) ? -1f : 1f;
                Vector3 v1offset = new(Ext.v1.x, 0f, Ext.v1.y);
                Vector3 v2offset = new(Ext.v2.x + Ext.v2DeviationRange.RandomInRange, 0f, Ext.v2.y + Ext.v2DeviationRange.RandomInRange);
                Quaternion qdir = Quaternion.LookRotation((destination - origin).Yto0());
                v1 = origin + qdir * v1offset * flipMult;
                v2 = origin + qdir * v2offset * flipMult;
                initialized = true;
            }
        }

        protected override void DrawAt(Vector3 drawLoc, bool flip = false) // 0.75 for each 1 in x, when v0 and v3 = (0,0), v1 and v2 = (a,+-b)
        {
            Vector3 bezier = GenMath.BezierCubicEvaluate(DistanceCoveredFraction, origin, v1, v2, endVector);
            nextPos = bezier;
            base.DrawAt(bezier, flip);
        }
    }
}
