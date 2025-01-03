using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Rimpact
{
    public class ElementDef : Def
    {
        public Color color = Color.white;
        public List<ElementCombo> reactsWith = [];
    }
}
