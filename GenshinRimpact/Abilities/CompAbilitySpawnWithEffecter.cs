using RimWorld;
using Verse;

namespace GenshinRimpact
{
    public class CompProperties_AbilitySpawnWithEffecter : CompProperties_AbilityEffect
    {
        public ThingDef thingDef;
        public EffecterDef effecter;
        public bool allowOnBuildings = true;
        public CompProperties_AbilitySpawnWithEffecter() => compClass = typeof(CompAbilitySpawnWithEffecter);
    }

    [HotSwap.HotSwappable]
    public class CompAbilitySpawnWithEffecter : CompAbilityEffect // Use this for Keqing teleports or something similar
    {
        public new CompProperties_AbilitySpawnWithEffecter Props => (CompProperties_AbilitySpawnWithEffecter)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            Thing t = GenSpawn.Spawn(Props.thingDef, target.Cell, parent.pawn.Map);
            t.SetFaction(parent.pawn.Faction);
            Props.effecter?.Spawn(target.Cell, parent.pawn.Map).Cleanup();
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
    }
}
