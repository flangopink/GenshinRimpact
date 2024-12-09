using Verse;

namespace GenshinRimpact
{
    public class HediffCompProperties_SpawnThing : HediffCompProperties
    {
        public ThingDef onApplied;
        public ThingDef onRemoved;
        public EffecterDef effecter;
        public HediffCompProperties_SpawnThing() => compClass = typeof(HediffComp_SpawnThing);
    }

    public class HediffComp_SpawnThing : HediffComp
    {
        public HediffCompProperties_SpawnThing Props => (HediffCompProperties_SpawnThing)props;

        private void Spawn(ThingDef thingDef)
        {
            if (thingDef == null) return;
            Pawn p = parent.pawn;
            Map map = p.MapHeld;
            IntVec3 pos = parent.pawn.PositionHeld;
            Props.effecter?.Spawn(pos, map).Cleanup();
            Thing thing = ThingMaker.MakeThing(thingDef);
            thing.SetFaction(parent.pawn.Faction);
            GenSpawn.CheckMoveItemsAside(pos, default, thing.def, map);
            GenPlace.TryPlaceThing(thing, pos, map, ThingPlaceMode.Near);
        }
        public override void CompPostPostAdd(DamageInfo? dinfo) => Spawn(Props.onApplied);
        public override void CompPostPostRemoved() => Spawn(Props.onRemoved);
    }
}
