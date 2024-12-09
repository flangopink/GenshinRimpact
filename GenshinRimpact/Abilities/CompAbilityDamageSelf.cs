using RimWorld;
using Verse;

namespace GenshinRimpact
{
    public class CompProperties_AbilityDamageSelf : CompProperties_AbilityEffect
    {
        public DamageDef damageDef;
        public float damageAmount = 10;
        public float armorPenetration = 9999; // Guaranteed hit, no deflection off your panties

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
            var dresult = Pawn.TakeDamage(new DamageInfo(Props.damageDef, Props.damageAmount, Props.armorPenetration, -1, instigator: Pawn, intendedTarget: Pawn, instigatorGuilty: false));
            BattleLogEntry_DamageTakenAbility battleLog = new(Pawn, RulePackDefOf.Event_AbilityUsed, parent.def, Pawn);
            Find.BattleLog.Add(battleLog);
            dresult.AssociateWithLog(battleLog);
        }
    }
}
