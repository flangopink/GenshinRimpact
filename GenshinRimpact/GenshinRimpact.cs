using RimWorld;
using Verse;
using HarmonyLib;
using System.Linq;
using System.Reflection;

namespace GenshinRimpact
{
    [StaticConstructorOnStartup]
    public static class GenshinRimpact_Harmony
    {
        static GenshinRimpact_Harmony()
        {
            Harmony harmony = new("flangopink.GenshinRimpact");
            harmony.PatchAll();
            Utils.LogMessage("Patched " + harmony.GetPatchedMethods().Count() + " patches!");
            //harmony.Patch(AccessTools.PropertyGetter(typeof(Pawn_AbilityTracker), "AllAbilitiesForReading"), transpiler: new HarmonyMethod(typeof(Patch_AllAbilitiesForReading), "Transpiler"));

            Utils.LogMessage("♥ Thank you for using GenshinRimpact v" + Assembly.GetExecutingAssembly().GetName().Version + "! ♥");
        }
    }
}
