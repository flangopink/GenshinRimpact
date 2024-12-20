using UnityEngine;

namespace GenshinRimpact
{
    public class AoEShapeParameters
    {
        public AoEShape shape = AoEShape.Radial;
        public float radius = 3.9f;
        public float angleRad = Mathf.PI;
        public float coneAngleDeg = 60f;
        public int coneWidth = 5;
    }
}
