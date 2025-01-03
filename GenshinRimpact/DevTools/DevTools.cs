using LudeonTK;
using System.Text;
using Verse;

namespace Rimpact
{
    public static class DevTools
    {
        [DebugAction("Genshin Rimpact", "Log all reactions", allowedGameStates = AllowedGameStates.Entry)]
        public static void SkipAllCooldowns()
        {
            StringBuilder sb = new();
            sb.Append("=== All elemental reactions ===");
            foreach(var item in Utils.AllReactionsForReading)
            {
                var data = item.Key;
                sb.AppendInNewLine($"Elem1: {data.firstElement}, Elem2: {data.secondElement}, Status: {data.status}, Reaction: {data.reaction}, Reaction Class: {item.Value}");
            }
            Log.Message(sb.ToString());
        }

        [DebugAction("Genshin Rimpact", "Spawn all visions", allowedGameStates = AllowedGameStates.PlayingOnMap, actionType = DebugActionType.ToolMap)]
        public static void SpawnAllVisions()
        {
            foreach (var item in Utils.AllVisionsForReading)
            {
                GenPlace.TryPlaceThing(ThingMaker.MakeThing(item.Key), UI.MouseCell(), Find.CurrentMap, ThingPlaceMode.Near);
            }
        }
    }
}
