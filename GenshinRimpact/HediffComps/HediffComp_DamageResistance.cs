using System.Collections.Generic;
using Verse;

namespace GenshinRimpact
{
    public class HediffCompProperties_DamageResistance : HediffCompProperties   // ignore it for now
    {
        public DamageDef damageDef;
        public float damageMultiplier = 1f;
        public float extraDamageAmount = -1f;

        public bool isExtra;
        public bool doMelee = true;
        public bool doRanged = true;

        public override IEnumerable<string> ConfigErrors(HediffDef parentDef)
        {
            foreach (var e in base.ConfigErrors(parentDef)) yield return e;
            if (damageDef == null)
            {
                yield return $"{parentDef} has HediffCompProperties_DamageInfusion but is missing a damageDef";
            }
        }
        public HediffCompProperties_DamageResistance() => compClass = typeof(HediffComp_DamageResistance);
    }

    public class HediffComp_DamageResistance : HediffComp
    {
        public HediffCompProperties_DamageResistance Props => (HediffCompProperties_DamageResistance)props;

        public override void CompPostMake()
        {
            parent.Severity = Props.damageMultiplier > 1f ? 1.1f : 0.9f; // >1 = Vulnerability, <1 = Resistance
        }

        public override string CompLabelInBracketsExtra => Props.damageDef.LabelCap + ", x" + Props.damageMultiplier;
    }
}
