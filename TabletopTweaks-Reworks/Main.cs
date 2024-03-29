﻿using HarmonyLib;
using TabletopTweaks.Core.Utilities;
using TabletopTweaks.Reworks.ModLogic;
using UnityModManagerNet;

namespace TabletopTweaks.Reworks {
    static class Main {
        public static bool Enabled;
        public static ModContextTTTReworks TTTContext;
        static bool Load(UnityModManager.ModEntry modEntry) {
            var harmony = new Harmony(modEntry.Info.Id);
            TTTContext = new ModContextTTTReworks(modEntry);
            TTTContext.LoadAllSettings();
            TTTContext.ModEntry.OnSaveGUI = OnSaveGUI;
            TTTContext.ModEntry.OnGUI = UMMSettingsUI.OnGUI;
            harmony.PatchAll();
            PostPatchInitializer.Initialize(TTTContext);
            return true;
        }

        static void OnSaveGUI(UnityModManager.ModEntry modEntry) {
            TTTContext.SaveAllSettings();
        }
    }
}
