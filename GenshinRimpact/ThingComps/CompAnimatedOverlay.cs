using UnityEngine;
using Verse;

namespace GenshinRimpact
{
    public class CompProperties_AnimatedOverlay : CompProperties
    {
        public bool isRotating;
        public bool isSquishy;
        public float rotationRange = 10f;
        public float scaleRange = 1f;
        public float scaleOffset = 1f;
        public float speed = 1;
        public GraphicData graphicData;
        //public int updateTicks = 60;

        public CompProperties_AnimatedOverlay()
        {
            compClass = typeof(CompAnimatedOverlay);
        }
    }

    public class CompAnimatedOverlay : ThingComp
    {
        public CompProperties_AnimatedOverlay Props => (CompProperties_AnimatedOverlay)props;

        private Matrix4x4 Matrix
        {
            get
            {
                Vector3 drawPos = parent.DrawPos + Props.graphicData.drawOffset;
                Quaternion rot;
                Vector3 scale;

                float sin = Mathf.Sin(Find.TickManager.TicksGame * Props.speed);
                float cos = Mathf.Cos(Find.TickManager.TicksGame * Props.speed);

                rot = Props.isRotating ? Quaternion.AngleAxis(cos * Props.rotationRange, Vector3.up) : Quaternion.identity;
                scale = Props.isSquishy ? new Vector3(Props.scaleOffset + Mathf.Abs(sin) * Props.scaleRange, 1f, Props.scaleOffset + Mathf.Abs(cos) * Props.scaleRange) : new Vector3(parent.DrawSize.x, 1f, parent.DrawSize.y);

                drawPos.y = AltitudeLayer.BuildingOnTop.AltitudeFor();

                //Log.Message("pos: " + drawPos + ", " + "rot: " + rot + ", " + "scale: " + scale);

                return Matrix4x4.TRS(drawPos, rot, scale);
            }
        }

        public override void PostDraw()
        {
            base.PostDraw();

            Mesh propMesh = Props.graphicData.Graphic.MeshAt(parent.Rotation);
            Material propsMaterial = Props.graphicData.Graphic.MatAt(parent.Rotation);

            //Log.Message(propMesh.ToString() + ", " + propsMaterial.ToString());

            Graphics.DrawMesh(propMesh, Matrix, propsMaterial, 0);
        }
    }
}
