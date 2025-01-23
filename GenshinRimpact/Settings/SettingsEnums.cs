using System;
using Verse;

namespace Rimpact
{
    public static class SettingsEnums
    {
        public static string TranslateEnum(this Settings_VisionDropMode e)
        {
            return e switch
            {
                Settings_VisionDropMode.Intact => "Rimpact_enum_DropIntact".Translate(),
                Settings_VisionDropMode.Masterless => "Rimpact_enum_MakeMasterless".Translate(),
                Settings_VisionDropMode.Destroy => "Rimpact_enum_DestroyVision".Translate(),
                _ => throw new NotImplementedException(),
            };
        }
        public static string TranslateEnum(this Settings_VisionMasterlessMode e)
        {
            return e switch
            {
                Settings_VisionMasterlessMode.RandomBasicMixed => "Rimpact_enum_RandomBasicMixedAbilities".Translate(),
                Settings_VisionMasterlessMode.RandomPremadeMixed => "Rimpact_enum_RandomPremadeMixedAbilities".Translate(),
                Settings_VisionMasterlessMode.RandomBasicElemental => "Rimpact_enum_RandomBasicElementalAbilities".Translate(),
                Settings_VisionMasterlessMode.RandomPremadeElemental => "Rimpact_enum_RandomPremadeElementalAbilities".Translate(),
                Settings_VisionMasterlessMode.RandomPremadeVision => "Rimpact_enum_RandomPremadeVision".Translate(),
                _ => throw new NotImplementedException(),
            };
        }
    }

    public enum Settings_VisionDropMode
    {
        Intact,
        Masterless,
        Destroy
    }
    public enum Settings_VisionMasterlessMode
    {
        RandomBasicMixed,
        RandomPremadeMixed,
        RandomBasicElemental,
        RandomPremadeElemental,
        RandomPremadeVision
    }
}
