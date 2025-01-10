using Verse;
using RimWorld;

namespace Rimpact
{
    public class Verb_SpawnWithFaction : Verb_CastAbility
    {
        protected override bool TryCastShot()
        {
            if (currentTarget.HasThing && currentTarget.Thing.Map != caster.Map)
            {
                return false;
            }
            base.TryCastShot();
            Thing t = ThingMaker.MakeThing(verbProps.spawnDef);
            Utils.TrySetFaction(t, caster.Faction);
            GenSpawn.Spawn(t, currentTarget.Cell, caster.Map);
            return true;
        }
    }
}
