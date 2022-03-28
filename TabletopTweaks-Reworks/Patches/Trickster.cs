using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.Utility;
using System.Linq;
using TabletopTweaks.Core.NewComponents;
using TabletopTweaks.Core.NewComponents.OwlcatReplacements;
using TabletopTweaks.Core.Utilities;
using static TabletopTweaks.Reworks.Main;

namespace TabletopTweaks.Reworks.Patches {
    class Trickster {
        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        static class BlueprintsCache_Init_Patch {
            static bool Initialized;

            static void Postfix() {
                if (Initialized) return;
                Initialized = true;
                TTTContext.Logger.LogHeader("Azata Rework");

                PatchTricksterStealthAbilityName();
                PatchTricksterStealth1();
                PatchTricksterStealth2();
            }
            static void PatchTricksterStealthAbilityName() {
                if (TTTContext.Homebrew.MythicReworks.Trickster.IsDisabled("TricksterStealthAbilityName")) { return; }
                var TricksterStealthTier1AbilityTarget = BlueprintTools.GetBlueprint<BlueprintAbility>("f131bc5d82f8b0a4b9bb28b2a176b8a8");

                TricksterStealthTier1AbilityTarget.SetName(TTTContext, "Trickster Stealth");
                TricksterStealthTier1AbilityTarget.SetDescription(TTTContext, "You can enter stealth during combat as a move action. " +
                    "This stealth is not broken by a single enemy detecting you — instead it acts similar to the invisibility spell, " +
                    "giving you total concealment against all creatures that did not succeed on a Perception check against you.\n" +
                    "This \"invisibility\" cannot be seen through by divination magic such as True Seeing, See Invisability, or Thoughtsense.");

                TTTContext.Logger.LogPatch("Patched", TricksterStealthTier1AbilityTarget);
            }
            static void PatchTricksterStealth1() {
                if (TTTContext.Homebrew.MythicReworks.Trickster.IsDisabled("TricksterStealth1")) { return; }
                var TricksterStealthTier1Feature = BlueprintTools.GetBlueprint<BlueprintFeature>("4e1948fed4201cf46b88836457c3bad8");
                var TricksterStealthTier1Buff = BlueprintTools.GetBlueprint<BlueprintBuff>("de4428f6828fbd14490a2d821d3063f8");

                var DivinationImmunityFeature = BlueprintTools.GetBlueprintReference<BlueprintUnitFactReference>("12435ed2443c41c78ac47b8ef076e997");

                TricksterStealthTier1Feature.SetDescription(TTTContext, "You can easily slip into shadow at any time. " +
                    "You can enter stealth during combat as a move action. This stealth is not broken by a single enemy " +
                    "detecting you — instead it acts similar to the invisibility spell, giving you total concealment against " +
                    "all creatures that did not succeed on a Perception check against you.\n" +
                    "This \"invisibility\" cannot be seen through by divination magic such as True Seeing, See Invisability, or Thoughtsense.");
                TricksterStealthTier1Buff.AddComponent<AddFacts>(c => {
                    c.m_Facts = new BlueprintUnitFactReference[] { DivinationImmunityFeature };
                });
                TTTContext.Logger.LogPatch("Patched", TricksterStealthTier1Feature);
                TTTContext.Logger.LogPatch("Patched", TricksterStealthTier1Buff);
            }
            static void PatchTricksterStealth2() {
                if (TTTContext.Homebrew.MythicReworks.Trickster.IsDisabled("TricksterStealth2")) { return; }
                var TricksterStealthTier2Feature = BlueprintTools.GetBlueprint<BlueprintFeature>("0d1071f085c5c4f408cd2bf1ec4f6adc");
                var TricksterStealthTier2Buff = BlueprintTools.GetBlueprint<BlueprintBuff>("cdfb5dca7d93d574cb15f59c0625059c");

                var DivinationImmunityFeature = BlueprintTools.GetBlueprintReference<BlueprintUnitFactReference>("12435ed2443c41c78ac47b8ef076e997");

                TricksterStealthTier2Feature.SetDescription(TTTContext, "You exceed at stealth, fading from sight with your every move. " +
                    "Your stealth in combat now works more akin to greater invisibility spell effect.\n" +
                    "This \"invisibility\" cannot be seen through by divination magic such as True Seeing, See Invisability, or Thoughtsense.");
                TricksterStealthTier2Buff.AddComponent<AddFacts>(c => {
                    c.m_Facts = new BlueprintUnitFactReference[] { DivinationImmunityFeature };
                });
                TTTContext.Logger.LogPatch("Patched", TricksterStealthTier2Feature);
                TTTContext.Logger.LogPatch("Patched", TricksterStealthTier2Buff);
            }
        }
    }
}
