using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace GenshinRimpact
{
    [HotSwap.HotSwappable]
    [StaticConstructorOnStartup]
    public class Gizmo_HediffEnergyShieldStatus : Gizmo
    {
        private readonly HediffComp_Shield shield;
        private readonly HediffComp_Disappears compDisappears;

        private static readonly Texture2D DefaultFullShieldBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.24f));
        private static readonly Texture2D EmptyShieldBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);
        private readonly Texture2D fillBarTex;

        public Gizmo_HediffEnergyShieldStatus(HediffComp_Shield shieldComp)
        {
            base.Order = -100f;
            shield = shieldComp;
            compDisappears = shield.parent.TryGetComp<HediffComp_Disappears>();
            if (shield.Props.elementDef != null && Utils.ElementalFillBars.TryGetValue(shield.Props.elementDef, out Texture2D tex))
                fillBarTex = tex;
            else fillBarTex = DefaultFullShieldBarTex;
        }

        public override float GetWidth(float maxWidth)
        {
            return 140f;
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            // Gizmo background
            Rect rect = new(topLeft.x, topLeft.y, GetWidth(maxWidth), Height);
            Rect rect2 = rect.ContractedBy(6f);
            Widgets.DrawWindowBackground(rect);

            // Hediff label
            Rect rect3 = rect2;
            rect3.height = rect.height / 2f;
            Text.Font = GameFont.Tiny;
            Widgets.Label(rect3, shield.parent.def.LabelCap + shield.Props.gizmoShowTimer
                       + (compDisappears != null ? "\n" + compDisappears.CompLabelInBracketsExtra : ""));

            // Shield energy
            Rect rect4 = rect2;
            rect4.yMin = rect2.y + rect2.height / 2f;
            float fillPercent = shield.energy / Mathf.Max(1f, shield.Props.maxEnergy);
            Widgets.FillableBar(rect4, fillPercent, fillBarTex, EmptyShieldBarTex, doBorder: false);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect4, (shield.energy * 100f).ToString("F0") + " / " + (shield.Props.maxEnergy * 100f).ToString("F0"));

            // Reset to avoid GUI errors
            Text.Anchor = TextAnchor.UpperLeft;
            TooltipHandler.TipRegion(rect2, shield.Props.disappearOnBreak ? "GR_ShieldGizmoTipBreakable".Translate() : "GR_ShieldGizmoTip".Translate());
            return new GizmoResult(GizmoState.Clear);
        }
    }
}
