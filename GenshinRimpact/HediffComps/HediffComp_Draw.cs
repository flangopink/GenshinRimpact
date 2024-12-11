using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace GenshinRimpact
{
    public class HediffCompProperties_Draw : HediffCompProperties
    {
        public GraphicData graphic;
        public HediffCompProperties_Draw() => compClass = typeof(HediffComp_Draw);
    }

    public class HediffComp_Draw : HediffComp
    {
        public virtual Graphic Graphic => (props as HediffCompProperties_Draw).graphic?.Graphic;

        public virtual void DrawAt(Vector3 drawPos)
        {
            Graphic?.Draw(drawPos, Pawn.Rotation, Pawn);
        }

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            if (Patches_HediffShieldManager.HediffDrawsByPawn.TryGetValue(Pawn, out var value))
            {
                value.Add(this);
            }
        }

        public override void CompPostPostRemoved()
        {
            if (Patches_HediffShieldManager.HediffDrawsByPawn.TryGetValue(Pawn, out var value))
            {
                value.Remove(this);
            }
        }
    }
}
