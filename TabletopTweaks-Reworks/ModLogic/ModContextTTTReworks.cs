using TabletopTweaks.Core.ModLogic;
using TabletopTweaks.MythicReoworks.Config;
using static UnityModManagerNet.UnityModManager;

namespace TabletopTweaks.Reworks.ModLogic {
    internal class ModContextTTTReworks : ModContextBase {
        public Homebrew Homebrew;

        public ModContextTTTReworks(ModEntry ModEntry) : base(ModEntry) {
            LoadAllSettings();
#if DEBUG
            Debug = true;
#endif
        }
        public override void LoadAllSettings() {
            LoadSettings("Homebrew.json", "TabletopTweaks.Reworks.Config", ref Homebrew);
            LoadBlueprints("TabletopTweaks.Reworks.Config", Main.TTTContext);
            LoadLocalization("TabletopTweaks.Reworks.Localization");
        }
        public override void AfterBlueprintCachePatches() {
            base.AfterBlueprintCachePatches();
            if (Debug) {
                Blueprints.RemoveUnused();
                SaveSettings(BlueprintsFile, Blueprints);
                ModLocalizationPack.RemoveUnused();
                SaveLocalization(ModLocalizationPack);
            }
        }
        public override void SaveAllSettings() {
            base.SaveAllSettings();
            SaveSettings("Homebrew.json", Homebrew);
        }
    }
}
