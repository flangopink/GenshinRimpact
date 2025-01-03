using Verse;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using static HarmonyLib.Code;
using System.Linq;

namespace Rimpact
{
    public class CompProperties_AbilityAoE : CompProperties_AbilityEffect//EffectWithDest
    {
        public AoEParameters aoeProperties;

        public EffecterDef effecterPreCast;
        public int effecterPreCastTicks;

        /*
        public EffecterDef effecterOnTrigger;
        public DamageDef damageDef;
        public HediffDef hediffDef;
        public float hediffSeverity = 1f;
        public float damageAmount = 10f;
        public int effecterPreCastTicks;
        //public int applyDelayTicks;

        public bool isDirect = true;
        public bool canFriendlyFire;
        public bool onlyAffectFriendlies;

        public bool isExplosive;
        public float explosionRadius = 3.9f;

        public AoEShapeParameters shapeParams;
        public AoEKnockbackParameters knockbackParams;

        public bool isPlunging;
        */

        public CompProperties_AbilityAoE() => compClass = typeof(CompAbilityAoE);

        public override IEnumerable<string> ConfigErrors(AbilityDef parentDef)
        {
            foreach (var error in base.ConfigErrors(parentDef))
                yield return error;
            if (aoeProperties == null)
                yield return $"{parentDef} has CompProperties_AbilityAoE but no <aoeProperties> set";
            else if (aoeProperties.shapeParams == null)
                yield return $"{parentDef} has CompProperties_AbilityAoE but no <shapeParams> set in <aoeProperties>";
        }
    }

    [HotSwap.HotSwappable]
    public class CompAbilityAoE : CompAbilityEffect//_WithDest
    {
        public new CompProperties_AbilityAoE Props => (CompProperties_AbilityAoE)props;

        public List<IntVec3> tmpCells = [];

        private Pawn Pawn => parent.pawn;
        private AoEParameters Params => Props.aoeProperties;

        //private int ticksLeftToApply;
        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            Utils.DoAoEAbility(target, Pawn, parent.def, Params.shapeParams, Params.damageAmount, Params.damageDef, Params.hediffDef, Params.hediffSeverity, Params.effecterOnTrigger, Params.isDirect, Params.canFriendlyFire, Params.onlyAffectFriendlies, Params.isExplosive, Params.explosionRadius, Props.screenShakeIntensity, Props.sound, Params.knockbackParams);
        }

        public override IEnumerable<PreCastAction> GetPreCastActions()
        {
            if (Props.effecterPreCast != null)
            {
                yield return new PreCastAction
                {
                    action = delegate (LocalTargetInfo a, LocalTargetInfo b)
                    {
                        parent.AddEffecterToMaintain(Props.effecterPreCast.Spawn(a.Cell, Pawn.Position, Pawn.Map), a.Cell, Pawn.Position, Props.effecterPreCastTicks, Pawn.MapHeld);
                    },
                    ticksAwayFromCast = Props.effecterPreCastTicks
                };
            }
        }

        public override void DrawEffectPreview(LocalTargetInfo target)
        {
            switch (Params.shapeParams.shape)
            {
                case AoEShape.Radial:
                    GenDraw.DrawFieldEdges(GenRadial.RadialCellsAround(target.Cell, Params.shapeParams.radius, true).ToList());
                    break;
                case AoEShape.HalfRadial:
                    GenDraw.DrawFieldEdges(Utils.GetHalfCircleCells(ref tmpCells, Pawn.Position, target.Cell, Pawn.MapHeld, Params.shapeParams.radius, Params.shapeParams.angleRad, false));
                    break;
                case AoEShape.HalfRadialFilled: 
                    GenDraw.DrawFieldEdges(Utils.GetHalfCircleCells(ref tmpCells, Pawn.Position, target.Cell, Pawn.MapHeld, Params.shapeParams.radius, Params.shapeParams.angleRad, true));
                    break;
                case AoEShape.Cone: 
                    GenDraw.DrawFieldEdges(Utils.ConeAffectedCells(ref tmpCells, Pawn.Position, target.Cell, Pawn.MapHeld, Params.shapeParams.radius, Params.shapeParams.coneAngleDeg, Params.shapeParams.coneWidth));
                    break;
                case AoEShape.Rectangular:
                    Utils.LogErrorOnce("CompAbilityAoE: fix rectangle aoe please.", 69697760);
                    break;
                default:
                    Utils.LogErrorOnce("CompAbilityAoE does not have an AoEShape.", 69697770);
                    break;
            }
            if (Params.knockbackParams != null && Params.knockbackParams.showLandingCells)
                GenDraw.DrawFieldEdges(Utils.GetKnockbackCells(Pawn.Position, target.Cell, Pawn.MapHeld, Params.knockbackParams), Color.cyan);
        }
        public override bool AICanTargetNow(LocalTargetInfo target) => !parent.pawn.IsColonistPlayerControlled;

        /*public override void Initialize(AbilityCompProperties props)
        {
            base.Initialize(props);
            if (ticksLeftToApply <= 0) ticksLeftToApply = Props.applyDelayTicks;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref ticksLeftToApply, "ticksLeftToApply");
        }*/
    }
}