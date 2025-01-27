using LudeonTK;
using RimWorld;
using System.Text;
using Verse;

namespace Rimpact
{
    [HotSwap.HotSwappable]
    public static class DevTools
    {
        [DebugAction("Rimpact", "Log all reactions", allowedGameStates = AllowedGameStates.Entry)]
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

        [DebugAction("Rimpact", "Spawn all visions", allowedGameStates = AllowedGameStates.PlayingOnMap, actionType = DebugActionType.ToolMap)]
        public static void SpawnAllVisions()
        {
            foreach (var item in Utils.AllVisionsForReading)
            {
                GenPlace.TryPlaceThing(ThingMaker.MakeThing(item.Key), UI.MouseCell(), Find.CurrentMap, ThingPlaceMode.Near);
            }
        }

        [DebugAction("Rimpact", "Apply random stat hediff", allowedGameStates = AllowedGameStates.PlayingOnMap, actionType = DebugActionType.ToolMapForPawns)]
        public static void ApplyRandomStatHediff(Pawn p)
        {
            Hediff h = p.health.GetOrAddHediff(Rimpact_DefOf.GR_Hediff_Dynamic);
            if (h != null && h is HediffDynamic hdyn)
            {
                HediffStageData stageData = new()
                {
                    pawn = p,
                    statOffsets =
                    [
                        new() { stat = StatDefOf.SocialImpact, value = Rand.Range(-1f,1f)},
                        new() { stat = StatDefOf.ShootingAccuracyPawn, value = Rand.Range(-8,8) }
                    ],
                    statFactors =
                    [
                        new() { stat = StatDefOf.MeleeCooldownFactor, value = Rand.Range(0.1f,1.1f) }
                    ]
                };
                //Utils.DynamicHediffs.StagePawns.Add(stage);
                hdyn.ApplyValues("test hediff!!! " + Rand.Range(0, 1000), new(Rand.Range(0f,1f), Rand.Range(0f, 1f), Rand.Range(0f, 1f)), stageData);
                Utils.LogMessage("added " + stageData + " to " + hdyn);
                //comp.vision = null;
                //comp.shouldUpdate = true;
            }
            else
            {
                Utils.LogError("Failed to add dynamic hediff.");
            }
        }

        [DebugAction("Rimpact", "Max energy pool", allowedGameStates = AllowedGameStates.PlayingOnMap, actionType = DebugActionType.Action)]
        public static void MaxEnergyPool()
        {
            var comp = Find.CurrentMap.GetComponent<MapComponent_EnergyPool>();
            comp.energy = comp.maxEnergy;
        }
    }
}
