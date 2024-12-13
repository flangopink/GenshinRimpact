using UnityEngine;
using Verse;

namespace GenshinRimpact
{
    public class ModExt_Bezier : DefModExtension
    {
        // v0 is caster position
        public Vector2 v1 = Vector2.zero; 
        public Vector2 v2 = Vector2.zero;
        // v3 is target position
        // t - time fraction, 0<=t<=1

        public bool v3isv0 = false;
        public float flipChance;

        public FloatRange v2DeviationRange;
    }
}
