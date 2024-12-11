using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;
using Verse.AI;

namespace GenshinRimpact
{

    [HotSwap.HotSwappable]
    [StaticConstructorOnStartup]
    public static class Utils
    {
        public static readonly Texture2D ClearBarTexture = BaseContent.ClearTex;
        public static readonly Texture2D DividerTex = ContentFinder<Texture2D>.Get("UI/Misc/NeedUnitDivider");

        public static readonly Dictionary<ElementDef, Texture2D> ElementalFillBars = [];

        public static readonly Dictionary<ReactionData, Type> AllReactionsForReading = [];
        public static readonly Dictionary<ThingDef, VisionDef> AllVisionsForReading = [];

        public static List<IntVec3> tmpCells = [];

        /*public static Color GetElementColor(Element element)
        {
            return element switch
            {
                Element.Pyro => ElementColors.Pyro,
                Element.Hydro => ElementColors.Hydro,
                Element.Anemo => ElementColors.Anemo,
                Element.Electro => ElementColors.Electro,
                Element.Dendro => ElementColors.Dendro,
                Element.Cryo => ElementColors.Cryo,
                Element.Geo => ElementColors.Geo,
                _ => Color.white,
            };
        }*/

        static Utils()
        {
            LogMessage("Constructor started!");

            foreach (var element in DefDatabase<ElementDef>.AllDefsListForReading)
            {
                //Log.Message("element - " + element);
                foreach (var combo in element.reactsWith)
                {
                    ReactionData data = new()
                    {
                        firstElement = element,
                        secondElement = combo.element,
                        status = combo.status,
                        reaction = combo.reaction,
                    };
                    //Log.Message("e1: " + data.firstElement + ", e2: " + data.secondElement + ", s: " + data.status + ", r: " + data.reaction + ", class: " + data.reaction?.reactionClass);
                    if (data.reaction == null) continue;

                    if (typeof(ElementalReaction).IsAssignableFrom(data.reaction.reactionClass))
                        AllReactionsForReading.Add(data, data.reaction.reactionClass);
                }
                ElementalFillBars.AddDistinct(element, SolidColorMaterials.NewSolidColorTexture(element.color));
            }
            LogMessage("Loaded " + AllReactionsForReading.Count + " reactions!");
            foreach (var t in DefDatabase<ThingDef>.AllDefsListForReading)
            {
                for (int i = 0; i < t.comps.Count; i++)
                {
                    if (t.comps[i] is CompProperties_VisionEquippableAbilities vcomp)
                    {
                        AllVisionsForReading.Add(t, vcomp.visionDef);
                    }
                }
            }
            LogMessage("Loaded " + AllVisionsForReading.Count + " visions!");
        }

        public static void LogMessage(string str) => Log.Message("<color=#f4abba>[GenshinRimpact]</color> " + str);
        public static void LogWarning(string str) => Log.Warning("<color=#f4abba>[GenshinRimpact]</color> " + str);
        public static void LogError(string str) => Log.Error("<color=#f4abba>[GenshinRimpact]</color> " + str);

        public static ElementalReactionDef GetReaction(ElementDef appliedElement, ElementDef otherElement, Status status)
        {
            ElementalReactionDef result = null;
            if (appliedElement != null && appliedElement.reactsWith != null)
            {
                var combo = appliedElement.reactsWith.FirstOrFallback(x => x.element == otherElement || x.status == status);
                if (combo.Equals(default(ElementCombo))) return null;
                result = combo.reaction;
            }
            return result;
        }

        public static List<Pawn> GetPawnsInRange(IntVec3 cell, Map map, float maxRange, bool requireLOS = false, bool affectDowned = false)
        {
            List<Pawn> list = [];
            float range = maxRange * maxRange;
            foreach (Pawn pawn in map.mapPawns.AllPawnsSpawned)
            {
                if (pawn.Spawned && (!pawn.Dead || (affectDowned && pawn.Downed)))
                {
                    float pawnDist = pawn.Position.DistanceToSquared(cell);
                    if (pawnDist <= range)
                    {
                        if (requireLOS && !GenSight.LineOfSight(cell, pawn.Position, map)) continue;
                        list.Add(pawn);
                    }
                }
            }
            return list;
        }

