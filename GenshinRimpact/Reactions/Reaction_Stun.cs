using Verse;

namespace Rimpact
{
    public class Reaction_Stun : ElementalReaction
    {
        public override void ApplyReaction(Thing thing, Thing instigator = null)
        {
            if (thing is Pawn pawn && !pawn.Dead)
            {
                pawn.stances.stunner.StunFor(Def.durationTicks, instigator);
                if (Def.isEffecterMaintained) Def.targetEffecter?.SpawnMaintained(pawn, pawn.MapHeld);
                else Def.targetEffecter?.SpawnAttached(pawn, pawn.MapHeld);
            }
        }
    }
}
