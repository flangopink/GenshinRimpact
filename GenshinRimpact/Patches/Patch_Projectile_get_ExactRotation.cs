using HarmonyLib;
using UnityEngine;
using Verse;

namespace GenshinRimpact
{
    [HarmonyPatch(typeof(Projectile), "get_ExactRotation")]
    public static class Patch_Projectile_get_ExactRotation
    {
        [HarmonyPrefix]
        public static bool ExactRotation_Prefix(Projectile __instance, ref Quaternion __result)
        {
            if (__instance.def.HasComp(typeof(CompProjectileVFX)))
            {
                CompProperties_ProjectileVFX props = __instance.TryGetComp<CompProjectileVFX>().Props;

                if (props.rotating)
                {
                    if (props.stopRotatingOnImpact && __instance.landed) return false;

                    __result = Quaternion.AngleAxis((props.counterClockwise ? -1 : 1) * (__instance.HashOffsetTicks() % 360) * props.rotationSpeed, Vector3.up);
                    return false;
                }
            }
            return true;
        }
    }
}
