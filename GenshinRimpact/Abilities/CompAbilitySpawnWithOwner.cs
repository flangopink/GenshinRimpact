using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Rimpact
{
    public class CompProperties_AbilitySpawnWithOwner : CompProperties_AbilityEffect
    {
        public ThingDef thingDef;
        public bool allowOnBuildings = true;
        public int spawnLimit = 1;
        public CompProperties_AbilitySpawnWithOwner() => compClass = typeof(CompAbilitySpawnWithOwner);
    }

    [HotSwap.HotSwappable]
    public class CompAbilitySpawnWithOwner : CompAbilityEffect // Use this for Keqing teleports or something similar
    {
        public new CompProperties_AbilitySpawnWithOwner Props => (CompProperties_AbilitySpawnWithOwner)props;
        public HashSet<Thing> spawnedThings = [];

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            if (Props.spawnLimit > 0 && spawnedThings.Count >= Props.spawnLimit) 
            {
                if (spawnedThings.TryMinBy(x => x.TickSpawned, out Thing replacedThing))
                {
                    spawnedThings.Remove(replacedThing);
                    replacedThing.Destroy();
                }
            }
            Thing t = ThingMaker.MakeThing(Props.thingDef);
            Utils.TrySetFaction(t, parent.pawn.Faction);
            var comp = t.TryGetComp<CompHasOwner>();
            if (comp != null) comp.ownerAbility = parent;
            else Utils.LogError(t.ToString() + " does not have CompHasOwner");
            GenSpawn.Spawn(t, target.Cell, parent.pawn.Map);
            spawnedThings.Add(t);
        }

        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            if (target.Cell.Filled(parent.pawn.Map) || (!Props.allowOnBuildings && target.Cell.GetEdifice(parent.pawn.Map) != null))
            {
                if (throwMessages)
                {
                    Messages.Message("CannotUseAbility".Translate(parent.def.label) + ": " + "AbilityOccupiedCells".Translate(), target.ToTargetInfo(parent.pawn.Map), MessageTypeDefOf.RejectInput, historical: false);
                }
                return false;
            }
            return true;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look(ref spawnedThings, "spawnedThings", LookMode.Deep);
        }
        public override bool AICanTargetNow(LocalTargetInfo target) => !parent.pawn.IsColonistPlayerControlled;
    }
}
