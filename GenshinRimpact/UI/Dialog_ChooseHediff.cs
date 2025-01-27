using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using static HarmonyLib.Code;

namespace Rimpact
{
    [HotSwap.HotSwappable]
    public class Dialog_ChooseHediff : Window
    {
        private static readonly Color HighlightColor = new(0.2f, 0.8f, 0.4f);
        public string title;
        public string text;

        public string buttonAcceptText;
        public Action buttonAcceptAction;

        public string buttonCancelText;
        public Action buttonCancelAction;

        public CompVisionEquippableAbilities comp;
        public List<HediffStageData> datas;
        public HediffStageData selectedData;
        private readonly List<HediffStage> _cachedStages = [];

        private Vector2 scrollPosition = Vector2.zero;
        private Rect selectionRect = Rect.zero;

        public Dialog_ChooseHediff(CompVisionEquippableAbilities parentComp, Pawn pawn)
        {
            title = "Rimpact_ChooseHediff_title".Translate();
            text = "Rimpact_ChooseHediff_text".Translate();
            comp = parentComp;
            datas = [..comp.hediffStageDatas];

            for(int i = 0; i < datas.Count; i++)
            {
                _cachedStages.Add(datas[i].ToHediffStage());
            }

            buttonAcceptText = "Accept".Translate();
            buttonAcceptAction = delegate 
            {
                if (selectedData != null)
                {
                    comp.appliedStageData = selectedData;
                    //HediffDynamic h = (HediffDynamic)HediffMaker.MakeHediff(Rimpact_DefOf.GR_Hediff_Dynamic, pawn);
                    //h.ApplyValues(comp.HediffDynamicLabel, selectedData.vision?.element?.color ?? Color.white, selectedData);
                    pawn.health.AddHediff(comp.HediffDynamicForReading);
                    Close();
                }
                else Messages.Message("Rimpact_ChooseHediff_Reject".Translate(), MessageTypeDefOf.RejectInput, false);
            };
            buttonCancelText = "Cancel".Translate();
            buttonCancelAction = delegate
            {
                Close();
            };
            layer = WindowLayer.Dialog;
            forcePause = true;
            absorbInputAroundWindow = true;
            onlyOneOfTypeAllowed = false;
        }

        public override void DoWindowContents(Rect inRect)
        {
            float num = inRect.y;
            if (!title.NullOrEmpty())
            {
                Text.Font = GameFont.Medium;
                Widgets.Label(new Rect(0f, num, inRect.width, 42f), title);
                num += 42f;
            }
            Text.Font = GameFont.Small;
            Rect outRect = new(inRect.x, num, inRect.width, inRect.height - 35f - 5f - num);
            float width = outRect.width - 16f;

            //int TESTCOUNT = 5;
            float approxHeight = _cachedStages.Count * 110f;// * TESTCOUNT;
            Rect viewRect = new(0f, 0f, width, approxHeight);

            Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
            Rect subRect = new(2f, 2f, viewRect.width-4, viewRect.height-4);

            if (selectionRect != Rect.zero)
            {
                GUI.color = HighlightColor;
                Widgets.DrawBox(selectionRect, 2);
                GUI.color = Color.white;
            }

            for (int i = 0; i < _cachedStages.Count; i++)
            {
                string statText = DoStatContents(_cachedStages[i]);
                if (datas[i].trait != null)
                {
                    statText += $"\n  {"Traits".Translate().Colorize(ColoredText.NameColor)}: {datas[i].trait.DataAtDegree(datas[i].traitDegree).LabelCap}";
                }
                subRect.height = Text.CalcHeight(statText, subRect.width) + 8f;
                if (Widgets.ButtonTextSubtle(subRect, statText))
                {
                    selectedData = datas[i];
                    selectionRect = subRect.ContractedBy(-2f);
                    Utils.LogMessage(selectionRect.ToString());
                }
                subRect.y += subRect.height + 8f;
            }
            Widgets.EndScrollView();

            float num4 = 2f;
            float num5 = inRect.width / num4;
            float width2 = num5 - 10f;
            if (Widgets.ButtonText(new Rect(num5 * (num4 - 1) + 10f, inRect.height - 35f, width2, 35f), buttonAcceptText))
            {
                buttonAcceptAction();
            }
            GUI.color = Color.white;
            if (Widgets.ButtonText(new Rect(0f, inRect.height - 35f, width2, 35f), buttonCancelText))
            {
                buttonCancelAction();
            }
        }

        private string DoStatContents(HediffStage stage)
        {
            StringBuilder sb = new();
            foreach (var stat in HediffStatsUtility.SpecialDisplayStats(stage, null))
            {
                sb.AppendLine($"  {stat.ValueString} {stat.LabelCap}");
            }
            sb.Remove(sb.Length-1, 1);
            return sb.ToString();
        }
    }
}
