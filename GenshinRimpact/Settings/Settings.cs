using RimWorld;
using System.Collections.Generic;
using System;
using UnityEngine;
using Verse;

namespace Rimpact
{
    public class Settings : ModSettings
    {
        // Energy pool
        public int energyPoolMax = 200;
        public string energyPoolMax_buffer;

        public int energyPoolRegenRateTicks = 2500;
        public string energyPoolRegenRateTicks_buffer;

        public int energyPoolCheckRateTicks = 300;
        public string energyPoolCheckRateTicks_buffer;

        // Abilities
        public int interruptedAbilityStunDuration = 90;
        public string interruptedAbilityStunDuration_buffer;

        public bool enableFriendlyFire = true;

        // Visions
        public bool enableVisionPassives = true;
        public bool enableVisionReawakening = true;
        public Settings_VisionDropMode visionDropMode;
        public Settings_VisionMasterlessMode visionMasterelssMode;

        // Hediff Sets
        public IntRange hediffSetsRange = new(2,4);
        public IntRange hediffTotalStatsRange = new(3,5);
        public int offsetWeight = 75;
        public int factorWeight = 20;
        public int traitWeight = 5;
        public string offsetWeight_buffer;
        public string factorWeight_buffer;
        public string traitWeight_buffer;
        //public IntRange hediffOffetsRange = new(1,3);
        //public IntRange hediffFactorsRange = new(1,2);

        public override void ExposeData()
        {
            Scribe_Values.Look(ref energyPoolMax, "energyPoolMax", 200);
            Scribe_Values.Look(ref energyPoolRegenRateTicks, "energyPoolRegenRateTicks", 2500);
            Scribe_Values.Look(ref energyPoolCheckRateTicks, "energyPoolCheckRateTicks", 300);

            Scribe_Values.Look(ref interruptedAbilityStunDuration, "interruptedAbilityStunDuration", 90);
            Scribe_Values.Look(ref enableFriendlyFire, "enableFriendlyFire", true);

            Scribe_Values.Look(ref enableVisionPassives, "enableVisionPassives", true);
            Scribe_Values.Look(ref enableVisionReawakening, "enableVisionReawakening", true);
            Scribe_Values.Look(ref visionDropMode, "visionDropMode", Settings_VisionDropMode.Masterless);
            Scribe_Values.Look(ref visionMasterelssMode, "visionMasterelssMode", Settings_VisionMasterlessMode.RandomPremadeVision);

            Scribe_Values.Look(ref hediffSetsRange, "hediffSetsRange", new(2,4));
            Scribe_Values.Look(ref hediffTotalStatsRange, "hediffTotalStatsRange", new(3,5));
            Scribe_Values.Look(ref offsetWeight, "offsetWeight", 75);
            Scribe_Values.Look(ref factorWeight, "factorWeight", 20);
            Scribe_Values.Look(ref traitWeight, "traitWeight", 5);
            //Scribe_Values.Look(ref hediffOffetsRange, "hediffOffetsRange", new(1,3));
            //Scribe_Values.Look(ref hediffFactorsRange, "hediffFactorsRange", new(1,2));
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
                ColumnWidth = 420f,
                verticalSpacing = 4f
            };

            // --- Energy pool ---
            l.Begin(inRect);
            Text.Anchor = TextAnchor.MiddleCenter;
            l.Label("Rimpact_Settings_Title_energyPool".Translate().Colorize(ColoredText.DateTimeColor));
            Text.Anchor = TextAnchor.UpperLeft;

            l.Label("Rimpact_Settings_energyPoolMax".Translate());
            l.TextFieldNumeric(ref s.energyPoolMax, ref s.energyPoolMax_buffer);
            if (Find.CurrentMap != null)
            {
                if (l.ButtonText("Rimpact_Apply".Translate()))
                {
                    if (Find.CurrentMap.GetComponent<MapComponent_EnergyPool>() is MapComponent_EnergyPool comp)
                    {
                        comp.maxEnergy = s.energyPoolMax;
                        if (comp.energy > comp.maxEnergy) comp.energy = comp.maxEnergy;
                    }
                    else Utils.LogError("Tried applying settings, but MapComponent_EnergyPool is missing on current map.");
                }
            }
            l.Label("Rimpact_Settings_energyPoolRegenRateTicks".Translate());
            l.TextFieldNumeric(ref s.energyPoolRegenRateTicks, ref s.energyPoolRegenRateTicks_buffer);
            l.Label("Rimpact_Settings_energyPoolCheckRateTicks".Translate());
            l.TextFieldNumeric(ref s.energyPoolCheckRateTicks, ref s.energyPoolCheckRateTicks_buffer);

