using HarmonyLib;
using RimWorld;
using System.Security.Cryptography;
using Verse;
using static RimWorld.PsychicRitualRoleDef;

namespace Rimpact
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
                var comp = thing.TryGetComp<CompVisionEquippableAbilities>();
                if (comp == null) return;

                if (comp.Props.visionDef.mustBeCapableOfViolence && pawn.WorkTagIsDisabled(WorkTags.Violent))
                {
                    __result = false;
                    cantReason = "IsIncapableOfViolence".Translate(pawn.LabelShort, pawn);
                    return;
                }

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
