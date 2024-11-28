using RimWorld;
using System.Security.Cryptography;
using UnityEngine;
using Verse;

namespace GenshinRimpact
{
    public class Gizmo_AbilityStacks : Gizmo
    {
        protected Ability ability;

        private CompAbilityWithStacks stackComp;
        private CompAbilityWithStacks StackComp => stackComp ??= ability.CompOfType<CompAbilityWithStacks>();

        //private const float Padding = 6f;

        //private const float Width = 140f;

        public override float GetWidth(float maxWidth)
        {
            return 140f;
        }

        public Gizmo_AbilityStacks(Ability ability)
        {
            this.ability = ability;
            Order = ability.def.uiOrder + 1;
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            Rect rect = new(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
            Rect position = rect.ContractedBy(6f);
            float num = position.height / 3f;
            Widgets.DrawWindowBackground(rect);
            GUI.BeginGroup(position);
            Text.Anchor = TextAnchor.UpperCenter;
            Widgets.Label(new Rect(0f, 0f, position.width, num), "GR_AbilityStacks".Translate().CapitalizeFirst());
            if (ability != null)
            {
                DrawStackBar(new Rect(0f, num, position.width, num + 2f), 2f, /*new Rect(0f, 0f, position.width, num * 2f),*/ drawLabel: false);
            }
            Rect rect2 = new(0f, num * 2f, position.width, Text.LineHeight);
            Widgets.Label(rect2, string.Format("{0}: {1} / {2}", "GR_Stacks".Translate().CapitalizeFirst(), StackComp.currentStacks, StackComp.maxStacks));
            Text.Anchor = TextAnchor.UpperLeft;
            /*if (Mouse.IsOver(rect2))
            {
                Widgets.DrawHighlight(rect2);
                TooltipHandler.TipRegion(rect2, "GR_StackCapacityDesc".Translate());
            }*/
            GUI.EndGroup();
            return new GizmoResult(GizmoState.Clear);
        }

        private void DrawStackBar(Rect gizmoRect, float customMargin = -1f, bool drawLabel = true)
        {
            if (gizmoRect.height > 70f)
            {
                float num = (gizmoRect.height - 70f) / 2f;
                gizmoRect.height = 70f;
                gizmoRect.y += num;
            }
            /*Rect rect2 = rectForTooltip ?? rect;
            if (Mouse.IsOver(rect2))
            {
                Widgets.DrawHighlight(rect2);
            }
            if (doTooltip && Mouse.IsOver(rect2))
            {
                TooltipHandler.TipRegion(rect2, new TipSignal(() => GetTipString(), rect2.GetHashCode()));
            }*/
            float num2 = 14f;
            float num3 = (customMargin >= 0f) ? customMargin : (num2 + 15f);
            if (gizmoRect.height < 50f)
            {
                num2 *= Mathf.InverseLerp(0f, 50f, gizmoRect.height);
            }
            if (drawLabel)
            {
                Text.Font = (gizmoRect.height > 55f) ? GameFont.Small : GameFont.Tiny;
                Text.Anchor = TextAnchor.LowerLeft;
                Widgets.Label(new Rect(gizmoRect.x + num3 + gizmoRect.width * 0.1f, gizmoRect.y, gizmoRect.width - num3 - gizmoRect.width * 0.1f, gizmoRect.height / 2f), ability.def.LabelCap);
                Text.Anchor = TextAnchor.UpperLeft;
            }
            Rect barRect = gizmoRect;
            if (drawLabel)
            {
                barRect.y += gizmoRect.height / 2f;
                barRect.height -= gizmoRect.height / 2f;
            }
            barRect = new(barRect.x + num3, barRect.y, barRect.width - num3 * 2f, barRect.height - num2);
            if (DebugSettings.ShowDevGizmos)
            {
                float lineHeight = Text.LineHeight;
                Rect rect4 = new(barRect.xMax - lineHeight, barRect.y - lineHeight, lineHeight, lineHeight);
                if (Widgets.ButtonImage(rect4.ContractedBy(4f), TexButton.Plus))
                {
                    StackComp.OffsetStacks(1);
                }
                Rect rect5 = new(rect4.xMin - lineHeight, barRect.y - lineHeight, lineHeight, lineHeight);
                if (Widgets.ButtonImage(rect5.ContractedBy(4f), TexButton.Minus))
                {
                    StackComp.OffsetStacks(-1);
                }
            }
            Rect fillBarRect = barRect;
            Rect fillRect = Widgets.FillableBar(fillBarRect, StackComp.CurrentStacksPercentage);
            for (int j = 1; (float)j < StackComp.maxStacks; j++)
            {
                DrawBarDivision(fillRect, (float)j / StackComp.maxStacks);
            }
            Text.Font = GameFont.Small;
        }

        private void DrawBarDivision(Rect barRect, float threshPct)
        {
            float num = 5f;
            Rect rect = new(barRect.x + barRect.width * threshPct - (num - 1f), barRect.y, num, barRect.height);
            if (threshPct < StackComp.CurrentStacksPercentage)
            {
                GUI.color = new Color(0f, 0f, 0f, 0.9f);
            }
            else
            {
                GUI.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            }
            Rect position = rect;
            position.yMax = position.yMin + 4f;
            GUI.DrawTextureWithTexCoords(position, Utils.DividerTex, new Rect(0f, 0.5f, 1f, 0.5f));
            Rect position2 = rect;
            position2.yMin = position2.yMax - 4f;
            GUI.DrawTextureWithTexCoords(position2, Utils.DividerTex, new Rect(0f, 0f, 1f, 0.5f));
            Rect position3 = rect;
            position3.yMin = position.yMax;
            position3.yMax = position2.yMin;
            if (position3.height > 0f)
            {
                GUI.DrawTextureWithTexCoords(position3, Utils.DividerTex, new Rect(0f, 0.4f, 1f, 0.2f));
            }
            GUI.color = Color.white;
        } 
    }
}
