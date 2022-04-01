using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Items.Ecnchantments;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.Utility;
using System.Linq;
using TabletopTweaks.Core.NewComponents;
using TabletopTweaks.Core.Utilities;
using static TabletopTweaks.Core.NewUnitParts.UnitPartCustomMechanicsFeatures;
using static TabletopTweaks.Reworks.Main;

namespace TabletopTweaks.Reworks.Patches {
    class Trickster {
        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        static class BlueprintsCache_Init_Patch {
            static bool Initialized;

            static void Postfix() {
                if (Initialized) return;
                Initialized = true;
                TTTContext.Logger.LogHeader("Trickster Rework");

                PatchTricksterProgression();
                PatchTricksterKnowledgeArcana2();
                PatchTricksterKnowledgeArcana3();
                PatchTricksterPerception1();
                PatchTricksterPerception2();
                PatchTricksterPerception3();
                PatchTricksterPersuasion2();
                PatchTricksterPersuasion3();
                PatchTricksterStealthAbilityName();
                PatchTricksterStealth1();
                PatchTricksterStealth2();
            }
            static void PatchTricksterProgression() {
                if (TTTContext.Homebrew.MythicReworks.Trickster.IsDisabled("Progression")) { return; }
                var TricksterProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("cc64789b0cc5df14b90da1ffee7bbeea");
                var TricksterRank1Selection = BlueprintTools.GetBlueprintReference<BlueprintFeatureBaseReference>("4fbc563529717de4d92052048143e0f1");
                var TricksterRank2Selection = BlueprintTools.GetBlueprintReference<BlueprintFeatureBaseReference>("5cd96c3460844fc458dc3e1656dafa42");
                var TricksterRank3Selection = BlueprintTools.GetBlueprintReference<BlueprintFeatureBaseReference>("446f4a8b32019f5478a8dfeddac74710");

                TricksterProgression.LevelEntries.TemporaryContext(entries => {
                    entries
                        .Where(e => e.Level == 3)
                        .ForEach(e => {
                            e.m_Features.Add(TricksterRank2Selection);
                        });
                    entries
                        .Where(e => e.Level == 7)
                        .ForEach(e => {
                            e.m_Features.Remove(f => f.Guid == TricksterRank2Selection.Guid);
                            e.m_Features.Add(TricksterRank3Selection);
                        });
                    entries
                        .Where(e => e.Level == 8)
                        .ForEach(e => {
                            e.m_Features.Remove(f => f.Guid == TricksterRank3Selection.Guid);
                            e.m_Features.Add(TricksterRank2Selection);
                        });
                });

                TTTContext.Logger.LogPatch("Patched", TricksterProgression);
            }
            static void PatchTricksterKnowledgeArcana2() {
                if (TTTContext.Homebrew.MythicReworks.Trickster.IsDisabled("TricksterKnowledgeArcana2")) { return; }
                var TricksterKnowledgeArcanaTier2Feature = BlueprintTools.GetBlueprint<BlueprintFeature>("7bbd9f681440a294382b527a554e419d");

                var Agile = BlueprintTools.GetBlueprintReference<BlueprintWeaponEnchantmentReference>("a36ad92c51789b44fa8a1c5c116a1328");
                var Bleed = BlueprintTools.GetBlueprintReference<BlueprintWeaponEnchantmentReference>("ac0108944bfaa7e48aa74f407e3944e3");
                var Corrosive = BlueprintTools.GetBlueprintReference<BlueprintWeaponEnchantmentReference>("633b38ff1d11de64a91d490c683ab1c8");
                var CruelEnchantment = BlueprintTools.GetBlueprintReference<BlueprintWeaponEnchantmentReference>("629c383ffb407224398bb71d1bd95d14");
                var Flaming = BlueprintTools.GetBlueprintReference<BlueprintWeaponEnchantmentReference>("30f90becaaac51f41bf56641966c4121");
                var Frost = BlueprintTools.GetBlueprintReference<BlueprintWeaponEnchantmentReference>("421e54078b7719d40915ce0672511d0b");
                var Furious = BlueprintTools.GetBlueprintReference<BlueprintWeaponEnchantmentReference>("b606a3f5daa76cc40add055613970d2a");
                var Furyborn = BlueprintTools.GetBlueprintReference<BlueprintWeaponEnchantmentReference>("091e2f6b2fad84a45ae76b8aac3c55c3");
                var GhostTouch = BlueprintTools.GetBlueprintReference<BlueprintWeaponEnchantmentReference>("47857e1a5a3ec1a46adf6491b1423b4f");
                var Heartseeker = BlueprintTools.GetBlueprintReference<BlueprintWeaponEnchantmentReference>("e252b26686ab66241afdf33f2adaead6");
                var Keen = BlueprintTools.GetBlueprintReference<BlueprintWeaponEnchantmentReference>("102a9c8c9b7a75e4fb5844e79deaf4c0");
                var Necrotic = BlueprintTools.GetBlueprintReference<BlueprintWeaponEnchantmentReference>("bad4134798e182c4487819dce9b43003");
                var Radiant = BlueprintTools.GetBlueprintReference<BlueprintWeaponEnchantmentReference>("5ac5c88157f7dde48a2a5b24caf40131");
                var Shock = BlueprintTools.GetBlueprintReference<BlueprintWeaponEnchantmentReference>("7bda5277d36ad114f9f9fd21d0dab658");
                var Thundering = BlueprintTools.GetBlueprintReference<BlueprintWeaponEnchantmentReference>("690e762f7704e1f4aa1ac69ef0ce6a96");
                var ViciousEnchantment = BlueprintTools.GetBlueprintReference<BlueprintWeaponEnchantmentReference>("a1455a289da208144981e4b1ef92cc56");

                TricksterKnowledgeArcanaTier2Feature
                    .GetComponent<TricksterArcanaAdditionalEnchantments>()
                    .WeaponEnchantments = new BlueprintWeaponEnchantmentReference[] {
                        Keen,
                        Agile,
                        Frost,
                        Flaming,
                        Corrosive,
                        Shock,
                        GhostTouch,
                        Heartseeker,
                        CruelEnchantment,
                        Bleed,
                        Furious,
                        Thundering,
                        Necrotic,
                        Radiant,
                        Furyborn,
                        ViciousEnchantment
                    };

                TTTContext.Logger.LogPatch("Patched", TricksterKnowledgeArcanaTier2Feature);
            }
            static void PatchTricksterKnowledgeArcana3() {
                if (TTTContext.Homebrew.MythicReworks.Trickster.IsDisabled("TricksterKnowledgeArcana3")) { return; }
                var TricksterKnowledgeArcanaTier3Feature = BlueprintTools.GetBlueprint<BlueprintFeature>("5e26c673173e423881e318d2f0ae84f0");

                var Axiomatic = BlueprintTools.GetBlueprintReference<BlueprintWeaponEnchantmentReference>("0ca43051edefcad4b9b2240aa36dc8d4");
                var BaneLiving = BlueprintTools.GetBlueprintReference<BlueprintWeaponEnchantmentReference>("e1d6f5e3cd3855b43a0cb42f6c747e1c");
                var BrilliantEnergy = BlueprintTools.GetBlueprintReference<BlueprintWeaponEnchantmentReference>("66e9e299c9002ea4bb65b6f300e43770");
                var Deteriorative = BlueprintTools.GetBlueprintReference<BlueprintWeaponEnchantmentReference>("bbe55d6e76b973d41bf5abeed643861d");
                var ElderBrass = BlueprintTools.GetBlueprintReference<BlueprintWeaponEnchantmentReference>("0a8f3559cfcc4d38961bd9658d026cc8");
                var ElderCorrosive2d6 = BlueprintTools.GetBlueprintReference<BlueprintWeaponEnchantmentReference>("55eece8fead0448aac01c44f37ea065a");
                var ElderIce2d6 = BlueprintTools.GetBlueprintReference<BlueprintWeaponEnchantmentReference>("c4c701aee76742188477a6f24505c222");
                var ElderShock2d6 = BlueprintTools.GetBlueprintReference<BlueprintWeaponEnchantmentReference>("5573c979b9dc403684166fe6e1c31c15");
                var Holy = BlueprintTools.GetBlueprintReference<BlueprintWeaponEnchantmentReference>("28a9964d81fedae44bae3ca45710c140");
                var Speed = BlueprintTools.GetBlueprintReference<BlueprintWeaponEnchantmentReference>("f1c0c50108025d546b2554674ea1c006");
                var Ultrasound = BlueprintTools.GetBlueprintReference<BlueprintWeaponEnchantmentReference>("582849db96824254ebcc68f0b7484e51");
                var Vorpal = BlueprintTools.GetBlueprintReference<BlueprintWeaponEnchantmentReference>("2f60bfcba52e48a479e4a69868e24ebc");

                TricksterKnowledgeArcanaTier3Feature
                    .GetComponent<TricksterArcanaAdditionalEnchantments>()
                    .WeaponEnchantments = new BlueprintWeaponEnchantmentReference[] {
                        ElderCorrosive2d6,
                        ElderBrass,
                        ElderIce2d6,
                        ElderShock2d6,
                        Axiomatic,
                        Holy,
                        Speed,
                        Ultrasound,
                        BrilliantEnergy,
                        Deteriorative,
                        BaneLiving,
                        Vorpal
                    };

             
                TTTContext.Logger.LogPatch("Patched", TricksterKnowledgeArcanaTier3Feature);
            }
            static void PatchTricksterPerception1() {
                if (TTTContext.Homebrew.MythicReworks.Trickster.IsDisabled("TricksterPerception1")) { return; }
                var TricksterPerceptionTier1Feature = BlueprintTools.GetBlueprint<BlueprintFeature>("8bc2f9b88a0cf704ea72d86c2a3e2aef");

                TricksterPerceptionTier1Feature.TemporaryContext(bp => {
                    bp.SetDescription(TTTContext, "You see more than other people.\n" +
                        "You are under a constant effect of the see invisibility spell, auto detect stealthing creatures, and reroll all concealment rolls.");
                    bp.AddComponent<AutoDetectStealth>();
                    bp.AddComponent<RerollConcealment>();
                });
                TTTContext.Logger.LogPatch("Patched", TricksterPerceptionTier1Feature);

            }
            static void PatchTricksterPerception2() {
                if (TTTContext.Homebrew.MythicReworks.Trickster.IsDisabled("TricksterPerception2")) { return; }
                var TricksterPerceptionTier2Feature = BlueprintTools.GetBlueprint<BlueprintFeature>("e9298851786c5334dba1398e9635a83d");

                TricksterPerceptionTier2Feature.TemporaryContext(bp => {
                    bp.SetDescription(TTTContext, "You can see the best ways to injure even the most resilient opponents.\n" +
                        "You ignore critical and sneak attack immunity, reroll all fortification checks and " +
                        "your critical hit range is increased by 2 with all weapons.");
                    bp.RemoveComponents<AutoDetectStealth>();
                    bp.AddComponent<IgnoreCritImmunity>();
                    bp.AddComponent<RerollFortification>();
                    bp.AddComponent<IncreaseCriticalRange>(c => {
                        c.CriticalRangeIncrease = 2;
                    });
                    bp.AddComponent<AddCustomMechanicsFeature>(c => {
                        c.Feature = CustomMechanicsFeature.BypassSneakAttackImmunity;
                    });
                });
                FeatTools.Selections.AllSelections
                    .Distinct()
                    .ForEach(selection => selection.RemoveFeatures(feature => {
                        var prerequisite = feature.GetComponent<PrerequisitePlayerHasFeature>();
                        if (prerequisite == null) { return false; }
                        return prerequisite.Feature == TricksterPerceptionTier2Feature;
                    }));

            TTTContext.Logger.LogPatch("Patched", TricksterPerceptionTier2Feature);
            }
            static void PatchTricksterPerception3() {
                if (TTTContext.Homebrew.MythicReworks.Trickster.IsDisabled("TricksterPerception3")) { return; }
                var TricksterPerceptionTier2Feature = BlueprintTools.GetBlueprint<BlueprintFeature>("e9298851786c5334dba1398e9635a83d");
                var TricksterPerceptionTier3Feature = BlueprintTools.GetBlueprint<BlueprintFeature>("c785d2718021449f895a960c7840b4d0");
                var TricksterPerception3Buff = BlueprintTools.GetModBlueprintReference<BlueprintUnitFactReference>(TTTContext, "TricksterPerception3Buff");

                TricksterPerceptionTier3Feature.TemporaryContext(bp => {
                    bp.SetDescription(TTTContext, "You sight has become so impecable that you can now see how to improve the actions of your allies.\n" +
                        "All allies within 60 feet of you gain the benifits of your perception tricks.");
                    bp.SetComponents();
                    bp.AddPrerequisiteFeature(TricksterPerceptionTier2Feature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] { TricksterPerception3Buff };
                    });
                });