            // --- Abilities ---
            l.NewColumn();
            Text.Anchor = TextAnchor.MiddleCenter;
            l.Label("Rimpact_Settings_Title_abilities".Translate().Colorize(ColoredText.ExpectationsColor));
            Text.Anchor = TextAnchor.UpperLeft;
            l.CheckboxLabeled("Rimpact_Settings_enableFriendlyFire".Translate(), ref s.enableFriendlyFire);
            l.Label("Rimpact_Settings_interruptedAbilityStunDuration".Translate());
            l.TextFieldNumeric(ref s.interruptedAbilityStunDuration, ref s.interruptedAbilityStunDuration_buffer);

            // --- Visions ---
            //l.NewColumn();
            l.GapLine(32f);
            Text.Anchor = TextAnchor.MiddleCenter;
            l.Label("Rimpact_Settings_Title_visions".Translate().Colorize(ColoredText.ImpactColor));
            Text.Anchor = TextAnchor.UpperLeft;
            l.CheckboxLabeled("Rimpact_Settings_enableVisionPassives".Translate(), ref s.enableVisionPassives);
            l.CheckboxLabeled("Rimpact_Settings_enableVisionReawakening".Translate().Colorize(ColoredText.ThreatColor), ref s.enableVisionReawakening);
            if (l.ButtonTextLabeledPct("Rimpact_Settings_visionDropMode".Translate().Colorize(ColoredText.ThreatColor), s.visionDropMode.TranslateEnum(), 0.6f, TextAnchor.MiddleLeft))
            {
                List<FloatMenuOption> menu = [];
                foreach (Settings_VisionDropMode e in Enum.GetValues(typeof(Settings_VisionDropMode)))
                {
                    menu.Add(new FloatMenuOption(e.TranslateEnum(), delegate
                    {
                        s.visionDropMode = e;
                    }));
                }
                Find.WindowStack.Add(new FloatMenu(menu));
            }
            if (s.enableVisionReawakening)
            {
                if (l.ButtonTextLabeledPct("Rimpact_Settings_visionMasterlessMode".Translate().Colorize(ColoredText.ThreatColor), s.visionMasterelssMode.TranslateEnum(), 0.6f, TextAnchor.MiddleLeft))
                {
                    List<FloatMenuOption> menu = [];
                    foreach (Settings_VisionMasterlessMode e in Enum.GetValues(typeof(Settings_VisionMasterlessMode)))
                    {
                        menu.Add(new FloatMenuOption(e.TranslateEnum(), delegate
                        {
                            s.visionMasterelssMode = e;
                        }));
                    }
                    Find.WindowStack.Add(new FloatMenu(menu));
                }
            }

            // --- Passives ---
            float l2y = inRect.y + l.CurHeight;
            float l2width = inRect.width - l.ColumnWidth;
            float l2height = inRect.height - l.CurHeight;
            l.GapLine(32f);
            Listing_Standard l2 = new()
            {
                ColumnWidth = 187f,
                verticalSpacing = 4f
            };
            Rect l2Rect = new(l2width, l2y, l2width, l2height);
            l2.Begin(l2Rect);

            Text.Anchor = TextAnchor.MiddleCenter;
            l2.Label("Rimpact_Settings_hediffSetsRange".Translate());
            Text.Anchor = TextAnchor.UpperLeft;
            l2.IntRange(ref s.hediffSetsRange, 1, 10);

            l2.NewColumn();
            Text.Anchor = TextAnchor.MiddleCenter;
            l2.Label("Rimpact_Settings_hediffTotalStatsRange".Translate());
            Text.Anchor = TextAnchor.UpperLeft;
            l2.IntRange(ref s.hediffTotalStatsRange, 1, 10);

            Listing_Standard l3 = new()
            {
                ColumnWidth = 125f,
                verticalSpacing = 4f
            };
            Rect l3Rect = new(inRect.x, inRect.y + l2.CurHeight - 32f, inRect.width, inRect.height);
            l3.Begin(l3Rect);

            l3.Label("Rimpact_Settings_offsetWeight".Translate());
            l3.TextFieldNumeric(ref s.offsetWeight, ref s.offsetWeight_buffer);

            l3.NewColumn();
            l3.Label("Rimpact_Settings_factorWeight".Translate());
            l3.TextFieldNumeric(ref s.factorWeight, ref s.factorWeight_buffer);

            l3.NewColumn();
            l3.Label("Rimpact_Settings_traitWeight".Translate());
            l3.TextFieldNumeric(ref s.traitWeight, ref s.traitWeight_buffer);
            //l.Label("Rimpact_Settings_hediffOffetsRange".Translate());
            //l.IntRange(ref s.hediffOffetsRange, 0, 10);
            //l.Label("Rimpact_Settings_hediffFactorsRange".Translate());
            //l.IntRange(ref s.hediffFactorsRange, 0, 10);
            l3.End();
            l2.End();
            l.End();

            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Rimpact_ModSettings".Translate();
        }
    }
}
