using Verse;

namespace GenshinRimpact
{
    public class Reaction_DoDamage : ElementalReaction
    {
        public override void ApplyReaction(Thing thing, Thing instigator = null)
        {
            if (Def.isExplosive) GenExplosion.DoExplosion(thing.Position, thing.Map, Def.effectRadius, Def.damageDef, instigator, (int)Def.damageAmount);
            else thing.TakeDamage(new DamageInfo(Def.damageDef, Def.damageAmount, instigator: instigator));
        }
    }
}
