using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace Rimpact
{
    public class HediffCompProperties_TurretGun : HediffCompProperties
    {
        public ThingDef turretDef;
        public float angleOffset;
        public bool autoAttack = true;
        public int burstDelayTicks = 30;

        //public List<PawnRenderNodeProperties> renderNodeProperties;

        public HediffCompProperties_TurretGun() => compClass = typeof(HediffComp_TurretGun);

        /*public override IEnumerable<string> ConfigErrors(HediffDef parentDef)
        {
            if (renderNodeProperties.NullOrEmpty())
            {
                yield break;
            }
            foreach (PawnRenderNodeProperties renderNodeProperty in renderNodeProperties)
            {
                if (!typeof(PawnRenderNode_TurretGun).IsAssignableFrom(renderNodeProperty.nodeClass))
                {
                    yield return "contains nodeClass which is not PawnRenderNode_TurretGun or subclass thereof.";
                }
            }
        }*/
    }

    [HotSwap.HotSwappable]
    public class HediffComp_TurretGun : HediffComp, IAttackTargetSearcher
    {
        private const int StartShootIntervalTicks = 10;

        private static readonly CachedTexture ToggleTurretIcon = new("UI/Commands/HoldFire");

        public Thing gun;

        protected int burstCooldownTicksLeft;

        protected int burstWarmupTicksLeft;

        protected LocalTargetInfo currentTarget = LocalTargetInfo.Invalid;

        private bool fireAtWill = true;

        private LocalTargetInfo lastAttackedTarget = LocalTargetInfo.Invalid;

        private int lastAttackTargetTick;

        //public float curRotation;

        public Thing Thing => parent.pawn;

        public HediffCompProperties_TurretGun Props => (HediffCompProperties_TurretGun)props;

        public Verb CurrentEffectiveVerb => AttackVerb;

        public LocalTargetInfo LastAttackedTarget => lastAttackedTarget;

        public int LastAttackTargetTick => lastAttackTargetTick;

        public CompEquippable GunCompEq => gun.TryGetComp<CompEquippable>();

        public Verb AttackVerb => GunCompEq.PrimaryVerb;

        private bool WarmingUp => burstWarmupTicksLeft > 0;

        private bool CanShoot
        {
            get
            {
                if (Thing is Pawn pawn)
                {
                    if (!pawn.Spawned || pawn.Downed || pawn.Dead || !pawn.Awake())
                    {
                        return false;
                    }
                    if (pawn.stances.stunner.Stunned)
                    {
                        return false;
                    }
                    if (TurretDestroyed)
                    {
                        return false;
                    }
                    if (pawn.IsColonyMechPlayerControlled && !fireAtWill)
                    {
                        return false;
                    }
                }
                //CompCanBeDormant compCanBeDormant = parent.TryGetComp<CompCanBeDormant>();
                //if (compCanBeDormant != null && !compCanBeDormant.Awake)
                //{
                //    return false;
                //}
                return true;
            }
        }

        public bool TurretDestroyed
        {
            get
            {
                if (Thing is Pawn pawn && AttackVerb.verbProps.linkedBodyPartsGroup != null && AttackVerb.verbProps.ensureLinkedBodyPartsGroupAlwaysUsable && PawnCapacityUtility.CalculateNaturalPartsAverageEfficiency(pawn.health.hediffSet, AttackVerb.verbProps.linkedBodyPartsGroup) <= 0f)
                {
                    return true;
                }
                return false;
            }
        }

        public bool AutoAttack => Props.autoAttack;

        public override void CompPostMake()
        {
            base.CompPostMake();
            MakeGun();
        }

        private void MakeGun()
        {
            gun = ThingMaker.MakeThing(Props.turretDef);
            UpdateGunVerbs();
        }

        private void UpdateGunVerbs()
        {
            List<Verb> allVerbs = gun.TryGetComp<CompEquippable>().AllVerbs;
            for (int i = 0; i < allVerbs.Count; i++)
            {
                Verb verb = allVerbs[i];
                verb.caster = Thing;
                verb.castCompleteCallback = delegate
                {
                    burstCooldownTicksLeft = AttackVerb.verbProps.defaultCooldownTime.SecondsToTicks();
                };
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (!CanShoot)
            {
                return;
            }
            //if (currentTarget.IsValid)
            //{
            //    curRotation = (currentTarget.Cell.ToVector3Shifted() - Thing.DrawPos).AngleFlat() + Props.angleOffset;
            //}
            AttackVerb.VerbTick();
            if (AttackVerb.state == VerbState.Bursting)
            {
                return;
            }
            if (WarmingUp)
            {
                burstWarmupTicksLeft--;
                if (burstWarmupTicksLeft == 0)
                {
                    AttackVerb.TryStartCastOn(currentTarget, surpriseAttack: false, canHitNonTargetPawns: true, preventFriendlyFire: false, nonInterruptingSelfCast: true);
                    lastAttackTargetTick = Find.TickManager.TicksGame;
                    lastAttackedTarget = currentTarget;
                }
                return;
            }
            if (burstCooldownTicksLeft > 0)
            {
                burstCooldownTicksLeft--;
            }
            if (burstCooldownTicksLeft <= 0 && Thing.IsHashIntervalTick(StartShootIntervalTicks))
            {
                currentTarget = (Thing)AttackTargetFinder.BestShootTargetFromCurrentPosition(this, TargetScanFlags.NeedThreat | TargetScanFlags.NeedAutoTargetable);
                if (currentTarget.IsValid)
                {
                    burstWarmupTicksLeft = Props.burstDelayTicks;
                }
                else
                {
                    ResetCurrentTarget();
                }
            }
        }

        private void ResetCurrentTarget()
        {
            currentTarget = LocalTargetInfo.Invalid;
            burstWarmupTicksLeft = 0;
        }

        public override IEnumerable<Gizmo> CompGetGizmos()
        {
            if (Thing is Pawn)
            {
                Command_Toggle command_Toggle = new()
                {
                    defaultLabel = "CommandToggleTurret".Translate(),
                    defaultDesc = "CommandToggleTurretDesc".Translate(),
                    isActive = () => fireAtWill,
                    icon = ToggleTurretIcon.Texture,
                    toggleAction = delegate
                    {
                        fireAtWill = !fireAtWill;
                    }
                };
                yield return command_Toggle;
            }
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look(ref burstCooldownTicksLeft, "burstCooldownTicksLeft", 0);
            Scribe_Values.Look(ref burstWarmupTicksLeft, "burstWarmupTicksLeft", 0);
            Scribe_TargetInfo.Look(ref currentTarget, "currentTarget");
            Scribe_Deep.Look(ref gun, "gun");
            Scribe_Values.Look(ref fireAtWill, "fireAtWill", defaultValue: true);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (gun == null)
                {
                    Log.Error("HediffComp_TurrentGun had null gun after loading. Recreating.");
                    MakeGun();
                }
                else
                {
                    UpdateGunVerbs();
                }
            }
        }
    }
}
