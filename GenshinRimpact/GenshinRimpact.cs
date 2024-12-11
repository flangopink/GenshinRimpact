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

            harmony.Patch(AccessTools.Method(typeof(Pawn), "SpawnSetup"), null, new HarmonyMethod(typeof(Patches_HediffShieldManager), "OnPawnSpawn"));
            harmony.Patch(AccessTools.Method(typeof(Pawn), "DeSpawn"), null, new HarmonyMethod(typeof(Patches_HediffShieldManager), "OnPawnDespawn"));
            harmony.Patch(AccessTools.Method(typeof(Pawn), "DrawAt"), null, new HarmonyMethod(typeof(Patches_HediffShieldManager), "PawnPostDrawAt"));
            harmony.Patch(AccessTools.Method(typeof(ThingWithComps), "PreApplyDamage"), null, new HarmonyMethod(typeof(Patches_HediffShieldManager), "PostPreApplyDamage"));
            //harmony.Patch(AccessTools.Method(typeof(Verb), "CanHitTarget"), null, "CanHitTargetFrom_Postfix".MyMethod());

            harmony.PatchAll();

            Utils.LogMessage("Patched " + harmony.GetPatchedMethods().Count() + " patches!");
            Utils.LogMessage("♥ Thank you for using GenshinRimpact v" + Assembly.GetExecutingAssembly().GetName().Version + "! ♥");
        }
    }
}
