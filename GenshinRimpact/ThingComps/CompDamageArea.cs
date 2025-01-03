using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Rimpact
{
    public class CompProperties_DamageArea : CompProperties
    {
        public DamageDef damageDef;
        public float damageAmount;
        public float damageAmountLast;
        public float armorPenetration;

        public int intervalTicks = 30; // 0.5 sec

        public bool onlyHitOnce;
        public bool damagePawnsOnly;

        public float radius = 0.9f;

        public bool rectangular;
        public int rectLength = 2; // [][][]
        public int rectWidth = 3;  // [][][]

        public bool highlightCells;
        public Color highlightColor = Color.white;

        public EffecterDef onTriggerEffecter;
        public EffecterDef endEffecter;

        public CompProperties_DamageArea() => compClass = typeof(CompDamageArea);
    }

    [HotSwap.HotSwappable]
    public class CompDamageArea : ThingComp
    {
        public CompProperties_DamageArea Props => (CompProperties_DamageArea)props;

        public Thing instigator;
        public AbilityDef abilityDef;
        private List<IntVec3> affectedCells = [];
        private List<Thing> hitThings = [];
        private IntVec3 prevPos;
        private IntVec3 pos;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            pos = IntVec3.Invalid;
            if (parent is Projectile proj)
            {
                instigator = proj.Launcher;
            }
        }

        public override void PostExposeData()
        {
            Scribe_References.Look(ref instigator, "instigator");
            Scribe_Defs.Look(ref abilityDef, "abilityDef");
            Scribe_Collections.Look(ref affectedCells, "affectedCells");
            Scribe_Collections.Look(ref hitThings, "hitThings", LookMode.Deep);
            Scribe_Values.Look(ref prevPos, "prevPos", parent.Position);
        }

        public override void PostDraw()
        {
            if (Props.highlightCells && affectedCells.Count > 0)
            {
                GenDraw.DrawFieldEdges(affectedCells, Props.highlightColor);
            }
        }

        public override void CompTick()
        {
            base.CompTick();
            if (!parent.Spawned || parent.MapHeld == null) return;
            if (pos != parent.Position) 
            {
                prevPos = pos;
                pos = parent.Position; 
                affectedCells = Props.rectangular ? Utils.GetCellsInRectangle(pos, pos - prevPos, parent.MapHeld, Props.rectLength, Props.rectWidth)
                                                  : GenRadial.RadialCellsAround(pos, Props.radius, true).ToList();
            }
            if (parent.IsHashIntervalTick(Props.intervalTicks))
            {
                DoDamageInCells(Props.damageAmount, parent.MapHeld);
                Props.onTriggerEffecter?.Spawn(pos, parent.MapHeld).Cleanup();
            }
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
            DoDamageInCells(Props.damageAmountLast, previousMap);
            Props.onTriggerEffecter?.Spawn(pos, previousMap).Cleanup();
        }

        private void DoDamageInCells(float amount, Map map)
        {
            for (int i = 0; i < affectedCells.Count; i++)
            {
                var things = affectedCells[i].GetThingList(map);
                for (int j = 0; j < things.Count; j++)
                {
                    var t = things[j];
                    if (t.Faction == parent.Faction) continue;
                    if (Props.damagePawnsOnly && t is not Pawn) continue;
                    if (Props.onlyHitOnce && hitThings.Contains(t)) continue;

                    var dresult = t.TakeDamage(new DamageInfo(Props.damageDef, amount, Props.armorPenetration, instigator: instigator));
                    if (!t.Destroyed) hitThings.Add(t);
                    if (instigator != null && abilityDef != null && t is Pawn p)
                    {
                        BattleLogEntry_DamageTakenAbility battleLog = new(p, RulePackDefOf.Event_AbilityUsed, abilityDef, instigator);
                        Find.BattleLog.Add(battleLog);
                        dresult.AssociateWithLog(battleLog);
                    }
                }
            }
        }
    }
}
