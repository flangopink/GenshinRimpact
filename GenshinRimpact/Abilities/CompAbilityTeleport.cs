using RimWorld;
using System.Linq;
using Verse;

namespace GenshinRimpact
{
    public class CompProperties_AbilityTeleport : CompProperties_AbilityEffect
    {
        public ThingDef teleportThingDef;
        public EffecterDef entryEffecter;
        public EffecterDef exitEffecter;
        public bool doEffecter = true;
        public CompProperties_AbilityTeleport() => compClass = typeof(CompAbilityTeleport);
    }

    [HotSwap.HotSwappable]
    public class CompAbilityTeleport : CompAbilityEffect
    {
        public new CompProperties_AbilityTeleport Props => (CompProperties_AbilityTeleport)props;
        private Pawn Pawn => parent.pawn;

        //public Thing teleportThing;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            if (Props.teleportThingDef != null)
            {
                var teleportThing = Pawn.MapHeld.spawnedThings.FirstOrDefault((Thing x) => x.def == Props.teleportThingDef && x.TryGetComp<CompHasOwner>()?.ownerAbility.pawn == Pawn); // poorly optimized but ok
                if (teleportThing != null)
                {
                    Utils.SkipTo(Pawn, teleportThing.Position, teleportThing.MapHeld, Props.entryEffecter, Props.exitEffecter, Props.doEffecter);
                    teleportThing.Destroy();
                }
                else Messages.Message("CannotUseAbility".Translate(parent.def.label) + ": " + "GR_TeleportDestinationDoesNotExist".Translate(), MessageTypeDefOf.RejectInput, false);
            }
            else SkipUtility.SkipTo(Pawn, target.Cell, Pawn.MapHeld);
        }

        //public override void PostExposeData()
        //{
        //    base.PostExposeData();
        //    Scribe_References.Look(ref teleportThing, "teleportThing");
        //}
    }
}
