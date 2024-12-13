using System.Collections.Generic;
using Verse;

namespace GenshinRimpact
{
    public class HediffCompProperties_DamageInfusion : HediffCompProperties
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
            else if (damageDef.GetModExtension<ModExt_Element>() is not ModExt_Element ext)
            {
                yield return $"{parentDef} has HediffCompProperties_DamageInfusion but is missing ModExt_Element in its damageDef";
            }
            else if (ext.element == null)
            {
                yield return $"{parentDef} has HediffCompProperties_DamageInfusion but is missing <element> in its damageDef's ModExt_Element";
            }
        }
        public HediffCompProperties_DamageInfusion() => compClass = typeof(HediffComp_DamageInfusion);
    }

    public class HediffComp_DamageInfusion : HediffComp
    {
        public HediffCompProperties_DamageInfusion Props => (HediffCompProperties_DamageInfusion)props;
    }
}
