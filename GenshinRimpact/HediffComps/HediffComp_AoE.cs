using RimWorld;
using Verse;

namespace GenshinRimpact
{
    public class HediffCompProperties_AoE : HediffCompProperties
    {
        public AbilityDef abilityDef;
        public int intervalTicks = 60;
        public EffecterDef effecterAttached;
        public EffecterDef effecterEnd;
        public EffecterDef effecterOnTrigger;
        public DamageDef damageDef;
        public HediffDef hediffDef;
        public float hediffSeverity = 1f;
        public float damageAmount = 10f;
        public float radius = 3.9f;
        public bool isExplosive;
        public bool isDirect;
        public bool canFriendlyFire;
        public bool onlyAffectFriendlies;
        public float screenShakeIntensity = 1f;
        public SoundDef sound;

        public HediffCompProperties_AoE()
        {
            compClass = typeof(HediffComp_AoE);
        }
    }

    public class HediffComp_AoE : HediffComp
    {
        private int tickCounter;
        private Effecter effecter;
        //private Ability ability;

        public HediffCompProperties_AoE Props => (HediffCompProperties_AoE)props;

        public override void CompPostMake()
        {
            base.CompPostMake();
            //ability = Pawn.abilities.GetAbility(Props.abilityDef);
            effecter ??= Props.effecterAttached.SpawnAttached(Pawn, Pawn.MapHeld);
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            tickCounter++;
            effecter?.EffectTick(Pawn, Pawn);
            if (tickCounter < Props.intervalTicks) return;
            Utils.DoAoEAbility(Pawn.Position, Pawn, Props.abilityDef, Props.damageAmount, Props.radius, Props.damageDef, Props.hediffDef, Props.hediffSeverity, Props.effecterOnTrigger, Props.isExplosive, Props.isDirect, Props.canFriendlyFire, Props.onlyAffectFriendlies, Props.screenShakeIntensity, Props.sound);
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look(ref tickCounter, "tickCounter", 0);
        }
    }
}
