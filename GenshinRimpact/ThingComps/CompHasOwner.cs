using Verse;

namespace GenshinRimpact
{
    public class CompProperties_HasOwner : CompProperties
    {
        public CompProperties_HasOwner() => compClass = typeof(CompHasOwner);
    }

    public class CompHasOwner : ThingComp
    {
        public Thing owner;
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_References.Look(ref owner, "owner");
        }
    }
}
