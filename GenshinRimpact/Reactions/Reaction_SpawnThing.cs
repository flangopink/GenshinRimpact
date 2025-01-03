using Verse;

namespace Rimpact
{
    public class Reaction_SpawnThing : ElementalReaction
    {
        public override void ApplyReaction(Thing target, Thing instigator = null)
        {
            IntVec3 pos = Def.spawnThingNearCaster ? instigator.PositionHeld : target.Position;
            Def.spawnedThingEffecter?.Spawn(pos, target.Map).Cleanup();
            if (Def.spawnedThing != null)
            {
                Thing thing = ThingMaker.MakeThing(Def.spawnedThing, Def.spawnedThingStuff);
                if (Def.setFaction) thing.SetFaction(instigator.Faction);
                //GenSpawn.CheckMoveItemsAside(pos, default, parent.def, map);
                GenPlace.TryPlaceThing(thing, pos, target.Map, ThingPlaceMode.Near);
            }
        }
    }
}
