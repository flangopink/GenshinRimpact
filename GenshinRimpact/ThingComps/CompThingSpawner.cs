using Verse;

namespace GenshinRimpact
{
    public class CompProperties_ThingSpawner : CompProperties
    {
        public ThingDef spawnThing;
        public ThingDef spawnThingStuff;
        public EffecterDef effecter;
        public bool spawnOnDestroy;
        public bool setFaction;

        public CompProperties_ThingSpawner() => compClass = typeof(CompThingSpawner);
    }
    public class CompThingSpawner : ThingComp
    {
        public CompProperties_ThingSpawner Props => (CompProperties_ThingSpawner)props;

        //private int ticksToSpawn;
        //private int delayTicks;

        /*public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref ticksToSpawn, "ticksToSpawn", 0);
            Scribe_Values.Look(ref delayTicks, "delayTicks", 0);
        }*/

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
            if (Props.spawnOnDestroy)
            {
                SpawnThing(previousMap);
            }
        }

        private void SpawnThing(Map map)
        {
            IntVec3 pos = parent.PositionHeld;
            Props.effecter?.Spawn(pos, map).Cleanup();
            if (Props.spawnThing != null)
            {
                Thing thing = ThingMaker.MakeThing(Props.spawnThing, Props.spawnThingStuff);
                if (Props.setFaction) thing.SetFaction(parent.Faction); 
                GenSpawn.CheckMoveItemsAside(pos, default, thing.def, map);
                GenPlace.TryPlaceThing(thing, pos, map, ThingPlaceMode.Near);
            }
        }
    }
}
