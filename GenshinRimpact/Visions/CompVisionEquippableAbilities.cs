using RimWorld;
using System.Collections.Generic;
using Verse;

namespace GenshinRimpact
{
    public class CompProperties_VisionEquippableAbilities : CompProperties
    {
        public VisionDef visionDef;

        public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
        {
            if (visionDef == null) yield return parentDef + " has CompProperties_VisionEquippableAbilities, but no VisionDef assigned";
        }

        public CompProperties_VisionEquippableAbilities()
        {
            compClass = typeof(CompVisionEquippableAbilities);
        }
    }

    public class CompVisionEquippableAbilities : CompEquippable
    {
        public CompProperties_VisionEquippableAbilities Props => (CompProperties_VisionEquippableAbilities)props;

        private List<Ability> abilities = [];
        private string hediffLabel;
        private Hediff hediff;

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
        
        public bool HasHediff => Props.visionDef.hediff != null;
        public string HediffLabel => hediffLabel ??= Props.visionDef.hediff.LabelCap;
        public Hediff HediffForReading => hediff ??= HediffMaker.MakeHediff(Props.visionDef.hediff, Holder);

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
            }
        }

        public override string CompInspectStringExtra()
        {
            string str = "Abilities".Translate() + ":";
            foreach (Ability ab in AbilitiesForReading) 
                str += "\n• " + ab.def.label;
            if (HasHediff)
            {
                str += "\n" + "GR_Passives".Translate() + ":";
                str += "\n• " + HediffLabel;
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
            if (HasHediff) pawn.health.AddHediff(HediffForReading);
        }

        public override void Notify_Unequipped(Pawn pawn)
        {
            //Log.Message("unbonked");
            foreach (Ability ability in AbilitiesForReading)
            {
                pawn.abilities.abilities.Remove(ability);
            }
            pawn.abilities.Notify_TemporaryAbilitiesChanged();
            if (HasHediff) pawn.health.RemoveHediff(HediffForReading);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look(ref abilities, "abilities");
            Scribe_References.Look(ref hediff, "hediff");
            if (Scribe.mode == LoadSaveMode.PostLoadInit && Holder != null)
            {
                foreach(Ability ability in AbilitiesForReading)
                {
                    ability.pawn = Holder;
                    ability.verb.caster = Holder;
                }
                if (HasHediff) HediffForReading.pawn = Holder;
            }
        }
    }
}