        public static List<Thing> GetThingsInRange(IntVec3 cell, Map map, float maxRange, bool requireLOS = false, ThingCategory category = 0)
        {
            List<Thing> list = [];
            float range = maxRange * maxRange;
            foreach (Thing t in map.spawnedThings)
            {
                if (category != 0 && t.def.category != category) continue;
                if (!t.Destroyed)
                {
                    float pawnDist = t.Position.DistanceToSquared(cell);
                    if (pawnDist <= range)
                    {
                        if (requireLOS && !GenSight.LineOfSight(cell, t.Position, map)) continue;
                        list.Add(t);
                    }
                }
            }
            return list;
        }

        public static List<ThingDef> GetThingDefsInRange(IntVec3 cell, Map map, float maxRange, bool requireLOS = false)
        {
            return GetThingsInRange(cell, map, maxRange, requireLOS).Select(x => x.def).ToList();
        }

        public static Rect VerticalFillableBar(Rect rect, float fillPercent, Texture2D fillTex, Texture2D bgTex, bool doBorder = false, bool flip = false)
        {
            if (bgTex != null)
            {
                GUI.DrawTexture(rect, bgTex);
                if (doBorder)
                {
                    rect = rect.ContractedBy(3f);
                }
            }
            if (!flip)
            {
                rect.y += rect.height;
                rect.height *= -1f;
            }
            Rect result = rect;
            rect.height *= fillPercent;
            GUI.DrawTexture(rect, fillTex);
            return result;
        }

        public static bool TargetFactionValid(this Pawn pawn, FactionFlags flags)
        {
            return flags switch
            {
                FactionFlags.All => true,
                FactionFlags.Neutral => pawn.Faction != Faction.OfPlayer && !pawn.Faction.HostileTo(Faction.OfPlayer),
                FactionFlags.Hostile => pawn.Faction.HostileTo(Faction.OfPlayer),
                _ => true,
            };
        }

        public static List<IntVec3> ConeAffectedCells(ref List<IntVec3> tmpCells, Ability ability, Pawn caster, LocalTargetInfo target, float range = 7.9f, float angle = 60f, float lineWidthEnd = 3f, bool canHitFilledCells = false)
        {
            tmpCells.Clear();
            IntVec3 intVec = target.Cell.ClampInsideMap(caster.Map);
            if (caster.Position == intVec)
            {
                return tmpCells;
            }
            Vector3 vector = caster.Position.ToVector3Shifted().Yto0();
            float lengthHorizontal = (intVec - caster.Position).LengthHorizontal;
            float num = (intVec.x - caster.Position.x) / lengthHorizontal;
            float num2 = (intVec.z - caster.Position.z) / lengthHorizontal;
            intVec.x = Mathf.RoundToInt(caster.Position.x + num * range);
            intVec.z = Mathf.RoundToInt(caster.Position.z + num2 * range);
            float target2 = Vector3.SignedAngle(intVec.ToVector3Shifted().Yto0() - vector, Vector3.right, Vector3.up);
            float num3 = lineWidthEnd / 2f;
            float num4 = Mathf.Sqrt(Mathf.Pow((intVec - caster.Position).LengthHorizontal, 2f) + Mathf.Pow(num3, 2f));
            float num5 = angle * Mathf.Asin(num3 / num4); //57.29578f
            int num6 = GenRadial.NumCellsInRadius(range);
            for (int i = 0; i < num6; i++)
            {
                IntVec3 intVec2 = caster.Position + GenRadial.RadialPattern[i];
                if (CanUseCell(intVec2, caster.Position, caster.MapHeld, range, ability?.verb, canHitFilledCells) && Mathf.Abs(Mathf.DeltaAngle(Vector3.SignedAngle(intVec2.ToVector3Shifted().Yto0() - vector, Vector3.right, Vector3.up), target2)) <= num5)
                {
                    tmpCells.Add(intVec2);
                }
            }
            List<IntVec3> list = GenSight.BresenhamCellsBetween(caster.Position, intVec);
            for (int j = 0; j < list.Count; j++)
            {
                IntVec3 intVec3 = list[j];
                if (!tmpCells.Contains(intVec3) && CanUseCell(intVec3, caster.Position, caster.MapHeld, range, ability?.verb, canHitFilledCells))
                {
                    tmpCells.Add(intVec3);
                }
            }
            return tmpCells;
        }

