using HarmonyLib;
using RimWorld;
using System.Reflection;

namespace GenshinRimpact
{
    public static class PrivateFields
    {
        public static FieldInfo Ability_inCooldown = AccessTools.Field(typeof(Ability), "inCooldown");
        public static FieldInfo Ability_cooldownEndTick = AccessTools.Field(typeof(Ability), "cooldownEndTick");
        public static FieldInfo Ability_cooldownDuration = AccessTools.Field(typeof(Ability), "cooldownDuration");
    }
}
