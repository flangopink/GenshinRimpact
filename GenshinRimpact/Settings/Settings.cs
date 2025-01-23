using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System;
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

        public bool enableFriendlyFire = true;

        // Visions
        public bool enableVisionPassives = true;
        public bool enableVisionReawakening = true;
        public Settings_VisionDropMode visionDropMode;
        public Settings_VisionMasterlessMode visionMasterelssMode;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref energyPoolRegenRateTicks, "energyPoolRegenRateTicks", 2500);
            Scribe_Values.Look(ref energyPoolMax, "energyPoolMax", 200);

            Scribe_Values.Look(ref interruptedAbilityStunDuration, "interruptedAbilityStunDuration", 90);
            Scribe_Values.Look(ref enableFriendlyFire, "enableFriendlyFire", true);

            Scribe_Values.Look(ref enableVisionPassives, "enableVisionPassives", true);
            Scribe_Values.Look(ref enableVisionReawakening, "enableVisionReawakening", true);
            Scribe_Values.Look(ref visionDropMode, "visionDropMode", Settings_VisionDropMode.Masterless);
            Scribe_Values.Look(ref visionMasterelssMode, "visionMasterelssMode", Settings_VisionMasterlessMode.RandomPremadeVision);
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
            l.Label("Rimpact_Settings_Title_energyPool".Translate());
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

            // --- Abilities ---
            l.NewColumn();
            Text.Anchor = TextAnchor.MiddleCenter;
            l.Label("Rimpact_Settings_Title_abilities".Translate());
            Text.Anchor = TextAnchor.UpperLeft;
            l.CheckboxLabeled("Rimpact_Settings_enableFriendlyFire".Translate(), ref s.enableFriendlyFire);
            l.Label("Rimpact_Settings_interruptedAbilityStunDuration".Translate());
            l.TextFieldNumeric(ref s.interruptedAbilityStunDuration, ref s.interruptedAbilityStunDuration_buffer);

            // --- Visions ---
            //l.NewColumn();
            l.GapLine(32f);
            Text.Anchor = TextAnchor.MiddleCenter;
            l.Label("Rimpact_Settings_Title_visions".Translate());
            Text.Anchor = TextAnchor.UpperLeft;
            l.CheckboxLabeled("Rimpact_Settings_enableVisionPassives".Translate(), ref s.enableVisionPassives);
            l.CheckboxLabeled("Rimpact_Settings_enableVisionReawakening".Translate(), ref s.enableVisionReawakening);
            if (l.ButtonTextLabeledPct("Rimpact_Settings_visionDropMode".Translate(), s.visionDropMode.TranslateEnum(), 0.6f, TextAnchor.MiddleLeft))
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
                if (l.ButtonTextLabeledPct("Rimpact_Settings_visionMasterlessMode".Translate(), s.visionMasterelssMode.TranslateEnum(), 0.6f, TextAnchor.MiddleLeft))
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
            l.End();

            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Rimpact_ModSettings".Translate();
        }
    }
}
