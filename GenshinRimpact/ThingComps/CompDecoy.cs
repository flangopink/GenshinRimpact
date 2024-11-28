using RimWorld;
using Verse.AI;
using Verse;

namespace GenshinRimpact
{
    public class CompProperties_Decoy : CompProperties
    {
        public IntRange jobDurationTicks = new(240, 360); // 3~6 sec
        public IntRange forcedAttackCount = new(2, 4);
        public float chanceToAggro = 1f;
        public int destroyAfterTicks = 600; // 10 sec
        public int updateInterval = 60; // if 0 then only initial pawns get affected
        public float range = 9.9f;
        public bool requireLOS = true;
        public bool ignoredByOtherDecoys = true;
        public FactionFlags targetFlags;
        public EffecterDef effecterSpawned;
        public EffecterDef effecterDestroyed;
        public bool explodeOnDestroy;
        public float explosionRadius = 2.9f;
        public DamageDef explosionDamageDef;

        public CompProperties_Decoy() => compClass = typeof(CompDecoy);
    }

    public class CompDecoy : ThingComp
    {
        public CompProperties_Decoy Props => (CompProperties_Decoy)props;
        private int timer;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref timer, "timer");
        }

        public override void CompTick()
        {
            base.CompTick();
            if (timer <= 0)
            {
                parent.Destroy();
                return;
            }
            if (parent.IsHashIntervalTick(Props.updateInterval)) AggroNearbyPawns();
            timer--;
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (timer <= 0) timer = Props.destroyAfterTicks;
            Props.effecterSpawned?.Spawn(parent.Position, parent.Map).Cleanup();
            AggroNearbyPawns();
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
            IntVec3 pos = parent.PositionHeld;
            Props.effecterDestroyed?.Spawn(pos, previousMap).Cleanup();
            if (Props.explodeOnDestroy)
            {
                GenExplosion.DoExplosion(pos, previousMap, Props.explosionRadius, Props.explosionDamageDef ?? DamageDefOf.Bomb, parent);
            }
        }

        public void AggroNearbyPawns()
        {
            if (parent == null || parent.Map == null) return; // Just in case

            foreach (Pawn pawn in Utils.GetPawnsInRange(parent.Position, parent.Map, Props.range, Props.requireLOS))
            {
                if (!pawn.TargetFactionValid(Props.targetFlags)) continue;
                if (Rand.Value > Props.chanceToAggro) return;

                if (pawn.CurJob == null || pawn.CurJob.AnyTargetIs(parent) || (Props.ignoredByOtherDecoys && pawn.CurJob?.targetA.Thing?.TryGetComp<CompDecoy>() != null)) return;

                Job job;
                if (pawn.CurrentEffectiveVerb?.verbProps.IsMeleeAttack ?? true)
                {
                    job = JobMaker.MakeJob(JobDefOf.AttackMelee, new LocalTargetInfo(parent));
                }
                else
                {
                    job = JobMaker.MakeJob(JobDefOf.AttackStatic, new LocalTargetInfo(parent));
                    job.maxNumStaticAttacks = Props.forcedAttackCount.RandomInRange;
                    job.endIfCantShootTargetFromCurPos = Props.requireLOS;
                }

                job.expiryInterval = Props.jobDurationTicks.RandomInRange;
                pawn.jobs.StopAll();
                pawn.jobs.StartJob(job, JobCondition.InterruptForced);
            }
        }
    }
}
