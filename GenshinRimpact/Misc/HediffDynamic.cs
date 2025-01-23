using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Rimpact
{
    [HotSwap.HotSwappable]
    public class HediffDynamic : HediffWithComps
    {
        public string _label;
        public Color _color;
        public HediffDataStage _cachedStageData;
        private HediffStage _stage;

        public override string Label
        {
            get
            {
                return _label; // TODO: Add override;
            }
        }
        public override Color LabelColor
        {
            get
            {
                return _color; // TODO: Add override;
            }
        }
        public override HediffStage CurStage
        {
            get
            {
                return _stage; // TODO: Add override;
            }
        }

        public void ApplyValues(string label, Color color, HediffDataStage stageData)
        {
            _label = label;
            _color = color;
            _cachedStageData = stageData;
            _stage = _cachedStageData.ToHediffStage();
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref _label, "_label", "NO LABEL. SAVE ERROR?");
            Scribe_Values.Look(ref _color, "_color", new(1,0,1));
            Scribe_Deep.Look(ref _cachedStageData, "_cachedStageData");
            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                if (_cachedStageData == null)
                {
                    Utils.LogError("_cachedStageData is null for " + def.defName + " with label: " + _label);
                }
                else _stage = _cachedStageData.ToHediffStage();
            }
            base.ExposeData();
        }
    }
}