        public static bool CanUseCell(IntVec3 c, IntVec3 pos, Map map, float range, Verb verb, bool canHitFilledCells = false)
        {
            if (!c.InBounds(map))
                return false;

            if (c == pos)
                return false;

            if (!canHitFilledCells && c.Filled(map))
                return false;

            if (!c.InHorDistOf(pos, range))
                return false;

            return verb?.TryFindShootLineFromTo(pos, c, out ShootLine _) ?? false;
        }

        public static List<IntVec3> AffectedLineCells(ref List<IntVec3> tmpCells, IntVec3 casterPos, IntVec3 targetPos, Map map, float range, Verb verb, bool canHitFilledCells = false)
        {
            tmpCells.Clear();
            //Utils.LogMessage(targetPos.ToString());
            IntVec3 intVec = targetPos.ClampInsideMap(map);
            if (casterPos == intVec)
            {
                return tmpCells;
            }
            var list = GenSight.BresenhamCellsBetween(casterPos, intVec);
            for (int j = 0; j < list.Count; j++)
            {
                IntVec3 intVec3 = list[j];
                if (!tmpCells.Contains(intVec3) && CanUseCell(intVec3, casterPos, map, range, verb, canHitFilledCells))
                {
                    tmpCells.Add(intVec3);
                }
            }
            return tmpCells;
        }

        public static List<IntVec3> GetHalfCircleCells(ref List<IntVec3> tmpCells, IntVec3 casterCell, IntVec3 targetCell, Map map, float radius, float angleRadians = Mathf.PI, bool filled = true)
        {
            tmpCells.Clear();
            if (casterCell == targetCell)
            {
                return tmpCells;
            }

            float halfAngleRadians = angleRadians / 2f;
            float startAngleRadians = Mathf.Atan2(targetCell.z - casterCell.z, targetCell.x - casterCell.x) - halfAngleRadians;

            for (float angle = startAngleRadians; angle <= startAngleRadians + angleRadians; angle += 0.1f)
            {
                if (filled)
                {
                    for (int r = 0; r <= radius; r++)
                    {
                        int x = Mathf.RoundToInt(casterCell.x + r * Mathf.Cos(angle));
                        int z = Mathf.RoundToInt(casterCell.z + r * Mathf.Sin(angle));
                        IntVec3 cell = new(x, 0, z);

                        if (!tmpCells.Contains(cell) && cell.InBounds(map))
                        {
                            tmpCells.Add(cell);
                        }
                    }
                }
                else
                {
                    int x = Mathf.RoundToInt(casterCell.x + radius * Mathf.Cos(angle));
                    int z = Mathf.RoundToInt(casterCell.z + radius * Mathf.Sin(angle));
                    IntVec3 cell = new(x, 0, z);

                    if (!tmpCells.Contains(cell) && cell.InBounds(map))
                    {
                        tmpCells.Add(cell);
                    }
                }
            }
            return tmpCells;
        }

