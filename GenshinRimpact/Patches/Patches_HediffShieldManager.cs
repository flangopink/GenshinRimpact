using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Rimpact
{
    public static class Patches_HediffShieldManager
    {
        public static Dictionary<Pawn, List<HediffComp_Draw>> HediffDrawsByPawn = [];

        public static void OnPawnSpawn(Pawn __instance)
        {
            HediffDrawsByPawn.Add(__instance, Utils.GetHediffCompsOfType<HediffComp_Draw>(__instance));
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
            foreach (HediffComp_DamageMultiplier item in Utils.GetHediffCompsOfType<HediffComp_DamageMultiplier>(pawn))
            {
                foreach (var mult in item.Props.damageMultipliers)
                {
                    if (mult.damageDef == dinfo.Def)
                        dinfo.SetAmount(dinfo.Amount * mult.multiplier);
                }
            }
            foreach (HediffComp_Shield item in Utils.GetHediffCompsOfType<HediffComp_Shield>(pawn))
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
