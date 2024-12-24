using System.Collections.Generic;
using Verse;

namespace GenshinRimpact
{
    public class HediffCompProperties_VisionTipStringExtra : HediffCompProperties
    {
        public VisionDef visionDef;

        public HediffCompProperties_VisionTipStringExtra()
        {
            compClass = typeof(HediffComp_VisionTipStringExtra);
        }

        public override IEnumerable<string> ConfigErrors(HediffDef parentDef)
        {
            foreach (var err in base.ConfigErrors(parentDef)) yield return err;
            if (visionDef == null) yield return "null visionDef in HediffCompProperties_VisionTipStringExtra of " + parentDef;
        }
    }

    [HotSwap.HotSwappable]
    public class HediffComp_VisionTipStringExtra : HediffComp
    {
        public HediffCompProperties_VisionTipStringExtra Props => (HediffCompProperties_VisionTipStringExtra)props;

        private string traitLabel;
        private string TraitLabel => traitLabel ??= Props.visionDef.trait.DataAtDegree(Props.visionDef.traitDegree).LabelCap;

        private string str;
        private string Str => str ??= $" {"Traits".Translate()}:\n  - " + TraitLabel;

        public override string CompTipStringExtra => Str;
    }
}
