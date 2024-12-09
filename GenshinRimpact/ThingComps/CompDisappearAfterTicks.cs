using Verse;

namespace GenshinRimpact
{
    public class CompProperties_DisappearAfterTicks : CompProperties
    {
        public int ticks = 300; // 5 sec
        public EffecterDef effecter;
        public CompProperties_DisappearAfterTicks() => compClass = typeof(CompDisappearAfterTicks);
    }

    public class CompDisappearAfterTicks : ThingComp
    {
        public CompProperties_DisappearAfterTicks Props => (CompProperties_DisappearAfterTicks)props;

        private int ticksLeft = -1;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (ticksLeft == -1) ticksLeft = Props.ticks;
        }

        public override void CompTick()
        {
            base.CompTick();
            if (ticksLeft <= 0) parent.Destroy();
            ticksLeft--;
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            Props.effecter?.Spawn(parent.PositionHeld, previousMap).Cleanup();
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref ticksLeft, "ticksLeft");
        }
    }
}
