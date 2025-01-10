using RimWorld;
using Verse;

namespace Rimpact 
{
    public class Verb_CastAbilityJumpExt : Verb_CastAbilityJump
    {
        public override ThingDef JumpFlyerDef => verbProps.spawnDef ?? base.JumpFlyerDef;
    }
}
