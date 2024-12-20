using RimWorld;
using System.Collections.Generic;
using Verse;

namespace GenshinRimpact
{
    public class HediffCompProperties_AoE : HediffCompProperties
    {
        public AoEParameters aoeProperties;

        public AbilityDef abilityDef;
        public int intervalTicks = 60;
        public EffecterDef effecterAttached;
        public EffecterDef effecterEnd;

        public float screenShakeIntensity = 1f;
        public SoundDef sound;

        public HediffCompProperties_AoE() => compClass = typeof(HediffComp_AoE);
        public override IEnumerable<string> ConfigErrors(HediffDef parentDef)
        {
            foreach (var error in base.ConfigErrors(parentDef))
                yield return error;
            if (aoeProperties == null)
                yield return $"{parentDef} has CompProperties_AbilityAoE but no <aoeProperties> set";
            else if (aoeProperties.shapeParams == null)
                yield return $"{parentDef} has CompProperties_AbilityAoE but no <shapeParams> set in <aoeProperties>";
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
            var parms = Props.aoeProperties;
            Utils.DoAoEAbility(Pawn.Position, Pawn, Props.abilityDef, parms.shapeParams, parms.damageAmount, parms.damageDef, parms.hediffDef, parms.hediffSeverity, parms.effecterOnTrigger, parms.isDirect, parms.canFriendlyFire, parms.onlyAffectFriendlies, parms.isExplosive, parms.explosionRadius, Props.screenShakeIntensity, Props.sound, parms.knockbackParams);
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look(ref tickCounter, "tickCounter", 0);
        }
    }
}
