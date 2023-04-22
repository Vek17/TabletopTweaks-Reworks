using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.FactLogic;
using TabletopTweaks.Core.NewComponents;
using TabletopTweaks.Core.NewComponents.AbilitySpecific;
using TabletopTweaks.Core.Utilities;
using static TabletopTweaks.Reworks.Main;

namespace TabletopTweaks.Reworks.Reworks {
    class MythicFeats {
        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        static class BlueprintsCache_Init_Patch {
            static bool Initialized;

            [HarmonyAfter(new string[] { "TabletopTweaks-Base" })]
            [HarmonyPostfix]
            static void Postfix() {
                if (Initialized) return;
                Initialized = true;
                TTTContext.Logger.LogHeader("Reworking Mythic Feats");
                PatchMythicImprovedCritical();
                PatchMythicSneakAttack();
                PatchSchoolMastery();
            }
            static void PatchMythicImprovedCritical() {
                if (TTTContext.Homebrew.MythicFeats.IsDisabled("MythicImprovedCritical")) { return; }

                var ImprovedCriticalMythicFeat = BlueprintTools.GetBlueprint<BlueprintParametrizedFeature>("8bc0190a4ec04bd489eec290aeaa6d07");

                ImprovedCriticalMythicFeat.TemporaryContext(bp => {
                    bp.SetDescription(TTTContext, "Your critical strikes with your chosen weapon are deadlier than most.\n" +
                        "Benefit: When you score a critical hit with your chosen weapon double the amount of weapon dice rolled.");
                    bp.RemoveComponents<ImprovedCriticalMythicParametrized>();
                    bp.AddComponent<ImprovedCriticalMythicParametrizedTTT>(c => {
                        c.DiceMultiplier = 2;
                    });
                });
                
                TTTContext.Logger.LogPatch(ImprovedCriticalMythicFeat);
            }
            static void PatchMythicSneakAttack() {
                if (TTTContext.Homebrew.MythicFeats.IsDisabled("MythicSneakAttack")) { return; }

                var SneakAttackerMythicFeat = BlueprintTools.GetBlueprint<BlueprintFeature>("d0a53bf03b978634890e5ebab4a90ecb");

                SneakAttackerMythicFeat.RemoveComponents<AddStatBonus>();
                SneakAttackerMythicFeat.AddComponent<MythicSneakAttack>();
                SneakAttackerMythicFeat.SetDescription(TTTContext, "Your sneak attacks are especially deadly.\n" +
                    "Benifit: Your sneak attack dice are one size larger than normal. " +
                    "For example if you would normally roll d6s for sneak attacks you would roll d8s instead.");
                TTTContext.Logger.LogPatch("Patched", SneakAttackerMythicFeat);
            }
            static void PatchSchoolMastery() {
                if (TTTContext.Homebrew.MythicFeats.IsDisabled("SchoolMastery")) { return; }
                var SchoolMasteryMythicFeat = BlueprintTools.GetBlueprint<BlueprintParametrizedFeature>("ac830015569352b458efcdfae00a948c");

                SchoolMasteryMythicFeat.TemporaryContext(bp => {
                    bp.SetDescription(TTTContext, "Select one school of magic. All spells you cast that belong to this school have their caster level increased by 2.");
                    if (bp.GetComponent<SchoolMasteryParametrized>()) {
                        bp.RemoveComponents<SchoolMasteryParametrized>();
                        bp.AddComponent<BonusCasterLevelParametrized>(c => {
                            c.Bonus = 2;
                            c.Descriptor = ModifierDescriptor.UntypedStackable;
                        });
                    } else if (bp.GetComponent<BonusCasterLevelParametrized>()) {
                        bp.GetComponent<BonusCasterLevelParametrized>().Bonus = 2;
                    }
                });

                TTTContext.Logger.LogPatch("Patched", SchoolMasteryMythicFeat);
            }
        }
    }
}
