using RimWorld;
using Verse;

namespace GenshinRimpact
{
    public class CompProperties_AbilityDamageSelf : CompProperties_AbilityEffect
    {
        public DamageDef damageDef;
        public float damageAmount;

        public CompProperties_AbilityDamageSelf() => compClass = typeof(CompAbilityDamageSelf);
    }

    [HotSwap.HotSwappable]
    public class CompAbilityDamageSelf : CompAbilityEffect
    {
        public new CompProperties_AbilityDamageSelf Props => (CompProperties_AbilityDamageSelf)props;

        private Pawn Pawn => parent.pawn;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            Pawn.TakeDamage(new DamageInfo(Props.damageDef, Props.damageAmount, -1, -1, Pawn, instigatorGuilty: false));
        }
    }
}
