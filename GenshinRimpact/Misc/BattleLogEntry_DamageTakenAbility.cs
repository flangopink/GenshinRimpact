using RimWorld;
using Verse;
using Verse.Grammar;

namespace GenshinRimpact
{
    public class BattleLogEntry_DamageTakenAbility : BattleLogEntry_DamageTaken
    {
        protected Pawn subjectPawn;
        protected Pawn initiatorPawn;
        protected RulePackDef ruleDef;
        protected AbilityDef abilityUsed;
        protected ThingDef initiatorThing;

        public BattleLogEntry_DamageTakenAbility() : base()
        {
        }

        public BattleLogEntry_DamageTakenAbility(Pawn recipient, RulePackDef ruleDef, AbilityDef ability, Thing initiator = null) : base(recipient, ruleDef, (Pawn)initiator)
        {
            abilityUsed = ability;
            subjectPawn = recipient;
            if (initiator is Pawn)
            {
                initiatorPawn = initiator as Pawn;
            }
            else if (initiator != null)
            {
                initiatorThing = initiator.def;
            }
            this.ruleDef = ruleDef;
        }

        protected override GrammarRequest GenerateGrammarRequest()
        {
            GrammarRequest result = default;
            result.Includes.Add(ruleDef);
            if (subjectPawn != null)
            {
                result.Rules.AddRange(GrammarUtility.RulesForPawn("SUBJECT", subjectPawn, result.Constants));
            }
            if (initiatorPawn != null)
            {
                result.Rules.AddRange(GrammarUtility.RulesForPawn("INITIATOR", initiatorPawn, result.Constants));
            }
            else if (initiatorThing != null)
            {
                result.Rules.AddRange(GrammarUtility.RulesForDef("INITIATOR", initiatorThing));
            }
            result.Rules.AddRange(GrammarUtility.RulesForDef("ABILITY", abilityUsed));
            if (subjectPawn == null)
            {
                result.Rules.Add(new Rule_String("SUBJECT_definite", "AreaLower".Translate()));
            }
            return result;
        }

        public override void ExposeData()
        {
            Scribe_Defs.Look(ref def, "def");
            Scribe_Defs.Look(ref ruleDef, "ruleDef");
            Scribe_Values.Look(ref logID, "logID", 0);
            Scribe_Values.Look(ref ticksAbs, "ticksAbs", 0);
            Scribe_Defs.Look(ref abilityUsed, "abilityUsed");
            Scribe_Defs.Look(ref initiatorThing, "initiatorThing");
            Scribe_References.Look(ref subjectPawn, "subjectPawn", saveDestroyedThings: true);
            Scribe_References.Look(ref initiatorPawn, "initiatorPawn", saveDestroyedThings: true);
        }
    }
}
