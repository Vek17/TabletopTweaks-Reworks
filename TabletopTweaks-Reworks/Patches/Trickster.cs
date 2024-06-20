using HarmonyLib;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using TabletopTweaks.Core.NewComponents;
using TabletopTweaks.Core.NewComponents.AbilitySpecific;
using TabletopTweaks.Core.NewComponents.OwlcatReplacements;
using TabletopTweaks.Core.NewUnitParts;
using TabletopTweaks.Core.Utilities;
using TabletopTweaks.Reworks.Config.LootTables;
using static TabletopTweaks.Core.NewUnitParts.UnitPartCustomMechanicsFeatures;
using static TabletopTweaks.Reworks.Main;

namespace TabletopTweaks.Reworks.Patches {
    internal static class Trickster {
        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        static class BlueprintsCache_Init_Patch {
            static bool Initialized;

            [HarmonyAfter(new string[] { "TabletopTweaks-Base" })]
            [HarmonyPostfix]
            static void Postfix() {
                if (Initialized) return;
                Initialized = true;
                TTTContext.Logger.LogHeader("Trickster Rework");

                PatchBoundOfPossibility();
                PatchTricksterProgression();
                PatchTricksterKnowledgeAthletics();
                PatchTricksterKnowledgeArcana();
                PatchTricksterKnowledgeWorld();
                PatchTricksterLoreNature();
                PatchTricksterLoreReligion();
                PatchTricksterMobility();
                PatchTricksterPerception();
                PatchTricksterPersuasion();
                PatchTricksterStealth();
                PatchTricksterTrickery();
                PatchTricksterUseMagicDevice();
            }
            static void PatchBoundOfPossibility() {
                if (TTTContext.Homebrew.MythicReworks.Trickster.IsDisabled("BoundOfPossibility")) { return; }

                var Artifact_TricksterCloakItem = BlueprintTools.GetBlueprint<BlueprintItemEquipmentShoulders>("50b398d4630d9f244a5db288124ff181");
                var Artifact_TricksterCloakItemFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("686fb5d5b4264e8d85f434efd9e7c5de");

                Artifact_TricksterCloakItem.SetDescription(TTTContext, "This cloak allows Trickster to roll any skill check twice and take the best result.");
                Artifact_TricksterCloakItemFeature.SetComponents();
                Artifact_TricksterCloakItemFeature.AddComponent<ModifyD20>(c => {
                    c.Rule = RuleType.SkillCheck;
                    c.TakeBest = true;
                    c.RollsAmount = 1;
                    c.RollResult = new ContextValue();
                    c.Bonus = new ContextValue();
                    c.Chance = new ContextValue();
                    c.ValueToCompareRoll = new ContextValue();
                    c.Skill = new StatType[0];
                    c.Value = new ContextValue();
                });

                TTTContext.Logger.LogPatch(Artifact_TricksterCloakItem);
                TTTContext.Logger.LogPatch(Artifact_TricksterCloakItemFeature);
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
            static void PatchTricksterKnowledgeAthletics() {
                var TricksterAthleticsTier1Feature = BlueprintTools.GetBlueprint<BlueprintFeature>("b7de8e5e4d67faf4791866966afc0a43");
                var TricksterAthleticsTier2Feature = BlueprintTools.GetBlueprint<BlueprintFeature>("198b6abf58a36404f8189787a082fa02");
                var TricksterAthleticsTier3Feature = BlueprintTools.GetBlueprint<BlueprintFeature>("e45bf795c4f84c3b8a83c011f8580491");

                var Icon_TricksterAthletics = AssetLoader.LoadInternal(TTTContext, "TricksterTricks", "Icon_TricksterAthletics.png");

                PatchIcons();
                PatchTricksterAthletics1();
                PatchTricksterAthletics2();
                PatchTricksterAthletics3();

                void PatchIcons() {
                    if (TTTContext.Homebrew.MythicReworks.Trickster.IsDisabled("UpdateIcons")) { return; }

                    TricksterAthleticsTier1Feature.m_Icon = Icon_TricksterAthletics;
                    TricksterAthleticsTier2Feature.m_Icon = Icon_TricksterAthletics;
                    TricksterAthleticsTier3Feature.m_Icon = Icon_TricksterAthletics;
                }
                void PatchTricksterAthletics1() {
                    TricksterAthleticsTier1Feature.SetName(TTTContext, "Athletics 1 rank");
                }
                void PatchTricksterAthletics2() {
                    TricksterAthleticsTier2Feature.SetName(TTTContext, "Athletics 2 rank");
                }
                void PatchTricksterAthletics3() {
                    TricksterAthleticsTier3Feature.SetName(TTTContext, "Athletics 3 rank");
                }
            }
            static void PatchTricksterKnowledgeArcana() {
                var TricksterKnowledgeArcanaTier1Feature = BlueprintTools.GetBlueprint<BlueprintFeature>("c7bb946de7454df4380c489a8350ba38");
                var TricksterKnowledgeArcanaTier2Feature = BlueprintTools.GetBlueprint<BlueprintFeature>("7bbd9f681440a294382b527a554e419d");
                var TricksterKnowledgeArcanaTier3Feature = BlueprintTools.GetBlueprint<BlueprintFeature>("5e26c673173e423881e318d2f0ae84f0");

                var Icon_TricksterKnowledgeArcana = AssetLoader.LoadInternal(TTTContext, "TricksterTricks", "Icon_TricksterKnowledgeArcana.png");

                PatchIcons();
                PatchTricksterKnowledgeArcana2();
                PatchTricksterKnowledgeArcana3();

                void PatchIcons() {
                    if (TTTContext.Homebrew.MythicReworks.Trickster.IsDisabled("UpdateIcons")) { return; }

                    TricksterKnowledgeArcanaTier1Feature.m_Icon = Icon_TricksterKnowledgeArcana;
                    TricksterKnowledgeArcanaTier2Feature.m_Icon = Icon_TricksterKnowledgeArcana;
                    TricksterKnowledgeArcanaTier3Feature.m_Icon = Icon_TricksterKnowledgeArcana;
                }
                void PatchTricksterKnowledgeArcana2() {
                    if (TTTContext.Homebrew.MythicReworks.Trickster.IsDisabled("TricksterKnowledgeArcana2")) { return; }

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
                    var NullifyingEnchantment = BlueprintTools.GetBlueprintReference<BlueprintWeaponEnchantmentReference>("efbe3a35fc7349845ac9f96b4c63312e");
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
                            NullifyingEnchantment,
                            Radiant,
                            Furyborn,
                            ViciousEnchantment
                        };

                    TTTContext.Logger.LogPatch("Patched", TricksterKnowledgeArcanaTier2Feature);
                }
                void PatchTricksterKnowledgeArcana3() {
                    if (TTTContext.Homebrew.MythicReworks.Trickster.IsDisabled("TricksterKnowledgeArcana3")) { return; }


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
            }
            static void PatchTricksterKnowledgeWorld() {
                var TricksterKnowledgeWorldTier1Feature = BlueprintTools.GetBlueprint<BlueprintFeature>("06983a66bd6bac04db3996cd3064d9f0");
                var TricksterKnowledgeWorldTier2Feature = BlueprintTools.GetBlueprint<BlueprintFeature>("8b6fe337865492645892cc8db5dd0e01");
                var TricksterKnowledgeWorldTier3Feature = BlueprintTools.GetBlueprint<BlueprintFeature>("26691ec239c84568bd27b055a1528912");

                var Icon_TricksterKnowledgeWorld = AssetLoader.LoadInternal(TTTContext, "TricksterTricks", "Icon_TricksterKnowledgeWorld.png");

                PatchIcons();
                PatchTricksterKnowledgeWorld1();
                PatchTricksterKnowledgeWorld2();
                PatchTricksterKnowledgeWorld3();

                void PatchIcons() {
                    if (TTTContext.Homebrew.MythicReworks.Trickster.IsDisabled("UpdateIcons")) { return; }

                    TricksterKnowledgeWorldTier1Feature.m_Icon = Icon_TricksterKnowledgeWorld;
                    TricksterKnowledgeWorldTier2Feature.m_Icon = Icon_TricksterKnowledgeWorld;
                    TricksterKnowledgeWorldTier3Feature.m_Icon = Icon_TricksterKnowledgeWorld;
                }
                void PatchTricksterKnowledgeWorld1() {

                }
                void PatchTricksterKnowledgeWorld2() {

                }
                void PatchTricksterKnowledgeWorld3() {

                }
            }
            static void PatchTricksterLoreNature() {
                var TricksterLoreNature1Feature = BlueprintTools.GetBlueprint<BlueprintFeature>("cb232b9ed5c216242a667e95527ad8e1");
                var TricksterLoreNature2Feature = BlueprintTools.GetBlueprint<BlueprintFeature>("9c437a5ba123619448322642ffbdc9e1");
                var TricksterLoreNature3Feature = BlueprintTools.GetBlueprint<BlueprintFeature>("b88ca3a5476ebcc4ea63d5c1e92ce222");

                var Icon_TricksterLoreNature = AssetLoader.LoadInternal(TTTContext, "TricksterTricks", "Icon_TricksterLoreNature.png");

                PatchIcons();
                PatchTricksterLoreNature1();
                PatchTricksterLoreNature2();
                PatchTricksterLoreNature3();

                void PatchIcons() {
                    if (TTTContext.Homebrew.MythicReworks.Trickster.IsDisabled("UpdateIcons")) { return; }

                    TricksterLoreNature1Feature.m_Icon = Icon_TricksterLoreNature;
                    TricksterLoreNature2Feature.m_Icon = Icon_TricksterLoreNature;
                    TricksterLoreNature3Feature.m_Icon = Icon_TricksterLoreNature;
                }
                void PatchTricksterLoreNature1() {

                }
                void PatchTricksterLoreNature2() {

                }
                void PatchTricksterLoreNature3() {
                    if (TTTContext.Homebrew.MythicReworks.Trickster.IsDisabled("TricksterLoreNature3")) { return; }

                    var lootTablePath = "TabletopTweaks.Reworks.Config.LootTables";
                    var lootTables = new List<LootTable>() {
                        LootTable.LoadTable("loot_Armor.json", lootTablePath),
                        LootTable.LoadTable("loot_Belt.json", lootTablePath),
                        LootTable.LoadTable("loot_Feet.json", lootTablePath),
                        LootTable.LoadTable("loot_Glasses.json", lootTablePath),
                        LootTable.LoadTable("loot_Gloves.json", lootTablePath),
                        LootTable.LoadTable("loot_Head.json", lootTablePath),
                        LootTable.LoadTable("loot_Neck.json", lootTablePath),
                        LootTable.LoadTable("loot_Ring.json", lootTablePath),
                        LootTable.LoadTable("loot_Shield.json", lootTablePath),
                        LootTable.LoadTable("loot_Shirt.json", lootTablePath),
                        LootTable.LoadTable("loot_Shoulders.json", lootTablePath),
                        LootTable.LoadTable("loot_Usable.json", lootTablePath),
                        LootTable.LoadTable("loot_Weapon.json", lootTablePath),
                        LootTable.LoadTable("loot_Wrist.json", lootTablePath)
                    };
                    if (Harmony.HasAnyPatches("TabletopTweaks-Base")) {
                        lootTables.Add(LootTable.LoadTable("loot_TTTBase.json", lootTablePath));
                    }
                    var loreNature3LootList = new List<BlueprintItemEquipmentReference>();
                    TricksterLoreNature3Feature.SetComponents();
                    TricksterLoreNature3Feature.AddPrerequisiteFeature(TricksterLoreNature2Feature);
                    TricksterLoreNature3Feature.AddComponent<TricksterLoreNatureRestLootTriggerTTT>(c => {
                        c.m_LootList = lootTables.SelectMany(table => table.Items).ToArray();
                        c.CROffset = 5;
                        c.CRRange = 15;
                        c.CostFloor = 20_000;
                        c.TableRolls = 3;
                        c.AddBest = 1;
                    });
                    TTTContext.Logger.LogPatch(TricksterLoreNature3Feature);
                }
            }
            static void PatchTricksterLoreReligion() {
                var TricksterLoreReligionTier1Feature = BlueprintTools.GetBlueprint<BlueprintFeature>("04177c4ddec20ae4ca04388f9cf23518");
                var TricksterLoreReligionTier2Progression = BlueprintTools.GetBlueprint<BlueprintFeature>("fd158d5f035346288776f18e76f6721e");
                var TricksterLoreReligionTier3Progression = BlueprintTools.GetBlueprint<BlueprintFeature>("eb196db561554341a571acb3216fc1dc");


                var Icon_TricksterLoreReligion = AssetLoader.LoadInternal(TTTContext, "TricksterTricks", "Icon_TricksterLoreReligion.png");

                PatchIcons();
                PatchTricksterLoreReligion1();
                PatchTricksterLoreReligion2();
                PatchTricksterLoreReligion3();

                void PatchIcons() {
                    if (TTTContext.Homebrew.MythicReworks.Trickster.IsDisabled("UpdateIcons")) { return; }

                    TricksterLoreReligionTier1Feature.m_Icon = Icon_TricksterLoreReligion;
                    TricksterLoreReligionTier2Progression.m_Icon = Icon_TricksterLoreReligion;
                    TricksterLoreReligionTier3Progression.m_Icon = Icon_TricksterLoreReligion;
                }
                void PatchTricksterLoreReligion1() {

                }
                void PatchTricksterLoreReligion2() {
                    if (TTTContext.Homebrew.MythicReworks.Trickster.IsDisabled("TricksterLoreReligion2/3")) { return; }

                    var DomainMastery = BlueprintTools.GetBlueprint<BlueprintFeature>("2de64f6a1f2baee4f9b7e52e3f046ec5");
                    var TricksterLoreReligionTier2Selection = BlueprintTools.GetBlueprint<BlueprintFeatureSelection>("ae4e619162a44996b77973f3abd7781a");

                    TricksterLoreReligionTier2Selection.m_AllFeatures = new BlueprintFeatureReference[0];
                    TricksterLoreReligionTier2Selection.m_Features = new BlueprintFeatureReference[0];
                    TricksterLoreReligionTier2Selection.IgnorePrerequisites = false;
                    TricksterLoreReligionTier2Selection.AddFeatures(NewContent.Classes.Trickster.TricksterDomains.ToArray());
                    TricksterLoreReligionTier2Progression.SetDescription(TTTContext, "You now know religion so well that you can use abilities usually reserved for clerics. " +
                        "You can select two domains. You gain abilities of those domains and can use spells from those domains once per day.\n" +
                        "Your effective cleric level for the purposes of these domains and spells is equal to your character level, " +
                        "and your effective wisdom modifier is equal to your mythic rank.");

                    DomainMastery.AddPrerequisiteFeature(TricksterLoreReligionTier2Progression, Prerequisite.GroupType.Any);

                    TTTContext.Logger.LogPatch(TricksterLoreReligionTier2Selection);
                    TTTContext.Logger.LogPatch(DomainMastery);
                }
                void PatchTricksterLoreReligion3() {
                    if (TTTContext.Homebrew.MythicReworks.Trickster.IsDisabled("TricksterLoreReligion2/3")) { return; }

                    var DomainMastery = BlueprintTools.GetBlueprint<BlueprintFeature>("2de64f6a1f2baee4f9b7e52e3f046ec5");
                    var TricksterLoreReligionTier3Selection = BlueprintTools.GetBlueprint<BlueprintFeatureSelection>("70a7b101edc24349ab67ac63b6bd0616");

                    TricksterLoreReligionTier3Selection.m_AllFeatures = new BlueprintFeatureReference[0];
                    TricksterLoreReligionTier3Selection.m_Features = new BlueprintFeatureReference[0];
                    TricksterLoreReligionTier3Selection.IgnorePrerequisites = false;
                    TricksterLoreReligionTier3Selection.AddFeatures(NewContent.Classes.Trickster.TricksterDomains.ToArray());
                    TricksterLoreReligionTier3Progression.SetDescription(TTTContext, "You now know enough about religion to start your own. You can select two additional domains.\n" +
                        "Your effective cleric level for the purposes of these domains and spells is equal to your character level, " +
                        "and your effective wisdom modifier is equal to your mythic rank.");

                    DomainMastery.AddPrerequisiteFeature(TricksterLoreReligionTier3Progression, Prerequisite.GroupType.Any);

                    TTTContext.Logger.LogPatch(TricksterLoreReligionTier3Selection);
                    TTTContext.Logger.LogPatch(DomainMastery);
                }
            }
            static void PatchTricksterMobility() {
                var TricksterMobilityTier1Feature = BlueprintTools.GetBlueprint<BlueprintFeature>("6079a3c329989a64cad3fbd4076d5fb9");
                var TricksterMobilityTier2Feature = BlueprintTools.GetBlueprint<BlueprintFeature>("d6bf5da423ddc7d4cb9cf46c5dbb7eb3");
                var TricksterMobilityTier3Feature = BlueprintTools.GetBlueprint<BlueprintFeature>("6db3651d9af54f28b5a3a5570f49f349");

                var Icon_TricksterMobility = AssetLoader.LoadInternal(TTTContext, "TricksterTricks", "Icon_TricksterMobility.png");

                PatchIcons();
                PatchTricksterMobility1();
                PatchTricksterMobility2();
                PatchTricksterMobility3();

                void PatchIcons() {
                    if (TTTContext.Homebrew.MythicReworks.Trickster.IsDisabled("UpdateIcons")) { return; }

                    TricksterMobilityTier1Feature.m_Icon = Icon_TricksterMobility;
                    TricksterMobilityTier2Feature.m_Icon = Icon_TricksterMobility;
                    TricksterMobilityTier3Feature.m_Icon = Icon_TricksterMobility;
                }
                void PatchTricksterMobility1() {
                    TricksterMobilityTier1Feature.SetName(TTTContext, "Mobility 1 rank");
                }
                void PatchTricksterMobility2() {
                    TricksterMobilityTier2Feature.SetName(TTTContext, "Mobility 2 rank");
                }
                void PatchTricksterMobility3() {
                    TricksterMobilityTier3Feature.SetName(TTTContext, "Mobility 3 rank");
                    if (TTTContext.Homebrew.MythicReworks.Trickster.IsDisabled("TricksterMobility3")) { return; }

                    TricksterMobilityTier3Feature.RemoveComponents<TricksterParry>();
                    TricksterMobilityTier3Feature.AddComponent<TricksterParryTTT>();

                    TTTContext.Logger.LogPatch(TricksterMobilityTier3Feature);
                }
            }
            static void PatchTricksterPerception() {
                var TricksterPerceptionTier1Feature = BlueprintTools.GetBlueprint<BlueprintFeature>("8bc2f9b88a0cf704ea72d86c2a3e2aef");
                var TricksterPerceptionTier2Feature = BlueprintTools.GetBlueprint<BlueprintFeature>("e9298851786c5334dba1398e9635a83d");
                var TricksterPerceptionTier3Feature = BlueprintTools.GetBlueprint<BlueprintFeature>("c785d2718021449f895a960c7840b4d0");

                var Icon_TricksterPerception = AssetLoader.LoadInternal(TTTContext, "TricksterTricks", "Icon_TricksterPerception.png");

                PatchIcons();
                PatchTricksterPerception1();
                PatchTricksterPerception2();
                PatchTricksterPerception3();

                void PatchIcons() {
                    if (TTTContext.Homebrew.MythicReworks.Trickster.IsDisabled("UpdateIcons")) { return; }

                    TricksterPerceptionTier1Feature.m_Icon = Icon_TricksterPerception;
                    TricksterPerceptionTier2Feature.m_Icon = Icon_TricksterPerception;
                    TricksterPerceptionTier3Feature.m_Icon = Icon_TricksterPerception;
                }
                void PatchTricksterPerception1() {
                    if (TTTContext.Homebrew.MythicReworks.Trickster.IsDisabled("TricksterPerception1")) { return; }

                    TricksterPerceptionTier1Feature.TemporaryContext(bp => {
                        bp.SetDescription(TTTContext, "You see more than other people.\n" +
                            "You are under a constant effect of the see invisibility spell, auto detect stealthing creatures, and reroll all concealment rolls.");
                        bp.AddComponent<AutoDetectStealth>();
                        bp.AddComponent<RerollConcealment>();
                    });
                    TTTContext.Logger.LogPatch("Patched", TricksterPerceptionTier1Feature);

                }
                void PatchTricksterPerception2() {
                    if (TTTContext.Homebrew.MythicReworks.Trickster.IsDisabled("TricksterPerception2")) { return; }

                    TricksterPerceptionTier2Feature.TemporaryContext(bp => {
                        bp.SetDescription(TTTContext, "You can see the best ways to injure even the most resilient opponents.\n" +
                            "You ignore critical and sneak attack immunity, reroll all fortification checks and " +
                            "your critical hit range is increased by 2 with all weapons. " +
                            "This critical range increase is not multiplied by improved critical or similar effects.");
                        bp.RemoveComponents<AutoDetectStealth>();
                        bp.AddComponent<IgnoreCritImmunity>();
                        bp.AddComponent<RerollFortification>();
                        bp.AddComponent<AddFlatCriticalRangeIncrease>(c => {
                            c.CriticalRangeIncrease = 2;
                            c.AllWeapons = true;
                        });
                        bp.AddComponent<AddCustomMechanicsFeature>(c => {
                            c.Feature = CustomMechanicsFeature.BypassSneakAttackImmunity;
                        });
                        bp.AddComponent<AddCustomMechanicsFeature>(c => {
                            c.Feature = CustomMechanicsFeature.BypassCriticalHitImmunity;
                        });
                    });
                    FeatTools.Selections.AllSelections
                        .Distinct()
                        .ForEach(selection => selection.RemoveFeatures(feature => {
                            var prerequisite = feature.GetComponent<PrerequisitePlayerHasFeature>();
                            if (prerequisite == null) { return false; }
                            var test = prerequisite.Feature == TricksterPerceptionTier2Feature;
                            Main.TTTContext.Logger.Log($"REMOVING TRICKSTER FEATS: {selection.name} - {feature.name} - {prerequisite.Feature.name} - {test}");
                            return prerequisite.Feature == TricksterPerceptionTier2Feature;
                        }));

                    TTTContext.Logger.LogPatch("Patched", TricksterPerceptionTier2Feature);
                }
                void PatchTricksterPerception3() {
                    if (TTTContext.Homebrew.MythicReworks.Trickster.IsDisabled("TricksterPerception3")) { return; }
                    var TricksterPerception3Buff = BlueprintTools.GetModBlueprintReference<BlueprintUnitFactReference>(TTTContext, "TricksterPerception3Buff");

                    TricksterPerceptionTier3Feature.TemporaryContext(bp => {
                        bp.SetDescription(TTTContext, "You sight has become so impeccable that you can now see how to improve the actions of your allies.\n" +
                            "All allies within 60 feet of you gain the benefits of your perception tricks.");
                        bp.SetComponents();
                        bp.AddPrerequisiteFeature(TricksterPerceptionTier2Feature);
                        bp.AddComponent<AddFacts>(c => {
                            c.m_Facts = new BlueprintUnitFactReference[] { TricksterPerception3Buff };
                        });
                    });

                    TTTContext.Logger.LogPatch("Patched", TricksterPerceptionTier3Feature);
                }
            }
            static void PatchTricksterPersuasion() {
                var TricksterPersuasionTier1Feature = BlueprintTools.GetBlueprint<BlueprintFeature>("4eefa883f5908a347a0b0a891fb859dd");
                var TricksterPersuasionTier2Feature = BlueprintTools.GetBlueprint<BlueprintFeature>("420bd51aa7d96fc4a8d3cc3bd7cddaa1");
                var TricksterPersuasionTier3Feature = BlueprintTools.GetBlueprint<BlueprintFeature>("9f677bc314b84cc388044c3786148fd9");

                var Icon_TricksterPersausion = AssetLoader.LoadInternal(TTTContext, "TricksterTricks", "Icon_TricksterPersausion.png");

                PatchIcons();
                PatchTricksterPersuasion1();
                PatchTricksterPersuasion2();
                PatchTricksterPersuasion3();

                void PatchIcons() {
                    if (TTTContext.Homebrew.MythicReworks.Trickster.IsDisabled("UpdateIcons")) { return; }

                    TricksterPersuasionTier1Feature.m_Icon = Icon_TricksterPersausion;
                    TricksterPersuasionTier2Feature.m_Icon = Icon_TricksterPersausion;
                    TricksterPersuasionTier3Feature.m_Icon = Icon_TricksterPersausion;
                }
                void PatchTricksterPersuasion1() {
                }
                void PatchTricksterPersuasion2() {
                    if (TTTContext.Homebrew.MythicReworks.Trickster.IsDisabled("TricksterPersuasion2")) { return; }

                    TricksterPersuasionTier2Feature.SetDescription(TTTContext, "You are so good at demoralizing your enemies that their will to hurt you wavers.\n" +
                        "Enemies affected by your demoralize ability must succeed at a Will saving throw with a DC of 10 + your ranks in Persuasion, " +
                        "or become staggered for one round. Additionally, when you successfully demoralize an enemy they take an additional penalty " +
                        "to their attack and damage rolls equal to 1 + half your mythic rank.");
                    TricksterPersuasionTier2Feature.RemoveComponents<AddMechanicsFeature>();
                    TricksterPersuasionTier2Feature.AddComponent<AddCustomMechanicsFeature>(c => {
                        c.Feature = CustomMechanicsFeature.TricksterReworkPersuasion2;
                    });

                    TTTContext.Logger.LogPatch(TricksterPersuasionTier2Feature);
                }
                void PatchTricksterPersuasion3() {
                    if (TTTContext.Homebrew.MythicReworks.Trickster.IsDisabled("TricksterPersuasion3")) { return; }

                    TricksterPersuasionTier3Feature.SetDescription(TTTContext, "You are so good at demoralizing your enemies that they panic and fail to defend themselves.\n" +
                        "Enemies affected by your demoralize have a 50% chance to attack the nearest target instead of acting normally. " +
                        "Additionally, when you successfully demoralize an enemy they take an additional penalty " +
                        "to their AC and spell resistance equal to your mythic rank, as well as a penalty all saving throws equal to 1 + half your mythic rank.");
                    TricksterPersuasionTier3Feature.RemoveComponents<AddMechanicsFeature>();
                    TricksterPersuasionTier3Feature.AddComponent<AddCustomMechanicsFeature>(c => {
                        c.Feature = CustomMechanicsFeature.TricksterReworkPersuasion3;
                    });

                    TTTContext.Logger.LogPatch(TricksterPersuasionTier3Feature);
                }
            }
            static void PatchTricksterStealth() {
                var TricksterStealthTier1AbilityTarget = BlueprintTools.GetBlueprint<BlueprintAbility>("f131bc5d82f8b0a4b9bb28b2a176b8a8");
                var TricksterStealthTier1Feature = BlueprintTools.GetBlueprint<BlueprintFeature>("4e1948fed4201cf46b88836457c3bad8");
                var TricksterStealthTier1Buff = BlueprintTools.GetBlueprint<BlueprintBuff>("de4428f6828fbd14490a2d821d3063f8");
                var TricksterStealthTier2Feature = BlueprintTools.GetBlueprint<BlueprintFeature>("0d1071f085c5c4f408cd2bf1ec4f6adc");
                var TricksterStealthTier2Buff = BlueprintTools.GetBlueprint<BlueprintBuff>("cdfb5dca7d93d574cb15f59c0625059c");
                var TricksterStealthTier3Feature = BlueprintTools.GetBlueprint<BlueprintFeature>("74d23b6698554e36974711eacc386290");

                var DivinationImmunityFeature = BlueprintTools.GetBlueprintReference<BlueprintUnitFactReference>("12435ed2443c41c78ac47b8ef076e997");

                var Icon_TricksterStealth = AssetLoader.LoadInternal(TTTContext, "TricksterTricks", "Icon_TricksterStealth.png");

                PatchIcons();
                PatchTricksterStealthAbilityName();
                PatchTricksterStealth1();
                PatchTricksterStealth2();
                PatchTricksterStealth3();

                void PatchIcons() {
                    if (TTTContext.Homebrew.MythicReworks.Trickster.IsDisabled("UpdateIcons")) { return; }

                    TricksterStealthTier1Feature.m_Icon = Icon_TricksterStealth;
                    TricksterStealthTier1Buff.m_Icon = Icon_TricksterStealth;
                    TricksterStealthTier2Feature.m_Icon = Icon_TricksterStealth;
                    TricksterStealthTier2Buff.m_Icon = Icon_TricksterStealth;
                    TricksterStealthTier3Feature.m_Icon = Icon_TricksterStealth;
                    TricksterStealthTier1AbilityTarget.m_Icon = Icon_TricksterStealth;
                }
                void PatchTricksterStealthAbilityName() {
                    if (TTTContext.Homebrew.MythicReworks.Trickster.IsDisabled("TricksterStealthAbilityName")) { return; }

                    TricksterStealthTier1AbilityTarget.SetName(TTTContext, "Trickster Stealth");
                    TricksterStealthTier1AbilityTarget.SetDescription(TTTContext, "You can enter stealth during combat as a move action. " +
                        "This stealth is not broken by a single enemy detecting you — instead it acts similar to the invisibility spell, " +
                        "giving you total concealment against all creatures that did not succeed on a Perception check against you.\n" +
                        "This \"invisibility\" cannot be seen through by divination magic such as true seeing, see invisibility, or thoughtsense.");

                    TTTContext.Logger.LogPatch("Patched", TricksterStealthTier1AbilityTarget);
                }
                void PatchTricksterStealth1() {
                    if (TTTContext.Homebrew.MythicReworks.Trickster.IsDisabled("TricksterStealth1")) { return; }

                    TricksterStealthTier1Feature.SetDescription(TTTContext, "You can easily slip into shadow at any time. " +
                        "You can enter stealth during combat as a move action. This stealth is not broken by a single enemy " +
                        "detecting you — instead it acts similar to the invisibility spell, giving you total concealment against " +
                        "all creatures that did not succeed on a Perception check against you.\n" +
                        "This \"invisibility\" cannot be seen through by divination magic such as true seeing, see invisibility, or thoughtsense.");
                    TricksterStealthTier1Buff.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] { DivinationImmunityFeature };
                    });
                    TTTContext.Logger.LogPatch("Patched", TricksterStealthTier1Feature);
                    TTTContext.Logger.LogPatch("Patched", TricksterStealthTier1Buff);
                }
                void PatchTricksterStealth2() {
                    if (TTTContext.Homebrew.MythicReworks.Trickster.IsDisabled("TricksterStealth2")) { return; }

                    TricksterStealthTier2Feature.SetDescription(TTTContext, "You exceed at stealth, fading from sight with your every move. " +
                        "Your stealth in combat now works more akin to greater invisibility spell effect.\n" +
                        "This \"invisibility\" cannot be seen through by divination magic such as true seeing, see invisibility, or thoughtsense.");
                    TricksterStealthTier2Buff.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] { DivinationImmunityFeature };
                    });
                    TTTContext.Logger.LogPatch("Patched", TricksterStealthTier2Feature);
                    TTTContext.Logger.LogPatch("Patched", TricksterStealthTier2Buff);
                }
                void PatchTricksterStealth3() {

                }
            }
            static void PatchTricksterTrickery() {
                var TricksterTrickeryTier1Feature = BlueprintTools.GetBlueprint<BlueprintFeature>("90bc71f1d8482184a9ede0bda4773d94");
                var TricksterTrickeryTier2Feature = BlueprintTools.GetBlueprint<BlueprintFeature>("1b8271896e4a3e14389729f3df6a847e");
                var TricksterTrickeryTier3Feature = BlueprintTools.GetBlueprint<BlueprintFeature>("64131f0ac1e2497a806856461bdcfe4e");
                var TricksterTrickeryTier1AbilityPoint = BlueprintTools.GetBlueprint<BlueprintAbility>("10e79ecd4d110e146b674121e860c6c5");
                var TricksterTrickeryTier1AbilityTarget = BlueprintTools.GetBlueprint<BlueprintAbility>("9550d593013bdaa4d982f3739e352f39");
                var TricksterTrickeryTier2AbilityPoint = BlueprintTools.GetBlueprint<BlueprintAbility>("9eef1740fffe6db4ca22e3e2dca3158e");
                var TricksterTrickeryTier2AbilityTarget = BlueprintTools.GetBlueprint<BlueprintAbility>("7a7bb136b533a744d8df0862faa3a1bd");
                var TricksterTrickeryTier3Ability = BlueprintTools.GetBlueprint<BlueprintAbility>("142482756c964ae081d349a670807d23");

                var Icon_TricksterTrickery = AssetLoader.LoadInternal(TTTContext, "TricksterTricks", "Icon_TricksterTrickery.png");

                PatchIcons();
                PatchTricksterTrickery1();
                PatchTricksterTrickery2();
                PatchTricksterTrickery3();

                void PatchIcons() {
                    if (TTTContext.Homebrew.MythicReworks.Trickster.IsDisabled("UpdateIcons")) { return; }

                    TricksterTrickeryTier1Feature.m_Icon = Icon_TricksterTrickery;
                    TricksterTrickeryTier2Feature.m_Icon = Icon_TricksterTrickery;
                    TricksterTrickeryTier3Feature.m_Icon = Icon_TricksterTrickery;

                    TricksterTrickeryTier1AbilityPoint.m_Icon = Icon_TricksterTrickery;
                    TricksterTrickeryTier1AbilityTarget.m_Icon = Icon_TricksterTrickery;
                    TricksterTrickeryTier2AbilityPoint.m_Icon = Icon_TricksterTrickery;
                    TricksterTrickeryTier2AbilityTarget.m_Icon = Icon_TricksterTrickery;
                    TricksterTrickeryTier3Ability.m_Icon = Icon_TricksterTrickery;
                }
                void PatchTricksterTrickery1() {
                }
                void PatchTricksterTrickery2() {
                }
                void PatchTricksterTrickery3() {
                    if (TTTContext.Homebrew.MythicReworks.Trickster.IsDisabled("TricksterTrickery3")) { return; }

                    TricksterTrickeryTier3Feature.SetDescription(TTTContext, "Living creatures are also just complex devices and can also be easily disabled. " +
                        "You can try to disable them, forcing them to make a Fortitude saving throw (DC 15 + your ranks in Trickery). If the target fails it, it dies. " +
                        "Additionally your previous Trickery abilities can now be used as a swift action and at close range instead of touch range.");
                    TricksterTrickeryTier3Feature.AddComponent<AutoMetamagic>(c => {
                        c.m_AllowedAbilities = AutoMetamagic.AllowedType.Any;
                        c.Metamagic = Metamagic.Quicken | Metamagic.Reach;
                        c.Abilities = new List<BlueprintAbilityReference>() {
                            TricksterTrickeryTier1AbilityPoint.ToReference<BlueprintAbilityReference>(),
                            TricksterTrickeryTier1AbilityTarget.ToReference<BlueprintAbilityReference>(),
                            TricksterTrickeryTier2AbilityPoint.ToReference<BlueprintAbilityReference>(),
                            TricksterTrickeryTier2AbilityTarget.ToReference<BlueprintAbilityReference>(),
                        };
                    });

                    TTTContext.Logger.LogPatch("Patched", TricksterTrickeryTier3Feature);
                }
            }
            static void PatchTricksterUseMagicDevice() {
                var TricksterUseMagicDeviceTier1Feature = BlueprintTools.GetBlueprint<BlueprintFeature>("647c848dd47b93d42bd3000bce93dff6");
                var TricksterUseMagicDeviceTier2Feature = BlueprintTools.GetBlueprint<BlueprintFeature>("1383f21534d8b6a45bdbdc8ddce7a187");
                var TricksterUseMagicDeviceTier3Feature = BlueprintTools.GetBlueprint<BlueprintFeature>("9d1446e4947544429a64e5fa20906ebf");
                var CompletelyNormalSpellFeat = BlueprintTools.GetBlueprintReference<BlueprintUnitFactReference>("094b6278f7b570f42aeaa98379f07cf2");

                var Icon_TricksterUseMagicDevice = AssetLoader.LoadInternal(TTTContext, "TricksterTricks", "Icon_TricksterUseMagicDevice.png");

                PatchIcons();
                PatchTricksterUseMagicDevice1();
                PatchTricksterUseMagicDevice2();
                PatchTricksterUseMagicDevice3();

                void PatchIcons() {
                    if (TTTContext.Homebrew.MythicReworks.Trickster.IsDisabled("UpdateIcons")) { return; }

                    TricksterUseMagicDeviceTier1Feature.m_Icon = Icon_TricksterUseMagicDevice;
                    TricksterUseMagicDeviceTier2Feature.m_Icon = Icon_TricksterUseMagicDevice;
                    TricksterUseMagicDeviceTier3Feature.m_Icon = Icon_TricksterUseMagicDevice;
                }
                void PatchTricksterUseMagicDevice1() {
                    TricksterUseMagicDeviceTier1Feature.SetName(TTTContext, "Use Magic Device 1 rank");
                }
                void PatchTricksterUseMagicDevice2() {
                    if (TTTContext.Homebrew.MythicReworks.Trickster.IsDisabled("TricksterUseMagicDevice2")) { return; }

                    TricksterUseMagicDeviceTier2Feature.SetName(TTTContext, "Use Magic Device 2 rank");
                    TricksterUseMagicDeviceTier2Feature.SetDescription(TTTContext, "Your mastery of magic items has reached new heights.\n" +
                        "Wands you use no longer lose charges for use and you can equip any magical items possible, regardless of requirements. " +
                        "Additionally you realize that spells may be a bit less magical than you previously thought " +
                        "and gain the metamagic Completely Normal Spell");
                    TricksterUseMagicDeviceTier2Feature.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            CompletelyNormalSpellFeat
                        };
                    });
                }
                void PatchTricksterUseMagicDevice3() {
                    TricksterUseMagicDeviceTier3Feature.SetName(TTTContext, "Use Magic Device 3 rank");
                }
            }
        }
        [HarmonyPatch(typeof(Demoralize), nameof(Demoralize.RunAction))]
        private static class Demoralize_Trickster_Rework {
            static readonly MethodInfo getter_RuleStatCheck_Success = AccessTools.PropertyGetter(typeof(RuleStatCheck), "Success");
            static readonly MethodInfo method_RunTricksterLogic = AccessTools.Method(typeof(Demoralize_Trickster_Rework), "RunTricksterLogic");

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
                var codes = new List<CodeInstruction>(instructions);
                var targetData = FindInsertionTarget(codes);
                //Main.TTTContext.Logger.Log($"Target({target.Index}, {target.Start}, {target.End})");
                //Utilities.ILUtils.LogIL(Main.TTTContext, codes);
                var labels = codes[targetData.TargetIndex].labels.ToList();
                codes[targetData.TargetIndex].labels.Clear();
                codes.InsertRange(targetData.TargetIndex, new CodeInstruction[] {
                    new CodeInstruction(OpCodes.Ldarg_0),
                    targetData.LoadVariable,
                    //codes[targetData+1].Clone(),
                    new CodeInstruction(OpCodes.Call, method_RunTricksterLogic),
                });
                codes[targetData.TargetIndex].labels = labels;
                //Utilities.ILUtils.LogIL(Main.TTTContext, codes);
                return codes.AsEnumerable();
            }
            private static InsertionData FindInsertionTarget(List<CodeInstruction> codes) {
                var targetIndex = -1;
                var loadVariable = -1;
                for (int i = 0; i < codes.Count; i++) {
                    if (codes[i].Calls(getter_RuleStatCheck_Success)) {
                        loadVariable = i - 1;
                    }
                    if (codes[i].opcode == OpCodes.Leave_S) {
                        targetIndex = i;
                    }
                }
                if (targetIndex < 0 || loadVariable < 0) {
                    Main.TTTContext.Logger.Log("Demoralize_Trickster_Rework: COULD NOT FIND TARGET");
                }
                return new InsertionData(targetIndex, codes[loadVariable].Clone());
            }
            private struct InsertionData {
                public int TargetIndex;
                public CodeInstruction LoadVariable;

                public InsertionData(int targetIndex, CodeInstruction loadVariable) {
                    TargetIndex = targetIndex;
                    LoadVariable = loadVariable;
                }
            }

            private static void RunTricksterLogic(Demoralize instance, RuleSkillCheck check) {
                var TricksterPersuasion2Buff = BlueprintTools.GetModBlueprint<BlueprintBuff>(TTTContext, "TricksterPersuasion2Buff");
                var TricksterPersuasion3Buff = BlueprintTools.GetModBlueprint<BlueprintBuff>(TTTContext, "TricksterPersuasion3Buff");
                var Staggered = BlueprintTools.GetBlueprint<BlueprintBuff>("df3950af5a783bd4d91ab73eb8fa0fd3");
                var Stunned = BlueprintTools.GetBlueprint<BlueprintBuff>("09d39b38bb7c6014394b6daced9bacd3");

                var data = ContextData<MechanicsContext.Data>.Current;
                var mechanicsContext = data?.Context;
                var caster = mechanicsContext?.MaybeCaster;
                int debuffDuration = 1 + (check.RollResult - check.DC) / 5 + (caster.Descriptor.State.Features.FrighteningThug ? 1 : 0);
                int saveDC = 10 + caster.Stats.SkillPersuasion.BaseValue;

                var TricksterPersuasion2 = caster.CustomMechanicsFeature(CustomMechanicsFeature.TricksterReworkPersuasion2).Value;
                var TricksterPersuasion3 = caster.CustomMechanicsFeature(CustomMechanicsFeature.TricksterReworkPersuasion3).Value;

                if (TricksterPersuasion2) {
                    instance.Target.Unit.Descriptor.AddBuff(TricksterPersuasion2Buff, mechanicsContext, new TimeSpan?(debuffDuration.Rounds().Seconds));
                }
                if (TricksterPersuasion3) {
                    instance.Target.Unit.Descriptor.AddBuff(TricksterPersuasion3Buff, mechanicsContext, new TimeSpan?(debuffDuration.Rounds().Seconds));
                }
                if (TricksterPersuasion2) {
                    if (!Game.Instance.Rulebook.TriggerEvent<RuleSavingThrow>(new RuleSavingThrow(instance.Target.Unit, SavingThrowType.Will, saveDC)).IsPassed) {
                        instance.Target.Unit.Descriptor.AddBuff(Staggered, mechanicsContext, new TimeSpan?(1.Rounds().Seconds));
                    }
                }
            }
        }
    }
}
