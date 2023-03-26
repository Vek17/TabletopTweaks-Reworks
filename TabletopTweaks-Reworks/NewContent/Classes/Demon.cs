using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Class.LevelUp;
using System.Linq;
using TabletopTweaks.Core.Utilities;
using static TabletopTweaks.Reworks.Main;

namespace TabletopTweaks.Reworks.NewContent.Classes {
    static class Demon {
        public static void AddTESTDEMONPLEASEIGNORE() {
            var MythicFeatSelection = BlueprintTools.GetBlueprintReference<BlueprintFeatureBaseReference>("9ee0f6745f555484299b0a1563b99d81");
            var MythicAbilitySelection = BlueprintTools.GetBlueprintReference<BlueprintFeatureBaseReference>("ba0e5a900b775be4a99702f1ed08914d");

            var DemonMythicShifterClassTTT = Helpers.CreateBlueprint<BlueprintArchetype>(TTTContext, "DemonMythicShifterClassTTT", bp => {
                bp.SetName(ClassTools.Classes.DemonMythicClass.LocalizedName);
                bp.SetDescription(TTTContext, "");
            });

            DemonMythicShifterClassTTT.RemoveFeatures = ClassTools.Classes.DemonMythicClass.Progression.LevelEntries.Select(entry => {
                var result = Helpers.CreateLevelEntry(entry.Level,
                    entry.m_Features.Where(f => f.Guid != MythicFeatSelection.Guid && f.Guid != MythicAbilitySelection.Guid).ToArray());
                return result;
            }).ToArray();
            DemonMythicShifterClassTTT.AddFeatures = new LevelEntry[] { };

            //if (TTTContext.AddedContent.Archetypes.IsDisabled("HolyBeast")) { return; }

            //DemonMythicClass.m_Archetypes = DemonMythicClass.m_Archetypes.AppendToArray(TESTDEMONPLEASEIGNORE.ToReference<BlueprintArchetypeReference>());
            TTTContext.Logger.LogPatch("Added", DemonMythicShifterClassTTT);
        }
        //[HarmonyPatch(typeof(SelectClass), nameof(SelectClass.Apply))]
        static class CharInfoClassEntryVM_Constructor_Patch {
            static void Postfix(LevelUpState state, UnitDescriptor unit) {
                if (state.SelectedClass == null) { return; }
                if (!unit.IsPlayerFaction) { return; }
                var TESTDEMONPLEASEIGNORE = BlueprintTools.GetModBlueprint<BlueprintArchetype>(TTTContext, "TESTDEMONPLEASEIGNORE");
                if (!Game.Instance?.LevelUpController.Preview?.Progression.CanAddArchetype(state.SelectedClass, TESTDEMONPLEASEIGNORE) ?? true) { return; }
                if (Game.Instance.LevelUpController.Preview.Progression.IsArchetype(TESTDEMONPLEASEIGNORE)) { return; }
                Game.Instance?.LevelUpController.AddArchetype(TESTDEMONPLEASEIGNORE);
            }
        }
    }
}
