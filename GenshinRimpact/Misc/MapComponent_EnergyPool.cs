using UnityEngine;
using Verse;

namespace Rimpact
{
    [HotSwap.HotSwappable]
    public class MapComponent_EnergyPool(Map map) : MapComponent(map)
    {
        public float energy = 50f;
        public float maxEnergy = 100f;

        private Gizmo_EnergyPool gizmo;
        public Gizmo_EnergyPool Gizmo => gizmo ??= new Gizmo_EnergyPool(this);
        public bool dontShowGizmo;

        public float EnergyPoolPercentage => energy / Mathf.Max(1f, maxEnergy);

        /*public override void MapComponentOnGUI()
        {
            Vector2 vector = Event.current.mousePosition + new Vector2(15f, 15f);
            Rect rect = new(vector.x, vector.y, 999f, 999f);
            Text.Font = GameFont.Small;
            DevGUI.Label(rect, energy.ToString());
        }*/

        public override void FinalizeInit()
        {
            maxEnergy = Utils.settings.energyPoolMax;
        }

        public override void MapComponentTick()
        {
            if (map.IsHashIntervalTick(Utils.settings.energyPoolRegenRateTicks))
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
