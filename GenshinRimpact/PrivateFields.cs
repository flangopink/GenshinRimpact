using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;

namespace Rimpact
{
    public static class PrivateFields
    {
        //public static FieldInfo Ability_inCooldown = AccessTools.Field(typeof(Ability), "inCooldown");
        //public static FieldInfo Ability_cooldownEndTick = AccessTools.Field(typeof(Ability), "cooldownEndTick");
        //public static FieldInfo Ability_cooldownDuration = AccessTools.Field(typeof(Ability), "cooldownDuration");
        public static FieldInfo ModContentPack_defs = AccessTools.Field(typeof(ModContentPack), "defs");
        public static FieldInfo HediffDefDatabase_defs = AccessTools.Field(typeof(DefDatabase<HediffDef>), "defsList");
    }
}
