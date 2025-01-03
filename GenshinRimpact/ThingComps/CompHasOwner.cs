using RimWorld;
using Verse;

namespace Rimpact
{
    public class CompProperties_HasOwner : CompProperties
    {
        public CompProperties_HasOwner() => compClass = typeof(CompHasOwner);
    }

    public class CompHasOwner : ThingComp
    {
        public Ability ownerAbility;

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            if (ownerAbility?.CompOfType<CompAbilitySpawnWithOwner>() is CompAbilitySpawnWithOwner comp)
                comp.spawnedThings.Remove(parent);
            else Utils.LogWarning(parent + " couldn't find CompAbilitySpawnWithOwner in " + ownerAbility);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_References.Look(ref ownerAbility, "ownerAbility");
        }

        public override string CompInspectStringExtra() => "Owner".Translate() + ": " + $"{ownerAbility.pawn.LabelCap}".Colorize(ColoredText.NameColor);
    }
}