using RimWorld;
using Verse;
using HarmonyLib;
using System.Linq;
using System.Reflection;

namespace Rimpact
{
    [StaticConstructorOnStartup]
    public static class Rimpact_Harmony
    {
        static Rimpact_Harmony()
        {
            Harmony harmony = new("flangopink.Rimpact");

            // Shield Hediff
            harmony.Patch(AccessTools.Method(typeof(Pawn), "SpawnSetup"), null, new HarmonyMethod(typeof(Patches_HediffShieldManager), "OnPawnSpawn"));
            harmony.Patch(AccessTools.Method(typeof(Pawn), "DeSpawn"), null, new HarmonyMethod(typeof(Patches_HediffShieldManager), "OnPawnDespawn"));
            harmony.Patch(AccessTools.Method(typeof(Pawn), "DrawAt"), null, new HarmonyMethod(typeof(Patches_HediffShieldManager), "PawnPostDrawAt"));
            harmony.Patch(AccessTools.Method(typeof(ThingWithComps), "PreApplyDamage"), null, new HarmonyMethod(typeof(Patches_HediffShieldManager), "PostPreApplyDamage"));
            
            // Damage Infusion
            harmony.Patch(AccessTools.Method(typeof(Bullet), "Impact"), transpiler: new HarmonyMethod(typeof(Patches_DamageInfusion), "Bullet_Impact_Transpiler"));
            harmony.Patch(AccessTools.Method(typeof(Verb_MeleeAttackDamage), "DamageInfosToApply"), prefix: new HarmonyMethod(typeof(Patches_DamageInfusion), "Verb_MeleeAttackDamage_DamageInfosToApply_Prefix"));
            harmony.Patch(AccessTools.Method(typeof(Verb_MeleeAttackDamage), "DamageInfosToApply"), postfix: new HarmonyMethod(typeof(Patches_DamageInfusion), "Verb_MeleeAttackDamage_DamageInfosToApply_Postfix"));

            harmony.PatchAll();

            Utils.LogMessage("Loaded " + Utils.AllReactionsForReading.Count + " reactions, " 
                                       + Utils.AllVisionsForReading.Count + " visions, "
                                       + harmony.GetPatchedMethods().Count() + " patches!");
            Utils.LogMessage("Assembly version: v" + Assembly.GetExecutingAssembly().GetName().Version);
            Utils.LogMessage("♥ Thank you for using Rimpact! ♥");
        }
    }
}
