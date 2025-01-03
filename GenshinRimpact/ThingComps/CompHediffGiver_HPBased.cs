using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Rimpact
{
    public class CompProperties_HediffGiver_HPBased : CompProperties
    {
        public int intervalTicks = 60;
        public float range = 3.9f;
        public float hpThreshold = 0.5f;
        public HediffDef hediffUnder;
        public HediffDef hediffOver;
        public FleckDef fleckOnHeal;

        public bool highlightCells;
        public Color highlightColor = Color.white;

        public CompProperties_HediffGiver_HPBased() => compClass = typeof(CompHediffGiver_HPBased);
    }

    public class CompHediffGiver_HPBased : ThingComp
    {
        public int tickCounter = 0;
        public List<IntVec3> tmpCells = [];
        public CompProperties_HediffGiver_HPBased Props => (CompProperties_HediffGiver_HPBased)props;

        public override void CompTick()
        {
            tickCounter++;
            if (tickCounter < Props.intervalTicks) return;

            tmpCells = GenRadial.RadialCellsAround(parent.Position, Props.range, true).ToList();
            for (int i = 0; i < tmpCells.Count(); i++)
            {
                if (!tmpCells[i].InBounds(parent.Map)) continue;
                List<Thing> thingList = tmpCells[i].GetThingList(parent.Map);
                for (int j = 0; j < thingList.Count; j++)
                {
                    Thing t = thingList[j];
                    if (t is Pawn p && p.Faction.AllyOrNeutralTo(parent.Faction))
                    {
                        Utils.LogMessage(p.Faction + " - " + parent.Faction);
                        Hediff h = p.health.summaryHealth.SummaryHealthPercent < Props.hpThreshold
                                   ? (Props.hediffUnder != null ? HediffMaker.MakeHediff(Props.hediffUnder, p) : null)  
                                   : (Props.hediffOver != null ? HediffMaker.MakeHediff(Props.hediffOver, p) : null);
                        if (h != null) p.health.AddHediff(h);
                        if (Props.fleckOnHeal != null) FleckMaker.AttachedOverlay(p, Props.fleckOnHeal, Vector3.zero);
                    }
                }
            }
            tickCounter = 0;
        }

        public override void PostDraw()
        {
            if (Props.highlightCells && tmpCells.Count > 0)
            {
                GenDraw.DrawFieldEdges(tmpCells, Props.highlightColor);
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref tickCounter, "tickCounter");
        }
    }
}
