using Verse;

namespace Rimpact
{
    public class CompProperties_ElementalHandler : CompProperties
    {
        public CompProperties_ElementalHandler() => compClass = typeof(CompElementalHandler);
    }

    public class CompElementalHandler : ThingComp
    {
        //public List<Element> elements;
        public ElementDef currentElement;
        public CompProperties_ElementalHandler Props => (CompProperties_ElementalHandler)props;

        public void ApplyElement(ElementDef element, Thing instigator = null) 
        {
            if (element != null) 
            { 
                Log.Error("Tried applying a null element.");
                return;
            }
            // check for reactions here and apply reaction
            //elements.Add(element);
            currentElement = element;
            Log.Message($"Applied element: {element} to {parent}. Instigator: {instigator}");
        }

        public void ApplyReaction(ElementalReaction reaction, Thing instigator = null)
        {
            if (reaction == null)
            {
                Log.Error("Tried applying a null reaction.");
                return;
            }
            reaction.ApplyReaction(parent, instigator);
            Log.Message($"Caused reaction: {reaction} to {parent}. Instigator: {instigator}");

            // please add handling the elements list here. i.e. removing them on reaction.
        }

        public override string CompInspectStringExtra()
        {
            if (currentElement == null) return "";
            string str = "";
            /*for (int i = 0; i < elements.Count; i++)
                str += elements[i].ToString() + (i == elements.Count - 1 ? "" : ", ");*/
            str += "\nElement applied: " + currentElement.LabelCap; 
            return str;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            //Scribe_Collections.Look(ref elements, "elements");
            Scribe_Defs.Look(ref currentElement, "currentElement");
        }
    }
}
