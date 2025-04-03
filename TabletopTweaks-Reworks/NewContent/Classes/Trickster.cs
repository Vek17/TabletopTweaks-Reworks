using Kingmaker.Assets.UnitLogic.Mechanics.Properties;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.Designers.Mechanics.Buffs;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.DLC;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.Enums.Damage;
using Kingmaker.ResourceLinks;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Abilities.Components.AreaEffects;
using Kingmaker.UnitLogic.Abilities.Components.Base;
using Kingmaker.UnitLogic.Abilities.Components.CasterCheckers;
using Kingmaker.UnitLogic.Abilities.Components.TargetCheckers;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Buffs.Components;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using Kingmaker.UnitLogic.Mechanics.Properties;
using Kingmaker.Utility;
using Kingmaker.Visual.Animation.Kingmaker.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using TabletopTweaks.Core.NewComponents;
using TabletopTweaks.Core.NewComponents.OwlcatReplacements;
using TabletopTweaks.Core.Utilities;
using static TabletopTweaks.Reworks.Main;

namespace TabletopTweaks.Reworks.NewContent.Classes {
    static class Trickster {
        private static BlueprintGuid TricksterDomainMasterID = TTTContext.Blueprints.GetDerivedMaster("TricksterDomainMasterID");
        private static BlueprintCharacterClassReference TricksterMythicClass => BlueprintTools.GetBlueprintReference<BlueprintCharacterClassReference>("8df873a8c6e48294abdb78c45834aa0a");
        private static BlueprintSpellsTableReference TricksterDomainSpellsKnown = null;
        private static BlueprintSpellsTableReference TricksterDomainSpellsPerDay = null;
        private static BlueprintUnitProperty TricksterDomainCLProperty = Helpers.CreateBlueprint<BlueprintUnitProperty>(TTTContext, "TricksterTTTDomainRankProperty", bp => {
            bp.AddComponent<SimplePropertyGetter>(c => {
                c.Property = UnitProperty.Level;
                c.Settings = new PropertySettings() {
                    m_Progression = PropertySettings.Progression.AsIs
                };
            });
            bp.BaseValue = 0;
        });
        private static BlueprintUnitProperty TricksterDomainStatProperty = Helpers.CreateBlueprint<BlueprintUnitProperty>(TTTContext, "TricksterTTTDomainStatProperty", bp => {
            bp.AddComponent<SimplePropertyGetter>(c => {
                c.Property = UnitProperty.MythicLevel;
                c.Settings = new PropertySettings() {
                    m_Progression = PropertySettings.Progression.AsIs
                };
            });
            bp.BaseValue = 0;
        });
        private static BlueprintUnitProperty TricksterDomainDCProperty = Helpers.CreateBlueprint<BlueprintUnitProperty>(TTTContext, "TricksterTTTDomainDCProperty", bp => {
            bp.AddComponent<CustomPropertyGetter>(c => {
                c.m_Property = TricksterDomainCLProperty.ToReference<BlueprintUnitPropertyReference>();
                c.Settings = new PropertySettings() {
                    m_Progression = PropertySettings.Progression.Div2
                };
            });
            bp.AddComponent<CustomPropertyGetter>(c => {
                c.m_Property = TricksterDomainStatProperty.ToReference<BlueprintUnitPropertyReference>();
                c.Settings = new PropertySettings() {
                    m_Progression = PropertySettings.Progression.AsIs
                };
            });
            bp.OperationOnComponents = BlueprintUnitProperty.MathOperation.Sum;
            bp.BaseValue = 10;
        });
        private static BlueprintFeature DomainMastery => BlueprintTools.GetBlueprint<BlueprintFeature>("2de64f6a1f2baee4f9b7e52e3f046ec5");
        public static List<BlueprintProgression> TricksterDomains = new List<BlueprintProgression>();

