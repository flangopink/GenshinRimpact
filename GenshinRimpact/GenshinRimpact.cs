using RimWorld;
using Verse;
using HarmonyLib;

namespace GenshinRimpact
{
    [StaticConstructorOnStartup]
    public static class GenshinRimpact_Harmony
    {
        static GenshinRimpact_Harmony()
        {
            Harmony harmony = new("flangopink.GenshinRimpact");
            harmony.PatchAll();
            //harmony.Patch(AccessTools.PropertyGetter(typeof(Pawn_AbilityTracker), "AllAbilitiesForReading"), transpiler: new HarmonyMethod(typeof(Patch_AllAbilitiesForReading), "Transpiler"));
        }
    }
}
