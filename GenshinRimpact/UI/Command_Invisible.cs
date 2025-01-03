using RimWorld;
using Verse;

namespace Rimpact
{
    public class Command_Invisible(Ability ability, Pawn pawn) : Command_Ability(ability, pawn)
    {
        public override bool Visible => false;
    }
}
