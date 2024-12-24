using System.Collections.Generic;
using System.Text;
using Verse;

namespace GenshinRimpact
{
    public class HediffCompProperties_DamageMultiplier : HediffCompProperties   // ignore it for now
    {
        public List<DamageMultiplier> damageMultipliers;

        public override IEnumerable<string> ConfigErrors(HediffDef parentDef)
        {
            foreach (var e in base.ConfigErrors(parentDef)) yield return e;
            if (damageMultipliers.NullOrEmpty())
            {
                yield return $"{parentDef} has HediffCompProperties_DamageMultiplier but <damageMultipliers> is null or empty";
            }
        }
        public HediffCompProperties_DamageMultiplier() => compClass = typeof(HediffComp_DamageMultiplier);
    }

    [HotSwap.HotSwappable]
    public class HediffComp_DamageMultiplier : HediffComp
    {
        public HediffCompProperties_DamageMultiplier Props => (HediffCompProperties_DamageMultiplier)props;

        private string str;
        private string Str
        {
            get
            {
                if (str == null)
                {

                    StringBuilder sb = new();
                    sb.AppendLine("\n " + "GR_DamageMultipliers".Translate() + ":");
                    foreach (var dmg in Props.damageMultipliers)
                    {
                        sb.AppendLine($"  - {dmg.damageDef.LabelCap}: x{dmg.multiplier.ToStringPercent()}");
                    }
                    str = sb.ToString();
                }
                return str;
            }
        }

        public override string CompTipStringExtra => Str;
    }
}
