using RimWorld;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace Rimpact
{
    public class HediffCompProperties_TendAndRegen : HediffCompProperties
    {
        public int intervalTicks = 300;
        public FloatRange healAmount = new(0.1f, 0.3f);
        public FloatRange tendQualityRange = new(0.3f, 0.6f);
        public bool tendAll;
        public bool healAll = true;
        public bool showExtra = true;
        public bool healOnce;
        public FleckDef fleckOnHeal;

        public HediffCompProperties_TendAndRegen()
        {
            compClass = typeof(HediffComp_TendAndRegen);
        }
    }

    [HotSwap.HotSwappable]
    public class HediffComp_TendAndRegen : HediffComp
    {
        public int tickCounter = 0;
        private bool healedOnce = false;

        public HediffCompProperties_TendAndRegen Props => (HediffCompProperties_TendAndRegen)props;

        public override string CompTipStringExtra 
        { 
            get
            {
                if (Props.showExtra)
                {
                    StringBuilder sb = new();
                    sb.Append("HealAmountRange".Translate(Props.healAmount.min.ToString("0.0"), Props.healAmount.max.ToString("0.0")));
                    sb.Append("\n");
                    sb.AppendInNewLine(Props.tendAll ? "TendsToAllInjuries".Translate() : "TendsToRandomInjuries".Translate()); 
                    sb.AppendInNewLine("TendQualityRange".Translate(Props.tendQualityRange.min * 100, Props.tendQualityRange.max * 100));
                    sb.Append("\n");
                    sb.AppendInNewLine("EffectInterval".Translate(Props.intervalTicks.TicksToSeconds(), "LetterSecond".Translate()));
                    return sb.ToString();
                }
                return null;
            } 
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            tickCounter++;
            if (tickCounter < Props.intervalTicks || healedOnce) return;

            Pawn pawn = parent.pawn;
            if (pawn.health != null)
            {
                List<Hediff> injuries = GetInjuries(pawn);
                if (injuries.Count > 0)
                {
                    var tendableInjuries = injuries.FindAll(x => x.TendableNow());
                    if (!tendableInjuries.NullOrEmpty()) 
                    {
                        if (Props.tendAll)
                        {
                            foreach (Hediff h in tendableInjuries)
                            {
                                //Log.Message("Tendable: " + h);
                                h.Tended(Props.tendQualityRange.RandomInRange, Props.tendQualityRange.max);
                            }
                        }
                        else
                        {
                            tendableInjuries.RandomElement().Tended(Props.tendQualityRange.RandomInRange, Props.tendQualityRange.max);
                        }
                    }

                    var healableInjuries = injuries.FindAll(x => x is Hediff_Injury);
                    if (!healableInjuries.NullOrEmpty())
                    {
                        if (Props.healAll)
                        {
                            foreach (Hediff h in healableInjuries)
                            {
                                //Log.Message("Healable: " + h);
                                h.Severity -= Props.healAmount.RandomInRange;
                            }
                        }
                        else
                        {
                            healableInjuries.RandomElement().Severity -= Props.healAmount.RandomInRange;
                        }
                    }
                }
            }
            tickCounter = 0;
            if (Props.fleckOnHeal != null) FleckMaker.AttachedOverlay(pawn, Props.fleckOnHeal, Vector3.zero);
            if (Props.healOnce) healedOnce = true;
        }

        public List<Hediff> GetInjuries(Pawn pawn)
        {
            List<Hediff> list = [];
            for (int i = 0; i < pawn.health.hediffSet.hediffs.Count; i++)
            {
                var h = pawn.health.hediffSet.hediffs[i];
                if (h is Hediff_Injury || h is Hediff_MissingPart)
                {
                    list.Add(h);
                }
            }
            return list;
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look(ref tickCounter, "tickCounter");
        }
    }
}
