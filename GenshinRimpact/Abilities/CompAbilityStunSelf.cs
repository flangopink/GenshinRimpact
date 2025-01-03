using RimWorld;
using Verse;

namespace Rimpact
{
    public class CompProperties_AbilityStunSelf : CompProperties_AbilityEffect
    {
        public int stunTicks = 60;
        public bool showMote;

        public CompProperties_AbilityStunSelf() => compClass = typeof(CompAbilityStunSelf);
    }

    [HotSwap.HotSwappable]
    public class CompAbilityStunSelf : CompAbilityEffect
    {
        public new CompProperties_AbilityStunSelf Props => (CompProperties_AbilityStunSelf)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            parent.pawn.stances.stunner.StunFor(Props.stunTicks, parent.pawn, false, Props.showMote);
        }
        public override bool AICanTargetNow(LocalTargetInfo target) => !parent.pawn.IsColonistPlayerControlled;
    }
}
