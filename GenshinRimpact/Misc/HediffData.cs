using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Rimpact
{
    /*public class HediffData : IExposable
    {
        public int id;
        public string defName;
        public string label;
        public string description;
        //public Color color;
        public HediffStageData stage;
        public VisionDef visionDef;
        public bool extraString;

        public void ExposeData()
        {
            Scribe_Values.Look(ref id, "id");
            Scribe_Values.Look(ref defName, "defName");
            Scribe_Values.Look(ref label, "label");
            Scribe_Values.Look(ref description, "description");
            Scribe_Deep.Look(ref stage, "stage");
            Scribe_Defs.Look(ref visionDef, "visionDef");
            Scribe_Values.Look(ref extraString, "extraString");
        }
        public override string ToString()
        {
            return $"HediffData{id}: {defName}, {label}, {stage}, {visionDef}";
        }
    }*/

    public class HediffStageData : IExposable
    {
        public Pawn pawn;
        public List<HediffDataStatModifier> statOffsets = [];
        public List<HediffDataStatModifier> statFactors = [];
        public VisionDef vision;
        public TraitDef trait;
        public int traitDegree;

        public HediffStageData() { }
        public HediffStageData(Pawn p, HediffStage hs, TraitDef t, int tdeg = 0, VisionDef v = null)
        {
            pawn = p;
            vision = v;
            trait = t;
            traitDegree = tdeg;
            for (int i = 0; i < hs.statOffsets.Count; i++)
            {
                statOffsets.Add(new() { stat = hs.statOffsets[i].stat, value = hs.statOffsets[i].value });
            }
            for (int i = 0; i < hs.statFactors.Count; i++)
            {
                statOffsets.Add(new() { stat = hs.statFactors[i].stat, value = hs.statFactors[i].value });
            }
        }

        public HediffStage ToHediffStage()
        {
            HediffStage stage = new()
            {
                statOffsets = [],
                statFactors = []
            };
            for (int i = 0; i < statOffsets.Count; i++)
            {
                stage.statOffsets.Add(statOffsets[i].ToStatModifier());
            }
            for (int i = 0; i < statFactors.Count; i++)
            {
                stage.statFactors.Add(statFactors[i].ToStatModifier());
            }
            return stage;
        }

        public override string ToString()
        {
            return $"Pawn: {pawn}, Vision: {vision}, Trait: {trait.DataAtDegree(traitDegree)}, {statOffsets.Count} Offsets: {statOffsets.ToStringSafeEnumerable()}, {statFactors.Count} Factors: {statFactors.ToStringSafeEnumerable()}";
        }

        public void ExposeData()
        {
            Scribe_References.Look(ref pawn, "pawn");
            Scribe_Collections.Look(ref statOffsets, "statOffsets", LookMode.Deep);
            Scribe_Collections.Look(ref statFactors, "statFactors", LookMode.Deep);
            Scribe_Defs.Look(ref vision, "vision");
            Scribe_Defs.Look(ref trait, "trait");
            Scribe_Values.Look(ref traitDegree, "traitDegree");
        }
    }
    public class HediffDataStatModifier : IExposable
    {
        public StatDef stat;
        public float value;

        public StatModifier ToStatModifier() => new() { stat = stat, value = value };

        public override string ToString()
        {
            return $"Stat: {stat}, Value: {value}";
        }

        public void ExposeData()
        {
            Scribe_Defs.Look(ref stat, "stat");
            Scribe_Values.Look(ref value, "value");
        }
    }
}
