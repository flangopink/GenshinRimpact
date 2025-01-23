using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Rimpact
{
    [HotSwap.HotSwappable]
    public class MapComponent_EnergyPool(Map map) : MapComponent(map)
    {
        public readonly Dictionary<Pawn, float> PawnEnergyCostMultipliers = [];

        private Gizmo_EnergyPool gizmo;
        public Gizmo_EnergyPool Gizmo => gizmo ??= new Gizmo_EnergyPool(this);

        public float energy = -1f;
        public float maxEnergy = 100f;
        public float EnergyPoolPercentage => energy / Mathf.Max(1f, maxEnergy);


        public bool dontShowGizmo;

        public float regenRateMultiplier = 1f;
        public int RegenRate => (int)(Utils.settings.energyPoolRegenRateTicks * regenRateMultiplier);

        public override void FinalizeInit()
        {
            maxEnergy = Utils.settings.energyPoolMax;
        }

        public override void MapGenerated()
        {
            if (energy == -1f) energy = maxEnergy;
        }

        public override void MapComponentTick()
        {
            if (map.IsHashIntervalTick(RegenRate))
            {
                UseEnergy(-1);
            }
        }

        public override void MapComponentOnGUI()
        {
            dontShowGizmo = false;
        }

        public void UseEnergy(float usage)
        {
            energy = Mathf.Clamp(energy - usage, 0, maxEnergy);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref energy, "energy", 0f);
        }
    }
}
