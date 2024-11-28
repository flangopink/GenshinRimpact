using Verse;

namespace GenshinRimpact
{
    public interface IElementalReaction
    {
        public ElementalReactionDef Def { get; set; }
        void ApplyReaction(Thing thing, Thing instigator = null);
    }

    public class ElementalReaction : IElementalReaction
    {
        public ElementalReactionDef Def { get; set; }
        public virtual void ApplyReaction(Thing thing, Thing instigator = null)
        {
            throw new System.NotImplementedException();
        }
    }
}
