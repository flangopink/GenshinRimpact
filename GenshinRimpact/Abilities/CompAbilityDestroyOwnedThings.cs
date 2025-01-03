using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Rimpact
{
    public class CompProperties_AbilityDestroyOwnedThings : CompProperties_AbilityEffect
    {
        public AbilityDef ownerAbilityDef;
        public bool triggerExplodeComp;
        public bool allAbilities;
        public CompProperties_AbilityDestroyOwnedThings() => compClass = typeof(CompAbilityDestroyOwnedThings);
    }

    [HotSwap.HotSwappable]
    public class CompAbilityDestroyOwnedThings : CompAbilityEffect // Use this for destroying Keqing teleport
    {
        public new CompProperties_AbilityDestroyOwnedThings Props => (CompProperties_AbilityDestroyOwnedThings)props;
        public HashSet<Thing> spawnedThings = [];

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            CompAbilitySpawnWithOwner comp = null;
            if (Props.allAbilities)
            {
                var abs = parent.pawn.abilities.abilities;
                for (int i = 0; i < abs.Count; i++)
                {
                    for (int j = 0; j < abs[i].comps.Count; j++)
                    {
                        if (abs[i].comps[j] is CompAbilitySpawnWithOwner c)
                        {
                            comp = c;
                            break;
                        }
                    }
                    if (comp != null)
                    {
                        DestroyOwnedThings(comp);
                    }
                }
            }
            else
            {
                if (Props.ownerAbilityDef == null) 
                {
                    Utils.LogError("Used CompAbilityDestroyOwnedThings with no <ownerAbilityDef> or <allAbilities> set");
                    return; 
                }
                var ab = parent.pawn.abilities.GetAbility(Props.ownerAbilityDef);
                for (int j = 0; j < ab.comps.Count; j++)
                {
                    if (ab.comps[j] is CompAbilitySpawnWithOwner c)
                    {
                        comp = c;
                        break;
                    }
                }
                DestroyOwnedThings(comp);
            }
            if (comp == null) Utils.LogError(parent.pawn.LabelCap + " does not have any CompAbilitySpawnWithOwner");
        }

        private void DestroyOwnedThings(CompAbilitySpawnWithOwner comp)
        {
            HashSet<Thing> toDestroy = [];
            foreach (Thing t in comp.spawnedThings)
            {
                if (Props.triggerExplodeComp)
                {
                    var explodeComp = t.TryGetComp<CompExplodeOnDestroyed>();
                    if (explodeComp != null)
                        explodeComp.shouldExplode = true;
                }
                toDestroy.Add(t);
            }
            foreach (Thing t in toDestroy) t.Destroy();
        }
        public override bool AICanTargetNow(LocalTargetInfo target) => !parent.pawn.IsColonistPlayerControlled;
    }
}
