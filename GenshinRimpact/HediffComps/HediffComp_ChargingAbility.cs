using RimWorld;
using UnityEngine;
using Verse;

namespace Rimpact
{
    public class HediffCompProperties_ChargingAbility : HediffCompProperties
    {
        public AbilityDef abilityDef;
        public int timerTicks = -1;
        public int maxCharge;
        public bool showCharge;
        public bool cancelOnTimerEnd;

        public HediffCompProperties_ChargingAbility()
        {
            compClass = typeof(HediffComp_ChargingAbility);
        }
    }

    public class HediffComp_ChargingAbility : HediffComp
    {
        public HediffCompProperties_ChargingAbility Props => (HediffCompProperties_ChargingAbility)props;

        public float currentCharge;
        private int timer;
        private bool doTimer;

        public override void CompPostMake()
        {
            base.CompPostMake();
            doTimer = Props.timerTicks <= 0;
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);

            if (currentCharge == Props.maxCharge || doTimer && timer >= Props.timerTicks)
            {
                Utils.TryDoAbility(Pawn, Props.abilityDef, new LocalTargetInfo(Pawn));
            }
            if (doTimer) timer++;
        }

        public override string CompLabelInBracketsExtra
        {
            get
            {
                if (Props.showCharge)
                {
                    return Mathf.RoundToInt(currentCharge) + " " + "GR_Charge".Translate();
                }
                return null;
            }
        }
        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look(ref timer, "timer");
            Scribe_Values.Look(ref currentCharge, "currentCharge");
        }
    }
}