        public static IntVec3 RedirectIntVec3ToMaxRange(IntVec3 casterPos, IntVec3 targetPos, Map map, float range)
        {
            Vector3 direction = (targetPos - casterPos).ToVector3();
            if (direction == Vector3.zero)
            {
                return targetPos;
            }
            direction.Normalize();
            float maxRange = range;
            IntVec3 newTargetPos = casterPos + (direction * maxRange).ToIntVec3();
            if (newTargetPos.InBounds(map))
            {
                return newTargetPos;
            }
            else
            {
                // If the furthest cell isn't valid, find the furthest valid cell in the direction.
                for (float i = maxRange; i > 0; i--)
                {
                    IntVec3 potentialTargetPos = casterPos + (direction * i).ToIntVec3();
                    if (potentialTargetPos.InBounds(map))
                    {
                        return potentialTargetPos;
                    }
                }
                return targetPos;
            }
        }

        public static List<IntVec3> GetCellsInRectangle(IntVec3 center, IntVec3 direction, Map map, int length, int width)
        {
            List<IntVec3> affectedCells = [];

            // Calculate the rotation angle from the direction vector
            float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;

            // Create the rectangle and rotate it around the origin
            Rect rect = new(-width / 2, -length / 2, width, length);
            Rect rotatedRect = RotateRectangleAroundPivot(rect, Vector2.zero, angle);

            // Convert the rotated rectangle to a list of affected cells
            for (int x = Mathf.FloorToInt(rotatedRect.xMin); x <= Mathf.CeilToInt(rotatedRect.xMax); x++)
            {
                for (int z = Mathf.FloorToInt(rotatedRect.yMin); z <= Mathf.CeilToInt(rotatedRect.yMax); z++)
                {
                    var cell = center + new IntVec3(x, 0, z);
                    if (cell.InBounds(map)) affectedCells.Add(cell);
                }
            }

            return affectedCells;
        }

        public static Rect RotateRectangleAroundPivot(Rect rect, Vector2 pivot, float angle)
        {
            float sin = Mathf.Sin(angle * Mathf.Deg2Rad);
            float cos = Mathf.Cos(angle * Mathf.Deg2Rad);

            float newX = (rect.x - pivot.x) * cos - (rect.y - pivot.y) * sin + pivot.x;
            float newY = (rect.x - pivot.x) * sin + (rect.y - pivot.y) * cos + pivot.y;

            return new Rect(newX, newY, rect.width, rect.height);
        }

        /*public static IntVec3 MaxRangeIntVec3(IntVec3 dest, float range)
        {
            float sqrRange = range * range;
            float mult = sqrRange / dest.SqrMagnitude;
            var result = (dest.ToVector3() * mult).ToIntVec3();
            Utils.LogMessage(range + " ... " +dest + " ... " + sqrRange + " ... "+ dest.SqrMagnitude + " ... " + mult + " ... " + result); 
            return mult > 1 ? result : dest;
        }*/

        public static void TryDoAbility(Pawn pawn, AbilityDef abilityDef, LocalTargetInfo targetCell)
        {
            Ability ab = pawn.abilities?.GetAbility(abilityDef);
            if (ab == null)
            {
                Log.Error("subAbility is null");
                return;
            }
            if (!ab.CanCast)
            {
                Log.Error("Can't cast subAbility");
                return;
            }
            Job job = ab.GetJob(targetCell, targetCell);
            pawn.jobs.StartJob(job, JobCondition.InterruptForced);
        }

