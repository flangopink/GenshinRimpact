using LudeonTK;
using System.Text;
using Verse;

namespace GenshinRimpact
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
    }
}
