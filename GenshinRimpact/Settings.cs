using UnityEngine;
using Verse;

namespace Rimpact
{
    public class Settings : ModSettings
    {
        // Energy pool
        public int energyPoolRegenRateTicks = 2500;
        public string energyPoolRegenRateTicks_buffer;

        public int energyPoolMax = 200;
        public string energyPoolMax_buffer;

        // Abilities
        public int interruptedAbilityStunDuration = 90;
        public string interruptedAbilityStunDuration_buffer;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref energyPoolRegenRateTicks, "energyPoolRegenRateTicks", 2500);
            Scribe_Values.Look(ref energyPoolMax, "energyPoolMax", 200);
            Scribe_Values.Look(ref interruptedAbilityStunDuration, "interruptedAbilityStunDuration", 90);
            base.ExposeData();
        }
    }

    [HotSwap.HotSwappable]
    public class RimpactMod : Mod
    {
        public static RimpactMod Rimpact;

        private readonly Settings s;

        public RimpactMod(ModContentPack content) : base(content)
        {
            Rimpact = this;
            s = GetSettings<Settings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard l = new()
            {
                ColumnWidth = 250f,
                verticalSpacing = 4f
            };
            l.Begin(inRect);
            Text.Anchor = TextAnchor.MiddleCenter;
            l.Label("Rimpact_Settings_Title_energyPool".Translate());
            Text.Anchor = TextAnchor.UpperLeft;
            l.Label("Rimpact_Settings_energyPoolMax".Translate());
            l.TextFieldNumeric(ref s.energyPoolMax, ref s.energyPoolMax_buffer);
            l.Label("Rimpact_Settings_energyPoolRegenRateTicks".Translate());
            l.TextFieldNumeric(ref s.energyPoolRegenRateTicks, ref s.energyPoolRegenRateTicks_buffer);

            l.NewColumn();
            Text.Anchor = TextAnchor.MiddleCenter;
            l.Label("Rimpact_Settings_Title_abilities".Translate());
            Text.Anchor = TextAnchor.UpperLeft;
            l.Label("Rimpact_Settings_interruptedAbilityStunDuration".Translate());
            l.TextFieldNumeric(ref s.interruptedAbilityStunDuration, ref s.interruptedAbilityStunDuration_buffer);

            l.End();

            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Rimpact_ModSettings".Translate();
        }
    }
}