        public static void AddTricksterDomains() {
            PrefabLink BlueAoE30Feet = BlueprintTools.GetBlueprint<BlueprintAbilityAreaEffect>("3635b48c6e8d54947bbd27c1be818677").Fx; // CommunityDomain
            PrefabLink WhiteAoE30Feet = BlueprintTools.GetBlueprint<BlueprintAbilityAreaEffect>("7269a28475a91d84486749bf47443c72").Fx; // CommunityDomain

            var AirDomainProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("750bfcd133cd52f42acbd4f7bc9cc365");
            var AnimalDomainProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("23d2f87aa54c89f418e68e790dba11e0");
            var ArtificeDomainProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("6454b37f50e10ae41bca83aaaa81ffc2");
            var ChaosDomainProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("5a5d19c246961484a97e1e5dded98ab2");
            var CharmDomainProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("b5c056787d1bf544588ec3a150ed0b3b");
            var CommunityDomainProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("b8bbe42616d61ac419971b7910d79fc8");
            var DarknessDomainProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("1e1b4128290b11a41ba55280ede90d7d");
            var DeathDomainProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("710d8c959e7036448b473ffa613cdeba");
            var DestructionDomainProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("269ff0bf4596f5248864bc2653a2f0e0");
            var EarthDomainProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("08bbcbfe5eb84604481f384c517ac800");
            var EvilDomainProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("a8936d29b6051a1418682da1878b644e");
            var FireDomainProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("881b2137a1779294c8956fe5b497cc35");
            var GloryDomainProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("f0a61a043bcdf0f4c8efc59962afafb8");
            var GoodDomainProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("243ab3e7a86d30243bdfe79c83e6adb4");
            var HealingDomainProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("b0a26ee984b6b6945b884467aa2f1baa");
            var KnowledgeDomainProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("28edbdbefca579b4ab4992e98af71981");
            var LawDomainProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("a723d11a5ae5df0488775e31fac9117d");
            var LiberationDomainProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("df2f14ced8710664ba7db914880c4a02");
            var LuckDomainProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("8bd8cfad69085654b9118534e4aa215e");
            var MadnessDomainProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("9ebe166b9b901c746b1858029f13a2c5");
            var MagicDomainProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("8f90e7129b0f3b742921c2c9c9bd64fc");
            var NobilityDomainProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("8480f2d1ca764774895ee6fd610a568e");
            var PlantDomainProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("467d2a1d2107da64395b591393baad17");
            var ProtectionDomainProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("b750650400d9d554b880dbf4c8347b24");
            var ReposeDomainProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("a2ab5a696d0dd134d94b2631151a15ee");
            var RuneDomainProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("6d4dac497c182754d8b1f49071cca3fd");
            var StrengthDomainProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("07854f99c8d029b4cbfdf6ae6c7bc452");
            var SunDomainProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("c85c8791ee13d4c4ea10d93c97a19afc");
            var TravelDomainProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("d169dd2de3630b749a2363c028bb6e7b");
            var TrickeryDomainProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("cc2d330bb0200e840aeb79140e770198");
            var WarDomainProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("8d454cbb7f25070419a1c8eaf89b5be5");
            var WaterDomainProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("e63d9133cebf2cf4788e61432a939084");
            var WeatherDomainProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("c18a821ee662db0439fb873165da25be");

            var IceSubdomainProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("2108d8e7019e4c1faa094d2d6725e936");
            var MurderSubdomainProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("3b75b9afcb5a485aab990f12c1a22e64");
            var UndeadSubdomainProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("4f0332ac85174cdcb47e2d866a7948c3");

            var ScalykindDomainProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("67034c0985b240e5ad6e41e905461951");

            CreateDomainSpellbookComponents();
            CreateAirDomain();
            CreateAnimalDomain();
            CreateArtificeDomain();
            CreateChaosDomain();
            CreateCharmDomain();
            CreateCommunityDomain();
            CreateDarknessDomain();
            CreateDeathDomain();
            CreateDestructionDomain();
            CreateEarthDomain();
            CreateEvilDomain();
            CreateFireDomain();
            CreateGloryDomain();
            CreateGoodDomain();
            CreateHealingDomain();
            CreateKnowledgeDomain();
            CreateLawDomain();
            CreateLiberationDomain();
            CreateLuckDomain();
            CreateMadnessDomain();
            CreateMagicDomain();
            CreateNobilityDomain();
            CreatePlantDomain();
            CreateProtectionDomain();
            CreateReposeDomain();
            CreateRuneDomain();
            CreateStrengthDomain();
            CreateSunDomain();
            CreateTravelDomain();
            CreateTrickeryDomain();
            CreateWarDomain();
            CreateWaterDomain();
            CreateWeatherDomain();

            CreateIceSubdomain();
            CreateMurderSubdomain();
            CreateUndeadSubdomain();

            CreateScalykindDomain();

            void CreateDomainSpellbookComponents() {
                TricksterDomainSpellsKnown = Helpers.CreateBlueprint<BlueprintSpellsTable>(TTTContext, "TricksterTTTDomainSpellsKnown", bp => {
                    bp.Levels = new SpellsLevelEntry[] {
                        SpellTools.CreateSpellLevelEntry(),
                        SpellTools.CreateSpellLevelEntry(0,1),
                        SpellTools.CreateSpellLevelEntry(0,1),
                        SpellTools.CreateSpellLevelEntry(0,1,1),
                        SpellTools.CreateSpellLevelEntry(0,1,1),
                        SpellTools.CreateSpellLevelEntry(0,1,1,1),
                        SpellTools.CreateSpellLevelEntry(0,1,1,1),
                        SpellTools.CreateSpellLevelEntry(0,1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(0,1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(0,1,1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(0,1,1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(0,1,1,1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(0,1,1,1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(0,1,1,1,1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(0,1,1,1,1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(0,1,1,1,1,1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(0,1,1,1,1,1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(0,1,1,1,1,1,1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(0,1,1,1,1,1,1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(0,1,1,1,1,1,1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(0,1,1,1,1,1,1,1,1,1)
                    };
                }).ToReference<BlueprintSpellsTableReference>();
                TricksterDomainSpellsPerDay = Helpers.CreateBlueprint<BlueprintSpellsTable>(TTTContext, "TricksterTTTDomainSpellsPerDay", bp => {
                    bp.Levels = new SpellsLevelEntry[] {
                        SpellTools.CreateSpellLevelEntry(),
                        SpellTools.CreateSpellLevelEntry(0,1),
                        SpellTools.CreateSpellLevelEntry(0,1),
                        SpellTools.CreateSpellLevelEntry(0,1,1),
                        SpellTools.CreateSpellLevelEntry(0,1,1),
                        SpellTools.CreateSpellLevelEntry(0,1,1,1),
                        SpellTools.CreateSpellLevelEntry(0,1,1,1),
                        SpellTools.CreateSpellLevelEntry(0,1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(0,1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(0,1,1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(0,1,1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(0,1,1,1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(0,1,1,1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(0,1,1,1,1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(0,1,1,1,1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(0,1,1,1,1,1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(0,1,1,1,1,1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(0,1,1,1,1,1,1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(0,1,1,1,1,1,1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(0,1,1,1,1,1,1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(0,1,1,1,1,1,1,1,1,1)
                    };
                }).ToReference<BlueprintSpellsTableReference>();
            }
            void CreateAirDomain() {
                //Base Feature
                var AirDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("39b0c7db785560041b436b558c9df2bb");
                var AirDomainBaseAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("b3494639791901e4db3eda6117ad878f");

                var TricksterTTTAirDomainBaseResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTAirDomainBaseResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterMythics,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 3,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                    bp.m_Max = 13;
                    bp.m_UseMax = true;
                });
                var TricksterTTTAirDomainBaseAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTAirDomainBaseAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(AirDomainBaseAbility);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Type = AbilityRankType.DamageBonus;
                        c.m_Progression = ContextRankProgression.Div2;
                    });
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionDealDamage() {
                                DamageType = new DamageTypeDescription() {
                                    Type = DamageType.Energy,
                                    Energy = DamageEnergyType.Electricity
                                },
                                Duration = new ContextDurationValue() {
                                    DiceCountValue = 0,
                                    BonusValue = 0
                                },
                                Value = new ContextDiceValue() {
                                    DiceType = DiceType.D6,
                                    DiceCountValue = 1,
                                    BonusValue = new ContextValue() {
                                        ValueType = ContextValueType.Rank,
                                        ValueRank = AbilityRankType.DamageBonus
                                    }
                                }
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTAirDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                    AirDomainBaseAbility.ComponentsArray.OfType<AbilityCasterHasNoFacts>().ForEach(c => {
                        bp.AddComponent(Helpers.CreateCopy(c));
                    });
                });
                var TricksterTTTAirDomainBaseFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTAirDomainBaseFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(AirDomainBaseFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTAirDomainBaseAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTAirDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                    bp.AddComponent<AddClassSkill>(c => {
                        c.Skill = StatType.SkillThievery;
                    });
                    bp.AddComponent<AddClassSkill>(c => {
                        c.Skill = StatType.SkillStealth;
                    });
                });
                //Greater Feature
                var AirDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("8a8e3f80abc04c34b98324823d14b057");
                var AirDomainCapstone = BlueprintTools.GetBlueprint<BlueprintFeature>("d5a54e5a3876f5a498abad99b1984cb5");

                var tricksterDomain = CreateTricksterDomain(AirDomainProgression);
                tricksterDomain.LevelEntries = new LevelEntry[] {
                    Helpers.CreateLevelEntry(1, TricksterTTTAirDomainBaseFeature),
                    Helpers.CreateLevelEntry(6, AirDomainGreaterFeature),
                    Helpers.CreateLevelEntry(12, AirDomainGreaterFeature),
                    Helpers.CreateLevelEntry(20, AirDomainCapstone)
                };
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateAnimalDomain() {
                //Base Feature
                var AnimalDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("d577aba79b5727a4ab74627c4c6ba23c");

                var TricksterTTTAnimalDomainBaseFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTAnimalDomainBaseFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(AnimalDomainBaseFeature);
                    bp.AddComponent<AddClassSkill>(c => {
                        c.Skill = StatType.SkillLoreNature;
                    });
                });
                //Greater Feature
                var AnimalCompanionSelectionDomain = BlueprintTools.GetBlueprint<BlueprintFeatureSelection>("2ecd6c64683b59944a7fe544033bb533");
                var AnimalCompanionRank = BlueprintTools.GetBlueprint<BlueprintFeature>("1670990255e4fe948a863bafd5dbda5d");
                var AnimalCompanionArchetypeSelection = BlueprintTools.GetBlueprintReference<BlueprintFeatureReference>("65af7290b4efd5f418132141aaa36c1b");
                var MountTargetFeature = BlueprintTools.GetBlueprintReference<BlueprintFeatureReference>("cb06f0e72ffb5c640a156bd9f8000c1d");

                var TricksterTTTDomainAnimalCompanionProgression = Helpers.CreateBlueprint<BlueprintProgression>(TTTContext, "TricksterTTTDomainAnimalCompanionProgression", bp => {
                    bp.m_Classes = new BlueprintProgression.ClassWithLevel[0];
                    bp.m_Archetypes = new BlueprintProgression.ArchetypeWithLevel[0];
                    bp.m_FeaturesRankIncrease = new List<BlueprintFeatureReference>();
                    ResourcesLibrary.GetRoot()
                        .Progression
                        .CharacterClasses
                        .ForEach(c => bp.AddClass(c));
                    bp.LevelEntries = new LevelEntry[] {
                        Helpers.CreateLevelEntry(4, AnimalCompanionRank),
                        Helpers.CreateLevelEntry(5, AnimalCompanionRank),
                        Helpers.CreateLevelEntry(6, AnimalCompanionRank),
                        Helpers.CreateLevelEntry(7, AnimalCompanionRank),
                        Helpers.CreateLevelEntry(8, AnimalCompanionRank),
                        Helpers.CreateLevelEntry(9, AnimalCompanionRank),
                        Helpers.CreateLevelEntry(10, AnimalCompanionRank),
                        Helpers.CreateLevelEntry(11, AnimalCompanionRank),
                        Helpers.CreateLevelEntry(12, AnimalCompanionRank),
                        Helpers.CreateLevelEntry(13, AnimalCompanionRank),
                        Helpers.CreateLevelEntry(14, AnimalCompanionRank),
                        Helpers.CreateLevelEntry(15, AnimalCompanionRank),
                        Helpers.CreateLevelEntry(16, AnimalCompanionRank),
                        Helpers.CreateLevelEntry(17, AnimalCompanionRank),
                        Helpers.CreateLevelEntry(18, AnimalCompanionRank),
                        Helpers.CreateLevelEntry(19, AnimalCompanionRank),
                        Helpers.CreateLevelEntry(20, AnimalCompanionRank)
                    };
                    bp.IsClassFeature = true;
                    bp.Ranks = 1;
                    bp.GiveFeaturesForPreviousLevels = true;
                });
                var TricksterTTTAnimalCompanionSelectionDomain = Helpers.CreateBlueprint<BlueprintFeatureSelection>(TTTContext, "TricksterTTTAnimalCompanionSelectionDomain", bp => {
                    bp.ApplyVisualsAndBasicSettings(AnimalCompanionSelectionDomain);
                    bp.m_AllFeatures = AnimalCompanionSelectionDomain.m_AllFeatures;
                    bp.m_Features = AnimalCompanionSelectionDomain.m_Features;
                    bp.Group = FeatureGroup.AnimalCompanion;
                    bp.AddComponent<AddFeatureOnApply>(c => {
                        c.m_Feature = AnimalCompanionArchetypeSelection;
                    });
                    bp.AddComponent<AddFeatureOnApply>(c => {
                        c.m_Feature = MountTargetFeature;
                    });
                    bp.AddComponent<AddFeatureOnApply>(c => {
                        c.m_Feature = AnimalCompanionArchetypeSelection;
                    });
                    bp.AddComponent<AddFeatureOnApply>(c => {
                        c.m_Feature = TricksterTTTDomainAnimalCompanionProgression.ToReference<BlueprintFeatureReference>();
                    });
                });

                var tricksterDomain = CreateTricksterDomain(AnimalDomainProgression);
                tricksterDomain.LevelEntries = new LevelEntry[] {
                    Helpers.CreateLevelEntry(1, TricksterTTTAnimalDomainBaseFeature),
                    Helpers.CreateLevelEntry(4, TricksterTTTAnimalCompanionSelectionDomain)
                };
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateArtificeDomain() {
                //Base Feature
                var ArtificeDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("01025d876ac28d349ac42d69ba462059");
                var ArtificeDomainBaseToggleAbility = BlueprintTools.GetBlueprint<BlueprintActivatableAbility>("18fd072abe74d144a916e3501533b76b");
                var ArtificeDomainBaseBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("af772f43b1e59e043968796b6b534057");
                var ArtificeDomainBaseAura = BlueprintTools.GetBlueprint<BlueprintAbilityAreaEffect>("f042f2d62e6785d4e8612a027de1f298");
                var ArtificeDomainBaseEffect = BlueprintTools.GetBlueprint<BlueprintBuff>("f0a5b502227438749a1ad4b638224339");

                var TricksterTTTArtificeDomainBaseResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTArtificeDomainBaseResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterClasses,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 0,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                });
                var TricksterTTTArtificeDomainBaseEffect = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterTTTArtificeDomainBaseEffect", bp => {
                    bp.ApplyVisualsAndBasicSettings(ArtificeDomainBaseEffect);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.StartPlusDivStep;
                        c.m_StartLevel = 8;
                        c.m_StepLevel = 3;
                    });
                    bp.AddComponent<SavingThrowBonusAgainstDescriptor>(c => {
                        c.ModifierDescriptor = ModifierDescriptor.UntypedStackable;
                        c.Value = 4;
                        c.SpellDescriptor = SpellDescriptor.Fatigue | SpellDescriptor.Exhausted;
                    });
                    bp.AddComponent<AddDamageResistancePhysical>(c => {
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Rank
                        };
                        c.Pool = new ContextValue();
                    });
                });
                var TricksterTTTArtificeDomainBaseAura = Helpers.CreateBlueprint<BlueprintAbilityAreaEffect>(TTTContext, "TricksterTTTArtificeDomainBaseAura", bp => {
                    bp.ApplyVisualsAndBasicSettings(ArtificeDomainBaseAura);
                    bp.Fx = BlueAoE30Feet;
                    bp.AddComponent<AbilityAreaEffectBuff>(c => {
                        c.m_Buff = TricksterTTTArtificeDomainBaseEffect.ToReference<BlueprintBuffReference>();
                        c.Condition = new ConditionsChecker() {
                            Conditions = new Condition[] {
                                new ContextConditionIsAlly()
                            }
                        };
                    });
                });
                var TricksterTTTArtificeDomainBaseBuff = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterTTTArtificeDomainBaseBuff", bp => {
                    bp.ApplyVisualsAndBasicSettings(ArtificeDomainBaseBuff);
                    bp.AddComponent<AddAreaEffect>(c => {
                        c.m_AreaEffect = TricksterTTTArtificeDomainBaseAura.ToReference<BlueprintAbilityAreaEffectReference>();
                    });
                });
                var TricksterTTTArtificeDomainBaseToggleAbility = Helpers.CreateBlueprint<BlueprintActivatableAbility>(TTTContext, "TricksterTTTArtificeDomainBaseToggleAbility", bp => {
                    bp.m_Buff = TricksterTTTArtificeDomainBaseBuff.ToReference<BlueprintBuffReference>();
                    bp.ApplyVisualsAndBasicSettings(ArtificeDomainBaseToggleAbility);
                    bp.AddComponent<ActivatableAbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTArtificeDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.SpendType = ActivatableAbilityResourceLogic.ResourceSpendType.NewRound;
                    });
                });
                var TricksterTTTArtificeDomainBaseFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTArtificeDomainBaseFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(ArtificeDomainBaseFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTArtificeDomainBaseToggleAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTArtificeDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });
                //Greater Feature
                var ArtificeDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("9c536f77cae0c5c46a9cf871003ebe43");

                var TricksterTTTArtificeDomainGreaterFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTArtificeDomainGreaterFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(ArtificeDomainGreaterFeature);
                });


                var tricksterDomain = CreateTricksterDomain(ArtificeDomainProgression);
                tricksterDomain.LevelEntries = new LevelEntry[] {
                    Helpers.CreateLevelEntry(1, TricksterTTTArtificeDomainBaseFeature),
                    Helpers.CreateLevelEntry(8, TricksterTTTArtificeDomainGreaterFeature)
                };
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateChaosDomain() {
                //Base Feature
                var ChaosDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("0c9d931180a19a646bcf4165f66bd318");
                var ChaosDomainBaseAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("ca1a4cd28737ae544a0a7e5415c79d9b");
                var ChaosDomainBaseBuff = BlueprintTools.GetBlueprintReference<BlueprintBuffReference>("96bbd279e0bed0f4fb208a1761f566b5");

                var TricksterTTTChaosDomainBaseResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTChaosDomainBaseResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterMythics,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 3,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                });
                var TricksterTTTChaosDomainBaseAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTChaosDomainBaseAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(ChaosDomainBaseAbility);
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionApplyBuff() {
                                m_Buff = ChaosDomainBaseBuff,
                                DurationValue = new ContextDurationValue() {
                                    Rate = DurationRate.Rounds,
                                    DiceCountValue = 0,
                                    BonusValue = 1
                                }
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTChaosDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                });
                var TricksterTTTChaosDomainBaseFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTChaosDomainBaseFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(ChaosDomainBaseFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTChaosDomainBaseAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTChaosDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });
                //Greater Feature
                var ChaosDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("41b647ee4591dc1448a665a62b7a7b5f");
                var ChaosDomainGreaterAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("5d8c4161d21f63e4a99b47d1e99e654e");
                var Anarchic = BlueprintTools.GetBlueprintReference<BlueprintItemEnchantmentReference>("57315bc1e1f62a741be0efde688087e9");
                var paladinweaponenchant00_precastbody = new PrefabLink() { AssetId = "1a75495d05cf88b4f9702ad5914b506c" };

                var TricksterTTTChaosDomainGreaterResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTChaosDomainGreaterResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = new BlueprintCharacterClassReference[0],
                        m_ClassDiv = ResourcesLibrary.GetRoot().Progression.m_CharacterClasses,
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        StartingLevel = 8,
                        StartingIncrease = 1,
                        LevelStep = 4,
                        PerStepIncrease = 1,
                        IncreasedByLevelStartPlusDivStep = true,
                        IncreasedByStat = false
                    };
                });
                var TricksterTTTChaosDomainGreaterAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTChaosDomainGreaterAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(ChaosDomainGreaterAbility);
                    bp.Animation = UnitAnimationActionCastSpell.CastAnimationStyle.Touch;
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.Div2;
                    });
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionEnchantWornItem() {
                                m_Enchantment = Anarchic,
                                DurationValue = new ContextDurationValue() {
                                    Rate = DurationRate.Rounds,
                                    DiceCountValue = 0,
                                    BonusValue = new ContextValue() {
                                        ValueType = ContextValueType.Rank
                                    }
                                }
                            },
                            new ContextActionSpawnFx() {
                                PrefabLink = paladinweaponenchant00_precastbody
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTChaosDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                });
                var TricksterTTTChaosDomainGreaterFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTChaosDomainGreaterFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(ChaosDomainGreaterFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTChaosDomainGreaterAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTChaosDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });

                var tricksterDomain = CreateTricksterDomain(ChaosDomainProgression);
                tricksterDomain.LevelEntries = new LevelEntry[] {
                    Helpers.CreateLevelEntry(1, TricksterTTTChaosDomainBaseFeature),
                    Helpers.CreateLevelEntry(8, TricksterTTTChaosDomainGreaterFeature)
                };
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateCharmDomain() {
                //Base Feature
                var CharmDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("4847d450fbef9b444abcc3a82337b426");
                var CharmDomainBaseAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("84cd24a110af59140b066bc2c69619bd");
                var DazeBuff = BlueprintTools.GetBlueprintReference<BlueprintBuffReference>("9934fedff1b14994ea90205d189c8759");

                var TricksterTTTCharmDomainBaseResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTCharmDomainBaseResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterMythics,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 3,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                    bp.m_Max = 13;
                    bp.m_UseMax = true;
                });
                var TricksterTTTCharmDomainBaseAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTCharmDomainBaseAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(CharmDomainBaseAbility);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.AsIs;
                    });
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new Conditional() {
                                ConditionsChecker = new ConditionsChecker() {
                                    Conditions = new Condition[] {
                                        new ContextConditionHitDice(){
                                            AddSharedValue = true
                                        }
                                    }
                                },
                                IfTrue = Helpers.CreateActionList(
                                    new ContextActionApplyBuff() {
                                        m_Buff = DazeBuff,
                                        DurationValue = new ContextDurationValue() {
                                            Rate = DurationRate.Rounds,
                                            DiceCountValue = 0,
                                            BonusValue = 1
                                        }
                                    }
                                ),
                                IfFalse = Helpers.CreateActionList(),
                            }
                        );
                    });
                    bp.AddComponent<ContextCalculateSharedValue>(c => {
                        c.Value = new ContextDiceValue() {
                            DiceCountValue = 0,
                            BonusValue = new ContextValue() {
                                ValueType = ContextValueType.Rank
                            }
                        };
                        c.Modifier = 1;
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTCharmDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                });
                var TricksterTTTCharmDomainBaseFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTCharmDomainBaseFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(CharmDomainBaseFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTCharmDomainBaseAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTCharmDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });
                //Greater Feature
                var CharmDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("d1fee57aa8f12b849b5abd5f2b7c4616");
                var CharmDomainGreaterAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("6e49baf9d5933874fa0b93b58ab4b3a2");
                var DominatePersonBuff = BlueprintTools.GetBlueprintReference<BlueprintBuffReference>("c0f4e1c24c9cd334ca988ed1bd9d201f");

                var TricksterTTTCharmDomainGreaterResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTCharmDomainGreaterResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterClasses,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 0,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                });
                var TricksterTTTCharmDomainGreaterAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTCharmDomainGreaterAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(CharmDomainGreaterAbility);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.AsIs;
                    });
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionSavingThrow() {
                                Type = SavingThrowType.Will,
                                CustomDC = new ContextValue(),
                                Actions = Helpers.CreateActionList(
                                    new ContextActionConditionalSaved() {
                                        Succeed = Helpers.CreateActionList(),
                                        Failed = Helpers.CreateActionList(
                                            new ContextActionApplyBuff() {
                                                m_Buff = DominatePersonBuff,
                                                Permanent = true,
                                                DurationValue = new ContextDurationValue() {
                                                    DiceCountValue = 0,
                                                    BonusValue = 1
                                                }
                                            }
                                        )
                                    }
                                )
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTCharmDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                });
                var TricksterTTTCharmDomainGreaterFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTCharmDomainGreaterFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(CharmDomainGreaterFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTCharmDomainGreaterAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTCharmDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });

                var tricksterDomain = CreateTricksterDomain(CharmDomainProgression);
                tricksterDomain.LevelEntries = new LevelEntry[] {
                    Helpers.CreateLevelEntry(1, TricksterTTTCharmDomainBaseFeature),
                    Helpers.CreateLevelEntry(8, TricksterTTTCharmDomainGreaterFeature)
                };
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateCommunityDomain() {
                //Base Feature
                var CommunityDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("102d61a114786894bb2b30568943ef1f");
                var CommunityDomainBaseAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("b1b8efd70ba5dd84aa6985d46dc299d5");

                var TricksterTTTCommunityDomainBaseResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTCommunityDomainBaseResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterMythics,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 3,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                    bp.m_Max = 13;
                    bp.m_UseMax = true;
                });
                var TricksterTTTCommunityDomainBaseAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTCommunityDomainBaseAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(CommunityDomainBaseAbility);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Type = AbilityRankType.DamageBonus;
                        c.m_Progression = ContextRankProgression.Div2;
                    });
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionHealTarget() {
                                Value = new ContextDiceValue() {
                                    DiceType = DiceType.D6,
                                    DiceCountValue = 1,
                                    BonusValue = new ContextValue() {
                                        ValueType = ContextValueType.Rank,
                                        ValueRank = AbilityRankType.DamageBonus
                                    }
                                }
                            },
                            new ContextActionDispelMagic() {
                                m_CountToRemove = 1,
                                m_MaxSpellLevel = new ContextValue(),
                                m_MaxCasterLevel = new ContextValue(),
                                Descriptor = SpellDescriptor.Sickened | SpellDescriptor.Shaken | SpellDescriptor.Fatigue,
                                ContextBonus = new ContextValue(),
                                OnSuccess = Helpers.CreateActionList(),
                                OnFail = Helpers.CreateActionList()
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTCommunityDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                    bp.AddComponent<AbilityUseOnRest>(c => {
                        c.BaseValue = 1;
                        c.AddCasterLevel = true;
                        c.MaxCasterLevel = 20;
                    });
                });
                var TricksterTTTCommunityDomainBaseFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTCommunityDomainBaseFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(CommunityDomainBaseFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTCommunityDomainBaseAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTCommunityDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.OnePlusDivStep;
                        c.m_StepLevel = 5;
                        c.m_UseMin = true;
                        c.m_Min = 1;
                    });
                });
                //Greater Feature
                var CommunityDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("4cddbb24833e1d24ea1ff0f59574284a");
                var CommunityDomainGreaterAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("76291e62d2496ad41824044aba3077ea");
                var CommunityDomainGreaterArea = BlueprintTools.GetBlueprint<BlueprintAbilityAreaEffect>("3635b48c6e8d54947bbd27c1be818677");
                var CommunityDomainGreaterBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("672b0149e68d73943ad09ac35a9f5f30");

                var TricksterTTTCommunityDomainGreaterResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTCommunityDomainGreaterResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = new BlueprintCharacterClassReference[0],
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 1,
                        IncreasedByLevel = false,
                        IncreasedByStat = false
                    };
                });
                var TricksterTTTCommunityDomainGreaterBuff = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterTTTCommunityDomainGreaterBuff", bp => {
                    bp.ApplyVisualsAndBasicSettings(CommunityDomainGreaterBuff);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_CustomProperty = TricksterDomainStatProperty.ToReference<BlueprintUnitPropertyReference>();
                        c.m_Progression = ContextRankProgression.AsIs;
                    });
                    bp.AddComponent<AddStatBonusAbilityValue>(c => {
                        c.Stat = StatType.AdditionalAttackBonus;
                        c.Descriptor = ModifierDescriptor.Sacred;
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Rank
                        };
                    });
                    bp.AddComponent<AddStatBonusAbilityValue>(c => {
                        c.Stat = StatType.SaveFortitude;
                        c.Descriptor = ModifierDescriptor.Sacred;
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Rank
                        };
                    });
                    bp.AddComponent<AddStatBonusAbilityValue>(c => {
                        c.Stat = StatType.SaveReflex;
                        c.Descriptor = ModifierDescriptor.Sacred;
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Rank
                        };
                    });
                    bp.AddComponent<AddStatBonusAbilityValue>(c => {
                        c.Stat = StatType.SaveWill;
                        c.Descriptor = ModifierDescriptor.Sacred;
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Rank
                        };
                    });
                });
                var TricksterTTTCommunityDomainGreaterAura = Helpers.CreateBlueprint<BlueprintAbilityAreaEffect>(TTTContext, "TricksterTTTCommunityDomainGreaterAura", bp => {
                    bp.ApplyVisualsAndBasicSettings(CommunityDomainGreaterArea);
                    bp.AddComponent<AbilityAreaEffectBuff>(c => {
                        c.m_Buff = TricksterTTTCommunityDomainGreaterBuff.ToReference<BlueprintBuffReference>();
                        c.Condition = new ConditionsChecker() {
                            Conditions = new Condition[] {
                                new ContextConditionIsAlly()
                            }
                        };
                    });
                });
                var TricksterTTTCommunityDomainGreaterAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTCommunityDomainGreaterAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(CommunityDomainGreaterAbility);
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionSpawnAreaEffect() {
                                m_AreaEffect = TricksterTTTCommunityDomainGreaterAura.ToReference<BlueprintAbilityAreaEffectReference>(),
                                DurationValue = new ContextDurationValue() {
                                    Rate = DurationRate.Hours,
                                    DiceCountValue = 0,
                                    BonusValue = 1
                                }
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTCommunityDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                });
                var TricksterTTTCommunityDomainGreaterFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTCommunityDomainGreaterFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(CommunityDomainGreaterFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTCommunityDomainGreaterAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTCommunityDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });

                var tricksterDomain = CreateTricksterDomain(CommunityDomainProgression);

                tricksterDomain.LevelEntries = new LevelEntry[] {
                    Helpers.CreateLevelEntry(1, TricksterTTTCommunityDomainBaseFeature),
                    Helpers.CreateLevelEntry(8, TricksterTTTCommunityDomainGreaterFeature)
                };
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateDarknessDomain() {
                //Base Feature
                var DarknessDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("9dc5863168155854fa8daf4a780f6663");
                var DarknessDomainBaseAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("39ed9d4b1e033e042aac4f9eb9c7315f");
                var DarknessDomainBaseBuff = BlueprintTools.GetBlueprintReference<BlueprintBuffReference>("64737e33d1d185b4194798e9abee76ca");

                var TricksterTTTDarknessDomainBaseResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTDarknessDomainBaseResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterMythics,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 3,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                });
                var TricksterTTTDarknessDomainBaseAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTDarknessDomainBaseAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(DarknessDomainBaseAbility);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.Div2;
                    });
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionApplyBuff() {
                                m_Buff = DarknessDomainBaseBuff,
                                DurationValue = new ContextDurationValue() {
                                    Rate = DurationRate.Rounds,
                                    DiceCountValue = 0,
                                    BonusValue = new ContextValue() {
                                        ValueType = ContextValueType.Rank
                                    }
                                }
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTDarknessDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                });
                var TricksterTTTDarknessDomainBaseFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTDarknessDomainBaseFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(DarknessDomainBaseFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTDarknessDomainBaseAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTDarknessDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });
                //Greater Feature
                var DarknessDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("0653cd3fee730654eb4daa6629e07fad");
                var DarknessDomainGreaterAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("31acd268039966940872c916782ae018");
                var DazzledBuff = BlueprintTools.GetBlueprintReference<BlueprintBuffReference>("df6d1025da07524429afbae248845ecc");

                var TricksterTTTDarknessDomainGreaterResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTDarknessDomainGreaterResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = new BlueprintCharacterClassReference[0],
                        m_ClassDiv = ResourcesLibrary.GetRoot().Progression.m_CharacterClasses,
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        StartingLevel = 8,
                        StartingIncrease = 1,
                        LevelStep = 4,
                        PerStepIncrease = 1,
                        IncreasedByLevelStartPlusDivStep = true,
                        IncreasedByStat = false
                    };
                });
                var TricksterTTTDarknessDomainGreaterAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTDarknessDomainGreaterAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(DarknessDomainGreaterAbility);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Type = AbilityRankType.DamageDice;
                        c.m_Progression = ContextRankProgression.Div2;
                    });
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Type = AbilityRankType.DamageBonus;
                        c.m_Progression = ContextRankProgression.AsIs;
                    });
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionDealDamage() {
                                DamageType = new DamageTypeDescription() {
                                    Type = DamageType.Energy,
                                    Energy = DamageEnergyType.Divine
                                },
                                Duration = new ContextDurationValue() {
                                    DiceCountValue = 0,
                                    BonusValue = 0
                                },
                                Value = new ContextDiceValue() {
                                    DiceType = DiceType.D8,
                                    DiceCountValue = new ContextValue() {
                                        ValueType = ContextValueType.Rank,
                                        ValueRank = AbilityRankType.DamageDice
                                    },
                                    BonusValue = 0
                                }
                            },
                            new ContextActionApplyBuff() {
                                m_Buff = DazzledBuff,
                                DurationValue = new ContextDurationValue() {
                                    DiceCountValue = 0,
                                    BonusValue = new ContextValue() {
                                        ValueType = ContextValueType.Rank,
                                        ValueRank = AbilityRankType.DamageBonus
                                    }
                                }
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTDarknessDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                    DarknessDomainGreaterAbility.ComponentsArray.OfType<AbilityCasterHasNoFacts>().ForEach(c => {
                        bp.AddComponent(Helpers.CreateCopy(c));
                    });
                });
                var TricksterTTTDarknessDomainGreaterFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTDarknessDomainGreaterFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(DarknessDomainGreaterFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTDarknessDomainGreaterAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTDarknessDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                    bp.AddComponent<AddClassSkill>(c => {
                        c.Skill = StatType.SkillThievery;
                    });
                    bp.AddComponent<AddClassSkill>(c => {
                        c.Skill = StatType.SkillStealth;
                    });
                });

                var tricksterDomain = CreateTricksterDomain(DarknessDomainProgression);
                tricksterDomain.LevelEntries = new LevelEntry[] {
                    Helpers.CreateLevelEntry(1, TricksterTTTDarknessDomainBaseFeature),
                    Helpers.CreateLevelEntry(8, TricksterTTTDarknessDomainGreaterFeature)
                };
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateDeathDomain() {
                //Base Feature
                var DeathDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("9809efa15e5f9ad478594479af575a5d");
                var DeathDomainBaseAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("979f63920af22344d81da5099c9ec32e");
                var DeathDomainBaseBuff = BlueprintTools.GetBlueprintReference<BlueprintBuffReference>("86b2c4ce787ea2e46986869b7a188f25");

                var TricksterTTTDeathDomainBaseResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTDeathDomainBaseResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterMythics,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 3,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                });
                var TricksterTTTDeathDomainBaseAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTDeathDomainBaseAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(DeathDomainBaseAbility);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.Div2;
                    });
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionApplyBuff() {
                                m_Buff = DeathDomainBaseBuff,
                                DurationValue = new ContextDurationValue() {
                                    Rate = DurationRate.Rounds,
                                    DiceCountValue = 0,
                                    BonusValue = new ContextValue() {
                                        ValueType = ContextValueType.Rank
                                    }
                                }
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTDeathDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                });
                var TricksterTTTDeathDomainBaseFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTDeathDomainBaseFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(DeathDomainBaseFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTDeathDomainBaseAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTDeathDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });
                //Greater Feature
                var DeathDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("b0acce833384b9b428f32517163c9117");

                var tricksterDomain = CreateTricksterDomain(DeathDomainProgression);
                tricksterDomain.LevelEntries = new LevelEntry[] {
                    Helpers.CreateLevelEntry(1, TricksterTTTDeathDomainBaseFeature),
                    Helpers.CreateLevelEntry(8, DeathDomainGreaterFeature)
                };
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateDestructionDomain() {
                //Base Feature
                var DestructionDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("2d3b9491bc05a114ab10e5b1b30dc86a");
                var DestructionDomainBaseActivateableAbility = BlueprintTools.GetBlueprint<BlueprintActivatableAbility>("e69898f762453514780eb5e467694bdb");
                var DestructionDomainBaseBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("0dfe08afb3cf3594987bab12d014e74b");

                var TricksterTTTDestructionDomainBaseResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTDestructionDomainBaseResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterClasses,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 0,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                });
                var TricksterTTTDestructionDomainBaseBuff = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterTTTDestructionDomainBaseBuff", bp => {
                    bp.ApplyVisualsAndBasicSettings(DestructionDomainBaseBuff);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Type = AbilityRankType.DamageBonus;
                        c.m_Progression = ContextRankProgression.Div2;
                    });
                    bp.AddComponent<AddContextStatBonus>(c => {
                        c.Stat = StatType.AdditionalDamage;
                        c.Descriptor = ModifierDescriptor.Morale;
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Rank,
                            ValueRank = AbilityRankType.DamageBonus
                        };
                    });
                });
                var TricksterTTTDestructionDomainBaseToggleAbility = Helpers.CreateBlueprint<BlueprintActivatableAbility>(TTTContext, "TricksterTTTDestructionDomainBaseToggleAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(DestructionDomainBaseActivateableAbility);
                    bp.m_Buff = TricksterTTTDestructionDomainBaseBuff.ToReference<BlueprintBuffReference>();
                    bp.AddComponent<ActivatableAbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTDestructionDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.SpendType = ActivatableAbilityResourceLogic.ResourceSpendType.Attack;
                    });
                    bp.ActivationType = AbilityActivationType.Immediately;
                    bp.DeactivateImmediately = true;
                    bp.DeactivateIfCombatEnded = false;
                });
                var TricksterTTTDestructionDomainBaseFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTDestructionDomainBaseFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(DestructionDomainBaseFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTDestructionDomainBaseToggleAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTDestructionDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });
                //Greater Feature
                var DestructionDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("b047e72c88cbdfe409ea0aaea3dfddf6");
                var DestructionDomainGreaterToggleAbility = BlueprintTools.GetBlueprint<BlueprintActivatableAbility>("8e78df631fccb0f42850d24d117d088c");
                var DestructionDomainGreaterBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("532eea2709f3fd8498102726dfca6ec7");
                var DestructionDomainGreaterAura = BlueprintTools.GetBlueprint<BlueprintAbilityAreaEffect>("5a6c8bb6faf11fc4bb1022c3683d12d3");
                var DestructionDomainGreaterEffect = BlueprintTools.GetBlueprint<BlueprintBuff>("f9de414e53a9c23419fa3cfc0daabde7");

                var TricksterTTTDestructionDomainGreaterResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTDestructionDomainGreaterResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterClasses,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 0,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                });
                var TricksterTTTDestructionDomainGreaterEffect = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterTTTDestructionDomainGreaterEffect", bp => {
                    bp.ApplyVisualsAndBasicSettings(DestructionDomainGreaterEffect);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Type = AbilityRankType.DamageBonus;
                        c.m_Progression = ContextRankProgression.Div2;
                    });
                    bp.AddComponent<TargetCritAutoconfirm>();
                    bp.AddComponent<AddIncomingDamageBonus>(c => {
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Rank,
                            ValueRank = AbilityRankType.DamageBonus
                        };
                        c.Descriptor = ModifierDescriptor.Morale;
                    });
                });
                var TricksterTTTDestructionDomainGreaterAura = Helpers.CreateBlueprint<BlueprintAbilityAreaEffect>(TTTContext, "TricksterTTTDestructionDomainGreaterAura", bp => {
                    bp.ApplyVisualsAndBasicSettings(DestructionDomainGreaterAura);
                    bp.AddComponent<AbilityAreaEffectBuff>(c => {
                        c.m_Buff = TricksterTTTDestructionDomainGreaterEffect.ToReference<BlueprintBuffReference>();
                        c.Condition = new ConditionsChecker() {
                            Conditions = new Condition[] { }
                        };
                    });
                });
                var TricksterTTTDestructionDomainGreaterBuff = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterTTTDestructionDomainGreaterBuff", bp => {
                    bp.ApplyVisualsAndBasicSettings(DestructionDomainGreaterBuff);
                    bp.AddComponent<AddAreaEffect>(c => {
                        c.m_AreaEffect = TricksterTTTDestructionDomainGreaterAura.ToReference<BlueprintAbilityAreaEffectReference>();
                    });
                });
                var TricksterTTTDestructionDomainGreaterToggleAbility = Helpers.CreateBlueprint<BlueprintActivatableAbility>(TTTContext, "TricksterTTTDestructionDomainGreaterToggleAbility", bp => {
                    bp.m_Buff = TricksterTTTDestructionDomainGreaterBuff.ToReference<BlueprintBuffReference>();
                    bp.ApplyVisualsAndBasicSettings(DestructionDomainGreaterToggleAbility);
                    bp.AddComponent<ActivatableAbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTDestructionDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.SpendType = ActivatableAbilityResourceLogic.ResourceSpendType.NewRound;
                    });
                });
                var TricksterTTTDestructionDomainGreaterFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTDestructionDomainGreaterFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(DestructionDomainGreaterFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTDestructionDomainGreaterToggleAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTDestructionDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });

                var tricksterDomain = CreateTricksterDomain(DestructionDomainProgression);

                tricksterDomain.LevelEntries = new LevelEntry[] {
                    Helpers.CreateLevelEntry(1, TricksterTTTDestructionDomainBaseFeature),
                    Helpers.CreateLevelEntry(8, TricksterTTTDestructionDomainGreaterFeature)
                };
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateEarthDomain() {
                //Base Feature
                var EarthDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("828d82a0e8c5a944bbdb6b12f802ff02");
                var EarthDomainBaseAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("3ff40918d33219942929f0dbfe5d1dee");

                var TricksterTTTEarthDomainBaseResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTEarthDomainBaseResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterMythics,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 3,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                    bp.m_Max = 13;
                    bp.m_UseMax = true;
                });
                var TricksterTTTEarthDomainBaseAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTEarthDomainBaseAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(EarthDomainBaseAbility);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Type = AbilityRankType.DamageBonus;
                        c.m_Progression = ContextRankProgression.Div2;
                    });
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionDealDamage() {
                                DamageType = new DamageTypeDescription() {
                                    Type = DamageType.Energy,
                                    Energy = DamageEnergyType.Acid
                                },
                                Duration = new ContextDurationValue() {
                                    DiceCountValue = 0,
                                    BonusValue = 0
                                },
                                Value = new ContextDiceValue() {
                                    DiceType = DiceType.D6,
                                    DiceCountValue = 1,
                                    BonusValue = new ContextValue() {
                                        ValueType = ContextValueType.Rank,
                                        ValueRank = AbilityRankType.DamageBonus
                                    }
                                }
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTEarthDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                    EarthDomainBaseAbility.ComponentsArray.OfType<AbilityCasterHasNoFacts>().ForEach(c => {
                        bp.AddComponent(Helpers.CreateCopy(c));
                    });
                });
                var TricksterTTTEarthDomainBaseFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTEarthDomainBaseFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(EarthDomainBaseFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTEarthDomainBaseAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTEarthDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                    bp.AddComponent<AddClassSkill>(c => {
                        c.Skill = StatType.SkillThievery;
                    });
                    bp.AddComponent<AddClassSkill>(c => {
                        c.Skill = StatType.SkillStealth;
                    });
                });
                //Greater Feature
                var EarthDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("761e8208dc1e2074d89d7cd968f432eb");
                var EarthDomainCapstone = BlueprintTools.GetBlueprint<BlueprintFeature>("3b8ca19cd6826324ca9b2e3120628268");

                var tricksterDomain = CreateTricksterDomain(EarthDomainProgression);
                tricksterDomain.LevelEntries = new LevelEntry[] {
                    Helpers.CreateLevelEntry(1, TricksterTTTEarthDomainBaseFeature),
                    Helpers.CreateLevelEntry(6, EarthDomainGreaterFeature),
                    Helpers.CreateLevelEntry(12, EarthDomainGreaterFeature),
                    Helpers.CreateLevelEntry(20, EarthDomainCapstone)
                };
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateEvilDomain() {
                //Base Feature
                var EvilDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("80de18178ff13304e8cf27ba3ef3d77d");
                var EvilDomainBaseAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("5f2d11d9ae72aa740926d8b865d23cb0");
                var EvilDomainBaseBuff = BlueprintTools.GetBlueprintReference<BlueprintBuffReference>("98e7d50d6e1e3b44da6d9cce84f18b43");

                var TricksterTTTEvilDomainBaseResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTEvilDomainBaseResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterMythics,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 3,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                });
                var TricksterTTTEvilDomainBaseAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTEvilDomainBaseAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(EvilDomainBaseAbility);
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionApplyBuff() {
                                m_Buff = EvilDomainBaseBuff,
                                DurationValue = new ContextDurationValue() {
                                    Rate = DurationRate.Rounds,
                                    DiceCountValue = 0,
                                    BonusValue = 1
                                }
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTEvilDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                });
                var TricksterTTTEvilDomainBaseFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTEvilDomainBaseFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(EvilDomainBaseFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTEvilDomainBaseAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTEvilDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });
                //Greater Feature
                var EvilDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("3784df3083cb6404fbce7a585be24bcf");
                var EvilDomainGreaterAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("ab548995edf186f449937413ea463cd5");
                var Unholy = BlueprintTools.GetBlueprintReference<BlueprintItemEnchantmentReference>("d05753b8df780fc4bb55b318f06af453");
                var paladinweaponenchant00_precastbody = new PrefabLink() { AssetId = "1a75495d05cf88b4f9702ad5914b506c" };

                var TricksterTTTEvilDomainGreaterResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTEvilDomainGreaterResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = new BlueprintCharacterClassReference[0],
                        m_ClassDiv = ResourcesLibrary.GetRoot().Progression.m_CharacterClasses,
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        StartingLevel = 8,
                        StartingIncrease = 1,
                        LevelStep = 4,
                        PerStepIncrease = 1,
                        IncreasedByLevelStartPlusDivStep = true,
                        IncreasedByStat = false
                    };
                });
                var TricksterTTTEvilDomainGreaterAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTEvilDomainGreaterAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(EvilDomainGreaterAbility);
                    bp.Animation = UnitAnimationActionCastSpell.CastAnimationStyle.Touch;
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.Div2;
                    });
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionEnchantWornItem() {
                                m_Enchantment = Unholy,
                                DurationValue = new ContextDurationValue() {
                                    Rate = DurationRate.Rounds,
                                    DiceCountValue = 0,
                                    BonusValue = new ContextValue() {
                                        ValueType = ContextValueType.Rank
                                    }
                                }
                            },
                            new ContextActionSpawnFx() {
                                PrefabLink = paladinweaponenchant00_precastbody
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTEvilDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                });
                var TricksterTTTEvilDomainGreaterFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTEvilDomainGreaterFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(EvilDomainGreaterFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTEvilDomainGreaterAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTEvilDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });

                var tricksterDomain = CreateTricksterDomain(EvilDomainProgression);
                tricksterDomain.LevelEntries = new LevelEntry[] {
                    Helpers.CreateLevelEntry(1, TricksterTTTEvilDomainBaseFeature),
                    Helpers.CreateLevelEntry(8, TricksterTTTEvilDomainGreaterFeature)
                };
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateFireDomain() {
                //Base Feature
                var FireDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("42cc125d570c5334c89c6499b55fc0a3");
                var FireDomainBaseAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("4ecdf240d81533f47a5279f5075296b9");

                var TricksterTTTFireDomainBaseResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTFireDomainBaseResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterMythics,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 3,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                    bp.m_Max = 13;
                    bp.m_UseMax = true;
                });
                var TricksterTTTFireDomainBaseAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTFireDomainBaseAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(FireDomainBaseAbility);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Type = AbilityRankType.DamageBonus;
                        c.m_Progression = ContextRankProgression.Div2;
                    });
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionDealDamage() {
                                DamageType = new DamageTypeDescription() {
                                    Type = DamageType.Energy,
                                    Energy = DamageEnergyType.Fire
                                },
                                Duration = new ContextDurationValue() {
                                    DiceCountValue = 0,
                                    BonusValue = 0
                                },
                                Value = new ContextDiceValue() {
                                    DiceType = DiceType.D6,
                                    DiceCountValue = 1,
                                    BonusValue = new ContextValue() {
                                        ValueType = ContextValueType.Rank,
                                        ValueRank = AbilityRankType.DamageBonus
                                    }
                                }
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTFireDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                    FireDomainBaseAbility.ComponentsArray.OfType<AbilityCasterHasNoFacts>().ForEach(c => {
                        bp.AddComponent(Helpers.CreateCopy(c));
                    });
                });
                var TricksterTTTFireDomainBaseFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTFireDomainBaseFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(FireDomainBaseFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTFireDomainBaseAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTFireDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                    bp.AddComponent<AddClassSkill>(c => {
                        c.Skill = StatType.SkillThievery;
                    });
                    bp.AddComponent<AddClassSkill>(c => {
                        c.Skill = StatType.SkillStealth;
                    });
                });
                //Greater Feature
                var FireDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("ef5aedb6a97071b46969b61a86a967db");
                var FireDomainCapstone = BlueprintTools.GetBlueprint<BlueprintFeature>("6c46620d4cab41b42be8dd8cfb1aa9d2");

                var tricksterDomain = CreateTricksterDomain(FireDomainProgression);
                tricksterDomain.LevelEntries = new LevelEntry[] {
                    Helpers.CreateLevelEntry(1, TricksterTTTFireDomainBaseFeature),
                    Helpers.CreateLevelEntry(6, FireDomainGreaterFeature),
                    Helpers.CreateLevelEntry(12, FireDomainGreaterFeature),
                    Helpers.CreateLevelEntry(20, FireDomainCapstone)
                };
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateGloryDomain() {
                //Base Feature
                var GloryDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("17e891b3964492f43aae44f994b5d454");
                var GloryDomainBaseAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("d018241b5a761414897ad6dc4df2db9f");
                var GloryDomainBaseBuff = BlueprintTools.GetBlueprintReference<BlueprintBuffReference>("55edcfff497a1e04a963f72c485da5cb");

                var TricksterTTTGloryDomainBaseResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTGloryDomainBaseResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterMythics,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 3,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                });
                var TricksterTTTGloryDomainBaseBuff = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterTTTGloryDomainBaseBuff", bp => {
                    bp.ApplyVisualsAndBasicSettings(GloryDomainBaseBuff);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Type = AbilityRankType.StatBonus;
                        c.m_Progression = ContextRankProgression.Div2;
                    });
                    bp.AddComponent<AbilityScoreCheckBonus>(c => {
                        c.Stat = StatType.Charisma;
                        c.Bonus = new ContextValue() {
                            ValueType = ContextValueType.Rank,
                            ValueRank = AbilityRankType.StatBonus
                        };
                        c.Descriptor = ModifierDescriptor.UntypedStackable;
                    });
                    bp.AddComponent<AddContextStatBonus>(c => {
                        c.Stat = StatType.SkillPersuasion;
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Rank,
                            ValueRank = AbilityRankType.StatBonus
                        };
                        c.Descriptor = ModifierDescriptor.UntypedStackable;
                    });
                    bp.AddComponent<AddContextStatBonus>(c => {
                        c.Stat = StatType.SkillUseMagicDevice;
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Rank,
                            ValueRank = AbilityRankType.StatBonus
                        };
                        c.Descriptor = ModifierDescriptor.UntypedStackable;
                    });
                    bp.AddComponent<AddInitiatorSkillRollTrigger>(c => {
                        c.Skill = StatType.Charisma;
                        c.Action = Helpers.CreateActionList(
                            new ContextActionRemoveSelf()
                        );
                    });
                    bp.AddComponent<AddInitiatorSkillRollTrigger>(c => {
                        c.Skill = StatType.SkillPersuasion;
                        c.Action = Helpers.CreateActionList(
                            new ContextActionRemoveSelf()
                        );
                    });
                    bp.AddComponent<AddInitiatorSkillRollTrigger>(c => {
                        c.Skill = StatType.SkillUseMagicDevice;
                        c.Action = Helpers.CreateActionList(
                            new ContextActionRemoveSelf()
                        );
                    });
                    bp.AddComponent<AddInitiatorSkillRollTrigger>(c => {
                        c.Skill = StatType.CheckBluff;
                        c.Action = Helpers.CreateActionList(
                            new ContextActionRemoveSelf()
                        );
                    });
                    bp.AddComponent<AddInitiatorSkillRollTrigger>(c => {
                        c.Skill = StatType.CheckDiplomacy;
                        c.Action = Helpers.CreateActionList(
                            new ContextActionRemoveSelf()
                        );
                    });
                    bp.AddComponent<AddInitiatorSkillRollTrigger>(c => {
                        c.Skill = StatType.CheckIntimidate;
                        c.Action = Helpers.CreateActionList(
                            new ContextActionRemoveSelf()
                        );
                    });
                });
                var TricksterTTTGloryDomainBaseAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTGloryDomainBaseAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(GloryDomainBaseAbility);
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionApplyBuff() {
                                m_Buff = TricksterTTTGloryDomainBaseBuff.ToReference<BlueprintBuffReference>(),
                                DurationValue = new ContextDurationValue() {
                                    Rate = DurationRate.Hours,
                                    DiceCountValue = 0,
                                    BonusValue = 1
                                }
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTGloryDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                });
                var TricksterTTTGloryDomainBaseFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTGloryDomainBaseFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(GloryDomainBaseFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTGloryDomainBaseAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<IncreaseSpellDescriptorDC>(c => {
                        c.Descriptor = SpellDescriptor.ChannelPositiveHarm;
                        c.BonusDC = 2;
                        c.ModifierDescriptor = ModifierDescriptor.UntypedStackable;
                        c.SpellsOnly = false;
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTGloryDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });
                //Greater Feature
                var GloryDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("bf41d1d2cf72e8545b51857f20fa58e7");
                var GloryDomainGreaterAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("c89e92387e940e541b02c1969cd1fe2a");
                var GloryDomainGreaterBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("ad66d8c8e4d6e314c8f2f1f41efeee64");
                var GloryDomainGreaterArea = BlueprintTools.GetBlueprint<BlueprintAbilityAreaEffect>("dc623fb49e4658f43b32bed21dafc38c");
                var HeroismBuff = BlueprintTools.GetBlueprintReference<BlueprintBuffReference>("87ab2fed7feaaff47b62a3320a57ad8d");

                var TricksterTTTGloryDomainGreaterResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTGloryDomainGreaterResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterClasses,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 0,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                });
                var TricksterTTTGloryDomainGreaterAura = Helpers.CreateBlueprint<BlueprintAbilityAreaEffect>(TTTContext, "TricksterTTTGloryDomainGreaterAura", bp => {
                    bp.ApplyVisualsAndBasicSettings(GloryDomainGreaterArea);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Type = AbilityRankType.DamageBonus;
                        c.m_Progression = ContextRankProgression.AsIs;
                    });
                    bp.AddComponent<AbilityAreaEffectBuff>(c => {
                        c.m_Buff = HeroismBuff;
                        c.Condition = new ConditionsChecker() {
                            Conditions = new Condition[] {
                                new ContextConditionIsAlly()
                            }
                        };
                    });

                });
                var TricksterTTTGloryDomainGreaterBuff = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterTTTGloryDomainGreaterBuff", bp => {
                    bp.ApplyVisualsAndBasicSettings(GloryDomainGreaterBuff);
                    bp.AddComponent<AddAreaEffect>(c => {
                        c.m_AreaEffect = TricksterTTTGloryDomainGreaterAura.ToReference<BlueprintAbilityAreaEffectReference>();
                    });
                });
                var TricksterTTTGloryDomainGreaterToggleAbility = Helpers.CreateBlueprint<BlueprintActivatableAbility>(TTTContext, "TricksterTTTGloryDomainGreaterToggleAbility", bp => {
                    bp.AddToDomainZealot();
                    bp.AddTricksterAbilityParams();
                    bp.m_Buff = TricksterTTTGloryDomainGreaterBuff.ToReference<BlueprintBuffReference>();
                    bp.AddComponent<ActivatableAbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTGloryDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.SpendType = ActivatableAbilityResourceLogic.ResourceSpendType.NewRound;
                    });
                    bp.m_DisplayName = GloryDomainGreaterAbility.m_DisplayName;
                    bp.m_Description = GloryDomainGreaterAbility.m_Description;
                    bp.m_DescriptionShort = GloryDomainGreaterAbility.m_DescriptionShort;
                    bp.m_Icon = GloryDomainGreaterAbility.m_Icon;
                    bp.Group = ActivatableAbilityGroup.None;
                    bp.WeightInGroup = 1;
                    bp.DeactivateIfCombatEnded = true;
                    bp.DeactivateIfOwnerDisabled = true;
                    bp.ActivationType = AbilityActivationType.WithUnitCommand;
                    bp.m_ActivateWithUnitCommand = Kingmaker.UnitLogic.Commands.Base.UnitCommand.CommandType.Swift;
                });
                var TricksterTTTGloryDomainGreaterFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTGloryDomainGreaterFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(GloryDomainGreaterFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTGloryDomainGreaterToggleAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTGloryDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });


                var tricksterDomain = CreateTricksterDomain(GloryDomainProgression);
                tricksterDomain.LevelEntries = new LevelEntry[] {
                    Helpers.CreateLevelEntry(1, TricksterTTTGloryDomainBaseFeature),
                    Helpers.CreateLevelEntry(8, TricksterTTTGloryDomainGreaterFeature)
                };
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateGoodDomain() {
                //Base Feature
                var GoodDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("f27684b3b72c2f546abf3ef2fb611a05");
                var GoodDomainBaseAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("017afe6934e10c3489176e759a5f01b0");
                var GoodDomainBaseBuff = BlueprintTools.GetBlueprintReference<BlueprintBuffReference>("f185e4585bda72b479956772944ee665");

                var TricksterTTTGoodDomainBaseResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTGoodDomainBaseResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterMythics,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 3,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                });
                var TricksterTTTGoodDomainBaseBuff = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterTTTGoodDomainBaseBuff", bp => {
                    bp.ApplyVisualsAndBasicSettings(GoodDomainBaseBuff);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.Div2;
                    });
                    bp.AddComponent<AddStatBonusAbilityValue>(c => {
                        c.Stat = StatType.AdditionalAttackBonus;
                        c.Descriptor = ModifierDescriptor.Sacred;
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Rank
                        };
                    });
                    bp.AddComponent<BuffAllSkillsBonusAbilityValue>(c => {
                        c.Descriptor = ModifierDescriptor.Sacred;
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Rank
                        };
                    });
                    bp.AddComponent<AddStatBonusAbilityValue>(c => {
                        c.Stat = StatType.SaveFortitude;
                        c.Descriptor = ModifierDescriptor.Sacred;
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Rank
                        };
                    });
                    bp.AddComponent<AddStatBonusAbilityValue>(c => {
                        c.Stat = StatType.SaveReflex;
                        c.Descriptor = ModifierDescriptor.Sacred;
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Rank
                        };
                    });
                    bp.AddComponent<AddStatBonusAbilityValue>(c => {
                        c.Stat = StatType.SaveWill;
                        c.Descriptor = ModifierDescriptor.Sacred;
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Rank
                        };
                    });
                });
                var TricksterTTTGoodDomainBaseAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTGoodDomainBaseAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(GoodDomainBaseAbility);
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionApplyBuff() {
                                m_Buff = TricksterTTTGoodDomainBaseBuff.ToReference<BlueprintBuffReference>(),
                                DurationValue = new ContextDurationValue() {
                                    Rate = DurationRate.Rounds,
                                    DiceCountValue = 0,
                                    BonusValue = 1
                                }
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTGoodDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                });
                var TricksterTTTGoodDomainBaseFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTGoodDomainBaseFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(GoodDomainBaseFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTGoodDomainBaseAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTGoodDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });
                //Greater Feature
                var GoodDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("c90f8979927db4b4fbf6159297e01af8");
                var GoodDomainGreaterAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("7fc3e743ba28fd64f977fb55b7536053");
                var Holy = BlueprintTools.GetBlueprintReference<BlueprintItemEnchantmentReference>("28a9964d81fedae44bae3ca45710c140");
                var paladinweaponenchant00_precastbody = new PrefabLink() { AssetId = "1a75495d05cf88b4f9702ad5914b506c" };

                var TricksterTTTGoodDomainGreaterResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTGoodDomainGreaterResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = new BlueprintCharacterClassReference[0],
                        m_ClassDiv = ResourcesLibrary.GetRoot().Progression.m_CharacterClasses,
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        StartingLevel = 8,
                        StartingIncrease = 1,
                        LevelStep = 4,
                        PerStepIncrease = 1,
                        IncreasedByLevelStartPlusDivStep = true,
                        IncreasedByStat = false
                    };
                });
                var TricksterTTTGoodDomainGreaterAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTGoodDomainGreaterAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(GoodDomainGreaterAbility);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.Div2;
                    });
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionEnchantWornItem() {
                                m_Enchantment = Holy,
                                DurationValue = new ContextDurationValue() {
                                    Rate = DurationRate.Rounds,
                                    DiceCountValue = 0,
                                    BonusValue = new ContextValue() {
                                        ValueType = ContextValueType.Rank
                                    }
                                }
                            },
                            new ContextActionSpawnFx() {
                                PrefabLink = paladinweaponenchant00_precastbody
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTGoodDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                });
                var TricksterTTTGoodDomainGreaterFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTGoodDomainGreaterFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(GoodDomainGreaterFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTGoodDomainGreaterAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTGoodDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });

                var tricksterDomain = CreateTricksterDomain(GoodDomainProgression);
                tricksterDomain.LevelEntries = new LevelEntry[] {
                    Helpers.CreateLevelEntry(1, TricksterTTTGoodDomainBaseFeature),
                    Helpers.CreateLevelEntry(8, TricksterTTTGoodDomainGreaterFeature)
                };
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateHealingDomain() {
                //Base Feature
                var HealingDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("303cf1c933f343c4d91212f8f4953e3c");
                var HealingDomainBaseAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("18f734e40dd7966438ab32086c3574e1");

                var TricksterTTTHealingDomainBaseResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTHealingDomainBaseResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterMythics,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 3,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                    bp.m_Max = 13;
                    bp.m_UseMax = true;
                });
                var TricksterTTTHealingDomainBaseAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTHealingDomainBaseAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(HealingDomainBaseAbility);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Type = AbilityRankType.DamageBonus;
                        c.m_Progression = ContextRankProgression.Div2;
                    });
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionHealTarget() {
                                Value = new ContextDiceValue() {
                                    DiceType = DiceType.D4,
                                    DiceCountValue = 1,
                                    BonusValue = new ContextValue() {
                                        ValueType = ContextValueType.Rank,
                                        ValueRank = AbilityRankType.DamageBonus
                                    }
                                }
                            }
                        );
                    });
                    bp.AddComponent<AbilityTargetHPCondition>();
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTHealingDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                    HealingDomainBaseAbility.ComponentsArray.OfType<AbilityCasterHasNoFacts>().ForEach(c => {
                        bp.AddComponent(Helpers.CreateCopy(c));
                    });
                });
                var TricksterTTTHealingDomainBaseFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTHealingDomainBaseFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(HealingDomainBaseFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTHealingDomainBaseAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTHealingDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                    bp.AddComponent<AddClassSkill>(c => {
                        c.Skill = StatType.SkillThievery;
                    });
                    bp.AddComponent<AddClassSkill>(c => {
                        c.Skill = StatType.SkillStealth;
                    });
                });
                //Greater Feature
                var HealingDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("b9ea4eb16ded8b146868540e711f81c8");

                var tricksterDomain = CreateTricksterDomain(HealingDomainProgression);
                tricksterDomain.LevelEntries = new LevelEntry[] {
                    Helpers.CreateLevelEntry(1, TricksterTTTHealingDomainBaseFeature),
                    Helpers.CreateLevelEntry(6, HealingDomainGreaterFeature)
                };
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateKnowledgeDomain() {
                //Base Feature
                var KnowledgeDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("5335f015063776d429a0b5eab97eb060");
                var KnowledgeDomainBaseAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("02a79a205bce6f5419dcdf26b64f13c6");
                var KnowledgeDomainBaseBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("35fa55fe2c60e4442b670a88a70c06c3");

                var TricksterTTTKnowledgeDomainBaseResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTKnowledgeDomainBaseResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterClasses,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 0,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                });
                var TricksterTTTKnowledgeDomainBaseBuff = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterTTTKnowledgeDomainBaseBuff", bp => {
                    bp.ApplyVisualsAndBasicSettings(KnowledgeDomainBaseBuff);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Type = AbilityRankType.StatBonus;
                        c.m_Progression = ContextRankProgression.DivStep;
                        c.m_StepLevel = 4;
                        c.m_Min = 1;
                        c.m_UseMin = true;
                    });
                    bp.AddComponent<AddImmunityToCriticalHits>();
                    bp.AddComponent<AddStatBonusAbilityValue>(c => {
                        c.Stat = StatType.AC;
                        c.Descriptor = ModifierDescriptor.Deflection;
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Rank,
                            ValueRank = AbilityRankType.StatBonus
                        };
                    });
                });
                var TricksterTTTKnowledgeDomainBaseToggleAbility = Helpers.CreateBlueprint<BlueprintActivatableAbility>(TTTContext, "TricksterTTTKnowledgeDomainBaseToggleAbility", bp => {
                    bp.AddToDomainZealot();
                    bp.AddTricksterAbilityParams();
                    bp.m_Buff = TricksterTTTKnowledgeDomainBaseBuff.ToReference<BlueprintBuffReference>();
                    bp.AddComponent<ActivatableAbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTKnowledgeDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.SpendType = ActivatableAbilityResourceLogic.ResourceSpendType.NewRound;
                    });
                    bp.m_DisplayName = KnowledgeDomainBaseAbility.m_DisplayName;
                    bp.SetDescription(TTTContext, "You can become semi-tangible as a standard action. " +
                        "While in this form, you are immune to critical hits and gain a +1 deflection bonus to AC. " +
                        "This bonus increases by 1 at 8th level and every 4 levels thereafter.");
                    bp.m_DescriptionShort = KnowledgeDomainBaseAbility.m_DescriptionShort;
                    bp.m_Icon = KnowledgeDomainBaseAbility.m_Icon;
                    bp.Group = ActivatableAbilityGroup.None;
                    bp.WeightInGroup = 1;
                    bp.DeactivateIfCombatEnded = true;
                    bp.DeactivateIfOwnerDisabled = true;
                    bp.ActivationType = AbilityActivationType.WithUnitCommand;
                    bp.m_ActivateWithUnitCommand = Kingmaker.UnitLogic.Commands.Base.UnitCommand.CommandType.Standard;
                });
                var TricksterTTTKnowledgeDomainBaseFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTKnowledgeDomainBaseFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(KnowledgeDomainBaseFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTKnowledgeDomainBaseToggleAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTKnowledgeDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });
                //Greater Feature
                var KnowledgeDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("74ac5dbc420501c4cae29a9db24e4e3a");
                var KnowledgeDomainGreaterAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("ec582b195ccb2ef4ea8dcd96a5a6e009");
                var KnowledgeDomainGreaterBuff = BlueprintTools.GetBlueprintReference<BlueprintBuffReference>("847d4ac01ce1b0247af53eb9b2a6b782");

                var TricksterTTTKnowledgeDomainGreaterResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTKnowledgeDomainGreaterResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = new BlueprintCharacterClassReference[0],
                        m_ClassDiv = ResourcesLibrary.GetRoot().Progression.m_CharacterClasses,
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        StartingLevel = 8,
                        StartingIncrease = 1,
                        LevelStep = 4,
                        PerStepIncrease = 1,
                        IncreasedByLevelStartPlusDivStep = true,
                        IncreasedByStat = false
                    };
                });
                var TricksterTTTKnowledgeDomainGreaterAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTKnowledgeDomainGreaterAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(KnowledgeDomainGreaterAbility);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.Div2;
                    });
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionApplyBuff() {
                                m_Buff = KnowledgeDomainGreaterBuff,
                                DurationValue = new ContextDurationValue() {
                                    Rate = DurationRate.Rounds,
                                    DiceCountValue = 0,
                                    BonusValue = new ContextValue() {
                                        ValueType = ContextValueType.Rank
                                    }
                                }
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTKnowledgeDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                });
                var TricksterTTTKnowledgeDomainGreaterFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTKnowledgeDomainGreaterFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(KnowledgeDomainGreaterFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTKnowledgeDomainGreaterAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTKnowledgeDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });

                var tricksterDomain = CreateTricksterDomain(KnowledgeDomainProgression);
                tricksterDomain.LevelEntries = new LevelEntry[] {
                    Helpers.CreateLevelEntry(1, TricksterTTTKnowledgeDomainBaseFeature),
                    Helpers.CreateLevelEntry(8, TricksterTTTKnowledgeDomainGreaterFeature)
                };
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateLawDomain() {
                //Base Feature
                var LawDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("9bd2d216e56a0db44be0df48ffc515af");
                var LawDomainBaseAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("a970537ea2da20e42ae709c0bb8f793f");
                var LawDomainBaseBuff = BlueprintTools.GetBlueprintReference<BlueprintBuffReference>("da0248dbcf8b8ab43bf3f17953d47044");

                var TricksterTTTLawDomainBaseResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTLawDomainBaseResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterMythics,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 3,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                });
                var TricksterTTTLawDomainBaseAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTLawDomainBaseAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(LawDomainBaseAbility);
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionApplyBuff() {
                                m_Buff = LawDomainBaseBuff,
                                DurationValue = new ContextDurationValue() {
                                    Rate = DurationRate.Rounds,
                                    DiceCountValue = 0,
                                    BonusValue = 1
                                }
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTLawDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                });
                var TricksterTTTLawDomainBaseFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTLawDomainBaseFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(LawDomainBaseFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTLawDomainBaseAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTLawDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });
                //Greater Feature
                var LawDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("3dc5e2b315ff07f438582a2468beb1fb");
                var LawDomainGreaterAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("0b1615ec2dabc6f4294a254b709188a4");
                var Axiomatic = BlueprintTools.GetBlueprintReference<BlueprintItemEnchantmentReference>("0ca43051edefcad4b9b2240aa36dc8d4");
                var paladinweaponenchant00_precastbody = new PrefabLink() { AssetId = "1a75495d05cf88b4f9702ad5914b506c" };

                var TricksterTTTLawDomainGreaterResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTLawDomainGreaterResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = new BlueprintCharacterClassReference[0],
                        m_ClassDiv = ResourcesLibrary.GetRoot().Progression.m_CharacterClasses,
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        StartingLevel = 8,
                        StartingIncrease = 1,
                        LevelStep = 4,
                        PerStepIncrease = 1,
                        IncreasedByLevelStartPlusDivStep = true,
                        IncreasedByStat = false
                    };
                });
                var TricksterTTTLawDomainGreaterAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTLawDomainGreaterAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(LawDomainGreaterAbility);
                    bp.Animation = UnitAnimationActionCastSpell.CastAnimationStyle.Touch;
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.Div2;
                    });
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionEnchantWornItem() {
                                m_Enchantment = Axiomatic,
                                DurationValue = new ContextDurationValue() {
                                    Rate = DurationRate.Rounds,
                                    DiceCountValue = 0,
                                    BonusValue = new ContextValue() {
                                        ValueType = ContextValueType.Rank
                                    }
                                }
                            },
                            new ContextActionSpawnFx() {
                                PrefabLink = paladinweaponenchant00_precastbody
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTLawDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                });
                var TricksterTTTLawDomainGreaterFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTLawDomainGreaterFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(LawDomainGreaterFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTLawDomainGreaterAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTLawDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });

                var tricksterDomain = CreateTricksterDomain(LawDomainProgression);
                tricksterDomain.LevelEntries = new LevelEntry[] {
                    Helpers.CreateLevelEntry(1, TricksterTTTLawDomainBaseFeature),
                    Helpers.CreateLevelEntry(8, TricksterTTTLawDomainGreaterFeature)
                };
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateLiberationDomain() {
                //Base Feature
                var LiberationDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("7cc934aa505172a40b4a10c14c7681c4");
                var LiberationDomainBaseActivateableAbility = BlueprintTools.GetBlueprint<BlueprintActivatableAbility>("edaac27ed85814b438ea7908b5226684");
                var FreedomOfMovementBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("1533e782fca42b84ea370fc1dcbf4fc1");

                var TricksterTTTLiberationDomainBaseResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTLiberationDomainBaseResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterClasses,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 0,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                });
                var TricksterTTTLiberationDomainBaseEffect = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterTTTLiberationDomainBaseEffect", bp => {
                    bp.ApplyVisualsAndBasicSettings(FreedomOfMovementBuff);
                    FreedomOfMovementBuff.ComponentsArray.ForEach(c => {
                        bp.AddComponent(Helpers.CreateCopy(c));
                    });
                });
                var TricksterTTTLiberationDomainBaseBuff = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterTTTLiberationDomainBaseBuff", bp => {
                    bp.m_Flags = BlueprintBuff.Flags.HiddenInUi;
                    bp.AddComponent<AddConditionTrigger>(c => {
                        c.Conditions = new UnitCondition[] {
                            UnitCondition.Paralyzed,
                            UnitCondition.Entangled,
                            UnitCondition.DifficultTerrain,
                            UnitCondition.Slowed,
                            UnitCondition.CantMove
                        };
                        c.Action = Helpers.CreateActionList(
                            new ContextActionApplyBuff() {
                                m_Buff = TricksterTTTLiberationDomainBaseEffect.ToReference<BlueprintBuffReference>(),
                                DurationValue = new ContextDurationValue() {
                                    DiceCountValue = 0,
                                    BonusValue = 1
                                }
                            },
                            new ContextSpendResource() {
                                m_Resource = TricksterTTTLiberationDomainBaseResource.ToReference<BlueprintAbilityResourceReference>(),
                                Value = 1
                            },
                            new Conditional() {
                                ConditionsChecker = new ConditionsChecker() {
                                    Conditions = new Condition[] {
                                        new ContextConditionCasterHasResource(){
                                            m_Resource = TricksterTTTLiberationDomainBaseResource.ToReference<BlueprintAbilityResourceReference>(),
                                            Amount = 1
                                        }
                                    }
                                },
                                IfTrue = Helpers.CreateActionList(),
                                IfFalse = Helpers.CreateActionList(
                                    new ContextActionRemoveSelf()
                                )
                            }
                        );
                    });
                });
                var TricksterTTTLiberationDomainBaseToggleAbility = Helpers.CreateBlueprint<BlueprintActivatableAbility>(TTTContext, "TricksterTTTLiberationDomainBaseToggleAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(LiberationDomainBaseActivateableAbility);
                    bp.m_Buff = TricksterTTTLiberationDomainBaseBuff.ToReference<BlueprintBuffReference>();
                    bp.ActivationType = AbilityActivationType.Immediately;
                    bp.DeactivateImmediately = true;
                    bp.DeactivateIfCombatEnded = false;
                    bp.IsOnByDefault = true;
                    bp.AddComponent<ActivatableAbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTLiberationDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.SpendType = ActivatableAbilityResourceLogic.ResourceSpendType.Never;
                    });
                });
                var TricksterTTTLiberationDomainBaseFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTLiberationDomainBaseFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(LiberationDomainBaseFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTLiberationDomainBaseToggleAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTLiberationDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });
                //Greater Feature
                var LiberationDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("25636a46d4e7a484d903946ef4a6f6db");
                var LiberationDomainGreaterToggleAbility = BlueprintTools.GetBlueprint<BlueprintActivatableAbility>("627cfc87590b0e14f863fdb9bc40787b");
                var LiberationDomainGreaterBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("aa561f70d2260524e82c794d6140677c");
                var LiberationDomainGreaterAura = BlueprintTools.GetBlueprint<BlueprintAbilityAreaEffect>("cd23b709497500142b59802d7bc85edc");
                var LiberationDomainGreaterEffect = BlueprintTools.GetBlueprint<BlueprintBuff>("649d53bad4b29ee42abc06ad28d297c8");

                var TricksterTTTLiberationDomainGreaterResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTLiberationDomainGreaterResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterClasses,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 0,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                });
                var TricksterTTTLiberationDomainGreaterAura = Helpers.CreateBlueprint<BlueprintAbilityAreaEffect>(TTTContext, "TricksterTTTLiberationDomainGreaterAura", bp => {
                    bp.ApplyVisualsAndBasicSettings(LiberationDomainGreaterAura);
                    bp.AddComponent<AbilityAreaEffectBuff>(c => {
                        c.m_Buff = LiberationDomainGreaterEffect.ToReference<BlueprintBuffReference>();
                        c.Condition = new ConditionsChecker() {
                            Conditions = new Condition[] {
                                new ContextConditionIsAlly()
                            }
                        };
                    });
                });
                var TricksterTTTLiberationDomainGreaterBuff = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterTTTLiberationDomainGreaterBuff", bp => {
                    bp.ApplyVisualsAndBasicSettings(LiberationDomainGreaterBuff);
                    bp.AddComponent<AddAreaEffect>(c => {
                        c.m_AreaEffect = TricksterTTTLiberationDomainGreaterAura.ToReference<BlueprintAbilityAreaEffectReference>();
                    });
                });
                var TricksterTTTLiberationDomainGreaterToggleAbility = Helpers.CreateBlueprint<BlueprintActivatableAbility>(TTTContext, "TricksterTTTLiberationDomainGreaterToggleAbility", bp => {
                    bp.m_Buff = TricksterTTTLiberationDomainGreaterBuff.ToReference<BlueprintBuffReference>();
                    bp.ApplyVisualsAndBasicSettings(LiberationDomainGreaterToggleAbility);
                    bp.AddComponent<ActivatableAbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTLiberationDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.SpendType = ActivatableAbilityResourceLogic.ResourceSpendType.NewRound;
                    });
                });
                var TricksterTTTLiberationDomainGreaterFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTLiberationDomainGreaterFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(LiberationDomainGreaterFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTLiberationDomainGreaterToggleAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTLiberationDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });
                var tricksterDomain = CreateTricksterDomain(LiberationDomainProgression);
                tricksterDomain.LevelEntries = new LevelEntry[] {
                    Helpers.CreateLevelEntry(1, TricksterTTTLiberationDomainBaseFeature),
                    Helpers.CreateLevelEntry(8, TricksterTTTLiberationDomainGreaterFeature)
                };
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateLuckDomain() {
                //Base Feature
                var LuckDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("2b3818bf4656c1a41b93467755662c78");
                var LuckDomainBaseAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("9af0b584f6f754045a0a79293d100ab3");
                var LuckDomainBaseBuff = BlueprintTools.GetBlueprintReference<BlueprintBuffReference>("3bc40c9cbf9a0db4b8b43d8eedf2e6ec");

                var TricksterTTTLuckDomainBaseResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTLuckDomainBaseResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterMythics,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 3,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                });
                var TricksterTTTLuckDomainBaseAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTLuckDomainBaseAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(LuckDomainBaseAbility);
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionApplyBuff() {
                                m_Buff = LuckDomainBaseBuff,
                                DurationValue = new ContextDurationValue() {
                                    Rate = DurationRate.Rounds,
                                    DiceCountValue = 0,
                                    BonusValue = 1
                                }
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTLuckDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                });
                var TricksterTTTLuckDomainBaseFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTLuckDomainBaseFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(LuckDomainBaseFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTLuckDomainBaseAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTLuckDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });
                //Greater Feature
                var LuckDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("dd58b458af054e642bf845c3f01307e5");
                var LuckDomainGreaterAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("0e0668a703fbfcf499d9aa9d918b71ea");

                var TricksterTTTLuckDomainGreaterResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTLuckDomainGreaterResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = new BlueprintCharacterClassReference[0],
                        m_ClassDiv = ResourcesLibrary.GetRoot().Progression.m_CharacterClasses,
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        StartingLevel = 6,
                        StartingIncrease = 1,
                        LevelStep = 6,
                        PerStepIncrease = 1,
                        IncreasedByLevelStartPlusDivStep = true,
                        IncreasedByStat = false
                    };
                });
                var TricksterTTTLuckDomainGreaterAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTLuckDomainGreaterAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(LuckDomainGreaterAbility);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.Div2;
                    });
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionApplyBuff() {
                                m_Buff = LuckDomainBaseBuff,
                                DurationValue = new ContextDurationValue() {
                                    Rate = DurationRate.Rounds,
                                    DiceCountValue = 0,
                                    BonusValue = new ContextValue() {
                                        ValueType = ContextValueType.Rank
                                    }
                                }
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTLuckDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                });
                var TricksterTTTLuckDomainGreaterFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTLuckDomainGreaterFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(LuckDomainGreaterFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTLuckDomainGreaterAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTLuckDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });

                var tricksterDomain = CreateTricksterDomain(LuckDomainProgression);
                tricksterDomain.LevelEntries = new LevelEntry[] {
                    Helpers.CreateLevelEntry(1, TricksterTTTLuckDomainBaseFeature),
                    Helpers.CreateLevelEntry(6, TricksterTTTLuckDomainGreaterFeature)
                };
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateMadnessDomain() {
                //Base Feature
                var MadnessDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("84bf46e8086dbdc438bac875ab0e5c2f");
                var MadnessDomainBaseAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("9246020fe13095346946ff3101d9f60d");
                var MadnessDomainBaseAbilityAttackRolls = BlueprintTools.GetBlueprint<BlueprintAbility>("c3e4ff89950f1d748be6f5958b1aa19c");
                var MadnessDomainBaseAbilitySavingThrows = BlueprintTools.GetBlueprint<BlueprintAbility>("c09446b861bac7b4b83877db863150d9");
                var MadnessDomainBaseAbilitySkillChecks = BlueprintTools.GetBlueprint<BlueprintAbility>("d92b2eac4dbf31f439e5bc9d2d467ff1");
                var MadnessDomainBaseAttackRollsBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("6c69ec7a32190d44d99e746588de4a9c");
                var MadnessDomainBaseSavingThrowsBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("53c721d7519ac3047b818516bb28b20f");
                var MadnessDomainBaseSkillChecksBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("3e42877e5e481894880df63ad924e320");

                var TricksterTTTMadnessDomainBaseResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTMadnessDomainBaseResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterMythics,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 3,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                    bp.m_Max = 13;
                    bp.m_UseMax = true;
                });
                var TricksterTTTMadnessDomainBaseAttackRollsBuff = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterTTTMadnessDomainBaseAttackRollsBuff", bp => {
                    bp.ApplyVisualsAndBasicSettings(MadnessDomainBaseAttackRollsBuff);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.Div2;
                    });
                    bp.AddComponent<ContextCalculateSharedValue>(c => {
                        c.ValueType = AbilitySharedValue.StatBonus;
                        c.Value = new ContextDiceValue() {
                            DiceCountValue = 0,
                            BonusValue = new ContextValue() {
                                ValueType = ContextValueType.Rank
                            }
                        };
                        c.Modifier = -1;
                    });
                    bp.AddComponent<AddStatBonusAbilityValue>(c => {
                        c.Stat = StatType.AdditionalAttackBonus;
                        c.Descriptor = ModifierDescriptor.UntypedStackable;
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Rank
                        };
                    });
                    bp.AddComponent<BuffAllSkillsBonusAbilityValue>(c => {
                        c.Descriptor = ModifierDescriptor.UntypedStackable;
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Shared,
                            ValueShared = AbilitySharedValue.StatBonus
                        };
                    });
                    bp.AddComponent<AddStatBonusAbilityValue>(c => {
                        c.Stat = StatType.SaveFortitude;
                        c.Descriptor = ModifierDescriptor.UntypedStackable;
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Shared,
                            ValueShared = AbilitySharedValue.StatBonus
                        };
                    });
                    bp.AddComponent<AddStatBonusAbilityValue>(c => {
                        c.Stat = StatType.SaveReflex;
                        c.Descriptor = ModifierDescriptor.UntypedStackable;
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Shared,
                            ValueShared = AbilitySharedValue.StatBonus
                        };
                    });
                    bp.AddComponent<AddStatBonusAbilityValue>(c => {
                        c.Stat = StatType.SaveWill;
                        c.Descriptor = ModifierDescriptor.UntypedStackable;
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Shared,
                            ValueShared = AbilitySharedValue.StatBonus
                        };
                    });
                });
                var TricksterTTTMadnessDomainBaseSavingThrowsBuff = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterTTTMadnessDomainBaseSavingThrowsBuff", bp => {
                    bp.ApplyVisualsAndBasicSettings(MadnessDomainBaseSavingThrowsBuff);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.Div2;
                    });
                    bp.AddComponent<ContextCalculateSharedValue>(c => {
                        c.ValueType = AbilitySharedValue.StatBonus;
                        c.Value = new ContextDiceValue() {
                            DiceCountValue = 0,
                            BonusValue = new ContextValue() {
                                ValueType = ContextValueType.Rank
                            }
                        };
                        c.Modifier = -1;
                    });
                    bp.AddComponent<AddStatBonusAbilityValue>(c => {
                        c.Stat = StatType.AdditionalAttackBonus;
                        c.Descriptor = ModifierDescriptor.UntypedStackable;
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Shared,
                            ValueShared = AbilitySharedValue.StatBonus
                        };
                    });
                    bp.AddComponent<BuffAllSkillsBonusAbilityValue>(c => {
                        c.Descriptor = ModifierDescriptor.UntypedStackable;
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Shared,
                            ValueShared = AbilitySharedValue.StatBonus
                        };
                    });
                    bp.AddComponent<AddStatBonusAbilityValue>(c => {
                        c.Stat = StatType.SaveFortitude;
                        c.Descriptor = ModifierDescriptor.UntypedStackable;
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Rank
                        };
                    });
                    bp.AddComponent<AddStatBonusAbilityValue>(c => {
                        c.Stat = StatType.SaveReflex;
                        c.Descriptor = ModifierDescriptor.UntypedStackable;
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Rank
                        };
                    });
                    bp.AddComponent<AddStatBonusAbilityValue>(c => {
                        c.Stat = StatType.SaveWill;
                        c.Descriptor = ModifierDescriptor.UntypedStackable;
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Rank
                        };
                    });
                });
                var TricksterTTTMadnessDomainBaseSkillChecksBuff = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterTTTMadnessDomainBaseSkillChecksBuff", bp => {
                    bp.ApplyVisualsAndBasicSettings(MadnessDomainBaseSkillChecksBuff);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.Div2;
                    });
                    bp.AddComponent<ContextCalculateSharedValue>(c => {
                        c.ValueType = AbilitySharedValue.StatBonus;
                        c.Value = new ContextDiceValue() {
                            DiceCountValue = 0,
                            BonusValue = new ContextValue() {
                                ValueType = ContextValueType.Rank
                            }
                        };
                        c.Modifier = -1;
                    });
                    bp.AddComponent<AddStatBonusAbilityValue>(c => {
                        c.Stat = StatType.AdditionalAttackBonus;
                        c.Descriptor = ModifierDescriptor.UntypedStackable;
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Shared,
                            ValueShared = AbilitySharedValue.StatBonus
                        };
                    });
                    bp.AddComponent<BuffAllSkillsBonusAbilityValue>(c => {
                        c.Descriptor = ModifierDescriptor.UntypedStackable;
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Rank
                        };
                    });
                    bp.AddComponent<AddStatBonusAbilityValue>(c => {
                        c.Stat = StatType.SaveFortitude;
                        c.Descriptor = ModifierDescriptor.UntypedStackable;
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Shared,
                            ValueShared = AbilitySharedValue.StatBonus
                        };
                    });
                    bp.AddComponent<AddStatBonusAbilityValue>(c => {
                        c.Stat = StatType.SaveReflex;
                        c.Descriptor = ModifierDescriptor.UntypedStackable;
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Shared,
                            ValueShared = AbilitySharedValue.StatBonus
                        };
                    });
                    bp.AddComponent<AddStatBonusAbilityValue>(c => {
                        c.Stat = StatType.SaveWill;
                        c.Descriptor = ModifierDescriptor.UntypedStackable;
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Shared,
                            ValueShared = AbilitySharedValue.StatBonus
                        };
                    });
                });
                var TricksterTTTMadnessDomainBaseAbilityAttackRolls = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTMadnessDomainBaseAbilityAttackRolls", bp => {
                    bp.ApplyVisualsAndBasicSettings(MadnessDomainBaseAbilityAttackRolls);
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionApplyBuff() {
                                m_Buff = TricksterTTTMadnessDomainBaseAttackRollsBuff.ToReference<BlueprintBuffReference>(),
                                DurationValue = new ContextDurationValue() {
                                    DiceCountValue = 0,
                                    BonusValue = 3
                                }
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTMadnessDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                });
                var TricksterTTTMadnessDomainBaseAbilitySavingThrows = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTMadnessDomainBaseAbilitySavingThrows", bp => {
                    bp.ApplyVisualsAndBasicSettings(MadnessDomainBaseAbilitySavingThrows);
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionApplyBuff() {
                                m_Buff = TricksterTTTMadnessDomainBaseSavingThrowsBuff.ToReference<BlueprintBuffReference>(),
                                DurationValue = new ContextDurationValue() {
                                    DiceCountValue = 0,
                                    BonusValue = 3
                                }
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTMadnessDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                });
                var TricksterTTTMadnessDomainBaseAbilitySkillChecks = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTMadnessDomainBaseAbilitySkillChecks", bp => {
                    bp.ApplyVisualsAndBasicSettings(MadnessDomainBaseAbilitySkillChecks);
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionApplyBuff() {
                                m_Buff = TricksterTTTMadnessDomainBaseSkillChecksBuff.ToReference<BlueprintBuffReference>(),
                                DurationValue = new ContextDurationValue() {
                                    DiceCountValue = 0,
                                    BonusValue = 3
                                }
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTMadnessDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                });
                var TricksterTTTMadnessDomainBaseAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTMadnessDomainBaseAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(MadnessDomainBaseAbility);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.Div2;
                    });
                    bp.AddComponent<AbilityVariants>(c => {
                        c.m_Variants = new BlueprintAbilityReference[] {
                            TricksterTTTMadnessDomainBaseAbilityAttackRolls.ToReference<BlueprintAbilityReference>(),
                            TricksterTTTMadnessDomainBaseAbilitySavingThrows.ToReference<BlueprintAbilityReference>(),
                            TricksterTTTMadnessDomainBaseAbilitySkillChecks.ToReference<BlueprintAbilityReference>()
                        };
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTMadnessDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                    TricksterTTTMadnessDomainBaseAbilityAttackRolls.m_Parent = bp.ToReference<BlueprintAbilityReference>();
                    TricksterTTTMadnessDomainBaseAbilitySavingThrows.m_Parent = bp.ToReference<BlueprintAbilityReference>();
                    TricksterTTTMadnessDomainBaseAbilitySkillChecks.m_Parent = bp.ToReference<BlueprintAbilityReference>();
                });
                var TricksterTTTMadnessDomainBaseFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTMadnessDomainBaseFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(MadnessDomainBaseFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTMadnessDomainBaseAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTMadnessDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });
                //Greater Feature
                var MadnessDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("9acc8ab2f313d0e49bb01e030c868e3f");
                var MadnessDomainGreaterAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("a3d470a27ec5e4540aeaf9723e9b8ae7");
                var MadnessDomainGreaterBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("73192f96dd97b634cb794ae42f92c2ff");
                var MadnessDomainGreaterArea = BlueprintTools.GetBlueprint<BlueprintAbilityAreaEffect>("19ee79b1da25ea049ba4fea92c2a4025");
                var MadnessDomainGreaterImmunityBuff = BlueprintTools.GetBlueprintReference<BlueprintBuffReference>("0d77f718092b9c149bfc43c40262e837");
                var MadnessDomainGreaterEffectBuff = BlueprintTools.GetBlueprintReference<BlueprintBuffReference>("90f09217c6366414fb3edde07838806e");
                var Confusion = BlueprintTools.GetBlueprintReference<BlueprintBuffReference>("886c7407dc629dc499b9f1465ff382df");
                var InsanityBuff = BlueprintTools.GetBlueprintReference<BlueprintBuffReference>("53808be3c2becd24dbe572f77a7f44f8");

                var TricksterTTTMadnessDomainGreaterResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTMadnessDomainGreaterResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterClasses,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 0,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                });
                var TricksterTTTMadnessDomainGreaterAura = Helpers.CreateBlueprint<BlueprintAbilityAreaEffect>(TTTContext, "TricksterTTTMadnessDomainGreaterAura", bp => {
                    bp.ApplyVisualsAndBasicSettings(MadnessDomainGreaterArea);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Type = AbilityRankType.DamageBonus;
                        c.m_Progression = ContextRankProgression.AsIs;
                    });
                    bp.AddComponent<AbilityAreaEffectRunAction>(c => {
                        c.UnitEnter = Helpers.CreateActionList(
                            new Conditional() {
                                ConditionsChecker = new ConditionsChecker() {
                                    Conditions = new Condition[] {
                                        new ContextConditionIsEnemy(),
                                        new ContextConditionHasBuff(){
                                            m_Buff = MadnessDomainGreaterImmunityBuff,
                                            Not = true
                                        },
                                        new ContextConditionHasBuff(){
                                            m_Buff = Confusion,
                                            Not = true
                                        },
                                        new ContextConditionHasBuff(){
                                            m_Buff = InsanityBuff,
                                            Not = true
                                        }
                                    }
                                },
                                IfFalse = Helpers.CreateActionList(),
                                IfTrue = Helpers.CreateActionList(
                                    new ContextActionSavingThrow() {
                                        Type = SavingThrowType.Will,
                                        CustomDC = new ContextValue(),
                                        Actions = Helpers.CreateActionList(
                                            new ContextActionConditionalSaved() {
                                                Succeed = Helpers.CreateActionList(
                                                    new ContextActionApplyBuff() {
                                                        m_Buff = MadnessDomainGreaterImmunityBuff,
                                                        DurationValue = new ContextDurationValue() {
                                                            Rate = DurationRate.Days,
                                                            DiceCountValue = 0,
                                                            BonusValue = 1
                                                        }
                                                    }
                                                ),
                                                Failed = Helpers.CreateActionList(
                                                    new ContextActionApplyBuff() {
                                                        m_Buff = MadnessDomainGreaterEffectBuff,
                                                        Permanent = true,
                                                        DurationValue = new ContextDurationValue() {
                                                            DiceCountValue = 0,
                                                            BonusValue = 1
                                                        }
                                                    },
                                                    new ContextActionApplyBuff() {
                                                        m_Buff = MadnessDomainGreaterImmunityBuff,
                                                        DurationValue = new ContextDurationValue() {
                                                            Rate = DurationRate.Days,
                                                            DiceCountValue = 0,
                                                            BonusValue = 1
                                                        }
                                                    }
                                                )
                                            }
                                        )
                                    }
                                )
                            }
                        );
                        c.UnitExit = Helpers.CreateActionList(
                            new Conditional() {
                                ConditionsChecker = new ConditionsChecker() {
                                    Conditions = new Condition[] {
                                        new ContextConditionIsEnemy(),
                                        new ContextConditionHasBuff(){
                                            m_Buff = MadnessDomainGreaterEffectBuff,
                                        }
                                    }
                                },
                                IfFalse = Helpers.CreateActionList(),
                                IfTrue = Helpers.CreateActionList(
                                    new ContextActionRemoveBuff() {
                                        m_Buff = MadnessDomainGreaterEffectBuff
                                    }
                                )
                            }
                        );
                        c.UnitMove = Helpers.CreateActionList();
                        c.Round = Helpers.CreateActionList();
                    });
                });
                var TricksterTTTMadnessDomainGreaterBuff = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterTTTMadnessDomainGreaterBuff", bp => {
                    bp.ApplyVisualsAndBasicSettings(MadnessDomainGreaterBuff);
                    bp.AddComponent<AddAreaEffect>(c => {
                        c.m_AreaEffect = TricksterTTTMadnessDomainGreaterAura.ToReference<BlueprintAbilityAreaEffectReference>();
                    });
                });
                var TricksterTTTMadnessDomainGreaterToggleAbility = Helpers.CreateBlueprint<BlueprintActivatableAbility>(TTTContext, "TricksterTTTMadnessDomainGreaterToggleAbility", bp => {
                    bp.AddToDomainZealot();
                    bp.AddTricksterAbilityParams();
                    bp.m_Buff = TricksterTTTMadnessDomainGreaterBuff.ToReference<BlueprintBuffReference>();
                    bp.AddComponent<ActivatableAbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTMadnessDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.SpendType = ActivatableAbilityResourceLogic.ResourceSpendType.NewRound;
                    });
                    bp.m_DisplayName = MadnessDomainGreaterAbility.m_DisplayName;
                    bp.m_Description = MadnessDomainGreaterAbility.m_Description;
                    bp.m_DescriptionShort = MadnessDomainGreaterAbility.m_DescriptionShort;
                    bp.m_Icon = MadnessDomainGreaterAbility.m_Icon;
                    bp.Group = ActivatableAbilityGroup.None;
                    bp.WeightInGroup = 1;
                    bp.DeactivateIfCombatEnded = true;
                    bp.DeactivateIfOwnerDisabled = true;
                    bp.ActivationType = AbilityActivationType.WithUnitCommand;
                    bp.m_ActivateWithUnitCommand = Kingmaker.UnitLogic.Commands.Base.UnitCommand.CommandType.Standard;
                });
                var TricksterTTTMadnessDomainGreaterFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTMadnessDomainGreaterFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(MadnessDomainGreaterFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTMadnessDomainGreaterToggleAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTMadnessDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });

                var tricksterDomain = CreateTricksterDomain(MadnessDomainProgression);
                tricksterDomain.LevelEntries = new LevelEntry[] {
                    Helpers.CreateLevelEntry(1, TricksterTTTMadnessDomainBaseFeature),
                    Helpers.CreateLevelEntry(8, TricksterTTTMadnessDomainGreaterFeature)
                };
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateMagicDomain() {
                //Base Feature
                var MagicDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("90f939eb611ac3743b5de3dd00135e22");
                var MagicDomainBaseAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("8e40da3ef31245d468de08394504920b");

                var TricksterTTTMagicDomainBaseResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTMagicDomainBaseResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterMythics,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 3,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                    bp.m_Max = 13;
                    bp.m_UseMax = true;
                });
                var TricksterTTTMagicDomainBaseAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTMagicDomainBaseAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(MagicDomainBaseAbility);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.Div2;
                    });
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionDealWeaponDamage()
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTMagicDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                });
                var TricksterTTTMagicDomainBaseFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTMagicDomainBaseFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(MagicDomainBaseFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTMagicDomainBaseAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTMagicDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });
                //Greater Feature
                var MagicDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("cf47e96abd88c9f418f8e67f5a14381f");
                var MagicDomainGreaterAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("88d06eced626a1f40badfd0cfb4b8f38");

                var TricksterTTTMagicDomainGreaterResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTMagicDomainGreaterResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = new BlueprintCharacterClassReference[0],
                        m_ClassDiv = ResourcesLibrary.GetRoot().Progression.m_CharacterClasses,
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 0,
                        StartingLevel = 8,
                        StartingIncrease = 1,
                        LevelStep = 4,
                        PerStepIncrease = 1,
                        IncreasedByLevelStartPlusDivStep = true,
                        IncreasedByStat = false
                    };
                });
                var TricksterTTTMagicDomainGreaterAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTMagicDomainGreaterAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(MagicDomainGreaterAbility);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.AsIs;
                    });
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionDispelMagic() {
                                OneRollForAll = true,
                                m_CheckType = Kingmaker.RuleSystem.Rules.RuleDispelMagic.CheckType.CasterLevel,
                                m_BuffType = ContextActionDispelMagic.BuffType.FromSpells,
                                m_StopAfterCountRemoved = true,
                                m_CountToRemove = 1,
                                m_MaxSpellLevel = new ContextValue(),
                                m_MaxCasterLevel = new ContextValue(),
                                ContextBonus = new ContextValue(),
                                OnSuccess = Helpers.CreateActionList(),
                                OnFail = Helpers.CreateActionList()
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTMagicDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                });
                var TricksterTTTMagicDomainGreaterFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTMagicDomainGreaterFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(MagicDomainGreaterFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTMagicDomainGreaterAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTMagicDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });

                var tricksterDomain = CreateTricksterDomain(MagicDomainProgression);
                tricksterDomain.LevelEntries = new LevelEntry[] {
                    Helpers.CreateLevelEntry(1, TricksterTTTMagicDomainBaseFeature),
                    Helpers.CreateLevelEntry(8, TricksterTTTMagicDomainGreaterFeature)
                };
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateNobilityDomain() {
                //Base Feature
                var NobilityDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("a1a7f3dd904ed8e45b074232f48190d1");
                var NobilityDomainBaseAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("7a305ef528cb7884385867a2db410102");
                var NobilityDomainBaseBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("07edff7cb9dec5e4cac69dae6bf159bd");

                var TricksterTTTNobilityDomainBaseResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTNobilityDomainBaseResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterMythics,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 3,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                    bp.m_Max = 13;
                    bp.m_UseMax = true;
                });
                var TricksterTTTNobilityDomainBaseBuff = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterTTTNobilityDomainBaseBuff", bp => {
                    bp.ApplyVisualsAndBasicSettings(NobilityDomainBaseBuff);
                    bp.AddComponent<AddStatBonus>(c => {
                        c.Stat = StatType.AdditionalAttackBonus;
                        c.Descriptor = ModifierDescriptor.Morale;
                        c.Value = 2;
                    });
                    bp.AddComponent<BuffAllSkillsBonus>(c => {
                        c.Descriptor = ModifierDescriptor.Morale;
                        c.Value = 2;
                    });
                    bp.AddComponent<BuffAllSavesBonus>(c => {
                        c.Descriptor = ModifierDescriptor.Morale;
                        c.Value = -2;
                    });
                });
                var TricksterTTTNobilityDomainBaseAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTNobilityDomainBaseAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(NobilityDomainBaseAbility);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.Div2;
                    });
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionApplyBuff() {
                                m_Buff = TricksterTTTNobilityDomainBaseBuff.ToReference<BlueprintBuffReference>(),
                                DurationValue = new ContextDurationValue() {
                                    Rate = DurationRate.Rounds,
                                    DiceCountValue = 0,
                                    BonusValue = new ContextValue() {
                                        ValueType = ContextValueType.Rank
                                    }
                                }
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTNobilityDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                });
                var TricksterTTTNobilityDomainBaseFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTNobilityDomainBaseFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(NobilityDomainBaseFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTNobilityDomainBaseAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTNobilityDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });
                //Greater Feature
                var NobilityDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("75acf3f9598248344b76f0b87ad27ac1");
                var NobilityDomainGreaterAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("2972215a5367ae44b8ddfe435a127a6e");
                var NobilityDomainGreaterBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("a78a13e8b6fbae1459faad20eb3ecc72");

                var TricksterTTTNobilityDomainGreaterResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTNobilityDomainGreaterResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterMythics,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 3,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                    bp.m_Max = 13;
                    bp.m_UseMax = true;
                });
                var TricksterTTTNobilityDomainGreaterBuff = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterTTTNobilityDomainGreaterBuff", bp => {
                    bp.ApplyVisualsAndBasicSettings(NobilityDomainGreaterBuff);
                    bp.AddComponent<AddStatBonus>(c => {
                        c.Stat = StatType.AdditionalAttackBonus;
                        c.Descriptor = ModifierDescriptor.Insight;
                        c.Value = 2;
                    });
                    bp.AddComponent<AddStatBonus>(c => {
                        c.Stat = StatType.AC;
                        c.Descriptor = ModifierDescriptor.Insight;
                        c.Value = 2;
                    });
                    bp.AddComponent<BuffAllSkillsBonus>(c => {
                        c.Descriptor = ModifierDescriptor.Insight;
                        c.Value = 2;
                    });
                    bp.AddComponent<ManeuverDefenceBonus>(c => {
                        c.Descriptor = ModifierDescriptor.Insight;
                        c.Bonus = -2;
                    });
                });
                var TricksterTTTNobilityDomainGreaterAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTNobilityDomainGreaterAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(NobilityDomainGreaterAbility);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.Div2;
                    });
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionApplyBuff() {
                                m_Buff = TricksterTTTNobilityDomainGreaterBuff.ToReference<BlueprintBuffReference>(),
                                DurationValue = new ContextDurationValue() {
                                    Rate = DurationRate.Rounds,
                                    DiceCountValue = 0,
                                    BonusValue = new ContextValue() {
                                        ValueType = ContextValueType.Rank
                                    }
                                }
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTNobilityDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                });
                var TricksterTTTNobilityDomainGreaterFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTNobilityDomainGreaterFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(NobilityDomainGreaterFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTNobilityDomainGreaterAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTNobilityDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });

                var tricksterDomain = CreateTricksterDomain(NobilityDomainProgression);
                tricksterDomain.LevelEntries = new LevelEntry[] {
                    Helpers.CreateLevelEntry(1, TricksterTTTNobilityDomainBaseFeature),
                    Helpers.CreateLevelEntry(8, TricksterTTTNobilityDomainGreaterFeature)
                };
                TricksterDomains.Add(tricksterDomain);
            }
            void CreatePlantDomain() {
                //Base Feature
                var PlantDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("e433267d36089d049b34900fde38032b");
                var PlantDomainBaseAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("82c76ed1de2e1114f8c08862cf2e6ee6");
                var PlantDomainBaseBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("77498fabb555d4c4ab6551047cdff178");
                var ReducePersonBuff = BlueprintTools.GetBlueprintReference<BlueprintBuffReference>("b0793973c61a19744a8630468e8f4174");

                var TricksterTTTPlantDomainBaseResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTPlantDomainBaseResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterMythics,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 3,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                    bp.m_Max = 13;
                    bp.m_UseMax = true;
                });
                var TricksterTTTPlantDomainBaseBuff = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterTTTPlantDomainBaseBuff", bp => {
                    bp.ApplyVisualsAndBasicSettings(PlantDomainBaseBuff);
                    bp.AddComponent<ChangeUnitSize>(c => {
                        c.SizeDelta = 1;
                    });
                    bp.AddComponent<AddGenericStatBonus>(c => {
                        c.Stat = StatType.Strength;
                        c.Descriptor = ModifierDescriptor.Size;
                        c.Value = 2;
                    });
                    bp.AddComponent<AddGenericStatBonus>(c => {
                        c.Stat = StatType.Dexterity;
                        c.Descriptor = ModifierDescriptor.Size;
                        c.Value = -2;
                    });
                });
                var TricksterTTTPlantDomainBaseAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTPlantDomainBaseAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(PlantDomainBaseAbility);
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new Conditional() {
                                ConditionsChecker = new ConditionsChecker() {
                                    Conditions = new Condition[] {
                                        new ContextConditionHasBuff(){
                                            m_Buff = ReducePersonBuff
                                        }
                                    }
                                },
                                IfTrue = Helpers.CreateActionList(
                                    new ContextActionRemoveBuff() {
                                        m_Buff = ReducePersonBuff
                                    }
                                ),
                                IfFalse = Helpers.CreateActionList(),
                            },
                            new ContextActionApplyBuff() {
                                m_Buff = TricksterTTTPlantDomainBaseBuff.ToReference<BlueprintBuffReference>(),
                                DurationValue = new ContextDurationValue() {
                                    Rate = DurationRate.Rounds,
                                    DiceCountValue = 0,
                                    BonusValue = 1
                                }
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTPlantDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                });
                var TricksterTTTPlantDomainBaseFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTPlantDomainBaseFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(PlantDomainBaseFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTPlantDomainBaseAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTPlantDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });
                //Greater Feature
                var PlantDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("24ec8901c8092264f864c7626ec3677e");
                var PlantDomainGreaterAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("04f780d86e7737445b052f0c0191a30b");
                var PlantDomainGreaterBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("58d86cc848805024abbbefd6abe2d433");

                var TricksterTTTPlantDomainGreaterResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTPlantDomainGreaterResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterClasses,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 0,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                });
                var TricksterTTTPlantDomainGreaterBuff = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterTTTPlantDomainGreaterBuff", bp => {
                    bp.ApplyVisualsAndBasicSettings(PlantDomainGreaterBuff);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Type = AbilityRankType.DamageBonus;
                        c.m_Progression = ContextRankProgression.Div2;
                    });
                    bp.AddComponent<AddTargetAttackRollTrigger>(c => {
                        c.OnlyHit = true;
                        c.OnlyMelee = true;
                        c.NotReach = true;
                        c.ActionOnSelf = Helpers.CreateActionList();
                        c.ActionsOnAttacker = Helpers.CreateActionList(
                            new ContextActionDealDamage() {
                                DamageType = new DamageTypeDescription() {
                                    Type = DamageType.Physical,
                                    Physical = new DamageTypeDescription.PhysicalData() {
                                        Form = PhysicalDamageForm.Piercing,
                                    }
                                },
                                Duration = new ContextDurationValue() {
                                    DiceCountValue = 0,
                                    BonusValue = 0
                                },
                                Value = new ContextDiceValue() {
                                    DiceType = DiceType.D6,
                                    DiceCountValue = 1,
                                    BonusValue = new ContextValue() {
                                        ValueType = ContextValueType.Rank,
                                        ValueRank = AbilityRankType.DamageBonus
                                    }
                                }
                            }
                        );
                    });
                });
                var TricksterTTTPlantDomainGreaterToggleAbility = Helpers.CreateBlueprint<BlueprintActivatableAbility>(TTTContext, "TricksterTTTPlantDomainGreaterToggleAbility", bp => {
                    bp.AddToDomainZealot();
                    bp.AddTricksterAbilityParams();
                    bp.m_Buff = TricksterTTTPlantDomainGreaterBuff.ToReference<BlueprintBuffReference>();
                    bp.AddComponent<ActivatableAbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTPlantDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.SpendType = ActivatableAbilityResourceLogic.ResourceSpendType.NewRound;
                    });
                    bp.m_DisplayName = PlantDomainGreaterAbility.m_DisplayName;
                    bp.SetDescription(TTTContext, "At 6th level, you can cause a host of wooden thorns to burst from your skin as a " +
                        "free action. While bramble armor is in effect, any foe striking you with a melee weapon without reach takes " +
                        "1d6 points of piercing damage + 1 point per two levels you possess in the class that gave you access to this domain.");
                    bp.m_DescriptionShort = PlantDomainGreaterAbility.m_DescriptionShort;
                    bp.m_Icon = PlantDomainGreaterAbility.m_Icon;
                    bp.Group = ActivatableAbilityGroup.None;
                    bp.WeightInGroup = 1;
                    bp.DeactivateIfCombatEnded = true;
                    bp.DeactivateIfOwnerDisabled = true;
                    bp.ActivationType = AbilityActivationType.WithUnitCommand;
                    bp.m_ActivateWithUnitCommand = Kingmaker.UnitLogic.Commands.Base.UnitCommand.CommandType.Free;
                });
                var TricksterTTTPlantDomainGreaterFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTPlantDomainGreaterFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(PlantDomainGreaterFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTPlantDomainGreaterToggleAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTPlantDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });

                var tricksterDomain = CreateTricksterDomain(PlantDomainProgression);
                tricksterDomain.LevelEntries = new LevelEntry[] {
                    Helpers.CreateLevelEntry(1, TricksterTTTPlantDomainBaseFeature),
                    Helpers.CreateLevelEntry(6, TricksterTTTPlantDomainGreaterFeature)
                };
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateProtectionDomain() {
                //Base Feature
                var ProtectionDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("a05a8959c594daa40a1c5add79566566");
                var ProtectionDomainBaseSelfBuffSupress = BlueprintTools.GetBlueprint<BlueprintBuff>("3dd1e499f7cd07c40873c9baa9b82e6e");
                var ProtectionDomainBaseAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("c5815bd0bf87bdb4fa9c440c8088149b");
                var ProtectionDomainBaseBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("2ddb4cfc3cfd04c46a66c6cd26df1c06");

                var TricksterTTTProtectionDomainBaseResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTProtectionDomainBaseResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterMythics,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 3,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                    bp.m_Max = 13;
                    bp.m_UseMax = true;
                });
                var TricksterTTTProtectionDomainBaseSelfBuffSupress = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterTTTProtectionDomainBaseSelfBuffSupress", bp => {
                    bp.ApplyVisualsAndBasicSettings(ProtectionDomainBaseSelfBuffSupress);
                });
                var TricksterTTTProtectionDomainBaseBuff = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterTTTProtectionDomainBaseBuff", bp => {
                    bp.ApplyVisualsAndBasicSettings(ProtectionDomainBaseBuff);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.OnePlusDivStep;
                        c.m_StepLevel = 5;
                        c.m_UseMin = true;
                        c.m_Min = 1;
                    });
                    bp.AddComponent<AddContextStatBonus>(c => {
                        c.Stat = StatType.SaveFortitude;
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Rank
                        };
                        c.Descriptor = ModifierDescriptor.Resistance;
                    });
                    bp.AddComponent<AddContextStatBonus>(c => {
                        c.Stat = StatType.SaveReflex;
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Rank
                        };
                        c.Descriptor = ModifierDescriptor.Resistance;
                    });
                    bp.AddComponent<AddContextStatBonus>(c => {
                        c.Stat = StatType.SaveWill;
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Rank
                        };
                        c.Descriptor = ModifierDescriptor.Resistance;
                    });
                });
                var TricksterTTTProtectionDomainBaseAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTProtectionDomainBaseAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(ProtectionDomainBaseAbility);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.OnePlusDivStep;
                        c.m_StepLevel = 5;
                        c.m_UseMin = true;
                        c.m_Min = 1;
                    });
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionApplyBuff() {
                                m_Buff = TricksterTTTProtectionDomainBaseBuff.ToReference<BlueprintBuffReference>(),
                                DurationValue = new ContextDurationValue() {
                                    Rate = DurationRate.Minutes,
                                    DiceCountValue = 0,
                                    BonusValue = 1
                                }
                            },
                            new ContextActionApplyBuff() {
                                m_Buff = TricksterTTTProtectionDomainBaseSelfBuffSupress.ToReference<BlueprintBuffReference>(),
                                DurationValue = new ContextDurationValue() {
                                    Rate = DurationRate.Minutes,
                                    DiceCountValue = 0,
                                    BonusValue = 1
                                },
                                ToCaster = true
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTProtectionDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                });
                var TricksterTTTProtectionDomainBaseFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTProtectionDomainBaseFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(ProtectionDomainBaseFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTProtectionDomainBaseAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTProtectionDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.OnePlusDivStep;
                        c.m_StepLevel = 5;
                        c.m_UseMin = true;
                        c.m_Min = 1;
                    });
                    bp.AddComponent<AddStatBonusIfHasFact>(c => {
                        c.m_CheckedFacts = new BlueprintUnitFactReference[] {
                            TricksterTTTProtectionDomainBaseSelfBuffSupress.ToReference<BlueprintUnitFactReference>()
                        };
                        c.Stat = StatType.SaveFortitude;
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Rank
                        };
                        c.Descriptor = ModifierDescriptor.Resistance;
                        c.InvertCondition = true;
                    });
                    bp.AddComponent<AddStatBonusIfHasFact>(c => {
                        c.m_CheckedFacts = new BlueprintUnitFactReference[] {
                            TricksterTTTProtectionDomainBaseSelfBuffSupress.ToReference<BlueprintUnitFactReference>()
                        };
                        c.Stat = StatType.SaveReflex;
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Rank
                        };
                        c.Descriptor = ModifierDescriptor.Resistance;
                        c.InvertCondition = true;
                    });
                    bp.AddComponent<AddStatBonusIfHasFact>(c => {
                        c.m_CheckedFacts = new BlueprintUnitFactReference[] {
                            TricksterTTTProtectionDomainBaseSelfBuffSupress.ToReference<BlueprintUnitFactReference>()
                        };
                        c.Stat = StatType.SaveWill;
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Rank
                        };
                        c.Descriptor = ModifierDescriptor.Resistance;
                        c.InvertCondition = true;
                    });
                });
                //Greater Feature
                var ProtectionDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("e2e9d41bfa7aa364592b9d57dd74c9db");
                var ProtectionDomainGreaterToggleAbility = BlueprintTools.GetBlueprint<BlueprintActivatableAbility>("0faf59bad9c5b0a4a812a9abf677a71b");
                var ProtectionDomainGreaterBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("20a4033ef90e66041b16817c7e03bf5c");
                var ProtectionDomainGreaterAura = BlueprintTools.GetBlueprint<BlueprintAbilityAreaEffect>("238adb5ade357384d97aa515ae5545e1");
                var ProtectionDomainGreaterEffect = BlueprintTools.GetBlueprint<BlueprintBuff>("fea7c44605c90f14fa40b2f2f5ae6339");

                var TricksterTTTProtectionDomainGreaterResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTProtectionDomainGreaterResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterClasses,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 0,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                });
                var TricksterTTTProtectionDomainGreaterEffect = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterTTTProtectionDomainGreaterEffect", bp => {
                    bp.ApplyVisualsAndBasicSettings(ProtectionDomainGreaterEffect);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Type = AbilityRankType.StatBonus;
                        c.m_Progression = ContextRankProgression.StartPlusDivStep;
                        c.m_StartLevel = 8;
                        c.m_StepLevel = 4;
                    });
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Type = AbilityRankType.DamageBonus;
                        c.m_Progression = ContextRankProgression.Custom;
                        c.m_CustomProgression = new ContextRankConfig.CustomProgressionItem[] {
                            new ContextRankConfig.CustomProgressionItem(){
                                BaseValue = 7
                            },
                            new ContextRankConfig.CustomProgressionItem(){
                                BaseValue = 13,
                                ProgressionValue = 5
                            },
                            new ContextRankConfig.CustomProgressionItem(){
                                BaseValue = 20,
                                ProgressionValue = 10
                            }
                        };
                    });
                    bp.AddComponent<AddContextStatBonus>(c => {
                        c.Stat = StatType.AC;
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Rank,
                            ValueRank = AbilityRankType.StatBonus
                        };
                        c.Descriptor = ModifierDescriptor.Deflection;
                    });
                    bp.AddComponent<ResistEnergyContext>(c => {
                        c.Type = DamageEnergyType.Fire;
                        c.ValueMultiplier = new ContextValue();
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Rank,
                            ValueRank = AbilityRankType.DamageBonus
                        };
                        c.Pool = new ContextValue();
                    });
                    bp.AddComponent<ResistEnergyContext>(c => {
                        c.Type = DamageEnergyType.Cold;
                        c.ValueMultiplier = new ContextValue();
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Rank,
                            ValueRank = AbilityRankType.DamageBonus
                        };
                        c.Pool = new ContextValue();
                    });
                    bp.AddComponent<ResistEnergyContext>(c => {
                        c.Type = DamageEnergyType.Sonic;
                        c.ValueMultiplier = new ContextValue();
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Rank,
                            ValueRank = AbilityRankType.DamageBonus
                        };
                        c.Pool = new ContextValue();
                    });
                    bp.AddComponent<ResistEnergyContext>(c => {
                        c.Type = DamageEnergyType.Electricity;
                        c.ValueMultiplier = new ContextValue();
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Rank,
                            ValueRank = AbilityRankType.DamageBonus
                        };
                        c.Pool = new ContextValue();
                    });
                    bp.AddComponent<ResistEnergyContext>(c => {
                        c.Type = DamageEnergyType.Acid;
                        c.ValueMultiplier = new ContextValue();
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Rank,
                            ValueRank = AbilityRankType.DamageBonus
                        };
                        c.Pool = new ContextValue();
                    });
                });
                var TricksterTTTProtectionDomainGreaterAura = Helpers.CreateBlueprint<BlueprintAbilityAreaEffect>(TTTContext, "TricksterTTTProtectionDomainGreaterAura", bp => {
                    bp.ApplyVisualsAndBasicSettings(ProtectionDomainGreaterAura);
                    bp.Fx = BlueAoE30Feet;
                    bp.AddComponent<AbilityAreaEffectBuff>(c => {
                        c.m_Buff = TricksterTTTProtectionDomainGreaterEffect.ToReference<BlueprintBuffReference>();
                        c.Condition = new ConditionsChecker() {
                            Conditions = new Condition[] {
                                new ContextConditionIsAlly()
                            }
                        };
                    });
                });
                var TricksterTTTProtectionDomainGreaterBuff = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterTTTProtectionDomainGreaterBuff", bp => {
                    bp.ApplyVisualsAndBasicSettings(ProtectionDomainGreaterBuff);
                    bp.AddComponent<AddAreaEffect>(c => {
                        c.m_AreaEffect = TricksterTTTProtectionDomainGreaterAura.ToReference<BlueprintAbilityAreaEffectReference>();
                    });
                });
                var TricksterTTTProtectionDomainGreaterToggleAbility = Helpers.CreateBlueprint<BlueprintActivatableAbility>(TTTContext, "TricksterTTTProtectionDomainGreaterToggleAbility", bp => {
                    bp.m_Buff = TricksterTTTProtectionDomainGreaterBuff.ToReference<BlueprintBuffReference>();
                    bp.ApplyVisualsAndBasicSettings(ProtectionDomainGreaterToggleAbility);
                    bp.AddComponent<ActivatableAbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTProtectionDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.SpendType = ActivatableAbilityResourceLogic.ResourceSpendType.NewRound;
                    });
                });
                var TricksterTTTProtectionDomainGreaterFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTProtectionDomainGreaterFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(ProtectionDomainGreaterFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTProtectionDomainGreaterToggleAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTProtectionDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });

                var tricksterDomain = CreateTricksterDomain(ProtectionDomainProgression);

                tricksterDomain.LevelEntries = new LevelEntry[] {
                    Helpers.CreateLevelEntry(1, TricksterTTTProtectionDomainBaseFeature),
                    Helpers.CreateLevelEntry(8, TricksterTTTProtectionDomainGreaterFeature)
                };
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateReposeDomain() {
                //Base Feature
                var ReposeDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("8526bc808c303034cb2b7832bccf1482");
                var ReposeDomainBaseAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("30dfb2e83f9de7246ad6cb44e36f2b4d");
                var UndeadType = BlueprintTools.GetBlueprintReference<BlueprintUnitFactReference>("734a29b693e9ec346ba2951b27987e33");
                var Staggered = BlueprintTools.GetBlueprintReference<BlueprintBuffReference>("df3950af5a783bd4d91ab73eb8fa0fd3");
                var CloakofDreamsEffectBuff = BlueprintTools.GetBlueprintReference<BlueprintBuffReference>("c9937d7846aa9ae46991e9f298be644a");

                var TricksterTTTReposeDomainBaseResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTReposeDomainBaseResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterMythics,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 3,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                    bp.m_Max = 13;
                    bp.m_UseMax = true;
                });
                var TricksterTTTReposeDomainBaseAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTReposeDomainBaseAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(ReposeDomainBaseAbility);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_CustomProperty = TricksterDomainStatProperty.ToReference<BlueprintUnitPropertyReference>();
                        c.m_Progression = ContextRankProgression.AsIs;
                        c.m_UseMin = true;
                        c.m_Min = 1;
                    });
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new Conditional() {
                                ConditionsChecker = new ConditionsChecker() {
                                    Conditions = new Condition[] {
                                        new ContextConditionHasFact(){
                                            m_Fact = UndeadType
                                        }
                                    }
                                },
                                IfTrue = Helpers.CreateActionList(
                                    new ContextActionApplyBuff() {
                                        m_Buff = Staggered,
                                        DurationValue = new ContextDurationValue() {
                                            Rate = DurationRate.Rounds,
                                            DiceCountValue = 0,
                                            BonusValue = new ContextValue() {
                                                ValueType = ContextValueType.Rank
                                            }
                                        }
                                    }
                                ),
                                IfFalse = Helpers.CreateActionList(
                                    new Conditional() {
                                        ConditionsChecker = new ConditionsChecker() {
                                            Conditions = new Condition[] {
                                                new ContextConditionHasBuff(){
                                                    m_Buff = Staggered
                                                }
                                            }
                                        },
                                        IfTrue = Helpers.CreateActionList(
                                            new ContextActionApplyBuff() {
                                                m_Buff = CloakofDreamsEffectBuff,
                                                DurationValue = new ContextDurationValue() {
                                                    Rate = DurationRate.Rounds,
                                                    DiceCountValue = 0,
                                                    BonusValue = 1
                                                }
                                            }
                                        ),
                                        IfFalse = Helpers.CreateActionList(
                                            new ContextActionApplyBuff() {
                                                m_Buff = Staggered,
                                                DurationValue = new ContextDurationValue() {
                                                    DiceCountValue = 0,
                                                    BonusValue = 1
                                                }
                                            }
                                        ),
                                    }
                                ),
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTReposeDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                });
                var TricksterTTTReposeDomainBaseFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTReposeDomainBaseFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(ReposeDomainBaseFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTReposeDomainBaseAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTReposeDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });
                //Greater Feature
                var ReposeDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("e06bfe3ad866c0e4f8a3d5516b844881");
                var ReposeDomainGreaterToggleAbility = BlueprintTools.GetBlueprint<BlueprintActivatableAbility>("d5a60f157610ab34cb72a98b4fc78953");
                var ReposeDomainGreaterBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("ed6064c44abb2474ebabb26c58db3e2a");
                var ReposeDomainGreaterAura = BlueprintTools.GetBlueprint<BlueprintAbilityAreaEffect>("7269a28475a91d84486749bf47443c72");
                var ReposeDomainGreaterEffect = BlueprintTools.GetBlueprint<BlueprintBuff>("6dff68e869eeef14a9964d94e03d1d59");
                var NegativeLevelsBuff = BlueprintTools.GetBlueprintReference<BlueprintBuffReference>("b02b6b9221241394db720ca004ea9194");

                var TricksterTTTReposeDomainGreaterResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTReposeDomainGreaterResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterClasses,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 0,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                });
                var TricksterTTTReposeDomainGreaterEffect = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterTTTReposeDomainGreaterEffect", bp => {
                    bp.ApplyVisualsAndBasicSettings(ReposeDomainGreaterEffect);
                    bp.AddComponent<BuffDescriptorImmunity>(c => {
                        c.Descriptor = SpellDescriptor.Death;
                    });
                    bp.AddComponent<SpellImmunityToSpellDescriptor>(c => {
                        c.Descriptor = SpellDescriptor.Death;
                    });
                    bp.AddComponent<AddImmunityToEnergyDrain>();
                    bp.AddComponent<SuppressBuffsTTT>(c => {
                        c.Continuous = true;
                        c.m_Buffs = new BlueprintBuffReference[] { NegativeLevelsBuff };
                    });
                });
                var TricksterTTTReposeDomainGreaterAura = Helpers.CreateBlueprint<BlueprintAbilityAreaEffect>(TTTContext, "TricksterTTTReposeDomainGreaterAura", bp => {
                    bp.ApplyVisualsAndBasicSettings(ReposeDomainGreaterAura);
                    bp.AddComponent<AbilityAreaEffectBuff>(c => {
                        c.m_Buff = TricksterTTTReposeDomainGreaterEffect.ToReference<BlueprintBuffReference>();
                        c.Condition = new ConditionsChecker() {
                            Conditions = new Condition[] {
                                new ContextConditionIsAlly(),
                                new ContextConditionHasFact(){
                                    m_Fact = UndeadType,
                                    Not = true
                                }
                            }
                        };
                    });
                });
                var TricksterTTTReposeDomainGreaterBuff = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterTTTReposeDomainGreaterBuff", bp => {
                    bp.ApplyVisualsAndBasicSettings(ReposeDomainGreaterBuff);
                    bp.AddComponent<AddAreaEffect>(c => {
                        c.m_AreaEffect = TricksterTTTReposeDomainGreaterAura.ToReference<BlueprintAbilityAreaEffectReference>();
                    });
                });
                var TricksterTTTReposeDomainGreaterToggleAbility = Helpers.CreateBlueprint<BlueprintActivatableAbility>(TTTContext, "TricksterTTTReposeDomainGreaterToggleAbility", bp => {
                    bp.AddToDomainZealot();
                    bp.m_Buff = TricksterTTTReposeDomainGreaterBuff.ToReference<BlueprintBuffReference>();
                    bp.ApplyVisualsAndBasicSettings(ReposeDomainGreaterToggleAbility);
                    bp.AddComponent<ActivatableAbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTReposeDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.SpendType = ActivatableAbilityResourceLogic.ResourceSpendType.NewRound;
                    });
                });
                var TricksterTTTReposeDomainGreaterFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTReposeDomainGreaterFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(ReposeDomainGreaterFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTReposeDomainGreaterToggleAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTReposeDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });

                var tricksterDomain = CreateTricksterDomain(ReposeDomainProgression);

                tricksterDomain.LevelEntries = new LevelEntry[] {
                    Helpers.CreateLevelEntry(1, TricksterTTTReposeDomainBaseFeature),
                    Helpers.CreateLevelEntry(8, TricksterTTTReposeDomainGreaterFeature)
                };
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateRuneDomain() {
                //Base Feature
                var RuneDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("b74c64a0152c7ee46b13ecdd72dda6f3");
                var RuneDomainBaseAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("56ad05dedfd9df84996f62108125eed5");
                var RuneDomainBaseAbilityAcidArea = BlueprintTools.GetBlueprint<BlueprintAbilityAreaEffect>("98c3a36f2a3636c49a3f77c001a25f29");
                var RuneDomainBaseAbilityColdArea = BlueprintTools.GetBlueprint<BlueprintAbilityAreaEffect>("8b8e98e8e0000f643ad97c744f3f850b");
                var RuneDomainBaseAbilityElectricityArea = BlueprintTools.GetBlueprint<BlueprintAbilityAreaEffect>("db868c576c69d0e4a8462645267c6cdc");
                var RuneDomainBaseAbilityFireArea = BlueprintTools.GetBlueprint<BlueprintAbilityAreaEffect>("9b786945d2ec1884184235a488e5cb9e");
                var RuneDomainBaseAbilityAcid = BlueprintTools.GetBlueprint<BlueprintAbility>("92c821ecc8d73564bad15a8a07ed40f2");
                var RuneDomainBaseAbilityCold = BlueprintTools.GetBlueprint<BlueprintAbility>("2b81ff42fcbe9434eaf00fb0a873f579");
                var RuneDomainBaseAbilityElectricity = BlueprintTools.GetBlueprint<BlueprintAbility>("b67978e3d5a6c9247a393237bc660339");
                var RuneDomainBaseAbilityFire = BlueprintTools.GetBlueprint<BlueprintAbility>("eddfe26a8a3892b47add3cb08db7069d");

                var TricksterTTTRuneDomainBaseResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTRuneDomainBaseResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterMythics,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 3,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                    bp.m_Max = 13;
                    bp.m_UseMax = true;
                });
                var TricksterTTTRuneDomainBaseAbilityAcidArea = Helpers.CreateBlueprint<BlueprintAbilityAreaEffect>(TTTContext, "TricksterTTTRuneDomainBaseAbilityAcidArea", bp => {
                    bp.ApplyVisualsAndBasicSettings(RuneDomainBaseAbilityAcidArea);
                    bp.AddComponent<AbilityAreaEffectRunAction>(c => {
                        c.UnitEnter = Helpers.CreateActionList(
                            new Conditional() {
                                ConditionsChecker = new ConditionsChecker() {
                                    Conditions = new Condition[] {
                                        new ContextConditionIsEnemy()
                                    }
                                },
                                IfFalse = Helpers.CreateActionList(),
                                IfTrue = Helpers.CreateActionList(
                                    new ContextActionDealDamage() {
                                        DamageType = new DamageTypeDescription() {
                                            Type = DamageType.Energy,
                                            Energy = DamageEnergyType.Acid
                                        },
                                        Duration = new ContextDurationValue() {
                                            DiceCountValue = 0,
                                            BonusValue = 0
                                        },
                                        Value = new ContextDiceValue() {
                                            DiceType = DiceType.D6,
                                            DiceCountValue = 1,
                                            BonusValue = new ContextValue() {
                                                ValueType = ContextValueType.Rank,
                                                ValueRank = AbilityRankType.DamageBonus
                                            }
                                        }
                                    },
                                    new ContextActionRemoveSelf()
                                )
                            }
                        );
                        c.UnitExit = Helpers.CreateActionList();
                        c.UnitMove = Helpers.CreateActionList();
                        c.Round = Helpers.CreateActionList();
                    });
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Type = AbilityRankType.DamageBonus;
                        c.m_Progression = ContextRankProgression.Div2;
                        c.m_UseMin = true;
                        c.m_Min = 1;
                    });
                });
                var TricksterTTTRuneDomainBaseAbilityAcid = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTRuneDomainBaseAbilityAcid", bp => {
                    bp.ApplyVisualsAndBasicSettings(RuneDomainBaseAbilityAcid);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Type = AbilityRankType.DamageBonus;
                        c.m_Progression = ContextRankProgression.Div2;
                        c.m_UseMin = true;
                        c.m_Min = 1;
                    });
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.AsIs;
                        c.m_UseMin = true;
                        c.m_Min = 1;
                    });
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionSpawnAreaEffect() {
                                m_AreaEffect = TricksterTTTRuneDomainBaseAbilityAcidArea.ToReference<BlueprintAbilityAreaEffectReference>(),
                                DurationValue = new ContextDurationValue() {
                                    DiceCountValue = 0,
                                    BonusValue = new ContextValue() {
                                        ValueType = ContextValueType.Rank
                                    }
                                },
                                OnUnit = true
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTRuneDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                });
                var TricksterTTTRuneDomainBaseAbilityColdArea = Helpers.CreateBlueprint<BlueprintAbilityAreaEffect>(TTTContext, "TricksterTTTRuneDomainBaseAbilityColdArea", bp => {
                    bp.ApplyVisualsAndBasicSettings(RuneDomainBaseAbilityColdArea);
                    bp.AddComponent<AbilityAreaEffectRunAction>(c => {
                        c.UnitEnter = Helpers.CreateActionList(
                            new Conditional() {
                                ConditionsChecker = new ConditionsChecker() {
                                    Conditions = new Condition[] {
                                        new ContextConditionIsEnemy()
                                    }
                                },
                                IfFalse = Helpers.CreateActionList(),
                                IfTrue = Helpers.CreateActionList(
                                    new ContextActionDealDamage() {
                                        DamageType = new DamageTypeDescription() {
                                            Type = DamageType.Energy,
                                            Energy = DamageEnergyType.Cold
                                        },
                                        Duration = new ContextDurationValue() {
                                            DiceCountValue = 0,
                                            BonusValue = 0
                                        },
                                        Value = new ContextDiceValue() {
                                            DiceType = DiceType.D6,
                                            DiceCountValue = 1,
                                            BonusValue = new ContextValue() {
                                                ValueType = ContextValueType.Rank,
                                                ValueRank = AbilityRankType.DamageBonus
                                            }
                                        }
                                    },
                                    new ContextActionRemoveSelf()
                                )
                            }
                        );
                        c.UnitExit = Helpers.CreateActionList();
                        c.UnitMove = Helpers.CreateActionList();
                        c.Round = Helpers.CreateActionList();
                    });
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Type = AbilityRankType.DamageBonus;
                        c.m_Progression = ContextRankProgression.Div2;
                        c.m_UseMin = true;
                        c.m_Min = 1;
                    });
                });
                var TricksterTTTRuneDomainBaseAbilityCold = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTRuneDomainBaseAbilityCold", bp => {
                    bp.ApplyVisualsAndBasicSettings(RuneDomainBaseAbilityCold);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Type = AbilityRankType.DamageBonus;
                        c.m_Progression = ContextRankProgression.Div2;
                        c.m_UseMin = true;
                        c.m_Min = 1;
                    });
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.AsIs;
                        c.m_UseMin = true;
                        c.m_Min = 1;
                    });
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionSpawnAreaEffect() {
                                m_AreaEffect = TricksterTTTRuneDomainBaseAbilityColdArea.ToReference<BlueprintAbilityAreaEffectReference>(),
                                DurationValue = new ContextDurationValue() {
                                    DiceCountValue = 0,
                                    BonusValue = new ContextValue() {
                                        ValueType = ContextValueType.Rank
                                    }
                                },
                                OnUnit = true
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTRuneDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                });
                var TricksterTTTRuneDomainBaseAbilityElectricityArea = Helpers.CreateBlueprint<BlueprintAbilityAreaEffect>(TTTContext, "TricksterTTTRuneDomainBaseAbilityElectricityArea", bp => {
                    bp.ApplyVisualsAndBasicSettings(RuneDomainBaseAbilityElectricityArea);
                    bp.AddComponent<AbilityAreaEffectRunAction>(c => {
                        c.UnitEnter = Helpers.CreateActionList(
                            new Conditional() {
                                ConditionsChecker = new ConditionsChecker() {
                                    Conditions = new Condition[] {
                                        new ContextConditionIsEnemy()
                                    }
                                },
                                IfFalse = Helpers.CreateActionList(),
                                IfTrue = Helpers.CreateActionList(
                                    new ContextActionDealDamage() {
                                        DamageType = new DamageTypeDescription() {
                                            Type = DamageType.Energy,
                                            Energy = DamageEnergyType.Electricity
                                        },
                                        Duration = new ContextDurationValue() {
                                            DiceCountValue = 0,
                                            BonusValue = 0
                                        },
                                        Value = new ContextDiceValue() {
                                            DiceType = DiceType.D6,
                                            DiceCountValue = 1,
                                            BonusValue = new ContextValue() {
                                                ValueType = ContextValueType.Rank,
                                                ValueRank = AbilityRankType.DamageBonus
                                            }
                                        }
                                    },
                                    new ContextActionRemoveSelf()
                                )
                            }
                        );
                        c.UnitExit = Helpers.CreateActionList();
                        c.UnitMove = Helpers.CreateActionList();
                        c.Round = Helpers.CreateActionList();
                    });
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Type = AbilityRankType.DamageBonus;
                        c.m_Progression = ContextRankProgression.Div2;
                        c.m_UseMin = true;
                        c.m_Min = 1;
                    });
                });
                var TricksterTTTRuneDomainBaseAbilityElectricity = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTRuneDomainBaseAbilityElectricity", bp => {
                    bp.ApplyVisualsAndBasicSettings(RuneDomainBaseAbilityElectricity);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Type = AbilityRankType.DamageBonus;
                        c.m_Progression = ContextRankProgression.Div2;
                        c.m_UseMin = true;
                        c.m_Min = 1;
                    });
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.AsIs;
                        c.m_UseMin = true;
                        c.m_Min = 1;
                    });
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionSpawnAreaEffect() {
                                m_AreaEffect = TricksterTTTRuneDomainBaseAbilityElectricityArea.ToReference<BlueprintAbilityAreaEffectReference>(),
                                DurationValue = new ContextDurationValue() {
                                    DiceCountValue = 0,
                                    BonusValue = new ContextValue() {
                                        ValueType = ContextValueType.Rank
                                    }
                                },
                                OnUnit = true
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTRuneDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                });
                var TricksterTTTRuneDomainBaseAbilityFireArea = Helpers.CreateBlueprint<BlueprintAbilityAreaEffect>(TTTContext, "TricksterTTTRuneDomainBaseAbilityFireArea", bp => {
                    bp.ApplyVisualsAndBasicSettings(RuneDomainBaseAbilityFireArea);
                    bp.AddComponent<AbilityAreaEffectRunAction>(c => {
                        c.UnitEnter = Helpers.CreateActionList(
                            new Conditional() {
                                ConditionsChecker = new ConditionsChecker() {
                                    Conditions = new Condition[] {
                                        new ContextConditionIsEnemy()
                                    }
                                },
                                IfFalse = Helpers.CreateActionList(),
                                IfTrue = Helpers.CreateActionList(
                                    new ContextActionDealDamage() {
                                        DamageType = new DamageTypeDescription() {
                                            Type = DamageType.Energy,
                                            Energy = DamageEnergyType.Fire
                                        },
                                        Duration = new ContextDurationValue() {
                                            DiceCountValue = 0,
                                            BonusValue = 0
                                        },
                                        Value = new ContextDiceValue() {
                                            DiceType = DiceType.D6,
                                            DiceCountValue = 1,
                                            BonusValue = new ContextValue() {
                                                ValueType = ContextValueType.Rank,
                                                ValueRank = AbilityRankType.DamageBonus
                                            }
                                        }
                                    },
                                    new ContextActionRemoveSelf()
                                )
                            }
                        );
                        c.UnitExit = Helpers.CreateActionList();
                        c.UnitMove = Helpers.CreateActionList();
                        c.Round = Helpers.CreateActionList();
                    });
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Type = AbilityRankType.DamageBonus;
                        c.m_Progression = ContextRankProgression.Div2;
                        c.m_UseMin = true;
                        c.m_Min = 1;
                    });
                });
                var TricksterTTTRuneDomainBaseAbilityFire = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTRuneDomainBaseAbilityFire", bp => {
                    bp.ApplyVisualsAndBasicSettings(RuneDomainBaseAbilityFire);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Type = AbilityRankType.DamageBonus;
                        c.m_Progression = ContextRankProgression.Div2;
                        c.m_UseMin = true;
                        c.m_Min = 1;
                    });
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.AsIs;
                        c.m_UseMin = true;
                        c.m_Min = 1;
                    });
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionSpawnAreaEffect() {
                                m_AreaEffect = TricksterTTTRuneDomainBaseAbilityFireArea.ToReference<BlueprintAbilityAreaEffectReference>(),
                                DurationValue = new ContextDurationValue() {
                                    DiceCountValue = 0,
                                    BonusValue = new ContextValue() {
                                        ValueType = ContextValueType.Rank
                                    }
                                },
                                OnUnit = true
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTRuneDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                });
                var TricksterTTTRuneDomainBaseAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTRuneDomainBaseAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(RuneDomainBaseAbility);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_CustomProperty = TricksterDomainStatProperty.ToReference<BlueprintUnitPropertyReference>();
                        c.m_Progression = ContextRankProgression.AsIs;
                        c.m_UseMin = true;
                        c.m_Min = 1;
                    });
                    bp.AddComponent<AbilityVariants>(c => {
                        c.m_Variants = new BlueprintAbilityReference[] {
                            TricksterTTTRuneDomainBaseAbilityAcid.ToReference<BlueprintAbilityReference>(),
                            TricksterTTTRuneDomainBaseAbilityCold.ToReference<BlueprintAbilityReference>(),
                            TricksterTTTRuneDomainBaseAbilityElectricity.ToReference<BlueprintAbilityReference>(),
                            TricksterTTTRuneDomainBaseAbilityFire.ToReference<BlueprintAbilityReference>()
                        };
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTRuneDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                });
                var TricksterTTTRuneDomainBaseFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTRuneDomainBaseFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(RuneDomainBaseFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTRuneDomainBaseAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTRuneDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });
                //Greater Feature
                var RuneDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("8a2064b6e41c90e4c8a2880deccac139");
                var RuneDomainGreaterAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("9171a3ce8ea8cac44894b240709804ce");
                var RuneDomainGreaterArea = BlueprintTools.GetBlueprint<BlueprintAbilityAreaEffect>("e26de8b0164db23458eb64c21fac2846");
                var RuneDomainGreaterBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("6f3e707ae4874f2409d288cfedbd848e");
                var NegativeLevelsBuff = BlueprintTools.GetBlueprintReference<BlueprintBuffReference>("b02b6b9221241394db720ca004ea9194");

                var TricksterTTTRuneDomainGreaterResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTRuneDomainGreaterResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = new BlueprintCharacterClassReference[0],
                        m_ClassDiv = ResourcesLibrary.GetRoot().Progression.m_CharacterClasses,
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 0,
                        StartingLevel = 8,
                        StartingIncrease = 1,
                        LevelStep = 4,
                        PerStepIncrease = 1,
                        IncreasedByLevelStartPlusDivStep = true,
                        IncreasedByStat = false
                    };
                    bp.m_Max = 20;
                    bp.m_UseMax = true;
                });
                var TricksterTTTRuneDomainGreaterBuff = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterTTTRuneDomainGreaterBuff", bp => {
                    bp.ApplyVisualsAndBasicSettings(RuneDomainGreaterBuff);
                    bp.AddComponent<BuffStatusCondition>(c => {
                        c.Condition = Kingmaker.UnitLogic.UnitCondition.CanNotAttack;
                    });
                    bp.AddComponent<SpellDescriptorComponent>(c => {
                        c.Descriptor = SpellDescriptor.Stun;
                    });
                });
                var TricksterTTTRuneDomainGreaterArea = Helpers.CreateBlueprint<BlueprintAbilityAreaEffect>(TTTContext, "TricksterTTTRuneDomainGreaterArea", bp => {
                    bp.ApplyVisualsAndBasicSettings(RuneDomainGreaterArea);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Type = AbilityRankType.StatBonus;
                        c.m_Progression = ContextRankProgression.Div2;
                    });
                    bp.AddComponent<AbilityAreaEffectRunAction>(c => {
                        c.UnitEnter = Helpers.CreateActionList(
                            new Conditional() {
                                ConditionsChecker = new ConditionsChecker() {
                                    Conditions = new Condition[] {
                                        new ContextConditionIsEnemy()
                                    }
                                },
                                IfFalse = Helpers.CreateActionList(),
                                IfTrue = Helpers.CreateActionList(
                                    new ContextActionSavingThrow() {
                                        Type = SavingThrowType.Will,
                                        CustomDC = new ContextValue(),
                                        Actions = Helpers.CreateActionList(
                                            new ContextActionConditionalSaved() {
                                                Succeed = Helpers.CreateActionList(),
                                                Failed = Helpers.CreateActionList(
                                                    new ContextActionApplyBuff() {
                                                        m_Buff = TricksterTTTRuneDomainGreaterBuff.ToReference<BlueprintBuffReference>(),
                                                        DurationValue = new ContextDurationValue() {
                                                            DiceCountValue = 0,
                                                            BonusValue = new ContextValue() {
                                                                ValueType = ContextValueType.Rank,
                                                                ValueRank = AbilityRankType.StatBonus
                                                            }
                                                        }
                                                    }
                                                )
                                            }
                                        )
                                    },
                                    new ContextActionRemoveSelf(),
                                    new ContextActionSpawnFx() {
                                        PrefabLink = new PrefabLink() {
                                            AssetId = "c14a2f46018cb0e41bfeed61463510ff"
                                        }
                                    }
                                )
                            }
                        );
                        c.UnitExit = Helpers.CreateActionList();
                        c.UnitMove = Helpers.CreateActionList();
                        c.Round = Helpers.CreateActionList();
                    });
                });
                var TricksterTTTRuneDomainGreaterAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTRuneDomainGreaterAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(RuneDomainGreaterAbility);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Type = AbilityRankType.StatBonus;
                        c.m_Progression = ContextRankProgression.Div2;
                        c.m_UseMin = true;
                        c.m_Min = 1;
                    });
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.AsIs;
                        c.m_UseMin = true;
                        c.m_Min = 1;
                    });
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionSpawnAreaEffect() {
                                m_AreaEffect = TricksterTTTRuneDomainGreaterArea.ToReference<BlueprintAbilityAreaEffectReference>(),
                                DurationValue = new ContextDurationValue() {
                                    DiceCountValue = 0,
                                    BonusValue = new ContextValue() {
                                        ValueType = ContextValueType.Rank
                                    }
                                },
                                OnUnit = true
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTRuneDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                });
                var TricksterTTTRuneDomainGreaterFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTRuneDomainGreaterFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(RuneDomainGreaterFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTRuneDomainGreaterAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTRuneDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });
                var tricksterDomain = CreateTricksterDomain(RuneDomainProgression);

                tricksterDomain.LevelEntries = new LevelEntry[] {
                    Helpers.CreateLevelEntry(1, TricksterTTTRuneDomainBaseFeature),
                    Helpers.CreateLevelEntry(8, TricksterTTTRuneDomainGreaterFeature)
                };
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateStrengthDomain() {
                //Base Feature
                var StrengthDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("526f99784e9fe4346824e7f210d46112");
                var StrengthDomainBaseAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("1d6364123e1f6a04c88313d83d3b70ee");
                var StrengthDomainBaseBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("94dfcf5f3a72ce8478c8de5db69e752b");

                var TricksterTTTStrengthDomainBaseResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTStrengthDomainBaseResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterMythics,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 3,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                    bp.m_Max = 13;
                    bp.m_UseMax = true;
                });
                var TricksterTTTStrengthDomainBaseBuff = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterTTTStrengthDomainBaseBuff", bp => {
                    bp.ApplyVisualsAndBasicSettings(StrengthDomainBaseBuff);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.Div2;
                        c.m_UseMin = true;
                        c.m_Min = 1;
                    });
                    bp.AddComponent<AbilityScoreCheckBonus>(c => {
                        c.Stat = StatType.SkillAthletics;
                        c.Descriptor = ModifierDescriptor.Enhancement;
                        c.Bonus = new ContextValue() {
                            ValueType = ContextValueType.Rank
                        };
                    });
                    bp.AddComponent<AttackTypeAttackBonus>(c => {
                        c.AttackBonus = 1;
                        c.Type = WeaponRangeType.Melee;
                        c.Descriptor = ModifierDescriptor.Enhancement;
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Rank
                        };
                    });
                });
                var TricksterTTTStrengthDomainBaseAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTStrengthDomainBaseAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(StrengthDomainBaseAbility);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_CustomProperty = TricksterDomainStatProperty.ToReference<BlueprintUnitPropertyReference>();
                        c.m_Progression = ContextRankProgression.AsIs;
                        c.m_UseMin = true;
                        c.m_Min = 1;
                    });
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionApplyBuff() {
                                m_Buff = TricksterTTTStrengthDomainBaseBuff.ToReference<BlueprintBuffReference>(),
                                DurationValue = new ContextDurationValue() {
                                    DiceCountValue = 0,
                                    BonusValue = 1
                                }
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTStrengthDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                });
                var TricksterTTTStrengthDomainBaseFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTStrengthDomainBaseFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(StrengthDomainBaseFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTStrengthDomainBaseAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTStrengthDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });
                //Greater Feature
                var StrengthDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("3298fd30e221ef74189a06acbf376d29");

                var TricksterTTTStrengthDomainGreaterFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTStrengthDomainGreaterFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(StrengthDomainGreaterFeature);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.Div2;
                    });
                    bp.AddComponent<AddContextStatBonus>(c => {
                        c.Stat = StatType.SkillAthletics;
                        c.Descriptor = ModifierDescriptor.Enhancement;
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Rank
                        };
                        c.Multiplier = 1;
                    });
                });

                var tricksterDomain = CreateTricksterDomain(StrengthDomainProgression);
                tricksterDomain.LevelEntries = new LevelEntry[] {
                    Helpers.CreateLevelEntry(1, TricksterTTTStrengthDomainBaseFeature),
                    Helpers.CreateLevelEntry(8, TricksterTTTStrengthDomainGreaterFeature)
                };
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateSunDomain() {
                //Base Feature
                var SunDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("3d8e38c9ed54931469281ab0cec506e9");

                var TricksterTTTSunDomainBaseFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTSunDomainBaseFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(SunDomainBaseFeature);
                    SunDomainBaseFeature.ComponentsArray.OfType<IncreaseSpellDamageByClassLevel>().ForEach(component => {
                        bp.AddComponent(Helpers.CreateCopy(component));
                        bp.GetComponent<IncreaseSpellDamageByClassLevel>().TemporaryContext(c => {
                            c.m_AdditionalClasses = ResourcesLibrary.GetRoot().Progression.m_CharacterClasses
                                .Where(reference => reference.deserializedGuid.m_Guid != Guid.Parse("67819271767a9dd4fbfd4ae700befea0"))
                                .ToArray();
                        });
                    });
                });
                //Greater Feature
                var SunDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("3e301c9d0e735b649955139ee0f5f165");
                var SunDomainGreaterToggleAbility = BlueprintTools.GetBlueprint<BlueprintActivatableAbility>("cb5652d2e74cac14498c2793b1bca857");
                var SunDomainGreaterBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("1389631f36e53704aba0e1c66ebbd393");
                var SunDomainGreaterAura = BlueprintTools.GetBlueprint<BlueprintAbilityAreaEffect>("cfe8c5683c759f047a56a4b5e77ac93f");
                var SunDomainGreaterImmunityBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("b77ed095b1821424594e3b102e5c9d35");
                var BlindnessBuff = BlueprintTools.GetBlueprintReference<BlueprintBuffReference>("187f88d96a0ef464280706b63635f2af");
                var UndeadType = BlueprintTools.GetBlueprintReference<BlueprintUnitFactReference>("734a29b693e9ec346ba2951b27987e33");

                var TricksterTTTSunDomainGreaterResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTSunDomainGreaterResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterClasses,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 0,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                });
                var TricksterTTTSunDomainGreaterEffect = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterTTTSunDomainGreaterEffect", bp => {
                    bp.ApplyVisualsAndBasicSettings(SunDomainGreaterImmunityBuff);
                });
                var TricksterTTTSunDomainGreaterAura = Helpers.CreateBlueprint<BlueprintAbilityAreaEffect>(TTTContext, "TricksterTTTSunDomainGreaterAura", bp => {
                    bp.ApplyVisualsAndBasicSettings(SunDomainGreaterAura);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Type = AbilityRankType.DamageBonus;
                        c.m_Progression = ContextRankProgression.AsIs;
                    });
                    bp.AddComponent<AbilityAreaEffectRunAction>(c => {
                        c.UnitEnter = Helpers.CreateActionList(
                            new Conditional() {
                                ConditionsChecker = new ConditionsChecker() {
                                    Conditions = new Condition[] {
                                        new ContextConditionIsEnemy(),
                                        new ContextConditionHasBuff(){
                                            m_Buff = SunDomainGreaterImmunityBuff.ToReference<BlueprintBuffReference>(),
                                            Not = true
                                        }
                                    }
                                },
                                IfFalse = Helpers.CreateActionList(),
                                IfTrue = Helpers.CreateActionList(
                                    new ContextActionSavingThrow() {
                                        Type = SavingThrowType.Fortitude,
                                        CustomDC = new ContextValue(),
                                        Actions = Helpers.CreateActionList(
                                            new ContextActionConditionalSaved() {
                                                Succeed = Helpers.CreateActionList(
                                                    new ContextActionApplyBuff() {
                                                        m_Buff = SunDomainGreaterImmunityBuff.ToReference<BlueprintBuffReference>(),
                                                        DurationValue = new ContextDurationValue() {
                                                            Rate = DurationRate.Days,
                                                            DiceCountValue = 0,
                                                            BonusValue = 1
                                                        }
                                                    }
                                                ),
                                                Failed = Helpers.CreateActionList(
                                                    new ContextActionApplyBuff() {
                                                        m_Buff = BlindnessBuff,
                                                        Permanent = true,
                                                        DurationValue = new ContextDurationValue() {
                                                            DiceCountValue = 0,
                                                            BonusValue = 1
                                                        }
                                                    },
                                                    new ContextActionApplyBuff() {
                                                        m_Buff = SunDomainGreaterImmunityBuff.ToReference<BlueprintBuffReference>(),
                                                        DurationValue = new ContextDurationValue() {
                                                            Rate = DurationRate.Days,
                                                            DiceCountValue = 0,
                                                            BonusValue = 1
                                                        }
                                                    }
                                                )
                                            }
                                        )
                                    }
                                )
                            }
                        );
                        c.UnitExit = Helpers.CreateActionList(
                            new Conditional() {
                                ConditionsChecker = new ConditionsChecker() {
                                    Conditions = new Condition[] {
                                        new ContextConditionIsEnemy(),
                                        new ContextConditionHasBuff(){
                                            m_Buff = BlindnessBuff,
                                        },
                                        new ContextConditionHasBuff(){
                                            m_Buff = SunDomainGreaterImmunityBuff.ToReference<BlueprintBuffReference>(),
                                        }
                                    }
                                },
                                IfFalse = Helpers.CreateActionList(),
                                IfTrue = Helpers.CreateActionList(
                                    new ContextActionRemoveBuff() {
                                        m_Buff = BlindnessBuff
                                    }
                                )
                            }
                        );
                        c.UnitMove = Helpers.CreateActionList();
                        c.Round = Helpers.CreateActionList(
                            new Conditional() {
                                ConditionsChecker = new ConditionsChecker() {
                                    Conditions = new Condition[] {
                                        new ContextConditionIsEnemy(),
                                        new ContextConditionHasFact(){
                                            m_Fact = UndeadType
                                        }
                                    }
                                },
                                IfFalse = Helpers.CreateActionList(),
                                IfTrue = Helpers.CreateActionList(
                                    new ContextActionDealDamage() {
                                        DamageType = new DamageTypeDescription() {
                                            Type = DamageType.Energy,
                                            Energy = DamageEnergyType.Divine
                                        },
                                        Duration = new ContextDurationValue() {
                                            DiceCountValue = 0,
                                            BonusValue = 0
                                        },
                                        Value = new ContextDiceValue() {
                                            DiceCountValue = 0,
                                            BonusValue = new ContextValue() {
                                                ValueType = ContextValueType.Rank,
                                                ValueRank = AbilityRankType.DamageBonus
                                            }
                                        }
                                    }
                                )
                            }
                        );
                    });
                });
                var TricksterTTTSunDomainGreaterBuff = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterTTTSunDomainGreaterBuff", bp => {
                    bp.ApplyVisualsAndBasicSettings(SunDomainGreaterBuff);
                    bp.AddComponent<AddAreaEffect>(c => {
                        c.m_AreaEffect = TricksterTTTSunDomainGreaterAura.ToReference<BlueprintAbilityAreaEffectReference>();
                    });
                });
                var TricksterTTTSunDomainGreaterToggleAbility = Helpers.CreateBlueprint<BlueprintActivatableAbility>(TTTContext, "TricksterTTTSunDomainGreaterToggleAbility", bp => {
                    bp.m_Buff = TricksterTTTSunDomainGreaterBuff.ToReference<BlueprintBuffReference>();
                    bp.ApplyVisualsAndBasicSettings(SunDomainGreaterToggleAbility);
                    bp.AddComponent<ActivatableAbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTSunDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.SpendType = ActivatableAbilityResourceLogic.ResourceSpendType.NewRound;
                    });
                });
                var TricksterTTTSunDomainGreaterFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTSunDomainGreaterFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(SunDomainGreaterFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTSunDomainGreaterToggleAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTSunDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });

                var tricksterDomain = CreateTricksterDomain(SunDomainProgression);
                tricksterDomain.LevelEntries = new LevelEntry[] {
                    Helpers.CreateLevelEntry(1, TricksterTTTSunDomainBaseFeature),
                    Helpers.CreateLevelEntry(8, TricksterTTTSunDomainGreaterFeature)
                };
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateTravelDomain() {
                //Base Feature
                var TravelDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("3079cdfba971d614ab4f49220c6cd228");
                var TravelDomainBaseAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("ce5ec0a87ad5c4746bdd4e9a1552b397");
                var TravelDomainBaseBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("95c9a43e41dd8924e96a22cd4836a3dc");

                var TricksterTTTTravelDomainBaseResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTTravelDomainBaseResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterMythics,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 3,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                    bp.m_Max = 13;
                    bp.m_UseMax = true;
                });
                var TricksterTTTTravelDomainBaseBuff = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterTTTTravelDomainBaseBuff", bp => {
                    bp.ApplyVisualsAndBasicSettings(TravelDomainBaseBuff);
                    bp.AddComponent<AddConditionImmunity>(c => {
                        c.Condition = UnitCondition.DifficultTerrain;
                    });
                });
                var TricksterTTTTravelDomainBaseAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTTravelDomainBaseAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(TravelDomainBaseAbility);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.OnePlusDivStep;
                        c.m_StepLevel = 5;
                        c.m_UseMin = true;
                        c.m_Min = 1;
                    });
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionApplyBuff() {
                                m_Buff = TricksterTTTTravelDomainBaseBuff.ToReference<BlueprintBuffReference>(),
                                DurationValue = new ContextDurationValue() {
                                    DiceCountValue = 0,
                                    BonusValue = 1
                                }
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTTravelDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                });
                var TricksterTTTTravelDomainBaseFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTTravelDomainBaseFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(TravelDomainBaseFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTTravelDomainBaseAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTTravelDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                    bp.AddComponent<BuffMovementSpeed>(c => {
                        c.Value = 10;
                        c.Descriptor = ModifierDescriptor.UntypedStackable;
                        c.ContextBonus = new ContextValue();
                    });
                });
                //Greater Feature
                var TravelDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("9c4b72c847277cd4c94933a647d846cc");
                var TravelDomainGreaterAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("867e6fd88d089c442be7cdd49f05a88e");
                var TricksterTTTTravelDomainGreaterResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTTravelDomainGreaterResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = new BlueprintCharacterClassReference[0],
                        m_ClassDiv = ResourcesLibrary.GetRoot().Progression.m_CharacterClasses,
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 0,
                        StartingLevel = 0,
                        StartingIncrease = 0,
                        LevelStep = 2,
                        PerStepIncrease = 1,
                        IncreasedByLevelStartPlusDivStep = true,
                        IncreasedByStat = false
                    };
                    bp.m_Max = 20;
                    bp.m_UseMax = true;
                });
                var TricksterTTTTravelDomainGreaterAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTTravelDomainGreaterAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(TravelDomainGreaterAbility);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.OnePlusDivStep;
                        c.m_StepLevel = 5;
                        c.m_UseMin = true;
                        c.m_Min = 1;
                    });

                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTTravelDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                    TravelDomainGreaterAbility.ComponentsArray.OfType<AbilityCustomDimensionDoor>().ForEach(c => {
                        bp.AddComponent(Helpers.CreateCopy(c));
                    });
                });
                var TricksterTTTTravelDomainGreaterFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTTravelDomainGreaterFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(TravelDomainGreaterFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTTravelDomainGreaterAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTTravelDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });

                var tricksterDomain = CreateTricksterDomain(TravelDomainProgression);
                tricksterDomain.LevelEntries = new LevelEntry[] {
                    Helpers.CreateLevelEntry(1, TricksterTTTTravelDomainBaseFeature),
                    Helpers.CreateLevelEntry(8, TricksterTTTTravelDomainGreaterFeature)
                };
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateTrickeryDomain() {
                //Base Feature
                var TrickeryDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("cd1f4a784e0820647a34fe9bd5ffa770");
                var TrickeryDomainBaseAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("ee7eb5b9c644a0347b36eec653d3dfcb");
                var TrickeryDomainBaseBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("a70b6d974a5c135489a812437d9607c3");

                var TricksterTTTTrickeryDomainBaseResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTTrickeryDomainBaseResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterMythics,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 3,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                    bp.m_Max = 13;
                    bp.m_UseMax = true;
                });
                var TricksterTTTTrickeryDomainBaseBuff = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterTTTTrickeryDomainBaseBuff", bp => {
                    bp.ApplyVisualsAndBasicSettings(TrickeryDomainBaseBuff);
                    bp.AddComponent<AddMirrorImage>(c => {
                        c.Count = new ContextDiceValue() {
                            DiceCountValue = 0,
                            BonusValue = 1
                        };
                        c.MaxCount = 1;
                    });
                });
                var TricksterTTTTrickeryDomainBaseAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTTrickeryDomainBaseAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(TrickeryDomainBaseAbility);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.AsIs;
                        c.m_UseMin = true;
                        c.m_Min = 1;
                    });
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionApplyBuff() {
                                m_Buff = TricksterTTTTrickeryDomainBaseBuff.ToReference<BlueprintBuffReference>(),
                                DurationValue = new ContextDurationValue() {
                                    Rate = DurationRate.Rounds,
                                    DiceCountValue = 0,
                                    BonusValue = new ContextValue() {
                                        ValueType = ContextValueType.Rank
                                    }
                                }
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTTrickeryDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                    TrickeryDomainBaseAbility.ComponentsArray.OfType<AbilityCasterHasNoFacts>().ForEach(c => {
                        bp.AddComponent(Helpers.CreateCopy(c));
                    });
                });
                var TricksterTTTTrickeryDomainBaseFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTTrickeryDomainBaseFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(TrickeryDomainBaseFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTTrickeryDomainBaseAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTTrickeryDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                    bp.AddComponent<AddClassSkill>(c => {
                        c.Skill = StatType.SkillThievery;
                    });
                    bp.AddComponent<AddClassSkill>(c => {
                        c.Skill = StatType.SkillStealth;
                    });
                });
                //Greater Feature
                var TrickeryDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("a681aa60d35344c4b9ceb49de4e169ac");
                var TrickeryDomainGreaterToggleAbility = BlueprintTools.GetBlueprint<BlueprintActivatableAbility>("574b97e5b1d391348b162c62b49bc4fd");
                var TrickeryDomainGreaterBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("869e06144e5716a4a847c75ba9f48c0c");
                var TrickeryDomainGreaterAura = BlueprintTools.GetBlueprint<BlueprintAbilityAreaEffect>("073c1b15304f15c438c2532ea886029e");
                var TrickeryDomainGreaterEffect = BlueprintTools.GetBlueprint<BlueprintBuff>("a575bebf4e6731f4a93a70699486d693");

                var TricksterTTTTrickeryDomainGreaterResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTTrickeryDomainGreaterResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterClasses,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 0,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                });
                var TricksterTTTTrickeryDomainGreaterEffect = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterTTTTrickeryDomainGreaterEffect", bp => {
                    bp.ApplyVisualsAndBasicSettings(TrickeryDomainGreaterEffect);
                    bp.AddComponent<BuffInvisibility>(c => {
                        c.m_StealthBonus = 20;
                        c.Chance = new ContextValue();
                    });
                });
                var TricksterTTTTrickeryDomainGreaterAura = Helpers.CreateBlueprint<BlueprintAbilityAreaEffect>(TTTContext, "TricksterTTTTrickeryDomainGreaterAura", bp => {
                    bp.ApplyVisualsAndBasicSettings(TrickeryDomainGreaterAura);
                    bp.Fx = BlueAoE30Feet;
                    bp.AddComponent<AbilityAreaEffectBuff>(c => {
                        c.m_Buff = TricksterTTTTrickeryDomainGreaterEffect.ToReference<BlueprintBuffReference>();
                        c.Condition = new ConditionsChecker() {
                            Conditions = new Condition[] {
                                new ContextConditionIsAlly()
                            }
                        };
                    });
                });
                var TricksterTTTTrickeryDomainGreaterBuff = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterTTTTrickeryDomainGreaterBuff", bp => {
                    bp.ApplyVisualsAndBasicSettings(TrickeryDomainGreaterBuff);
                    bp.AddComponent<AddAreaEffect>(c => {
                        c.m_AreaEffect = TricksterTTTTrickeryDomainGreaterAura.ToReference<BlueprintAbilityAreaEffectReference>();
                    });
                });
                var TricksterTTTTrickeryDomainGreaterToggleAbility = Helpers.CreateBlueprint<BlueprintActivatableAbility>(TTTContext, "TricksterTTTTrickeryDomainGreaterToggleAbility", bp => {
                    bp.m_Buff = TricksterTTTTrickeryDomainGreaterBuff.ToReference<BlueprintBuffReference>();
                    bp.ApplyVisualsAndBasicSettings(TrickeryDomainGreaterToggleAbility);
                    bp.AddComponent<ActivatableAbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTTrickeryDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.SpendType = ActivatableAbilityResourceLogic.ResourceSpendType.NewRound;
                    });
                });
                var TricksterTTTTrickeryDomainGreaterFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTTrickeryDomainGreaterFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(TrickeryDomainGreaterFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTTrickeryDomainGreaterToggleAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTTrickeryDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });

                var tricksterDomain = CreateTricksterDomain(TrickeryDomainProgression);

                tricksterDomain.LevelEntries = new LevelEntry[] {
                    Helpers.CreateLevelEntry(1, TricksterTTTTrickeryDomainBaseFeature),
                    Helpers.CreateLevelEntry(8, TricksterTTTTrickeryDomainGreaterFeature)
                };
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateWarDomain() {
                //Base Feature
                var WarDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("640c20da7d6fcbc43b0d30a0a762f122");
                var WarDomainBaseAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("fbef6b2053ab6634a82df06f76c260e3");
                var WarDomainBaseBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("aefec65136058694ab20cd71941eec81");

                var TricksterTTTWarDomainBaseResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTWarDomainBaseResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterMythics,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 3,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                    bp.m_Max = 13;
                    bp.m_UseMax = true;
                });
                var TricksterTTTWarDomainBaseBuff = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterTTTWarDomainBaseBuff", bp => {
                    bp.ApplyVisualsAndBasicSettings(WarDomainBaseBuff);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.Div2;
                        c.m_UseMin = true;
                        c.m_Min = 1;
                    });
                    bp.AddComponent<WeaponAttackTypeDamageBonus>(c => {
                        c.AttackBonus = 1;
                        c.Type = WeaponRangeType.Melee;
                        c.Descriptor = ModifierDescriptor.UntypedStackable;
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Rank
                        };
                    });
                });
                var TricksterTTTWarDomainBaseAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTWarDomainBaseAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(WarDomainBaseAbility);
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionApplyBuff() {
                                m_Buff = TricksterTTTWarDomainBaseBuff.ToReference<BlueprintBuffReference>(),
                                DurationValue = new ContextDurationValue() {
                                    Rate = DurationRate.Rounds,
                                    DiceCountValue = 0,
                                    BonusValue = 1
                                }
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTWarDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                    WarDomainBaseAbility.ComponentsArray.OfType<AbilityCasterHasNoFacts>().ForEach(c => {
                        bp.AddComponent(Helpers.CreateCopy(c));
                    });
                });
                var TricksterTTTWarDomainBaseFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTWarDomainBaseFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(WarDomainBaseFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTWarDomainBaseAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTWarDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                    bp.AddComponent<AddClassSkill>(c => {
                        c.Skill = StatType.SkillThievery;
                    });
                    bp.AddComponent<AddClassSkill>(c => {
                        c.Skill = StatType.SkillStealth;
                    });
                });
                //Greater Feature
                var WarDomainGreaterFeatSelection = BlueprintTools.GetBlueprint<BlueprintFeatureSelection>("79c6421dbdb028c4fa0c31b8eea95f16");
                var tricksterDomain = CreateTricksterDomain(WarDomainProgression);

                tricksterDomain.LevelEntries = new LevelEntry[] {
                    Helpers.CreateLevelEntry(1, TricksterTTTWarDomainBaseFeature),
                    Helpers.CreateLevelEntry(8, WarDomainGreaterFeatSelection)
                };
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateWaterDomain() {
                //Base Feature
                var WaterDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("4c21ad24f55f64d4fb722f40720d9ab0");
                var WaterDomainBaseAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("5e1db2ef80ff361448549beeb7785791");

                var TricksterTTTWaterDomainBaseResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTWaterDomainBaseResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterMythics,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 3,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                    bp.m_Max = 13;
                    bp.m_UseMax = true;
                });
                var TricksterTTTWaterDomainBaseAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTWaterDomainBaseAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(WaterDomainBaseAbility);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Type = AbilityRankType.DamageBonus;
                        c.m_Progression = ContextRankProgression.Div2;
                    });
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionDealDamage() {
                                DamageType = new DamageTypeDescription() {
                                    Type = DamageType.Energy,
                                    Energy = DamageEnergyType.Cold
                                },
                                Duration = new ContextDurationValue() {
                                    DiceCountValue = 0,
                                    BonusValue = 0
                                },
                                Value = new ContextDiceValue() {
                                    DiceType = DiceType.D6,
                                    DiceCountValue = 1,
                                    BonusValue = new ContextValue() {
                                        ValueType = ContextValueType.Rank,
                                        ValueRank = AbilityRankType.DamageBonus
                                    }
                                }
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTWaterDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                    WaterDomainBaseAbility.ComponentsArray.OfType<AbilityDeliverProjectile>().ForEach(c => {
                        bp.AddComponent(Helpers.CreateCopy(c));
                    });
                });
                var TricksterTTTWaterDomainBaseFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTWaterDomainBaseFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(WaterDomainBaseFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTWaterDomainBaseAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTWaterDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                    bp.AddComponent<AddClassSkill>(c => {
                        c.Skill = StatType.SkillThievery;
                    });
                    bp.AddComponent<AddClassSkill>(c => {
                        c.Skill = StatType.SkillStealth;
                    });
                });
                //Greater Feature
                var WaterDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("8f8d0892cbe15b54ebe10552603349b2");
                var WaterDomainCapstone = BlueprintTools.GetBlueprint<BlueprintFeature>("6ec8672a9dd06604b93d56c33904aee9");

                var tricksterDomain = CreateTricksterDomain(WaterDomainProgression);
                tricksterDomain.LevelEntries = new LevelEntry[] {
                    Helpers.CreateLevelEntry(1, TricksterTTTWaterDomainBaseFeature),
                    Helpers.CreateLevelEntry(6, WaterDomainGreaterFeature),
                    Helpers.CreateLevelEntry(12, WaterDomainGreaterFeature),
                    Helpers.CreateLevelEntry(20, WaterDomainCapstone)
                };
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateWeatherDomain() {
                //Base Feature
                var WeatherDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("1c37869ee06ca33459f16f23f4969e7d");
                var WeatherDomainBaseAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("f166325c271dd29449ba9f98d11542d9");
                var WeatherDomainBaseBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("d29d7ab73b547954f9395c4738c3ecd5");

                var TricksterTTTWeatherDomainBaseResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTWeatherDomainBaseResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterMythics,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 3,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                    bp.m_Max = 13;
                    bp.m_UseMax = true;
                });
                var TricksterTTTWeatherDomainBaseBuff = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterTTTWeatherDomainBaseBuff", bp => {
                    bp.ApplyVisualsAndBasicSettings(WeatherDomainBaseBuff);
                    bp.AddComponent<AddStatBonus>(c => {
                        c.Stat = StatType.AdditionalAttackBonus;
                        c.Descriptor = ModifierDescriptor.UntypedStackable;
                        c.Value = -2;
                    });
                });
                var TricksterTTTWeatherDomainBaseAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTWeatherDomainBaseAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(WeatherDomainBaseAbility);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Type = AbilityRankType.DamageBonus;
                        c.m_Progression = ContextRankProgression.Div2;
                    });
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionDealDamage() {
                                DamageType = new DamageTypeDescription() {
                                    Type = DamageType.Energy,
                                    Energy = DamageEnergyType.Magic
                                },
                                Duration = new ContextDurationValue() {
                                    DiceCountValue = 0,
                                    BonusValue = 0
                                },
                                Value = new ContextDiceValue() {
                                    DiceType = DiceType.D6,
                                    DiceCountValue = 1,
                                    BonusValue = new ContextValue() {
                                        ValueType = ContextValueType.Rank,
                                        ValueRank = AbilityRankType.DamageBonus
                                    }
                                }
                            },
                            new ContextActionApplyBuff() {
                                m_Buff = TricksterTTTWeatherDomainBaseBuff.ToReference<BlueprintBuffReference>(),
                                DurationValue = new ContextDurationValue() {
                                    Rate = DurationRate.Rounds,
                                    DiceCountValue = 0,
                                    BonusValue = 1
                                }
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTWeatherDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                });
                var TricksterTTTWeatherDomainBaseFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTWeatherDomainBaseFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(WeatherDomainBaseFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTWeatherDomainBaseAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTWeatherDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                    bp.AddComponent<AddClassSkill>(c => {
                        c.Skill = StatType.SkillThievery;
                    });
                    bp.AddComponent<AddClassSkill>(c => {
                        c.Skill = StatType.SkillStealth;
                    });
                });
                //Greater Feature
                var WeatherDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("8e44306af595c8d44aad2f1260fd7be2");
                var WeatherDomainGreaterAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("c2a55b2d9b8d29747b893723a1142fae");
                var InsideTheStormBuff = BlueprintTools.GetBlueprintReference<BlueprintUnitFactReference>("32e90ae6f8c7656448d9e80173222012");

                var TricksterTTTWeatherDomainGreaterResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTWeatherDomainGreaterResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterClasses,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 0,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                    bp.m_Max = 20;
                    bp.m_UseMax = true;
                });
                var TricksterTTTWeatherDomainGreaterAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTWeatherDomainGreaterAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(WeatherDomainGreaterAbility);
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new Conditional() {
                                ConditionsChecker = new ConditionsChecker() {
                                    Conditions = new Condition[] {
                                        new ContextConditionCasterHasFact(){
                                            m_Fact = InsideTheStormBuff
                                        }
                                    }
                                },
                                IfTrue = Helpers.CreateActionList(
                                    new ContextActionDealDamage() {
                                        DamageType = new DamageTypeDescription() {
                                            Type = DamageType.Energy,
                                            Energy = DamageEnergyType.Electricity
                                        },
                                        Duration = new ContextDurationValue() {
                                            DiceCountValue = 0,
                                            BonusValue = 0
                                        },
                                        Value = new ContextDiceValue() {
                                            DiceType = DiceType.D10,
                                            DiceCountValue = 3,
                                            BonusValue = 0
                                        }
                                    }
                                ),
                                IfFalse = Helpers.CreateActionList(
                                    new ContextActionDealDamage() {
                                        DamageType = new DamageTypeDescription() {
                                            Type = DamageType.Energy,
                                            Energy = DamageEnergyType.Electricity
                                        },
                                        Duration = new ContextDurationValue() {
                                            DiceCountValue = 0,
                                            BonusValue = 0
                                        },
                                        Value = new ContextDiceValue() {
                                            DiceType = DiceType.D6,
                                            DiceCountValue = 3,
                                            BonusValue = 0
                                        }
                                    }
                                ),
                            }
                        );
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTWeatherDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                    WeatherDomainGreaterAbility.ComponentsArray.OfType<AbilityCustomDimensionDoor>().ForEach(c => {
                        bp.AddComponent(Helpers.CreateCopy(c));
                    });
                });
                var TricksterTTTWeatherDomainGreaterFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTWeatherDomainGreaterFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(WeatherDomainGreaterFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTWeatherDomainGreaterAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTWeatherDomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });

                var tricksterDomain = CreateTricksterDomain(WeatherDomainProgression);
                tricksterDomain.LevelEntries = new LevelEntry[] {
                    Helpers.CreateLevelEntry(1, TricksterTTTWeatherDomainBaseFeature),
                    Helpers.CreateLevelEntry(8, TricksterTTTWeatherDomainGreaterFeature)
                };
                TricksterDomains.Add(tricksterDomain);
            }

            void CreateIceSubdomain() {
                //Base Feature
                var IceSubdomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("fe2be05e3ee04b818554a2fc93f7605b");
                var TricksterTTTWaterDomainBaseAbility = BlueprintTools.GetModBlueprint<BlueprintAbility>(TTTContext,"TricksterTTTWaterDomainBaseAbility");
                var TricksterTTTWaterDomainBaseResource = BlueprintTools.GetModBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTWaterDomainBaseResource");
                var SubtypeCold = BlueprintTools.GetBlueprintReference<BlueprintUnitFactReference>("5e4d22d5cb6869e499f5fdc82e2127ad");

                var TricksterTTTIceSubdomainBaseFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTIceSubdomainBaseFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(IceSubdomainBaseFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTWaterDomainBaseAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTWaterDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });
                //Greater Feature
                var IceSubdomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("b105ed7a854948bda3d7ee23e60b7c70");
                var IceSubdomainGreaterActivatableAbility = BlueprintTools.GetBlueprint<BlueprintActivatableAbility>("0b6be620495f41afa9ee181c84b5dfb4");
                var IceSubdomainGreaterBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("d909098fa0444193ba1a9c38f43cfd03");

                var TricksterTTTIceSubdomainGreaterResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTIceSubdomainGreaterResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterClasses,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 0,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                });
                var TricksterTTTIceSubdomainGreaterBuff = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterTTTIceSubdomainGreaterBuff", bp => {
                    bp.ApplyVisualsAndBasicSettings(IceSubdomainGreaterBuff);
                    bp.AddComponent<BuffDescriptorImmunity>(c => {
                        c.Descriptor = SpellDescriptor.Cold;
                    });
                    bp.AddComponent<AddEnergyImmunity>(c => {
                        c.Type = DamageEnergyType.Cold;
                    });
                    bp.AddComponent<AddEnergyVulnerability>(c => {
                        c.Type = DamageEnergyType.Fire;
                    });
                    bp.AddComponent<SpellDescriptorComponent>(c => {
                        c.Descriptor = SpellDescriptor.Polymorph;
                    });
                    bp.AddComponent<PolymorphBonuses>(c => {
                        c.masterShifterBonus = 4;
                    });
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] { 
                            SubtypeCold
                        };
                    });
                    bp.AddComponent<AddFactContextActions>(c => {
                        c.Activated = Helpers.CreateActionList(
                            new ContextActionRemoveBuffsByDescriptor() {
                                NotSelf = true,
                                SpellDescriptor = SpellDescriptor.Polymorph
                            }
                        );
                        c.Deactivated = Helpers.CreateActionList();
                        c.NewRound = Helpers.CreateActionList();
                        c.Dispose = Helpers.CreateActionList();
                    });
                    
                    bp.AddContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.Custom;
                        c.m_BaseValueType = ContextRankBaseValueType.CharacterLevel;
                        c.m_CustomProgression = new ContextRankConfig.CustomProgressionItem[] { 
                            new ContextRankConfig.CustomProgressionItem() {
                                BaseValue = 15,
                                ProgressionValue = 5
                            },
                            new ContextRankConfig.CustomProgressionItem() {
                                BaseValue = 30,
                                ProgressionValue = 10
                            }
                        };
                        c.m_DisableRankBonus = true;
                    });
                });
                var TricksterTTTIceSubdomainGreaterToggleAbility = Helpers.CreateBlueprint<BlueprintActivatableAbility>(TTTContext, "TricksterTTTIceSubdomainGreaterToggleAbility", bp => {
                    bp.AddToDomainZealot();
                    bp.AddTricksterAbilityParams();
                    bp.m_Buff = TricksterTTTIceSubdomainGreaterBuff.ToReference<BlueprintBuffReference>();
                    bp.AddComponent<ActivatableAbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTIceSubdomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.SpendType = ActivatableAbilityResourceLogic.ResourceSpendType.NewRound;
                    });
                    bp.m_DisplayName = IceSubdomainGreaterActivatableAbility.m_DisplayName;
                    bp.m_Description = IceSubdomainGreaterActivatableAbility.m_Description;
                    bp.m_DescriptionShort = IceSubdomainGreaterActivatableAbility.m_DescriptionShort;
                    bp.m_Icon = IceSubdomainGreaterActivatableAbility.m_Icon;
                    bp.Group = ActivatableAbilityGroup.None;
                    bp.WeightInGroup = 1;
                    bp.DeactivateIfCombatEnded = true;
                    bp.DeactivateIfOwnerDisabled = true;
                    bp.ActivationType = AbilityActivationType.WithUnitCommand;
                    bp.m_ActivateWithUnitCommand = Kingmaker.UnitLogic.Commands.Base.UnitCommand.CommandType.Standard;
                });
                var TricksterTTTIceSubdomainGreaterFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTIceSubdomainGreaterFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(IceSubdomainGreaterFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTIceSubdomainGreaterToggleAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTIceSubdomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });

                var tricksterDomain = CreateTricksterDomain(IceSubdomainProgression);
                tricksterDomain.LevelEntries = new LevelEntry[] {
                    Helpers.CreateLevelEntry(1, TricksterTTTIceSubdomainBaseFeature),
                    Helpers.CreateLevelEntry(8, TricksterTTTIceSubdomainGreaterFeature)
                };
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateMurderSubdomain() {
                //Base Feature
                var MurderSubdomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("b94f3f27224b4421be9d6aaa6f46b088");
                var TricksterTTTDeathDomainBaseAbility = BlueprintTools.GetModBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTDeathDomainBaseAbility");
                var TricksterTTTDeathDomainBaseResource = BlueprintTools.GetModBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTDeathDomainBaseResource");
                var Dlc6Reward = BlueprintTools.GetBlueprintReference<BlueprintDlcRewardReference>("b94f823171a84e30ad7a1b892433ab5d");

                var TricksterTTTMurderSubdomainBaseFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTMurderSubdomainBaseFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(MurderSubdomainBaseFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTDeathDomainBaseAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTDeathDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });
                //Greater Feature
                var MurderSubdomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("f7dbd9bc4b634f0dae18c49cc851bb69");
                var MurderSubdomainGreaterBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("aea3898392404a01b7b117c448286d2c");
                var MurderSubdomainGreaterAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("901aa07653684c7d87f58b970c25d1b4");

                var TricksterTTTMurderSubdomainGreaterResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTMurderSubdomainGreaterResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = new BlueprintCharacterClassReference[0],
                        m_ClassDiv = ResourcesLibrary.GetRoot().Progression.m_CharacterClasses,
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        StartingLevel = 8,
                        StartingIncrease = 1,
                        LevelStep = 4,
                        PerStepIncrease = 1,
                        IncreasedByLevelStartPlusDivStep = true,
                        IncreasedByStat = false
                    };
                });
                var TricksterTTTMurderSubdomainGreaterBuff = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterTTTMurderSubdomainGreaterBuff", bp => {
                    bp.ApplyVisualsAndBasicSettings(MurderSubdomainGreaterBuff);
                    bp.AddComponent<AdditionalDiceOnAttack>(c => {
                        c.CriticalHit = true;
                        c.InitiatorConditions = new ConditionsChecker { 
                            Conditions = new Condition[0]
                        };
                        c.TargetConditions = new ConditionsChecker {
                            Conditions = new Condition[0]
                        };
                        c.Value = new ContextDiceValue() { 
                            DiceCountValue = new ContextValue(),
                            BonusValue = new ContextValue() { 
                                ValueType = ContextValueType.Rank
                            }
                        };
                        c.DamageType = new DamageTypeDescription() {
                            Type = DamageType.Direct
                        };
                    });
                    bp.AddContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.Div2;
                        c.m_BaseValueType = ContextRankBaseValueType.CharacterLevel;
                        c.m_DisableRankBonus = true;
                    });
                });
                var TricksterTTTMurderSubdomainGreaterAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTMurderSubdomainGreaterAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(MurderSubdomainGreaterAbility);
                    bp.AddTricksterContextRankConfig(c => {
                        c.m_Type = AbilityRankType.DamageBonus;
                        c.m_Progression = ContextRankProgression.Div2;
                    });
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionApplyBuff() {
                                m_Buff = TricksterTTTMurderSubdomainGreaterBuff.ToReference<BlueprintBuffReference>(),
                                DurationValue = new ContextDurationValue() {
                                    Rate = DurationRate.Rounds,
                                    DiceCountValue = 0,
                                    BonusValue = new ContextValue() {
                                        ValueType = ContextValueType.Rank
                                    }
                                },
                                IsNotDispelable = true,
                                ToCaster = true
                            }
                        );
                    });
                    bp.AddContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.Div2;
                        c.m_BaseValueType = ContextRankBaseValueType.CharacterLevel;
                        c.m_DisableRankBonus = true;
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTMurderSubdomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                });
                var TricksterTTTMurderSubdomainGreaterFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTMurderSubdomainGreaterFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(MurderSubdomainGreaterFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTMurderSubdomainGreaterAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTMurderSubdomainGreaterResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });

                var tricksterDomain = CreateTricksterDomain(MurderSubdomainProgression);
                tricksterDomain.LevelEntries = new LevelEntry[] {
                    Helpers.CreateLevelEntry(1, TricksterTTTMurderSubdomainBaseFeature),
                    Helpers.CreateLevelEntry(8, TricksterTTTMurderSubdomainGreaterFeature)
                };
                tricksterDomain.AddComponent<DlcCondition>(c => {
                    c.m_DlcReward = Dlc6Reward;
                });
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateUndeadSubdomain() {
                //Base Feature
                var UndeadSubdomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("665fabebb632402d948fbe45e03a0b86");
                var UndeadSubdomainBaseAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("3b12a2a6898d403ebf918d9a6aeb02b0");
                var UndeadSubdomainBaseBuff = BlueprintTools.GetBlueprintReference<BlueprintBuffReference>("82412cb7918c411b9731985a3b3cb3e0");
                var TouchItem = BlueprintTools.GetBlueprintReference<BlueprintItemWeaponReference>("bb337517547de1a4189518d404ec49d4");

                var TricksterTTTUndeadDomainBaseResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTUndeadDomainBaseResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterMythics,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 3,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                });
                var TricksterTTTUndeadDomainBaseAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTUndeadDomainBaseAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(UndeadSubdomainBaseAbility);
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionApplyBuff() {
                                m_Buff = UndeadSubdomainBaseBuff,
                                DurationValue = new ContextDurationValue() {
                                    Rate = DurationRate.Rounds,
                                    DiceCountValue = 0,
                                    BonusValue = new ContextValue() {
                                        ValueType = ContextValueType.Rank
                                    }
                                },
                                IsNotDispelable = true
                            }
                        );
                    });
                    bp.AddComponent<AbilityDeliverTouch>(c => {
                        c.m_TouchWeapon = TouchItem;
                    });
                    bp.AddContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.Div2;
                        c.m_BaseValueType = ContextRankBaseValueType.CharacterLevel;
                        c.m_DisableRankBonus = true;
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTUndeadDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                });
                var TricksterTTTUndeadDomainBaseFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTUndeadDomainBaseFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(UndeadSubdomainBaseFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTUndeadDomainBaseAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTUndeadDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });
                //Greater Feature
                var DeathDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("b0acce833384b9b428f32517163c9117");

                var tricksterDomain = CreateTricksterDomain(UndeadSubdomainProgression);
                tricksterDomain.LevelEntries = new LevelEntry[] {
                    Helpers.CreateLevelEntry(1, TricksterTTTUndeadDomainBaseFeature),
                    Helpers.CreateLevelEntry(8, DeathDomainGreaterFeature)
                };
                TricksterDomains.Add(tricksterDomain);
            }

            void CreateScalykindDomain() {
                //Base Feature
                var ScalykindDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("e22881ef284148fa972af9f521abf66d");
                var ScalykindDomainBaseFeatureAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("8050cf5b55b04c1ca41705a9ed51ba2a");
                var FascinateCommonBuff = BlueprintTools.GetBlueprintReference<BlueprintBuffReference>("9c70d2ae017665b4b845e6c299cb7439");
                var TouchItem = BlueprintTools.GetBlueprintReference<BlueprintItemWeaponReference>("bb337517547de1a4189518d404ec49d4");

                var TricksterTTTScalykindDomainDCProperty = Helpers.CreateBlueprint<BlueprintUnitProperty>(TTTContext, "TricksterTTTScalykindDomainDCProperty", bp => {
                    bp.AddComponent<SimplePropertyGetter>(c => {
                        c.Property = UnitProperty.MythicLevel;
                        c.Settings = new PropertySettings() {
                            m_Progression = PropertySettings.Progression.AsIs
                        };
                    });
                    bp.AddComponent<SimplePropertyGetter>(c => {
                        c.Property = UnitProperty.MythicLevel;
                        c.Settings = new PropertySettings() {
                            m_Progression = PropertySettings.Progression.AsIs
                        };
                    });
                    bp.BaseValue = 10;
                });
                var TricksterTTTScalykindDomainBaseResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTScalykindDomainBaseResource", bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterMythics,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = 3,
                        LevelIncrease = 1,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                });
                var TricksterTTTScalykindDomainBaseAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "TricksterTTTScalykindDomainBaseAbility", bp => {
                    bp.ApplyVisualsAndBasicSettings(ScalykindDomainBaseFeatureAbility);
                    bp.AddComponent<AbilityEffectRunAction>(c => {
                        c.Actions = Helpers.CreateActionList(
                            new ContextActionSavingThrow() {
                                m_ConditionalDCIncrease = new ContextActionSavingThrow.ConditionalDCIncrease[0],
                                Type = SavingThrowType.Will,
                                HasCustomDC = true,
                                CustomDC = new ContextValue() { 
                                    m_CustomProperty = TricksterTTTScalykindDomainDCProperty.ToReference<BlueprintUnitPropertyReference>(),
                                    ValueType = ContextValueType.CasterCustomProperty
                                },
                                Actions = Helpers.CreateActionList(
                                    new ContextActionConditionalSaved() {
                                        Succeed = Helpers.CreateActionList(),
                                        Failed = Helpers.CreateActionList(
                                            new ContextActionDealDamage() {
                                                DamageType = new DamageTypeDescription() {
                                                    Common = new DamageTypeDescription.CommomData(),
                                                    Physical = new DamageTypeDescription.PhysicalData(),
                                                    Type = DamageType.Untyped
                                                },
                                                Duration = new ContextDurationValue(),
                                                Value = new ContextDiceValue() {
                                                    DiceType = DiceType.D6,
                                                    DiceCountValue = 1,
                                                    BonusValue = new ContextValue() {
                                                        ValueType = ContextValueType.Rank
                                                    }
                                                }
                                            },
                                            new ContextActionApplyBuff() {
                                                m_Buff = FascinateCommonBuff,
                                                DurationValue = new ContextDurationValue() {
                                                    m_IsExtendable = true,
                                                    DiceCountValue = 0,
                                                    BonusValue = 1
                                                },
                                                AsChild = true
                                            }
                                        )
                                    }
                                )
                            }
                        );
                    });
                    bp.AddComponent<AbilityTargetIsAlly>(c => {
                        c.Not = false;
                    });
                    bp.AddComponent<SpellDescriptorComponent>(c => {
                        c.Descriptor = SpellDescriptor.MindAffecting | SpellDescriptor.GazeAttack;
                    });
                    bp.AddContextRankConfig(c => {
                        c.m_Progression = ContextRankProgression.AsIs;
                        c.m_BaseValueType = ContextRankBaseValueType.MythicLevel;
                        c.m_DisableRankBonus = true;
                    });
                    bp.AddComponent<AbilityResourceLogic>(c => {
                        c.m_RequiredResource = TricksterTTTScalykindDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.m_IsSpendResource = true;
                        c.Amount = 1;
                    });
                });
                var TricksterTTTScalykindDomainBaseFeature = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "TricksterTTTScalykindDomainBaseFeature", bp => {
                    bp.ApplyVisualsAndBasicSettings(ScalykindDomainBaseFeature);
                    bp.AddComponent<AddFacts>(c => {
                        c.m_Facts = new BlueprintUnitFactReference[] {
                            TricksterTTTScalykindDomainBaseAbility.ToReference<BlueprintUnitFactReference>()
                        };
                    });
                    bp.AddComponent<AddAbilityResources>(c => {
                        c.m_Resource = TricksterTTTScalykindDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                        c.RestoreAmount = true;
                    });
                });
                //Greater Feature
                var ScalykindCompanionSelectionDomain = BlueprintTools.GetBlueprint<BlueprintFeature>("de9a327caa5b45eeb05ed9fe0f0ed4ce");

                var tricksterDomain = CreateTricksterDomain(ScalykindDomainProgression);
                tricksterDomain.LevelEntries = new LevelEntry[] {
                    Helpers.CreateLevelEntry(1, TricksterTTTScalykindDomainBaseFeature),
                    Helpers.CreateLevelEntry(4, ScalykindCompanionSelectionDomain)
                };
                TricksterDomains.Add(tricksterDomain);
            }

            static BlueprintProgression CreateTricksterDomain(BlueprintProgression domain) {
                return domain.CreateCopy(TTTContext, $"TricksterTTT{domain.name}", TricksterDomainMasterID, bp => {
                    var SpellList = bp.GetComponent<LearnSpellList>()?.m_SpellList;
                    var NoFeature = bp.GetComponents<PrerequisiteNoFeature>()?.Select(c => c.m_Feature).ToArray();
                    bp.m_Classes = new BlueprintProgression.ClassWithLevel[0];
                    bp.m_Archetypes = new BlueprintProgression.ArchetypeWithLevel[0];
                    bp.m_FeaturesRankIncrease = new List<BlueprintFeatureReference>();
                    bp.IsClassFeature = true;

                    var domainSpellTable = BlueprintTools.GetModBlueprintReference<BlueprintSpellsTableReference>(TTTContext, "TricksterTTTDomainSpellsPerDay");

                    var domainSpellbook = Helpers.CreateDerivedBlueprint<BlueprintSpellbook>(TTTContext,
                        $"TricksterTTT{domain.name}Spellbook",
                        TricksterDomainMasterID,
                        new SimpleBlueprint[] { domain, SpellList },
                        bp => {
                            bp.Name = domain.m_DisplayName;
                            bp.CastingAttribute = StatType.Charisma;
                            bp.AllSpellsKnown = true;
                            bp.CantripsType = CantripsType.Orisions;
                            bp.HasSpecialSpellList = false;
                            bp.SpecialSpellListName = new Kingmaker.Localization.LocalizedString();
                            bp.m_SpellsPerDay = TricksterDomainSpellsPerDay;
                            bp.m_SpellsKnown = TricksterDomainSpellsKnown;
                            bp.m_SpellSlots = new BlueprintSpellsTableReference();
                            bp.m_SpellList = SpellList;
                            bp.m_MythicSpellList = SpellList;
                            bp.m_CharacterClass = TricksterMythicClass;
                            bp.IsArcane = false;
                            bp.IsMythic = true;
                            bp.Spontaneous = true;
                        }
                    );
                    bp.RemoveComponents<LearnSpellList>();
                    bp.RemoveComponents<Prerequisite>();
                    bp.AddComponent<AddMythicSpellbook>(c => {
                        c.m_Spellbook = domainSpellbook.ToReference<BlueprintSpellbookReference>();
                        c.m_CasterLevel = new ContextValue() {
                            ValueType = ContextValueType.CasterCustomProperty,
                            m_CustomProperty = TricksterDomainCLProperty.ToReference<BlueprintUnitPropertyReference>()
                        };
                    });
                    bp.AddComponent<RecalculateOnLevelUp>();
                    bp.AddPrerequisite<PrerequisiteNoFeature>(c => {
                        c.m_Feature = domain.ToReference<BlueprintFeatureReference>();
                    });
                    NoFeature.ForEach(feature => {
                        bp.AddPrerequisite<PrerequisiteNoFeature>(c => {
                            c.m_Feature = feature;
                        });
                    });
                    domain.AddPrerequisite<PrerequisiteNoFeature>(c => {
                        c.m_Feature = bp.ToReference<BlueprintFeatureReference>();
                    });
                    NoFeature.ForEach(feature => {
                        feature.Get().AddPrerequisite<PrerequisiteNoFeature>(c => {
                            c.m_Feature = bp.ToReference<BlueprintFeatureReference>();
                        });
                    });
                });
            }
        }
        private static void ApplyVisualsAndBasicSettings(this BlueprintFeature blueprint, BlueprintFeature copyFrom) {
            blueprint.TemporaryContext(bp => {
                bp.m_DisplayName = copyFrom.m_DisplayName;
                bp.m_Description = copyFrom.m_Description;
                bp.m_DescriptionShort = copyFrom.m_DescriptionShort;
                bp.m_Icon = copyFrom.m_Icon;
                bp.IsClassFeature = true;
                bp.ReapplyOnLevelUp = true;
                copyFrom.ComponentsArray.OfType<AddFeatureOnClassLevel>().ForEach(c => {
                    bp.AddComponent(Helpers.CreateCopy(c));
                });
            });
        }
        private static void ApplyVisualsAndBasicSettings(this BlueprintAbility blueprint, BlueprintAbility copyFrom) {
            blueprint.AddToDomainZealot();
            blueprint.AddTricksterAbilityParams();
            blueprint.TemporaryContext(bp => {
                bp.m_DisplayName = copyFrom.m_DisplayName;
                bp.m_Description = copyFrom.m_Description;
                bp.m_DescriptionShort = copyFrom.m_DescriptionShort;
                bp.m_Icon = copyFrom.m_Icon;
                bp.Type = copyFrom.Type;
                bp.Range = copyFrom.Range;
                bp.ActionType = copyFrom.ActionType;
                bp.m_IsFullRoundAction = copyFrom.m_IsFullRoundAction;
                bp.SpellResistance = copyFrom.SpellResistance;
                bp.EffectOnEnemy = copyFrom.EffectOnEnemy;
                bp.EffectOnAlly = copyFrom.EffectOnAlly;
                bp.CanTargetEnemies = copyFrom.CanTargetEnemies;
                bp.CanTargetFriends = copyFrom.CanTargetFriends;
                bp.CanTargetSelf = copyFrom.CanTargetSelf;
                bp.CanTargetPoint = copyFrom.CanTargetPoint;
                bp.m_DefaultAiAction = copyFrom.m_DefaultAiAction;
                bp.Animation = copyFrom.Animation;
                bp.LocalizedDuration = copyFrom.LocalizedDuration;
                bp.LocalizedSavingThrow = copyFrom.LocalizedSavingThrow;
                bp.MaterialComponent = copyFrom.MaterialComponent;
                bp.ResourceAssetIds = copyFrom.ResourceAssetIds;
                copyFrom.ComponentsArray.OfType<AbilityDeliverTouch>().ForEach(c => {
                    bp.AddComponent(Helpers.CreateCopy(c));
                });
                copyFrom.ComponentsArray.OfType<AbilityDeliverProjectile>().ForEach(c => {
                    bp.AddComponent(Helpers.CreateCopy(c));
                });
                copyFrom.ComponentsArray.OfType<AbilityTargetsAround>().ForEach(c => {
                    bp.AddComponent(Helpers.CreateCopy(c));
                });
                copyFrom.ComponentsArray.OfType<AbilityAoERadius>().ForEach(c => {
                    bp.AddComponent(Helpers.CreateCopy(c));
                });
                copyFrom.ComponentsArray.OfType<SpellDescriptorComponent>().ForEach(c => {
                    bp.AddComponent(Helpers.CreateCopy(c));
                });
                copyFrom.ComponentsArray.OfType<SpellComponent>().ForEach(c => {
                    bp.AddComponent(Helpers.CreateCopy(c));
                });
                copyFrom.ComponentsArray.OfType<AbilitySpawnFx>().ForEach(c => {
                    bp.AddComponent(Helpers.CreateCopy(c));
                });
            });
        }
        private static void ApplyVisualsAndBasicSettings(this BlueprintActivatableAbility blueprint, BlueprintActivatableAbility copyFrom) {
            blueprint.AddToDomainZealot();
            blueprint.AddTricksterAbilityParams();
            blueprint.TemporaryContext(bp => {
                bp.m_DisplayName = copyFrom.m_DisplayName;
                bp.m_Description = copyFrom.m_Description;
                bp.m_DescriptionShort = copyFrom.m_DescriptionShort;
                bp.m_Icon = copyFrom.m_Icon;
                bp.Group = copyFrom.Group;
                bp.WeightInGroup = copyFrom.WeightInGroup;
                bp.DeactivateIfCombatEnded = copyFrom.DeactivateIfCombatEnded;
                bp.DeactivateIfOwnerDisabled = copyFrom.DeactivateIfOwnerDisabled;
                bp.ActivationType = copyFrom.ActivationType;
                bp.m_ActivateWithUnitCommand = copyFrom.m_ActivateWithUnitCommand;
            });
        }
        private static void ApplyVisualsAndBasicSettings(this BlueprintBuff blueprint, BlueprintBuff copyFrom) {
            blueprint.TemporaryContext(bp => {
                bp.m_Flags = copyFrom.m_Flags;
                bp.m_DisplayName = copyFrom.m_DisplayName;
                bp.m_Description = copyFrom.m_Description;
                bp.m_DescriptionShort = copyFrom.m_DescriptionShort;
                bp.m_Icon = copyFrom.m_Icon;
                bp.IsClassFeature = true;
                bp.ResourceAssetIds = copyFrom.ResourceAssetIds;
                bp.FxOnRemove = copyFrom.FxOnRemove;
                bp.FxOnStart = copyFrom.FxOnStart;
                copyFrom.ComponentsArray.OfType<AbilitySpawnFx>().ForEach(c => {
                    bp.AddComponent(Helpers.CreateCopy(c));
                });
            });
        }
        private static void ApplyVisualsAndBasicSettings(this BlueprintAbilityAreaEffect blueprint, BlueprintAbilityAreaEffect copyFrom) {
            blueprint.AddTricksterAbilityParams();
            blueprint.TemporaryContext(bp => {
                bp.Fx = copyFrom.Fx;
                bp.Shape = copyFrom.Shape;
                bp.Size = copyFrom.Size;
                bp.AggroEnemies = copyFrom.AggroEnemies;
                bp.AffectEnemies = copyFrom.AffectEnemies;
                bp.AffectDead = copyFrom.AffectDead;
                bp.IgnoreSleepingUnits = copyFrom.IgnoreSleepingUnits;
                bp.SpellResistance = copyFrom.SpellResistance;
                bp.m_TargetType = copyFrom.m_TargetType;
                bp.m_AllowNonContextActions = copyFrom.m_AllowNonContextActions;
            });
        }
        private static void AddTricksterContextRankConfig(this BlueprintScriptableObject blueprint, Action<ContextRankConfig> init = null) {
            blueprint.AddContextRankConfig(c => {
                c.m_BaseValueType = ContextRankBaseValueType.CustomProperty;
                c.m_CustomProperty = TricksterDomainCLProperty.ToReference<BlueprintUnitPropertyReference>();
                init?.Invoke(c);
            });
        }
        private static void AddTricksterAbilityParams(this BlueprintScriptableObject blueprint) {
            blueprint.AddComponent<ContextSetAbilityParams>(c => {
                c.DC = new ContextValue() {
                    ValueType = ContextValueType.CasterCustomProperty,
                    m_CustomProperty = TricksterDomainDCProperty.ToReference<BlueprintUnitPropertyReference>()
                };
                c.CasterLevel = new ContextValue() {
                    ValueType = ContextValueType.CasterCustomProperty,
                    m_CustomProperty = TricksterDomainCLProperty.ToReference<BlueprintUnitPropertyReference>()
                };
                c.Concentration = new ContextValue() {
                    ValueType = ContextValueType.CasterCustomProperty,
                    m_CustomProperty = TricksterDomainCLProperty.ToReference<BlueprintUnitPropertyReference>()
                };
                c.SpellLevel = -1;
            });
        }
        private static void AddToDomainZealot(this BlueprintAbility ability) {
            var AutoMetamagic = DomainMastery.GetComponent<AutoMetamagic>();
            AutoMetamagic.Abilities.Add(ability.ToReference<BlueprintAbilityReference>());
        }
        private static void AddToDomainZealot(this BlueprintActivatableAbility ability) {
            var component = DomainMastery.GetComponent<IncreaseActivatableAbilitySpeed>();
            if (component == null) {
                DomainMastery.AddComponent<IncreaseActivatableAbilitySpeed>(c => {
                    c.NewCommandType = Kingmaker.UnitLogic.Commands.Base.UnitCommand.CommandType.Swift;
                    c.m_Abilities = new BlueprintActivatableAbilityReference[0];
                });
                component = DomainMastery.GetComponent<IncreaseActivatableAbilitySpeed>();
            }
            component.m_Abilities = component.m_Abilities.AppendToArray(ability.ToReference<BlueprintActivatableAbilityReference>());
        }

        public static void AddTricksterTricks() {
            var Icon_TricksterPersausion = AssetLoader.LoadInternal(TTTContext, "TricksterTricks", "Icon_TricksterPersausion.png");

            var SongOfDiscordBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("2e1646c2449c88a4188e58043455a43a");
            var TricksterPerceptionTier1Feature = BlueprintTools.GetBlueprintReference<BlueprintUnitFactReference>("8bc2f9b88a0cf704ea72d86c2a3e2aef");
            var TricksterPerceptionTier2Feature = BlueprintTools.GetBlueprintReference<BlueprintUnitFactReference>("e9298851786c5334dba1398e9635a83d");

            var TricksterPerception3EffectBuff = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterPerception3EffectBuff", bp => {
                bp.m_Flags = BlueprintBuff.Flags.HiddenInUi;
                bp.SetName(TTTContext, "Perception 3 Effect");
                bp.SetDescription(TTTContext, "");
                bp.IsClassFeature = true;
                bp.AddComponent<AddFactsFromCaster>(c => {
                    c.m_Facts = new BlueprintUnitFactReference[] {
                        TricksterPerceptionTier1Feature,
                        TricksterPerceptionTier2Feature
                    };
                });
            });
            var TricksterPerception3Area = Helpers.CreateBlueprint<BlueprintAbilityAreaEffect>(TTTContext, "TricksterPerception3Area", bp => {
                bp.Shape = AreaEffectShape.Cylinder;
                bp.m_TargetType = BlueprintAbilityAreaEffect.TargetType.Ally;
                bp.Size = 60.Feet();
                bp.Fx = new PrefabLink();
                bp.AddComponent<AbilityAreaEffectRunAction>(c => {
                    c.UnitEnter = Helpers.CreateActionList(
                        Helpers.Create<Conditional>(condition => {
                            condition.ConditionsChecker = new ConditionsChecker() {
                                Conditions = new Condition[] {
                                        new ContextConditionIsAlly(),
                                        new ContextConditionIsCaster(){
                                            Not = true
                                        }
                                }
                            };
                            condition.IfFalse = Helpers.CreateActionList();
                            condition.IfTrue = Helpers.CreateActionList(
                                Helpers.Create<ContextActionApplyBuff>(a => {
                                    a.m_Buff = TricksterPerception3EffectBuff.ToReference<BlueprintBuffReference>();
                                    a.AsChild = true;
                                    a.IsNotDispelable = true;
                                    a.Permanent = true;
                                    a.DurationValue = new ContextDurationValue() {
                                        DiceCountValue = new ContextValue(),
                                        BonusValue = new ContextValue()
                                    };
                                })
                            );
                        }));
                    c.UnitExit = Helpers.CreateActionList(
                        Helpers.Create<Conditional>(condition => {
                            condition.ConditionsChecker = new ConditionsChecker() {
                                Conditions = new Condition[] {
                                        new ContextConditionIsAlly(),
                                        new ContextConditionIsCaster(){
                                            Not = true
                                        }
                                }
                            };
                            condition.IfFalse = Helpers.CreateActionList();
                            condition.IfTrue = Helpers.CreateActionList(
                                Helpers.Create<ContextActionRemoveBuff>(a => {
                                    a.m_Buff = TricksterPerception3EffectBuff.ToReference<BlueprintBuffReference>();
                                })
                            );
                        }));
                    c.UnitMove = Helpers.CreateActionList();
                    c.Round = Helpers.CreateActionList();
                });
            });
            var TricksterPerception3Buff = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterPerception3Buff", bp => {
                bp.m_Flags = BlueprintBuff.Flags.HiddenInUi;
                bp.SetName(TTTContext, "Aeon Bane Increase Resource Feature");
                bp.SetDescription(TTTContext, "");
                bp.IsClassFeature = true;
                bp.AddComponent<AddAreaEffect>(c => {
                    c.m_AreaEffect = TricksterPerception3Area.ToReference<BlueprintAbilityAreaEffectReference>();
                });
            });
            var TricksterPersuasion2Buff = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "TricksterPersuasion2Buff", bp => {
                bp.SetName(TTTContext, "Trickster Persuasion 2");
                bp.SetDescription(TTTContext, "Creature suffers penalties to its attack and damage rolls equal to " +
                    "1 + half your mythic rank.");
                bp.m_Icon = Icon_TricksterPersausion;
                bp.Stacking = StackingType.Prolong;
                bp.IsClassFeature = true;
                bp.AddComponent<AddContextStatBonus>(c => {
                    c.Stat = StatType.AdditionalAttackBonus;
                    c.Value = new ContextValue() {
                        ValueType = ContextValueType.Rank
                    };
                    c.Descriptor = ModifierDescriptor.UntypedStackable;
                    c.Multiplier = -1;
                });
                bp.AddComponent<AddContextStatBonus>(c => {
                    c.Stat = StatType.AdditionalDamage;
                    c.Value = new ContextValue() {
                        ValueType = ContextValueType.Rank
                    };
                    c.Descriptor = ModifierDescriptor.UntypedStackable;
                    c.Multiplier = -1;
                });
                bp.AddContextRankConfig(c => {
                    c.m_BaseValueType = ContextRankBaseValueType.MythicLevel;
                    c.m_Progression = ContextRankProgression.OnePlusDivStep;
                    c.m_StepLevel = 2;
                });
            });
            var TricksterPersuasion3Buff = SongOfDiscordBuff.CreateCopy(TTTContext, "TricksterPersuasion3Buff", bp => {
                bp.SetName(TTTContext, "Trickster Persuasion 3");
                bp.SetDescription(TTTContext, "Creature has a 50% chance to attack the nearest target each turn. " +
                    "Additionally they suffer a penalty to spell resistance equal to equal to your mythic rank, as well as pentalities to AC and saving throws equal to 1 + half your mythic rank.");
                bp.m_Icon = Icon_TricksterPersausion;
                bp.Stacking = StackingType.Prolong;
                bp.IsClassFeature = true;
                bp.RemoveComponents<SpellDescriptorComponent>();
                bp.AddComponent<AddSpellResistancePenaltyTTT>(c => {
                    c.Penalty = new ContextValue() {
                        ValueType = ContextValueType.Rank
                    };
                });
                bp.AddComponent<AddContextStatBonus>(c => {
                    c.Stat = StatType.AC;
                    c.Value = new ContextValue() {
                        ValueType = ContextValueType.Rank,
                        ValueRank = AbilityRankType.StatBonus
                    };
                    c.Descriptor = ModifierDescriptor.UntypedStackable;
                    c.Multiplier = -1;
                });
                bp.AddComponent<AddContextStatBonus>(c => {
                    c.Stat = StatType.SaveFortitude;
                    c.Value = new ContextValue() {
                        ValueType = ContextValueType.Rank,
                        ValueRank = AbilityRankType.StatBonus
                    };
                    c.Descriptor = ModifierDescriptor.UntypedStackable;
                    c.Multiplier = -1;
                });
                bp.AddComponent<AddContextStatBonus>(c => {
                    c.Stat = StatType.SaveReflex;
                    c.Value = new ContextValue() {
                        ValueType = ContextValueType.Rank,
                        ValueRank = AbilityRankType.StatBonus
                    };
                    c.Descriptor = ModifierDescriptor.UntypedStackable;
                    c.Multiplier = -1;
                });
                bp.AddComponent<AddContextStatBonus>(c => {
                    c.Stat = StatType.SaveWill;
                    c.Value = new ContextValue() {
                        ValueType = ContextValueType.Rank,
                        ValueRank = AbilityRankType.StatBonus
                    };
                    c.Descriptor = ModifierDescriptor.UntypedStackable;
                    c.Multiplier = -1;
                });
                bp.AddContextRankConfig(c => {
                    c.m_BaseValueType = ContextRankBaseValueType.MythicLevel;
                    c.m_Type = AbilityRankType.Default;
                    c.m_Progression = ContextRankProgression.AsIs;
                });
                bp.AddContextRankConfig(c => {
                    c.m_BaseValueType = ContextRankBaseValueType.MythicLevel;
                    c.m_Type = AbilityRankType.StatBonus;
                    c.m_Progression = ContextRankProgression.OnePlusDivStep;
                    c.m_StepLevel = 2;
                });
            });
        }
    }
}
