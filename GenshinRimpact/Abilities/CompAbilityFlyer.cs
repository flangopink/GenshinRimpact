using RimWorld;
using Verse;
using Verse.Sound;

namespace Rimpact
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
    public class CompAbilityFlyer : CompAbilityEffect, ICompAbilityEffectOnJumpCompleted // Used with Verb_JumpExt
    {
        public new CompProperties_AbilityFlyer Props => (CompProperties_AbilityFlyer)props;

        private Pawn Pawn => parent.pawn;

        /*public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            
            Map map = Pawn.Map;
            if (map == null)
            {
                Utils.LogError("Null map for CompAbilityFlyer");
                return;
            }

            if (Props.flyerDef != null)
            {
                bool isSelected = Find.Selector.IsSelected(Pawn);
                PawnFlyer pawnFlyer = PawnFlyer.MakeFlyer(Props.flyerDef, Pawn, target.Cell, Props.flightEffecter, Props.endSound);
                if (pawnFlyer != null)
                {
                    Utils.LogMessage("b");
                    FleckMaker.ThrowDustPuff(target.Cell.ToVector3Shifted() + Gen.RandomHorizontalVector(0.5f), map, 2f);
                    GenSpawn.Spawn(pawnFlyer, target.Cell, map);
                    if (isSelected)
                    {
                        Find.Selector.Select(Pawn, false, false);
                    }
                }
                Props.startSound?.PlayOneShot(Pawn);
                Props.onStartEffecter?.Spawn(target.Cell, map).Cleanup();
            }
        }*/

        public void OnJumpCompleted(IntVec3 origin, LocalTargetInfo target)
        {
            if (Props.abilityOnFinish != null)
            {
                Utils.TryDoAbility(Pawn, Props.abilityOnFinish, target); //.Thing is Pawn p && Props.abilityOnFinish.verbProperties.targetParams.canTargetPawns ? p : target.Cell);
            }
        }

        public override bool AICanTargetNow(LocalTargetInfo target) => !Pawn.IsColonistPlayerControlled;
    }
}
