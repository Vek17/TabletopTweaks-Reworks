using TabletopTweaks.Core.ModLogic;
using TabletopTweaks.MythicReoworks.Config;
using static UnityModManagerNet.UnityModManager;

namespace TabletopTweaks.Reworks.ModLogic {
    internal class ModContextTTTMythicReworks : ModContextBase {
        public Homebrew Homebrew;

        public ModContextTTTMythicReworks(ModEntry ModEntry) : base(ModEntry) {
            LoadAllSettings();
        }
        public override void LoadAllSettings() {
            LoadSettings("Homebrew.json", "TabletopTweaks.Reworks.Config", ref Homebrew);
            LoadBlueprints("TabletopTweaks.Reworks.Config", Main.TTTContext);
            LoadLocalization("TabletopTweaks.Reworks.Localization");
        }
    }
}
