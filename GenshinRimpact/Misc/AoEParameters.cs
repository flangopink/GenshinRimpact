using Verse;

namespace Rimpact
{
    public class AoEParameters
    {
        public EffecterDef effecterOnTrigger;
        public DamageDef damageDef;
        public HediffDef hediffDef;
        public float hediffSeverity = 1f;
        public float damageAmount = 10f;

        public bool isDirect = true;
        public bool canFriendlyFire;
        public bool onlyAffectFriendlies;

        public bool isExplosive;
        public float explosionRadius = 3.9f;

        public AoEShapeParameters shapeParams = new();
        public AoEKnockbackParameters knockbackParams = new();

        public bool isPlunging;
        public bool atCasterPos;
    }
}
