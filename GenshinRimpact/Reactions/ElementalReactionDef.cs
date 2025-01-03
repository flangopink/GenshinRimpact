using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Rimpact
{
    public class ElementalReactionDef : Def
    {
        public Type reactionClass;

        public Color color = Color.white;

        public string iconPath;
        public Texture2D icon = BaseContent.BadTex;

        public string popupText; // Default is label

        //public List<ElementCombo> reqCombos = [];

        // Properties that may be useful
        public int durationTicks;

        public EffecterDef casterEffecter;
        public EffecterDef targetEffecter;
        public bool isEffecterMaintained;

        public HediffDef casterHediffDef;
        public HediffDef targetHediffDef;
        public float hediffSeverity;

        public DamageDef damageDef;
        public float damageAmount;

        public ElementDef spreadElement;

        public bool isExplosive;
        public bool requireLOS;
        public float effectRadius;
        public ThingCategory affectedThingCategories;

        public ThingDef spawnedThing;
        public ThingDef spawnedThingStuff;
        public EffecterDef spawnedThingEffecter;
        public int spawnedThingCount;
        public bool spawnThingNearCaster; // Default is near target
        public bool setFaction;

        public bool removesAllElements;
        public bool removesAllStatuses;

        public override void PostLoad()
        {
            base.PostLoad();
            LongEventHandler.ExecuteWhenFinished(delegate
            {
                if (typeof(ElementalReaction).IsAssignableFrom(reactionClass))
                {
                    ElementalReaction er = Activator.CreateInstance(reactionClass) as ElementalReaction;
                    er.Def = this;
                }
            });
            if (!iconPath.NullOrEmpty())
            {
                LongEventHandler.ExecuteWhenFinished(delegate
                {
                    icon = ContentFinder<Texture2D>.Get(iconPath);
                });
            }
        }

        public override IEnumerable<string> ConfigErrors()
        {
            foreach (var e in base.ConfigErrors()) yield return e;

            if (reactionClass == null)
            {
                yield return "has null reactionClass.";
            }
        }
    }
}
