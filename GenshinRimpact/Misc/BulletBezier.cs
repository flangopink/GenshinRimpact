using Verse;
using RimWorld;
using UnityEngine;

namespace GenshinRimpact
{
    public class BulletBezier : Bullet
    {
        ModExt_Bezier Ext => def.GetModExtension<ModExt_Bezier>();

        private bool flipped;
        private float XFlip => flipped ? -1f : 1f;
        private float RandomV2Deviation => Rand.Range(Ext.v2_minDeviation, Ext.v2_maxDeviation);
        public override Quaternion ExactRotation => Quaternion.LookRotation((nextPos - ExactPosition).Yto0());

        private Vector3 v1offset;
        private Vector3 v2offset;
        private Vector3 nextPos;
        private Vector3 endVector;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            endVector = Ext.v3isv0 ? origin : destination;
            nextPos = destination;
            flipped = Rand.Chance(Ext.flipChance);
            var v2x = Ext.v2.x + RandomV2Deviation;
            var v2z = Ext.v2.y + RandomV2Deviation;
            v1offset = new(XFlip * Ext.v1.x, 0f, Ext.v1.y);
            v2offset = new(XFlip * v2x, 0f, v2z);
        }

        protected override void DrawAt(Vector3 drawLoc, bool flip = false) // 0.75 for each 1 in x, when v0 and v3 = (0,0), v1 and v2 = (x,+-y)
        {
            Vector3 dir = destination - origin;
            Quaternion qdir = Quaternion.LookRotation(dir.Yto0());
            Vector3 v1 = origin + qdir * v1offset;
            Vector3 v2 = origin + qdir * v2offset;
            Vector3 bezier = GenMath.BezierCubicEvaluate(DistanceCoveredFraction, origin, v1, v2, endVector);
            nextPos = bezier;
            base.DrawAt(bezier, flip);
        }
    }
}