        public static void DoAoEAbility(LocalTargetInfo targetCell, Pawn caster, AbilityDef abilityDef, float damageAmount = 10f, float radius = 3.9f, DamageDef damageDef = null, HediffDef hediffDef = null, float hediffSeverity = 1f, EffecterDef effecterOnTrigger = null, bool isExplosive = false, bool isDirect = false, bool canFriendlyFire = false, bool onlyAffectFriendlies = false, float explosionScreenShake = 0f, SoundDef explosionSound = null, AoEShape shape = AoEShape.Radial, float angleRad = Mathf.PI)
        {
            IntVec3 cell = targetCell.Cell;
            Map map = caster.Map;
            List<Thing> affectedThings = [];
            List<Thing> ignoredThings = [];

            effecterOnTrigger?.Spawn(cell, map).Cleanup();

            List<IntVec3> cells = [];
            switch (shape)
            {
                case AoEShape.Radial:
                    cells = GenRadial.RadialCellsAround(cell, radius, true).ToList();
                    break;
                case AoEShape.HalfRadial:
                    GetHalfCircleCells(ref cells, caster.Position, cell, map, radius, angleRad, false); // PI/2 by default
                    break;
                case AoEShape.HalfRadialFilled:
                    GetHalfCircleCells(ref cells, caster.Position, cell, map, radius, angleRad, true); // PI/2 by default
                    break;
            }
            //int cellNum = GenRadial.NumCellsInRadius(radius);
            for (int i = 0; i < cells.Count; i++)
            {
                //IntVec3 intVec = cell + GenRadial.RadialPattern[i];
                if (!cells[i].InBounds(map)) continue;

                List<Thing> thingList = cells[i].GetThingList(map);
                for (int j = 0; j < thingList.Count; j++)
                {
                    Thing thing = thingList[j];
                    //Log.Message(thing);
                    // If not friendly, but only affect friendlies
                    if ((thing.Faction == null || thing.Faction.HostileTo(caster.Faction)) && onlyAffectFriendlies)
                    {
                        ignoredThings.Add(thing);
                        continue;
                    }
                    if (thing.Faction != null && thing.Faction.IsPlayer)
                    {
                        if (canFriendlyFire)
                        {
                            //Log.Message("added player");
                            affectedThings.Add(thing); // affect if can friendly fire
                            continue;
                        }
                        else
                        {
                            ignoredThings.Add(thing); // ignore if can't
                            continue;
                        }
                    }
                    // Hostile and neutrals
                    //Log.Message("added non-player");
                    affectedThings.Add(thing);
                }
            }

            if (isDirect)
            {
                for (int i = 0; i < affectedThings.Count; i++)
                {
                    Thing thing = affectedThings[i];

                    ModExt_Element ext = null;
                    if (damageDef != null)
                    {
                        ext = damageDef?.GetModExtension<ModExt_Element>();

                        var dresult = thing.TakeDamage(new(damageDef, damageAmount, instigator: caster, intendedTarget: thing));
                        if (thing is Pawn pawn)
                        {
                            BattleLogEntry_DamageTakenAbility battleLog = new(pawn, RulePackDefOf.Event_AbilityUsed, abilityDef, caster);
                            Find.BattleLog.Add(battleLog);
                            dresult.AssociateWithLog(battleLog);
                        }
                    }
                    if (hediffDef != null)
                    {
                        ext = hediffDef?.GetModExtension<ModExt_Element>();

                        if (thing is Pawn pawn)
                        {
                            Hediff h = HediffMaker.MakeHediff(hediffDef, pawn);
                            h.Severity = hediffSeverity;
                            pawn.health.AddHediff(h);
                            //BattleLogEntry_AbilityUsed battleLog = new(Pawn, pawn, parent.def, RulePackDefOf.Event_AbilityUsed);
                            //Find.BattleLog.Add(battleLog);
                        }
                    }
                    if (ext != null && thing.TryGetComp<CompElementalHandler>() is CompElementalHandler comp) comp.ApplyElement(ext.element, caster);
                }
            }
            if (isExplosive)
            {
                GenExplosion.DoExplosion(cell, caster.MapHeld, radius, damageDef, caster, (int)damageAmount, ignoredThings: ignoredThings, screenShakeFactor: explosionScreenShake, explosionSound: explosionSound);
            }
        }
    }

    [Flags]
    public enum FactionFlags
    {
        Hostile = 0,
        Neutral = 1,
        All = 2
    }

    public enum AoEShape
    {
        Radial = 0,
        HalfRadial = 1,
        HalfRadialFilled = 2,
        Rectangular = 3 // Unused for now
    }
}
