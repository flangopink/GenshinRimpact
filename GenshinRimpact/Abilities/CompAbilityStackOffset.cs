using Verse;
using RimWorld;
using System.Collections.Generic;

namespace Rimpact
{
    public class CompProperties_AbilityStackOffset : CompProperties_AbilityEffect
    {
        public string stackName;
        public int amount = 1;
        public bool isGiver;
        public bool cantUseWhenEmpty = true;
        public bool requiresExactAmount;

        public CompProperties_AbilityStackOffset() => compClass = typeof(CompAbilityStackOffset);
    }

    [HotSwap.HotSwappable]
    public class CompAbilityStackOffset : CompAbilityEffect
    {
        public new CompProperties_AbilityStackOffset Props => (CompProperties_AbilityStackOffset)props;

        private Ability shab;
        private CompAbilityStackHandler sh;

        public override void Initialize(AbilityCompProperties props)
        {
            base.Initialize(props);
            if (shab == null || sh == null)
            {
                bool found = false;
                List<Ability> abs = parent.pawn.abilities?.abilities;
                List<AbilityComp> abcomps;
                for (int i = 0; i < abs.Count; i++)
                {
                    if (found) break;
                    abcomps = abs[i].comps;
                    for (int j = 0; j < abcomps.Count; j++)
                    {
                        if (abcomps[j] is CompAbilityStackHandler cash && cash.Props.stackName == Props.stackName)
                        {
                            shab = abs[i];
                            sh = cash;
                            found = true;
                            Utils.LogMessage("Got StackHandler " + sh + " for ability " + parent);
                            break;
                        }
                    }
                }
                if (!found) Utils.LogError("Failed to find a StackHandler for ability " + parent);
            }
        }

        public override void PostApplied(List<LocalTargetInfo> targets, Map map)
        {
            base.PostApplied(targets, map);
            var prev = sh.currentStacks;
            sh.OffsetStacks(Props.amount);
            var now = sh.currentStacks;
            Utils.LogMessage(Props.stackName + " stacks changed: " + prev + " -> " + now);
        }

        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            if (Props.cantUseWhenEmpty && sh.currentStacks == 0)
            {
                Messages.Message("GR_CantUseAbilityNotEnoughStacks".Translate(Props.stackName, Props.amount), MessageTypeDefOf.RejectInput, false);
                return false;
            }
            else if (Props.requiresExactAmount && sh.currentStacks - Props.amount < 0)
            {
                Messages.Message("GR_CantUseAbilityNotEnoughStacks_NeedAmount".Translate(Props.stackName, Props.amount), MessageTypeDefOf.RejectInput, false);
                return false;
            }
            return base.Valid(target, throwMessages);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_References.Look(ref shab, "shab");
        }
        public override bool AICanTargetNow(LocalTargetInfo target) => !parent.pawn.IsColonistPlayerControlled && Valid(target);
    }
}
