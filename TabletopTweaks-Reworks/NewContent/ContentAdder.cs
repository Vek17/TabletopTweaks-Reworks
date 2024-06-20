using HarmonyLib;
using Kingmaker.Blueprints.JsonSystem;
using static TabletopTweaks.Reworks.Main;

namespace TabletopTweaks.Reworks.NewContent {
    class ContentAdder {
        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        static class BlueprintsCache_Init_Patch {
            static bool Initialized;

            [HarmonyPriority(799)]
            static void Postfix() {
                if (Initialized) return;
                Initialized = true;
                TTTContext.Logger.LogHeader("Loading New Content");

                MythicAbilities.DimensionalRetribution.AddDimensionalRetribution();
                MythicAbilities.SpellCastersOnslaught.AddSpellCastersOnslaught();
                Classes.Lich.AddLichFeatures();
                Classes.Aeon.AddAeonFeatures();
                Classes.Azata.AddAzataFeatures();
                Classes.Trickster.AddTricksterDomains();
                Classes.Trickster.AddTricksterTricks();
                //Classes.Demon.AddTESTDEMONPLEASEIGNORE();
            }
        }
    }
}
