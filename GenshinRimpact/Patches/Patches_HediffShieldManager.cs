using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace GenshinRimpact
{
    public static class Patches_HediffShieldManager
    {
        public static Dictionary<Pawn, List<HediffComp_Draw>> HediffDrawsByPawn = [];

        private static List<HediffComp_Draw> GetDrawComps(Pawn p)
        {
            List<HediffComp_Draw> list = [];
            var hediffs = p.health.hediffSet.hediffs;
            for (int i = 0; i < hediffs.Count; i++)
            {
                if (hediffs[i].TryGetComp<HediffComp_Draw>() is HediffComp_Draw drawComp)
                    list.Add(drawComp);
            }
            return list;
        }
        private static List<HediffComp_Shield> GetShieldComps(Pawn p)
        {
            List<HediffComp_Shield> list = [];
            var hediffs = p.health.hediffSet.hediffs;
            for (int i = 0; i < hediffs.Count; i++)
            {
                if (hediffs[i].TryGetComp<HediffComp_Shield>() is HediffComp_Shield drawComp)
                    list.Add(drawComp);
            }
            return list;
        }

        public static void OnPawnSpawn(Pawn __instance)
        {
            HediffDrawsByPawn.Add(__instance, GetDrawComps(__instance));
        }

        public static void OnPawnDespawn(Pawn __instance)
        {
            HediffDrawsByPawn.Remove(__instance);
        }

        public static void PawnPostDrawAt(Pawn __instance, Vector3 drawLoc)
        {
            if (HediffDrawsByPawn.TryGetValue(__instance, out var value))
            {
                for (int i = 0; i < value.Count; i++)
                {
                    value[i].DrawAt(drawLoc);
                }
            }
        }

        public static void PostPreApplyDamage(ThingWithComps __instance, ref DamageInfo dinfo, ref bool absorbed)
        {
            if (absorbed || __instance is not Pawn pawn)
            {
                return;
            }
            foreach (HediffComp_Shield item in GetShieldComps(pawn))
            {
                item.PreApplyDamage(ref dinfo, ref absorbed);
                if (absorbed)
                {
                    break;
                }
            }
        }

        /*public static void CanHitTargetFrom_Postfix(Verb __instance, ref bool __result)
        {
            if (__result && __instance.CasterIsPawn)
            {
                Pawn casterPawn = __instance.CasterPawn;
                if (casterPawn != null && casterPawn.health.hediffSet.hediffs.OfType<HediffWithComps>().SelectMany((HediffWithComps hediff) => hediff.comps).OfType<HediffComp_Shield>()
                    .Any((HediffComp_Shield shield) => !shield.AllowVerbCast(__instance)))
                {
                    __result = false;
                }
            }
        }*/
    }
}
