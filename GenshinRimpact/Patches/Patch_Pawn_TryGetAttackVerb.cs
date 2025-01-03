using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.Jobs;
using Verse;
using Verse.AI;

namespace Rimpact
{
    /*[HarmonyPatch(typeof(JobGiver_AIDefendPoint), "TryFindShootingPosition")]
    [HotSwap.HotSwappable]
    public static class Patch_JobGiver_AIDefendPoint_TryFindShootingPosition
    {
        [HarmonyPrefix]
        public static bool Prefix(JobGiver_AIFightEnemy __instance, ref bool __result, ref IntVec3 dest, Pawn pawn)
        {
            dest = IntVec3.Invalid;
            Thing enemyTarget = pawn.mindState.enemyTarget;
            var abs = pawn.abilities.AICastableAbilities(pawn.mindState.enemyTarget, true);
            if (!abs.NullOrEmpty())
            {
                Ability ability = abs.Where(x=>!x.OnCooldown).RandomElement();
                CastPositionRequest newReq = default;
                newReq.caster = pawn;
                newReq.target = enemyTarget;
                newReq.verb = ability.verb;
                newReq.maxRangeFromTarget = ability.verb.verbProps.range;
                newReq.wantCoverFromTarget = false;
                newReq.preferredCastPosition = pawn.Position;
                __result = CastPositionFinder.TryFindCastPosition(newReq, out dest);
                return false; 
            }
            return true;
        }
    }*/

    // ability.StartCooldown(ability.def.cooldownTicksRange.RandomInRange);

    [HarmonyBefore(["legodude17.mvcf"])]
    [HarmonyPatch(typeof(JobDriver_CastAbility), "MakeNewToils")]
    [HotSwap.HotSwappable]
    public static class Patch_JobDriver_CastAbility_MakeNewToils
    {
        [HarmonyPrefix]
        public static bool Prefix(JobDriver_CastAbility __instance, ref IEnumerable<Toil> __result)
        {
            List<Toil> toils = [];
            var job = __instance.job;
            var ab = job.ability;
            __instance.FailOnDespawnedOrNull(TargetIndex.A);
            __instance.FailOn(() => !ab.CanCast && !ab.Casting);
            __instance.AddFinishAction(delegate
            {
                if (job.ability != null && job.def.abilityCasting)
                {
                    ab.StartCooldown(ab.def.cooldownTicksRange.RandomInRange);
                }
            });
            Toil toil = ToilMaker.MakeToil("MakeNewToils");
            toil.initAction = __instance.pawn.pather.StopDead;
            toil.defaultCompleteMode = ToilCompleteMode.Instant;
            toils.Add(toil);
            Toil toil2 = Toils_Combat.CastVerb(TargetIndex.A, TargetIndex.B, canHitNonTargetPawns: false);
            if (job.ability != null && job.ability.def.showCastingProgressBar && job.verbToUse != null)
            {
                toil2.WithProgressBar(TargetIndex.A, () => job.verbToUse.WarmupProgress);
            }
            toils.Add(toil2);
            __result = toils;
            return false;
        }
    }

    [HarmonyPatch(typeof(Pawn), "TryGetAttackVerb")]
    [HotSwap.HotSwappable]
    public static class Patch_Pawn_TryGetAttackVerb
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn __instance, ref Verb __result, Thing target)
        {
            var abs = __instance.abilities;
            if (abs == null || abs.abilities.NullOrEmpty()) return;
            List<Ability> list = abs.AICastableAbilities(target, true);
            if (list.NullOrEmpty()) return;
            if (__instance.Position.Standable(__instance.Map) && __instance.Map.pawnDestinationReservationManager.CanReserve(__instance.Position, __instance, __instance.Drafted))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].verb.CanHitTarget(target))
                    {
                        __result = list[i].verb;
                        return;
                    }
                }
                for (int j = 0; j < list.Count; j++)
                {
                    LocalTargetInfo localTargetInfo = list[j].AIGetAOETarget();
                    if (localTargetInfo.IsValid)
                    {
                        __result = list[j].verb;
                        return;
                    }
                }
                for (int k = 0; k < list.Count; k++)
                {
                    if (list[k].verb.targetParams.canTargetSelf)
                    {
                        __result = list[k].verb;
                        return;
                    }
                }
            }
        }
    }
}
