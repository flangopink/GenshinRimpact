using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace GenshinRimpact
{
    [HarmonyPatch(typeof(EquipmentUtility), "CanEquip",
            [typeof(Thing), typeof(Pawn), typeof(string), typeof(bool)],
            [ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out, ArgumentType.Normal])]
    public class Patch_EquipmentUtility_CanEquip
    {
        //private Dictionary<Thing, CompProperties> tmpDict = [];

        [HarmonyPostfix]
        public static void Postfix(Thing thing, Pawn pawn, ref string cantReason, ref bool __result)
        {
            if (__result)
            {
                if (!thing.HasComp<CompVisionEquippableAbilities>()) return;
                var allEq = pawn.equipment.AllEquipmentListForReading;
                for (int i = 0; i < allEq.Count; i++)
                {
                    if (allEq[i].HasComp<CompVisionEquippableAbilities>())
                    {
                        __result = false;
                        cantReason = "GR_AnotherVisionIsCurrentlyEquipped".Translate();
                        return;
                    }
                }
            }
        }
    }
}
