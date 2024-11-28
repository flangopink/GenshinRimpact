using Verse;
using RimWorld;
using System.Collections.Generic;

namespace GenshinRimpact
{
    public class CompProperties_AbilityAoE : CompProperties_AbilityEffect//EffectWithDest
    {
        public EffecterDef effecterPreCast;
        public EffecterDef effecterOnTrigger;
        public DamageDef damageDef;
        public HediffDef hediffDef;
        public float hediffSeverity = 1f;
        public float damageAmount = 10f;
        public float radius = 3.9f;
        //public int applyDelayTicks;
        public int effecterPreCastTicks;
        public bool isExplosive;
        public bool isDirect;
        public bool canFriendlyFire;
        public bool onlyAffectFriendlies;

        public CompProperties_AbilityAoE() => compClass = typeof(CompAbilityAoE);
    }

    [HotSwap.HotSwappable]
    public class CompAbilityAoE : CompAbilityEffect//_WithDest
    {
        public new CompProperties_AbilityAoE Props => (CompProperties_AbilityAoE)props;

        private Pawn Pawn => parent.pawn;

        //private int ticksLeftToApply;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            Utils.DoAoEAbility(target, Pawn, parent.def, Props.damageAmount, Props.radius, Props.damageDef, Props.hediffDef, Props.hediffSeverity, Props.effecterOnTrigger, Props.isExplosive, Props.isDirect, Props.canFriendlyFire, Props.onlyAffectFriendlies, Props.screenShakeIntensity, Props.sound);
        }

        public override IEnumerable<PreCastAction> GetPreCastActions()
        {
            if (Props.effecterPreCast != null)
            {
                yield return new PreCastAction
                {
                    action = delegate (LocalTargetInfo a, LocalTargetInfo b)
                    {
                        parent.AddEffecterToMaintain(Props.effecterPreCast.Spawn(a.Cell, Pawn.Position, Pawn.Map), a.Cell, Pawn.Position, Props.effecterPreCastTicks, Pawn.MapHeld);
                    },
                    ticksAwayFromCast = Props.effecterPreCastTicks
                };
            }
        }

        /*public override void Initialize(AbilityCompProperties props)
        {
            base.Initialize(props);
            if (ticksLeftToApply <= 0) ticksLeftToApply = Props.applyDelayTicks;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref ticksLeftToApply, "ticksLeftToApply");
        }*/
    }
}