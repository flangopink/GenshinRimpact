using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Rimpact
{
    public class CompProperties_VisionEquippableAbilities : CompProperties
    {
        public VisionDef visionDef;
        public bool doDescExtra;

        public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
        {
            if (visionDef == null) yield return parentDef + " has CompProperties_VisionEquippableAbilities, but no VisionDef assigned";
        }

        public CompProperties_VisionEquippableAbilities()
        {
            compClass = typeof(CompVisionEquippableAbilities);
        }
    }

    [HotSwap.HotSwappable]
    [StaticConstructorOnStartup]
    public class CompVisionEquippableAbilities : CompEquippable
    {
        public CompProperties_VisionEquippableAbilities Props => (CompProperties_VisionEquippableAbilities)props;

        private static readonly Texture2D ChooseHediffSetIcon = ContentFinder<Texture2D>.Get("UI/ChooseHediffSetIcon");

        private List<Ability> abilities = [];
        private string hediffLabel;
        private Hediff hediff;
        private HediffDynamic hediffDynamic;
        private string traitLabel;
        private Trait trait;

        public HashSet<HediffStageData> hediffStageDatas = [];
        public HediffStageData appliedStageData;
        private bool CanChooseHediff => Utils.settings.enableVisionPassives && appliedStageData == null;

        public List<Ability> AbilitiesForReading
        {
            get
            {
                if (abilities.NullOrEmpty())
                {
                    foreach (AbilityDef def in Props.visionDef.abilities)
                    {
                        //Log.Message("added " + def.defName + " to " + parent.def.defName);
                        abilities.Add(AbilityUtility.MakeAbility(def, Holder));
                    }
                }
                return abilities;
            }
        }
        
        public bool HasHediff => Utils.settings.enableVisionPassives && Props.visionDef.hediff != null;
        public bool HasTrait => trait != null || Props.visionDef.trait != null;
        public string HediffLabel => hediffLabel ??= Props.visionDef.hediff.LabelCap;
        public string HediffDynamicLabel => appliedStageData.vision.LabelCap + $" ({"GR_Passive".Translate()})";
        public string TraitLabel => traitLabel ??= appliedStageData?.trait != null ? appliedStageData.trait.DataAtDegree(appliedStageData.traitDegree).LabelCap
                                                                                  : Props.visionDef.trait.DataAtDegree(Props.visionDef.traitDegree).LabelCap;
        public Hediff HediffForReading => hediff ??= HediffMaker.MakeHediff(Props.visionDef.hediff, Holder);
        public HediffDynamic HediffDynamicForReading 
        { 
            get
            {
                if (hediffDynamic == null)
                {
                    hediffDynamic = (HediffDynamic)Holder.health.AddHediff(Rimpact_DefOf.GR_Hediff_Dynamic);
                    hediffDynamic.ApplyValues(HediffDynamicLabel, Props.visionDef?.element?.color ?? Color.white, appliedStageData);
                    if (appliedStageData.trait != null && !Holder.story.traits.HasTrait(appliedStageData.trait))
                    {
                        Holder.story.traits.GainTrait(TraitForReading);
                    }
                }
                return hediffDynamic;
            } 
        }
        public Trait TraitForReading
        {
            get
            {
                if (appliedStageData?.trait != null && Props.visionDef.trait != null)
                {
                    Utils.LogErrorOnce("this is unlikely, but: could not add a trait from the set because the visionDef already has a trait. tell the dev if this is bothering you. i'm too tired to rewrite all of this right now.", 69006900);
                }
                return trait ??= appliedStageData?.trait != null ? new(appliedStageData.trait, appliedStageData.traitDegree)
                                                                 : new(Props.visionDef.trait, Props.visionDef.traitDegree);
            }
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            parent.SetColor(Props.visionDef.element.color);
            if (Holder != null)
            {
                foreach (Ability ability in AbilitiesForReading)
                {
                    ability.pawn = Holder;
                    ability.verb.caster = Holder;
                }
                if (HasHediff) HediffForReading.pawn = Holder;
                else
                {
                    if (Holder.health.hediffSet.HasHediff(HediffForReading.def))
                        Holder.health.RemoveHediff(HediffForReading);
                }
            }
            if (appliedStageData == null && hediffStageDatas.Count == 0)
            {
                AddHediffSets();
            }
        }

        private void AddHediffSets()
        {
            for (int i = 0; i < Utils.settings.hediffSetsRange.RandomInRange; i++)
            {
                hediffStageDatas.Add(Utils.MakeRandomHediffStageData(Utils.settings.hediffTotalStatsRange, Holder, Props.visionDef));
            }
        }

        public override string CompInspectStringExtra()
        {
            string str;
            if (Props.doDescExtra)
            {
                str = "Abilities".Translate() + ":";
                foreach (Ability ab in AbilitiesForReading)
                    str += "\n• " + ab.def.label;
                if (HasHediff)
                {
                    str += "\n" + "GR_Passives".Translate() + ":";
                    str += "\n• " + HediffLabel;
                }
                if (HasTrait)
                {
                    str += "\n" + "Traits".Translate() + ":";
                    str += "\n• " + TraitLabel;
                }
            }
            else
            {
                if (AbilitiesForReading.Count == 0)
                {
                    str = "GR_VisionDescriptionStatsOnly".Translate();
                }
                else if (HasHediff || HasTrait)
                {
                    str = "GR_VisionDescription".Translate();
                }
                else str = "GR_VisionDescriptionAbilitiesOnly".Translate();
                str += "\n \n" + "GR_VisionDescriptionSeeTab".Translate($"[{"GR_TabVision".Translate()}]".Colorize(ColoredText.CurrencyColor)).Resolve();
            }
            return str;
        }

        public virtual void UsedOnce()
        {
        }

        public override void Notify_Equipped(Pawn pawn)
        {
            //Log.Message("bonked");
            foreach (Ability ability in AbilitiesForReading)
            {
                ability.pawn = pawn;
                ability.verb.caster = pawn;
                pawn.abilities.abilities.Add(ability);
            }
            pawn.abilities.Notify_TemporaryAbilitiesChanged();
            if (HasHediff)
            {
                pawn.health.AddHediff(HediffForReading);
            }
            if (appliedStageData != null)
            {
                pawn.health.AddHediff(HediffDynamicForReading);
            }
            if (HasTrait)
                if (!pawn.story.traits.HasTrait(trait?.def ?? Props.visionDef.trait))
                    pawn.story.traits.GainTrait(TraitForReading);
        }

        public override void Notify_Unequipped(Pawn pawn)
        {
            //Log.Message("unbonked");
            foreach (Ability ability in AbilitiesForReading)
            {
                pawn.abilities.RemoveAbility(ability.def);
            }
            pawn.abilities.Notify_TemporaryAbilitiesChanged();
            if (HasHediff) 
            { 
                pawn.health.RemoveHediff(HediffForReading);
            }
            if (appliedStageData != null)
            {
                pawn.health.RemoveHediff(HediffDynamicForReading);
            }
            if (HasTrait)
                if (pawn.story.traits.HasTrait(trait?.def ?? Props.visionDef.trait)) 
                    pawn.story.traits.RemoveTrait(TraitForReading);
        }

        public override IEnumerable<Gizmo> CompGetEquippedGizmosExtra()
        {
            if (CanChooseHediff)
            {
                yield return new Command_Action()
                {
                    defaultLabel = "Rimpact_ChooseHediffSet".Translate(),
                    defaultDesc = "Rimpact_ChooseHediffSetDesc".Translate(),
                    icon = ChooseHediffSetIcon,
                    action = delegate
                    {
                        Find.WindowStack.Add(new Dialog_ChooseHediff(this, Holder));
                    }
                };
                if (Prefs.DevMode)
                {
                    yield return new Command_Action()
                    {
                        defaultLabel = "DEV: Reroll sets",
                        action = delegate
                        {
                            hediffStageDatas.Clear();
                            AddHediffSets();
                        }
                    };
                }
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_References.Look(ref hediff, "hediff");
            Scribe_References.Look(ref hediffDynamic, "hediffDynamic");
            Scribe_Collections.Look(ref hediffStageDatas, "hediffStageDatas", LookMode.Deep);
            Scribe_Deep.Look(ref appliedStageData, "appliedStageData");

            if (Scribe.mode == LoadSaveMode.PostLoadInit && Holder != null)
            {
                abilities = Holder.abilities.abilities.Where(x => Props.visionDef.abilities.Contains(x.def)).ToList();

                if (HasHediff) HediffForReading.pawn = Holder;
                if (hediffDynamic != null) HediffDynamicForReading.pawn = Holder;
                if (HasTrait) TraitForReading.pawn = Holder;
            }
        }
    }
}