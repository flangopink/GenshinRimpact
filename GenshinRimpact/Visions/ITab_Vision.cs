using RimWorld;
using System.Text;
using UnityEngine;
using Verse;

namespace Rimpact
{
    [HotSwap.HotSwappable]
    public class ITab_Vision : ITab
    {
        private static readonly Vector2 WinSize = new(360f, 400f);
        protected const float TopPadding = 20f;
        public const float TitleHeight = 30f;
        private string tmpString;

        private CompVisionEquippableAbilities SelComp => SelThing?.TryGetComp<CompVisionEquippableAbilities>();

        public ITab_Vision()
        {
            size = WinSize;
            labelKey = "GR_TabVision";
            tutorTag = "Vision";
        }

        protected override void FillTab()
        {
            Rect rect = new Rect(0f, 20f, size.x, size.y - TopPadding).ContractedBy(10f);
            Rect rect2 = rect;
            rect2.y = 10f;
            rect2.height = TitleHeight;
            Rect rect3 = rect;
            rect3.xMax = rect.center.x;// - 17f;
            rect3.y = rect2.yMax + 17f;
            rect3.yMax = rect.yMax;
            Rect rect4 = rect;
            rect4.x = rect.center.x + 17f;
            rect4.xMax = rect.center.x - 17f;
            rect4.y = rect2.yMax + 17f;
            rect4.yMax = rect.yMax;
            DrawTitle(rect2, SelThing);
            Widgets.DrawLineHorizontal(10f, rect3.y - 10f, rect2.xMax - 10f, Widgets.SeparatorLineColor);

            DrawInfoPanelLeft(rect);
            //DrawLineVertical(rect3.xMax + 17f, rect3.y, rect3.yMax - rect2.yMax - 17f, Widgets.SeparatorLineColor);
        }

        /*private static void DrawLineVertical(float x, float y, float length, Color color)
        {
            Widgets.DrawBoxSolid(new Rect(x, y, 1f, length), color);
        }*/

        public void DrawTitle(Rect rect, Thing thing)
        {
            Rect position = rect;
            position.width = position.height;

            var oldGUIColor = GUI.color; // Save the value to prevent float errors.
            GUI.color *= 2f;
            GenUI.DrawTextureWithMaterial(position, thing.def.uiIcon, thing.Graphic.MatSingle);
            GUI.color = oldGUIColor;

            Text.Anchor = TextAnchor.MiddleLeft;
            Text.Font = GameFont.Medium;
            Rect rect2 = rect;
            rect2.x += rect.height + 10f;
            rect2.width -= rect.height + 10f;
            Widgets.LabelFit(rect2, thing.LabelCap);

            Text.Anchor = TextAnchor.MiddleRight;
            var elem = SelComp.Props.visionDef.element;
            if (elem != null)
            {
                Rect rect3 = rect2;
                rect3.xMax -= 27f;
                Widgets.LabelFit(rect3, elem.ToString().Colorize(elem.color));
            }

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
        }

        public void DrawInfoPanelLeft(Rect rect)
        {
            //float y = rect.y;
            Rect rect2 = new(rect.x + 4f, rect.y + 4f, rect.width, rect.height);
            //Widgets.BeginScrollView(rect, ref scrollPosition, rect2);
            if (SelComp.AbilitiesForReading.Count > 0)
            {
                rect2.y += 24f;
                Widgets.Label(rect2, "Abilities".Translate() + ":");
                string abName;
                Texture2D abIcon;
                for (int i = 0; i < SelComp.AbilitiesForReading.Count; i++)
                {
                    rect2.y += 24f;
                    abName = SelComp.AbilitiesForReading[i].def.LabelCap.Colorize(SelComp.Props.visionDef.element?.color ?? Color.white);
                    abIcon = SelComp.AbilitiesForReading[i].def.uiIcon;
                    Rect iconRect = new(rect2.x, rect2.y, 18f, 18f);
                    Rect labelRect = new(rect2.x + iconRect.width + 4f, rect2.y - 1f, rect2.width, rect2.height);
                    Widgets.DrawTextureFitted(iconRect, abIcon, 1f);
                    Widgets.Label(labelRect, abName);

                    var rect3 = rect2;
                    rect3.xMax -= 17f;
                    Text.Anchor = TextAnchor.UpperRight;
                    Widgets.Label(rect3, SelComp.AbilitiesForReading[i].def.casterMustBeCapableOfViolence ? "GR_Violent".Translate().Colorize(ColoredText.ThreatColor) : "GR_Safe".Translate().Colorize(ColoredText.ExpectationsColor));
                    Text.Anchor = TextAnchor.UpperLeft;
                }
                rect2.y += 4f;
            }
            if (SelComp.HasHediff)
            {
                var hediff = SelComp.Props.visionDef.hediff;
                rect2.y += 24f;
                Widgets.Label(rect2, "GR_Passives".Translate() + ":");
                rect2.y += 24f;
                StringBuilder sb = new();
                foreach (var stat in HediffStatsUtility.SpecialDisplayStats(hediff.stages[0], null))
                {
                    sb.AppendLine($"  {stat.ValueString} {stat.LabelCap}");
                }
                tmpString = sb.ToString();
                Widgets.Label(rect2, tmpString);
                rect2.y += Text.CalcHeight(tmpString, rect2.width) * 0.675f;
                if (hediff.comps != null)
                {
                    sb.Clear();
                    for (int i = 0; i < hediff.comps.Count; i++)
                    {
                        if (hediff.comps[i] is HediffCompProperties_DamageMultiplier multComp)
                        {
                            rect2.y += 24f;
                            Widgets.Label(rect2, "GR_DamageMultipliers".Translate() + ":");
                            rect2.y += 24f;
                            foreach (var dmg in multComp.damageMultipliers)
                            {
                                sb.AppendLine($" - {dmg.damageDef.LabelCap}: x{dmg.multiplier.ToStringPercent()}");
                            }
                        }
                    }
                    tmpString = sb.ToString();
                    Widgets.Label(rect2, tmpString);
                }
            }
            if (SelComp.HasTrait)
            {
                rect2.y += 24f;
                Widgets.Label(rect2, "Traits".Translate() + ":");
                rect2.y += 24f;
                Widgets.Label(rect2, " - " + SelComp.TraitLabel);
            }
            /*else
            {
                rect2.y += 24f;
                Widgets.Label(rect2, "Traits".Translate() + ":");
                rect2.y += 24f;
                Widgets.Label(rect2, " - " + "Sample Text");
            }*/
            //Widgets.EndScrollView();
        }
    }
}
