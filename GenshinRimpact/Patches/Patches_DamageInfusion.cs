using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Verse;

namespace GenshinRimpact
{
    public static class Patches_DamageInfusion
    {
        // Transpiler - Ranged DamageDef Replacement
        public static IEnumerable<CodeInstruction> Bullet_Impact_Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase methodBase)
        {
            List<CodeInstruction> il = instructions.ToList();
            LocalVariableInfo damageDef = methodBase.GetMethodBody().LocalVariables.First((LocalVariableInfo lv) => lv.LocalType == typeof(DamageInfo));

            for (int i = 0; i < il.Count; i++)
            {
                if (il[i].opcode == OpCodes.Ldc_I4_1 &&
                    il[i+1].opcode == OpCodes.Ldc_I4_2 &&
                    il[i+2].opcode == OpCodes.Ldc_I4_1 &&
                    il[i+3].opcode == OpCodes.Call)
                {
                    yield return il[i];
                    yield return il[i+1];
                    yield return il[i+2];
                    yield return il[i+3];
                    i += 3;
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Projectile), "launcher"));
                    yield return new CodeInstruction(OpCodes.Ldloca_S, damageDef.LocalIndex);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Patches_DamageInfusion), "TryReplaceDamageInfusion")); // this returns bool

                    continue;
                }
                yield return il[i];
            }
        }

        public static void TryReplaceDamageInfusion(Thing launcher, ref DamageInfo dinfo)
        {
            if (launcher is Pawn p)
            {
                var hediffs = p.health.hediffSet.hediffs;
                for (int i = 0; i < hediffs.Count; i++)
                {
                    if (hediffs[i].TryGetComp<HediffComp_DamageInfusion>() is HediffComp_DamageInfusion comp)
                    {
                        if (comp.Props.damageDef != null)
                        {
                            dinfo.Def = comp.Props.damageDef;
                            dinfo.SetAmount(dinfo.Amount * comp.Props.damageMultiplier);
                        }
                    }
                }
            }
        }

        // Prefix - DamageDef Replacement
        public static bool Verb_MeleeAttackDamage_DamageInfosToApply_Prefix(ref Verb __instance, ref IEnumerable<DamageInfo> __result, ref LocalTargetInfo target)
        {
            if (__instance.Caster is Pawn p)
            {
                var hediffs = p.health.hediffSet.hediffs;
                for (int i = 0; i < hediffs.Count; i++)
                {
                    if (hediffs[i].TryGetComp<HediffComp_DamageInfusion>() is HediffComp_DamageInfusion comp)
                    {
                        if (comp.Props.doMelee && comp.Props.damageDef != null)
                        {
                            float dmg = __instance.verbProps.AdjustedMeleeDamageAmount(__instance, p);
                            float armorPenetration = __instance.verbProps.AdjustedArmorPenetration(__instance, p);
                            DamageDef def = comp.Props.damageDef;
                            BodyPartGroupDef bodyPartGroupDef;
                            HediffDef hediffDef = null;
                            QualityCategory qc = QualityCategory.Normal;
                            bodyPartGroupDef = __instance.verbProps.AdjustedLinkedBodyPartsGroup(__instance.tool);

                            dmg = Rand.Range(dmg * 0.8f, dmg * 1.2f) * comp.Props.damageMultiplier;
                            if (dmg >= 1f)
                                hediffDef = __instance.HediffCompSource?.Def;
                            else dmg = 1f;

                            ThingDef source = __instance.EquipmentSource?.def ?? p.def;
                            __instance.EquipmentSource?.TryGetQuality(out qc);

                            Vector3 direction = (target.Thing.Position - p.Position).ToVector3();
                            bool instigatorGuilty = !p.Drafted;
                            DamageInfo damageInfo = new(def ?? DamageDefOf.Blunt, dmg, armorPenetration, -1f, p, null, source, DamageInfo.SourceCategory.ThingOrUnknown, null, instigatorGuilty);
                            damageInfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
                            damageInfo.SetWeaponBodyPartGroup(bodyPartGroupDef);
                            damageInfo.SetWeaponHediff(hediffDef);
                            damageInfo.SetAngle(direction);
                            damageInfo.SetTool(__instance.tool);
                            damageInfo.SetWeaponQuality(qc);
                            __result = Enumerable.Empty<DamageInfo>().Concat(damageInfo);
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        // Postfix - Additional ExtraDamage
        public static void Verb_MeleeAttackDamage_DamageInfosToApply_Postfix(ref Verb __instance, ref IEnumerable<DamageInfo> __result, ref LocalTargetInfo target)
        {
            if (__instance.Caster is Pawn p)
            {
                var hediffs = p.health.hediffSet.hediffs;
                for (int i = 0; i < hediffs.Count; i++)
                {
                    if (hediffs[i].TryGetComp<HediffComp_DamageInfusion>() is HediffComp_DamageInfusion comp)
                    {
                        if (comp.Props.doMelee && comp.Props.isExtra)
                        {
                            ExtraDamage extra = new()
                            {
                                def = comp.Props.damageDef,
                                amount = comp.Props.extraDamageAmount
                            };
                            float dmg = GenMath.RoundRandom(extra.AdjustedDamageAmount(__instance, p)) * comp.Props.damageMultiplier;
                            float armorPenetration2 = extra.AdjustedArmorPenetration(__instance, p);
                            Vector3 direction = (target.Thing.Position - p.Position).ToVector3();

                            ThingDef source;
                            source = __instance.EquipmentSource?.def ?? p.def;

                            DamageInfo damageInfo2 = new(extra.def, dmg, armorPenetration2, -1f, p, null, source);
                            damageInfo2.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
                            damageInfo2.SetWeaponBodyPartGroup(__instance.verbProps.AdjustedLinkedBodyPartsGroup(__instance.tool));
                            damageInfo2.SetAngle(direction);
                            __result = Enumerable.Empty<DamageInfo>().Concat(damageInfo2);
                        }
                    }
                }
            }
        }
    }
}
