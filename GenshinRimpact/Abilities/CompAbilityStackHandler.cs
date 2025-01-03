using Verse;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;

namespace Rimpact
{
    public class CompProperties_AbilityStackHandler : AbilityCompProperties
    {
        //public float initialStacks;
        public string stackName = "UNNAMED"; // Grimheart, etc.
        public int maxStacks = 3;
        public CompProperties_AbilityStackHandler() => compClass = typeof(CompAbilityStackHandler);
    }
    public class CompAbilityStackHandler : AbilityComp
    {
        public CompProperties_AbilityStackHandler Props => (CompProperties_AbilityStackHandler)props;

        public int currentStacks;
        public int maxStacks;

        public override void Initialize(AbilityCompProperties props)
        {
            base.Initialize(props);
            maxStacks = Props.maxStacks;
        }

        public float CurrentStacksPercentage
        {
            get => (float)currentStacks / maxStacks; // 2.0 / 6 = 0.33
        }

        public override bool CanCast => currentStacks <= 0;

        public void OffsetStacks(int offset)
        {
            currentStacks = Mathf.Clamp(currentStacks + offset, 0, maxStacks);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            yield return new Gizmo_AbilityStacks(parent);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref currentStacks, "currentStacks");
            //Scribe_Values.Look(ref maxStacks, "maxStacks");
        }
    }
}
