using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace Rimpact
{
    [HarmonyPatch(typeof(Pawn_EquipmentTracker), "GetGizmos")]
    [HotSwap.HotSwappable]
    public static class Patch_Pawn_EquipmentTracker_GetGizmos
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn_EquipmentTracker __instance, ref IEnumerable<Gizmo> __result)
        {
            for (int i = 0; i < __instance.AllEquipmentListForReading.Count; i++)
            { 
                if (__instance.AllEquipmentListForReading[i].GetComp<CompVisionEquippableAbilities>() is CompVisionEquippableAbilities comp)
                {
                    foreach (Gizmo g in comp.CompGetEquippedGizmosExtra())
                    {
                        __result = __result.AddItem(g);
                    }
                }
            }
        }
    }
}
