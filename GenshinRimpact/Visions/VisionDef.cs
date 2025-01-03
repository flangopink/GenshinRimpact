using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Rimpact
{
    public class VisionDef : Def
    {
        public ElementDef element;
        public List<AbilityDef> abilities = [];
        public HediffDef hediff;
        public TraitDef trait;
        public int traitDegree = 0;
        public bool mustBeCapableOfViolence = true;
    }
}
