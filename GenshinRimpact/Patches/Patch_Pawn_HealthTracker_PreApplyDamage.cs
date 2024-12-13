using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Verse;

namespace GenshinRimpact
{
    //[HarmonyPatch(typeof(Pawn_HealthTracker), "PreApplyDamage")]
    public static class Patch_Pawn_HealthTracker_PreApplyDamage
    {
        /*[HarmonyPrefix]
        public static bool Prefix(Pawn_HealthTracker __instance, ref DamageInfo dinfo)
        {
            var hediffs = __instance.hediffSet.hediffs;
            for (int i = 0; i < hediffs.Count; i++)
            {
                if (hediffs[i].TryGetComp<HediffComp_DamageResistance>() is HediffComp_DamageResistance comp)
                {
                    if (comp.Props.damageDef != null)
                    {
                        dinfo.SetAmount(dinfo.Amount * comp.Props.damageMultiplier);
                    }
                }
            }
            return true;
        }*/

        /*[HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, bool ___absorbed)
        {
            List<CodeInstruction> il = instructions.ToList();
            List<CodeInstruction> copied = [];
            bool isCopying = false;

            for (int i = 0; i < il.Count; i++)
            {
                if (isCopying) copied.Add(il[i]);

                if (il[i].opcode == OpCodes.Ldc_I4_0 &&
                    il[i+1].opcode == OpCodes.Ret &&
                    il[i+2].opcode == OpCodes.Ldarg_1 &&
                    il[i+3].opcode == OpCodes.Conv_R4)
                {
                    yield return il[i];
                    yield return il[i+1];
                    isCopying = true;
                    i += 1;
                    continue;
                }

                if (il[i].opcode == OpCodes.Starg_S &&
                    il[i-1].opcode == OpCodes.Call &&
                    il[i-2].opcode == OpCodes.Mul &&
                    il[i-3].opcode == OpCodes.Call)
                {
                    isCopying = false;
                    bool replacedLdsfld = false;
                    bool replacedCall = false;
                    yield return il[i];
                    foreach (var inst in copied)
                    {
                        if (!replacedLdsfld && inst.opcode == OpCodes.Ldsfld)
                        {
                            inst.operand = AccessTools.Field(typeof(GenshinDefOf), "GR_StaggerDurationFactor");
                            replacedLdsfld = true;
                        }
                        if (!replacedCall && inst.opcode == OpCodes.Call)
                        {
                            inst.operand = AccessTools.Method(typeof(Patch_StaggerHandler_StaggerFor_Transpiler), "GetStat");
                            replacedCall = true;
                        }
                        yield return inst;
                    }
                    continue;
                }

                yield return il[i];
            }
        }

        public static float GetStat(this Thing thing, StatDef stat)
        {
            return thing.GetStatValue(stat);
        }*/
    }
}