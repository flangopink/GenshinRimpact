using RimWorld;
using Verse;
using Verse.Sound;

namespace GenshinRimpact
{
    public class CompProperties_AbilityDash : CompProperties_AbilityEffect
    {
        public AbilityDef subAbilityOnFinish; // should be Command_Invisible

        public bool damagePawnsOnPath;
        public bool attackOnFinish;

        public DamageDef damageDef;
        public float damageAmount = 5f;

        public float altitudeMultiplier = 1f;

        public SoundDef startSound;
        public SoundDef endSound;

        public EffecterDef flightEffecter;
        public EffecterDef onStartEffecter;
        public EffecterDef onDashingEffecter;
        public EffecterDef onFinishEffecter;

        public CompProperties_AbilityDash() => compClass = typeof(CompAbilityDash);
    }

    [HotSwap.HotSwappable]
    public class CompAbilityDash : CompAbilityEffect
    {
        public new CompProperties_AbilityDash Props => (CompProperties_AbilityDash)props;

        private Pawn Pawn => parent.pawn;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);

            Map map = Pawn.Map;
            if (map == null)
            {
                Log.Error("Null map for CompAbilityDash");
                return;
            }

            DashingPawn dashingPawn = (DashingPawn)PawnFlyer.MakeFlyer(DefDatabase<ThingDef>.GetNamed("DashingPawn"), Pawn, target.Cell, Props.flightEffecter, Props.endSound);
            dashingPawn.ability = parent;

            dashingPawn.target = target.Thing == null ? target.CenterVector3 : target.Thing.InteractionCell.ToVector3();

            GenSpawn.Spawn(dashingPawn, Pawn.Position, map);

            Props.startSound?.PlayOneShot(Pawn);
            Props.onStartEffecter?.Spawn(Pawn.Position, map).Cleanup();
        }
    }
}
