using Verse;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rimpact
{
    public class CompProperties_AoESpawner : CompProperties
    {
        public ThingDef spawnThing;
        public ThingDef shootThing;
        public int destroyAfterTicks = 300;
        public float range = 0.9f;
        public float minRange;
        //public EffecterDef startEffecter;
        public EffecterDef spawnEffecter;
        public EffecterDef endEffecter;
        public IntRange spawnDelayTicks = IntRange.zero;
        public IntRange spawnCount = IntRange.one;
        public IntVec3 shootOffset = IntVec3.Zero;
        public bool isEffecterAtTarget;
        public bool highlightCells;
        public Color highlightColor = Color.white;
        
        public CompProperties_AoESpawner() => compClass = typeof(CompAoESpawner);
    }
    public class CompAoESpawner : ThingComp
    {
        public CompProperties_AoESpawner Props => (CompProperties_AoESpawner)props;

        private int ticksLeft;
        private List<IntVec3> cachedCells = [];

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref ticksLeft, "ticksLeft", 0);
        }

        public override void CompTick()
        {
            base.CompTick();
            if (ticksLeft > 0)
            {
                if (parent.IsHashIntervalTick(Props.spawnDelayTicks.RandomInRange))
                {
                    Map map = parent.Map;
                    IntVec3 pos = parent.Position;

                    if (!parent.Spawned || map == null) return;

                    var cells = GenRadial.RadialCellsAround(pos, Props.minRange, Props.range);
                    if (cachedCells.EnumerableNullOrEmpty()) cachedCells = cells.ToList();

                    for (int i = 0; i < Props.spawnCount.RandomInRange; i++)
                    {
                        IntVec3 cell = cells.RandomElement();
                        if (!cell.InBounds(map)) continue;
                        if (Props.spawnThing != null) SpawnThing(map, cell);
                        if (Props.shootThing != null) ShootProjectile(map, cell, Props.shootOffset);
                    }
                }
                ticksLeft--;
            }
            else parent.Destroy();
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (ticksLeft <= 0) ticksLeft = Props.destroyAfterTicks;
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
            Props.endEffecter?.Spawn(parent.PositionHeld, previousMap).Cleanup();
        }

        private void SpawnThing(Map map, IntVec3 pos)
        {
            Props.spawnEffecter?.Spawn(pos, map).Cleanup();
            if (Props.spawnThing != null)
            {
                Thing thing = ThingMaker.MakeThing(Props.spawnThing);
                thing.SetFaction(parent.Faction); 
                GenSpawn.CheckMoveItemsAside(pos, default, thing.def, map);
                GenPlace.TryPlaceThing(thing, pos, map, ThingPlaceMode.Near);
            }
        }

        private void ShootProjectile(Map map, IntVec3 target, IntVec3 offset)
        {
            IntVec3 launchCell = (target + offset).ClampInsideMap(map);
            Props.spawnEffecter?.Spawn(Props.isEffecterAtTarget ? target : launchCell, map).Cleanup();
            Projectile proj = (Projectile)GenSpawn.Spawn(Props.shootThing, launchCell, map);
            proj.Launch(parent, target, target, ProjectileHitFlags.All);
        }

        public override void PostDraw()
        {
            base.PostDraw();
            if (Props.highlightCells) GenDraw.DrawFieldEdges(cachedCells, Props.highlightColor);
        }
    }
}
