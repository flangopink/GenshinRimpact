using RimWorld;
using Verse;

namespace Rimpact
{
    public class CompProperties_ThingFaction : CompProperties
    {
        public CompProperties_ThingFaction() => compClass = typeof(CompThingFaction);
    }

    public class CompThingFaction : ThingComp // because for some reason things that aren't pawns or buildings can't have factions.
    {
        public Faction ownerFaction;
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_References.Look(ref ownerFaction, "ownerFaction");
        }

        public override string CompInspectStringExtra() => "Faction".Translate() + ": " + $"{ownerFaction.NameColored}";

    }
}