                TTTContext.Logger.LogPatch("Patched", TricksterPerceptionTier3Feature);
            }
            static void PatchTricksterPersuasion2() {
            
            }
            static void PatchTricksterPersuasion3() {

            }
            static void PatchTricksterStealthAbilityName() {
                if (TTTContext.Homebrew.MythicReworks.Trickster.IsDisabled("TricksterStealthAbilityName")) { return; }
                var TricksterStealthTier1AbilityTarget = BlueprintTools.GetBlueprint<BlueprintAbility>("f131bc5d82f8b0a4b9bb28b2a176b8a8");

                TricksterStealthTier1AbilityTarget.SetName(TTTContext, "Trickster Stealth");
                TricksterStealthTier1AbilityTarget.SetDescription(TTTContext, "You can enter stealth during combat as a move action. " +
                    "This stealth is not broken by a single enemy detecting you — instead it acts similar to the invisibility spell, " +
                    "giving you total concealment against all creatures that did not succeed on a Perception check against you.\n" +
                    "This \"invisibility\" cannot be seen through by divination magic such as true seeing, see invisability, or thoughtsense.");

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
                    "This \"invisibility\" cannot be seen through by divination magic such as true seeing, see invisability, or thoughtsense.");
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
                    "This \"invisibility\" cannot be seen through by divination magic such as true seeing, see invisability, or thoughtsense.");
                TricksterStealthTier2Buff.AddComponent<AddFacts>(c => {
                    c.m_Facts = new BlueprintUnitFactReference[] { DivinationImmunityFeature };
                });
                TTTContext.Logger.LogPatch("Patched", TricksterStealthTier2Feature);
                TTTContext.Logger.LogPatch("Patched", TricksterStealthTier2Buff);
            }
            static void PatchTricksterTrickery1() {

            }
            static void PatchTricksterTrickery2() {

            }
            static void PatchTricksterTrickery3() {

            }
        }
    }
}
