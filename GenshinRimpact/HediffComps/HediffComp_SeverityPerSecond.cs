using UnityEngine;
using Verse;

namespace GenshinRimpact
{
    public class HediffCompProperties_SeverityPerSecond : Verse.HediffCompProperties_SeverityPerSecond
    {
        public bool showSecondsToRecover = true;

        public HediffCompProperties_SeverityPerSecond()
        {
            compClass = typeof(HediffComp_SeverityPerSecond);
        }
    }

    public class HediffComp_SeverityPerSecond : Verse.HediffComp_SeverityPerSecond
    {
        public HediffCompProperties_SeverityPerSecond Props => (HediffCompProperties_SeverityPerSecond)props;

        public override string CompLabelInBracketsExtra // you lazy assholes.
        {
            get
            {
                if (Props.showSecondsToRecover && severityPerSecond < 0f)
                {
                    return Mathf.RoundToInt(parent.Severity / Mathf.Abs(severityPerSecond)) + "LetterSecond".Translate();
                }
                return null;
            }
        }

        public override string CompTipStringExtra
        {
            get
            {
                if (Props.showSecondsToRecover && severityPerSecond < 0f)
                {
                    return "SecondsToRecover".Translate((parent.Severity / Mathf.Abs(severityPerSecond)).ToString("0.0")).Resolve();
                }
                return null;
            }
        }

    }
}
