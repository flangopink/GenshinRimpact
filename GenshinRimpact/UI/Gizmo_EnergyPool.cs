using RimWorld;
using UnityEngine;
using Verse;

namespace Rimpact
{
    [HotSwap.HotSwappable]
    [StaticConstructorOnStartup]
    public class Gizmo_EnergyPool : Gizmo
    {
        private readonly MapComponent_EnergyPool pool;

        private static readonly Texture2D FullBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.35f, 0.55f, 0.45f));
        private static readonly Texture2D EmptyBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.175f, 0.275f, 0.225f));

        public Gizmo_EnergyPool(MapComponent_EnergyPool pool)
        {
            base.Order = -200f;
            this.pool = pool;
        }

        public override float GetWidth(float maxWidth)
        {
            return 85f;
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            // Gizmo background
            Rect rect = new(topLeft.x, topLeft.y, GetWidth(maxWidth), Height);
            Rect rect2 = rect.ContractedBy(6f);
            Widgets.DrawWindowBackground(rect);

            
            // Shield energy
            Rect rect4 = rect2;
            rect4.yMin = rect2.y + rect2.height / 2f;
            float fillPercent = pool.EnergyPoolPercentage;
            //Utils.FillableCircleBar(rect2, fillPercent, FullBarTex, EmptyBarTex);
            Utils.VerticalFillableBar(rect2, fillPercent, FullBarTex, EmptyBarTex, false);

            Text.Anchor = TextAnchor.MiddleCenter;

            // Hediff label
            Rect rect3 = rect2;
            rect3.height = rect.height / 2f;
            Text.Font = GameFont.Small;
            Widgets.Label(rect, pool.energy.ToString("F0") + " / " + pool.maxEnergy.ToString("F0"));
            Text.Font = GameFont.Tiny;

            var label = "GR_EnergyPool".Translate();
            //Widgets.Label(rect4, label);

            float num = Text.CalcHeight(label, rect.width + 0.1f);
            Rect rectLabel = new(rect.x, rect.yMax - num + 12f, rect.width, num);
            GUI.DrawTexture(rectLabel, TexUI.GrayTextBG);
            Text.Anchor = TextAnchor.UpperCenter;
            Widgets.Label(rectLabel, label);

            // Reset to avoid GUI errors
            Text.Anchor = TextAnchor.UpperLeft;
            TooltipHandler.TipRegion(rect2, "GR_EnergyPoolGizmoTip".Translate() + "\n\n" + "GR_EnergyRegenRate".Translate() + ": " + pool.RegenRate.ToStringTicksToPeriod());
            return new GizmoResult(GizmoState.Clear);
        }
    }
}
