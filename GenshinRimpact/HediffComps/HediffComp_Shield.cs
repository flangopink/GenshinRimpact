using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Rimpact
{
    public class HediffCompProperties_Shield : HediffCompProperties_Draw
    {
        //public GraphicData graphic;

        public FleckDef absorbedFleck;
        public EffecterDef brokenEffecter;

        public AbilityDef abilityOnBreak;
        public bool doAbilityOnInstigator;

        public SoundDef soundAdded;
        public SoundDef soundBroken;
        public SoundDef soundRecharging;
        public SoundDef soundEnded;

        public ElementDef elementDef;

        public Vector2 defaultScale = new(2,2);
        public Color defaultColor = Color.magenta;

        public float initialEnergy = -1f;
        public float maxEnergy = 1f;
        public float energyPerTick = 0f;
        public float energyLossPerDamage = 0.033f;
        public float energyPctOnReset = 0.2f;
        public int rechargeDelay = 180;

        public List<ElementResistance> elementResistances;

        //public bool blockRangedVerbs;

        public bool absorbMelee = true;
        public bool absorbRanged = true;
        public bool absorbExplosive = true;

        public bool allowOverkillDamage;
        public bool disappearOnBreak;
        public bool throwDust;

        public bool gizmoShowTimer;

        public HediffCompProperties_Shield()
        {
            compClass = typeof(HediffComp_Shield);
        }
        public override void ResolveReferences(HediffDef parent)
        {
            base.ResolveReferences(parent);
            /*if (breakOn == null)
            {
                breakOn = ((maxEnergy > 0f) ? new List<DamageDef> { DamageDefOf.EMP } : new List<DamageDef>());
            }*/
            graphic ??= new GraphicData
            {
                graphicClass = typeof(Graphic_Single),
                texPath = "Other/ShieldBubble",
                shaderType = ShaderTypeDefOf.Transparent,
                drawSize = defaultScale,
                color = elementDef?.color ?? defaultColor
            };
        }
    }

    [HotSwap.HotSwappable]
    [StaticConstructorOnStartup]
    public class HediffComp_Shield : HediffComp_Draw
    {
        public float energy;
        public bool useEnergy;
        protected int ticksTillReset;
        protected Vector3 impactAngleVect;
        private float lastReceivedDamage;

        public virtual HediffCompProperties_Shield Props => props as HediffCompProperties_Shield;
        public virtual bool ShieldActive => energy > 0f || !useEnergy;
        public override bool CompShouldRemove => Props.disappearOnBreak && energy <= 0f;

        public override IEnumerable<Gizmo> CompGetGizmos()
        {
            if (Pawn.Faction == Faction.OfPlayer && Find.Selector.SingleSelectedThing == Pawn)
            {
                yield return new Gizmo_HediffEnergyShieldStatus(this);
            }
            else yield return null;
        }

        public override void DrawAt(Vector3 drawPos)
        {
            drawPos.y = AltitudeLayer.MoteOverhead.AltitudeFor();
            drawPos += Props.graphic.drawOffset;
            Graphics.DrawMesh(MeshPool.plane10, Matrix4x4.TRS(drawPos, Quaternion.AngleAxis(0f, Vector3.up), new Vector3(Props.graphic.drawSize.x, 1f, Props.graphic.drawSize.y)), Graphic.MatSingleFor(base.Pawn), 0);
        }

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            useEnergy = Props.maxEnergy > 0f;
            energy = useEnergy ? (Props.initialEnergy == -1f ? Props.maxEnergy : Props.initialEnergy) : -1f;
            Props.soundAdded?.PlayOneShot(Pawn);
        }

        public virtual void PreApplyDamage(ref DamageInfo dinfo, ref bool absorbed)
        {
            if (absorbed || !ShieldActive)
            {
                return;
            }
            /*if (Props.breakOn.Contains(dinfo.Def))
            {
                Break();
                return;
            }*/
            bool gotDamaged = (Props.absorbMelee && !dinfo.Def.isRanged && !dinfo.Def.isExplosive)
                           || (Props.absorbRanged && dinfo.Def.isRanged)
                           || (Props.absorbExplosive && dinfo.Def.isExplosive);

            if (gotDamaged && AbsorbDamage(ref dinfo))
            {
                absorbed = true;
                impactAngleVect = Vector3Utility.HorizontalVectorFromAngle(dinfo.Angle);
                Vector3 loc = Pawn.TrueCenter() + impactAngleVect.RotatedBy(180f) * 0.5f;
                float dmgScale = Mathf.Min(10f, 2f + lastReceivedDamage / 10f);
                FleckMaker.Static(loc, Pawn.Map, Props.absorbedFleck ?? FleckDefOf.ExplosionFlash, dmgScale);
                if (Props.throwDust)
                {
                    for (int i = 0; i < dmgScale; i++)
                    {
                        FleckMaker.ThrowDustPuff(loc, Pawn.Map, Rand.Range(0.8f, 1.2f));
                    }
                }
            }
            /*bool flag2 = false;
            switch (Props.damageOnAttack)
            {
                case AttackType.Melee:
                    flag2 = !dinfo.Def.isRanged;
                    break;
                case AttackType.Ranged:
                    flag2 = dinfo.Def.isRanged || dinfo.Def.isExplosive;
                    break;
                case AttackType.Both:
                    flag2 = true;
                    break;
            }
            if (flag2 && dinfo.Instigator != null)
            {
                ApplyDamage(dinfo);
            }*/
        }

        /*protected virtual void ApplyDamage(DamageInfo dinfo)
        {
            dinfo.Instigator.TakeDamage(new DamageInfo(Props.damageType, (float)Props.damageAmount, Props.armorPenetration, -1f, (Thing)null, (BodyPartRecord)null, (ThingDef)null, DamageInfo.SourceCategory.ThingOrUnknown, (Thing)null, true, true));
        }*/

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look(ref energy, "energy", 0f);
            Scribe_Values.Look(ref useEnergy, "useEnergy", defaultValue: false);
            Scribe_Values.Look(ref ticksTillReset, "ticksTillReset", 0);
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (useEnergy)
            {
                if (ticksTillReset > 0)
                {
                    ticksTillReset--;
                    if (ticksTillReset <= 0)
                    {
                        Reset();
                    }
                }
                else if (energy <= Props.maxEnergy)
                {
                    energy += Props.energyPerTick;
                }
            }
            /*if (ShieldActive && sustainer == null)
            {
                sustainer = Props.sustainer?.TrySpawnSustainer(base.Pawn);
            }
            else
            {
                sustainer.Maintain();
            }*/
        }

        protected virtual void Break()
        {
            Props.soundBroken?.PlayOneShot(Pawn);
            Props.brokenEffecter?.SpawnAttached(Pawn, Pawn.Map).Cleanup();
            if (Props.throwDust)
            {
                for (int i = 0; i < 6; i++)
                {
                    FleckMaker.ThrowDustPuff(Pawn.TrueCenter() + Vector3Utility.HorizontalVectorFromAngle(Rand.Range(0, 360)) * Rand.Range(0.3f, 0.6f), Pawn.Map, Rand.Range(0.8f, 1.2f));
                } 
            }
            energy = 0f;
            if (!Props.disappearOnBreak)
            {
                ticksTillReset = Props.rechargeDelay;
                if (ticksTillReset <= 0)
                {
                    Reset();
                }
            }
        }

        protected virtual bool AbsorbDamage(ref DamageInfo dinfo)
        {
            if (useEnergy)
            {
                float receivedDamage = dinfo.Amount;
                if (dinfo.Def.GetModExtension<ModExt_Element>() is ModExt_Element ext)
                {
                    foreach (var res in Props.elementResistances)
                    {
                        if (res.element == ext.element)
                            receivedDamage *= res.factor;
                    }
                }
                lastReceivedDamage = receivedDamage;

                float num = receivedDamage * Props.energyLossPerDamage;
                if (num < energy)
                {
                    energy -= num;
                    dinfo.SetAmount(0f);
                    return true;
                }
                Break();
                if (Props.abilityOnBreak != null && !parent.pawn.DeadOrDowned)
                {
                    Utils.TryDoAbility(parent.pawn, Props.abilityOnBreak, Props.doAbilityOnInstigator ? dinfo.Instigator : parent.pawn.Position);
                }
                if (Props.allowOverkillDamage)
                {
                    dinfo.SetAmount(receivedDamage - energy / Props.energyLossPerDamage);
                    return false;
                }
                else return true;
            }
            dinfo.SetAmount(0f);
            return true;
        }

        protected virtual void Reset()
        {
            ticksTillReset = 0;
            energy = Props.maxEnergy * Props.energyPctOnReset;
            Props.soundRecharging?.PlayOneShot(Pawn);
            FleckMaker.ThrowLightningGlow(Pawn.TrueCenter(), Pawn.Map, 3f);
        }

        public override void CompPostPostRemoved()
        {
            Props.soundEnded?.PlayOneShot(Pawn);
            base.CompPostPostRemoved();
        }
    }
}