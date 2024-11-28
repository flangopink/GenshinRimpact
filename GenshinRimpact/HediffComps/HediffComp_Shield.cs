using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse.AI;
using Verse;
using Verse.Sound;

namespace GenshinRimpact
{
    public class HediffCompProperties_Shield : HediffCompProperties
    {
        public float EnergyShieldEnergyMax = 1f;
        public float EnergyShieldRechargeRate = 0f;

        public float minDrawSize = 1.2f;
        public float maxDrawSize = 1.55f;

        public float DrawX = 1f;
        public float DrawY = 1f;

        public bool blockRangedVerbs;

        public bool absorbMelee = true;
        public bool absorbRanged = true;

        public bool brokenByEMP;

        public bool disappearOnBreak;

        public HediffCompProperties_Shield()
        {
            compClass = typeof(HediffComp_Shield);
        }
    }

    [StaticConstructorOnStartup]
    public class HediffComp_Shield : HediffComp
    {
        [StaticConstructorOnStartup]
        public class Gizmo_HediffEnergyShieldStatus : Gizmo
        {
            public HediffComp_Shield shield;

            private static readonly Texture2D FullShieldBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.24f));

            private static readonly Texture2D EmptyShieldBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);

            public Gizmo_HediffEnergyShieldStatus()
            {
                base.Order = -100f;
            }

            public override float GetWidth(float maxWidth)
            {
                return 140f;
            }

            public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
            {
                Rect overRect = new(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
                Find.WindowStack.ImmediateWindow(984688, overRect, WindowLayer.GameUI, delegate
                {
                    Rect val = overRect.AtZero().ContractedBy(6f);
                    Rect rect = val;
                    rect.height = overRect.height / 2f;
                    Text.Font = GameFont.Tiny;
                    Widgets.Label(rect, shield.parent.LabelCap);
                    Rect rect2 = val;
                    rect2.yMin = overRect.height / 2f;
                    float fillPercent = shield.Energy / Mathf.Max(1f, shield.EnergyMax);
                    Widgets.FillableBar(rect2, fillPercent, FullShieldBarTex, EmptyShieldBarTex, doBorder: false);
                    Text.Font = GameFont.Small;
                    Text.Anchor = TextAnchor.MiddleCenter;
                    Widgets.Label(rect2, (shield.Energy * 100f).ToString("F0") + " / " + (shield.EnergyMax * 100f).ToString("F0"));
                    Text.Anchor = TextAnchor.UpperLeft;
                });
                return new GizmoResult(GizmoState.Clear);
            }
        }

        private Gizmo_HediffEnergyShieldStatus _CompEnergyShieldStatus;

        public float energy;

        public int ticksToReset = -1;

        public int lastKeepDisplayTick = -9999;

        public Vector3 impactAngleVect;

        public int lastAbsorbDamageTick = -9999;

        public const float MaxDamagedJitterDist = 0.05f;

        public const int JitterDurationTicks = 8;

        public int StartingTicksToReset = 3200;

        public float EnergyOnReset = 0.2f;

        public float EnergyLossPerDamage = 0.033f;

        public int KeepDisplayingTicks = 1000;

        //public float ApparelScorePerEnergyMax = 0.25f;

        public static readonly Material BubbleMat = MaterialPool.MatFrom("Other/ShieldBubble", ShaderDatabase.Transparent);

        public virtual HediffCompProperties_Shield Props => props as HediffCompProperties_Shield;

        public virtual float EnergyMax => Props.EnergyShieldEnergyMax;

        public virtual float EnergyGainPerTick => Props.EnergyShieldRechargeRate / 60f;

        public float Energy => energy;

        public override bool CompShouldRemove => Props.disappearOnBreak && energy <= 0;

        public virtual ShieldState ShieldState
        {
            get
            {
                if (ticksToReset > 0)
                {
                    return ShieldState.Resetting;
                }
                return ShieldState.Active;
            }
        }

        public virtual bool ShouldDisplay
        {
            get
            {
                Pawn pawn = base.Pawn;
                return pawn.Spawned && !pawn.Dead && !pawn.Downed && (pawn.InAggroMentalState || pawn.Drafted || (pawn.Faction.HostileTo(Faction.OfPlayer) && !pawn.IsPrisoner) || Find.TickManager.TicksGame < lastKeepDisplayTick + KeepDisplayingTicks);
            }
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look(ref energy, "energy", 0f);
            Scribe_Values.Look(ref ticksToReset, "ticksToReset", -1);
            Scribe_Values.Look(ref lastKeepDisplayTick, "lastKeepDisplayTick", 0);
        }

        public virtual IEnumerable<Gizmo> GetShieldGizmos()
        {
            if (Find.Selector.SingleSelectedThing == Pawn)
            {
                _CompEnergyShieldStatus ??= new Gizmo_HediffEnergyShieldStatus { shield = this };
                yield return _CompEnergyShieldStatus;
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (Pawn == null)
            {
                energy = 0f;
            }
            else if (ShieldState == ShieldState.Resetting)
            {
                ticksToReset--;
                if (ticksToReset <= 0)
                {
                    Reset();
                }
            }
            else if (ShieldState == ShieldState.Active)
            {
                energy += EnergyGainPerTick;
                if (energy > EnergyMax)
                {
                    energy = EnergyMax;
                }
            }
        }

        public virtual bool CheckPreAbsorbDamage(DamageInfo dinfo)
        {
            if (ShieldState != 0)
            {
                return false;
            }
            if (dinfo.Def == null)
            {
                return false;
            }
            if (Pawn.Map == null)
            {
                return false;
            }
            if (Props != null)
            {
                if (dinfo.Def == DamageDefOf.EMP && Props.brokenByEMP)
                {
                    energy = 0f;
                    Break();
                    return false;
                }
                if ((dinfo.Def.isRanged && Props.absorbRanged) || (!dinfo.Def.isRanged && Props.absorbMelee))
                {
                    energy -= dinfo.Amount * EnergyLossPerDamage;
                    if (energy < 0f)
                    {
                        Break();
                    }
                    else
                    {
                        AbsorbedDamage(dinfo);
                    }
                    return true;
                }
            }
            return false;
        }

        public override void CompPostMake()
        {
            base.CompPostMake();
        }

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
        }

        public virtual void KeepDisplaying()
        {
            lastKeepDisplayTick = Find.TickManager.TicksGame;
        }

        public virtual void AbsorbedDamage(DamageInfo dinfo)
        {
            SoundDefOf.EnergyShield_AbsorbDamage.PlayOneShot(new TargetInfo(Pawn.Position, Pawn.Map));
            impactAngleVect = Vector3Utility.HorizontalVectorFromAngle(dinfo.Angle);
            Vector3 loc = Pawn.TrueCenter() + impactAngleVect.RotatedBy(180f) * 0.5f;
            float num = Mathf.Min(10f, 2f + dinfo.Amount / 10f);
            FleckMaker.Static(loc, Pawn.Map, FleckDefOf.ExplosionFlash, num);
            for (int i = 0; i < (int)num; i++)
            {
                Rand.PushState();
                FleckMaker.ThrowDustPuff(loc, Pawn.Map, Rand.Range(0.8f, 1.2f));
                Rand.PopState();
            }
            lastAbsorbDamageTick = Find.TickManager.TicksGame;
            KeepDisplaying();
        }

        public virtual void Break()
        {
            if (Pawn.Spawned)
            {
                float scale = Mathf.Lerp(Props.minDrawSize, Props.maxDrawSize, energy);
                EffecterDefOf.Shield_Break.SpawnAttached(Pawn, Pawn.MapHeld, scale);
                FleckMaker.Static(Pawn.TrueCenter(), Pawn.Map, FleckDefOf.ExplosionFlash, 12f);
                for (int i = 0; i < 6; i++)
                {
                    FleckMaker.ThrowDustPuff(Pawn.TrueCenter() + Vector3Utility.HorizontalVectorFromAngle(Rand.Range(0, 360)) * Rand.Range(0.3f, 0.6f), Pawn.Map, Rand.Range(0.8f, 1.2f));
                }
            }
            energy = 0f;
            ticksToReset = StartingTicksToReset;
        }

        public virtual void Reset()
        {
            if (Pawn.Spawned)
            {
                SoundDefOf.EnergyShield_Reset.PlayOneShot(new TargetInfo(Pawn.Position, Pawn.Map));
                FleckMaker.ThrowLightningGlow(Pawn.TrueCenter(), Pawn.Map, 3f);
            }
            ticksToReset = -1;
            energy = EnergyOnReset;
        }

        public virtual void DrawWornExtras()
        {
            if (ShieldState == ShieldState.Active && ShouldDisplay)
            {
                float num = Mathf.Lerp(Props.minDrawSize, Props.maxDrawSize, energy);
                Vector3 val = Pawn.Drawer.DrawPos;
                val.y = AltitudeLayer.MoteOverhead.AltitudeFor();
                int num2 = Find.TickManager.TicksGame - lastAbsorbDamageTick;
                if (num2 < JitterDurationTicks)
                {
                    float num3 = (JitterDurationTicks - num2) / JitterDurationTicks * MaxDamagedJitterDist;
                    val += impactAngleVect * num3;
                    num -= num3;
                }
                float angle = Rand.Range(0, 360);
                Vector3 s = new(num, 1f, num);
                Matrix4x4 matrix = default;
                matrix.SetTRS(val, Quaternion.AngleAxis(angle, Vector3.up), s);
                Graphics.DrawMesh(MeshPool.plane10, matrix, BubbleMat, 0);
            }
        }

        public virtual bool AllowVerbCast(IntVec3 root, Map map, LocalTargetInfo targ, Verb verb)
        {
            return !Props.blockRangedVerbs || verb is not Verb_LaunchProjectile || ReachabilityImmediate.CanReachImmediate(root, targ, map, PathEndMode.Touch, null);
        }
    }

}
