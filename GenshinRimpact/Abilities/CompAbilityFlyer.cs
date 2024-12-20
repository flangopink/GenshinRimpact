using RimWorld;
using Verse;
using Verse.Sound;

namespace GenshinRimpact
{
    public class CompProperties_AbilityFlyer : CompProperties_AbilityEffect
    {
        public AbilityDef abilityOnFinish; // should be Command_Invisible, // or, better yet, use OnJumpCompleted in a comp.
        public ThingDef flyerDef;

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

        public CompProperties_AbilityFlyer() => compClass = typeof(CompAbilityFlyer);
    }

    [HotSwap.HotSwappable]
    public class CompAbilityFlyer : CompAbilityEffect, ICompAbilityEffectOnJumpCompleted
    {
        public new CompProperties_AbilityFlyer Props => (CompProperties_AbilityFlyer)props;

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

            var dashingPawn = /*(DashingPawn)*/PawnFlyer.MakeFlyer(Props.flyerDef, Pawn, target.Cell, Props.flightEffecter, Props.endSound);
            //dashingPawn.ability = parent;

            //dashingPawn.target = target.Thing == null ? target.CenterVector3 : target.Thing.InteractionCell.ToVector3();

            GenSpawn.Spawn(dashingPawn, Pawn.Position, map);

            Props.startSound?.PlayOneShot(Pawn);
            Props.onStartEffecter?.Spawn(Pawn.Position, map).Cleanup();
        }

        public void OnJumpCompleted(IntVec3 origin, LocalTargetInfo target)
        {
            if (Props.abilityOnFinish != null)
            {
                Utils.TryDoAbility(Pawn, Props.abilityOnFinish, target); //.Thing is Pawn p && Props.abilityOnFinish.verbProperties.targetParams.canTargetPawns ? p : target.Cell);
            }
        }
    }
}
