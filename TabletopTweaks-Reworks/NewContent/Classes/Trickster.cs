using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.ResourceLinks;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Abilities.Components.AreaEffects;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Buffs.Components;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using Kingmaker.UnitLogic.Mechanics.Properties;
using Kingmaker.Utility;
using System.Collections.Generic;
using System.Linq;
using TabletopTweaks.Core.NewComponents;
using TabletopTweaks.Core.Utilities;
using static TabletopTweaks.Reworks.Main;

namespace TabletopTweaks.Reworks.NewContent.Classes {
    static class Trickster {
        private static BlueprintGuid TricksterDomainMasterID = TTTContext.Blueprints.GetDerivedMaster("TricksterDomainMasterID");
        private static BlueprintGuid[] TricksterSpellResource = new BlueprintGuid[9] {
            TTTContext.Blueprints.GetDerivedMaster("TricksterSpellResource1"),
            TTTContext.Blueprints.GetDerivedMaster("TricksterSpellResource2"),
            TTTContext.Blueprints.GetDerivedMaster("TricksterSpellResource3"),
            TTTContext.Blueprints.GetDerivedMaster("TricksterSpellResource4"),
            TTTContext.Blueprints.GetDerivedMaster("TricksterSpellResource5"),
            TTTContext.Blueprints.GetDerivedMaster("TricksterSpellResource6"),
            TTTContext.Blueprints.GetDerivedMaster("TricksterSpellResource7"),
            TTTContext.Blueprints.GetDerivedMaster("TricksterSpellResource8"),
            TTTContext.Blueprints.GetDerivedMaster("TricksterSpellResource9")
        };
        private static BlueprintCharacterClassReference TricksterMythicClass => BlueprintTools.GetBlueprintReference<BlueprintCharacterClassReference>("8df873a8c6e48294abdb78c45834aa0a");
        private static BlueprintSpellsTableReference TricksterDomainSpellsKnown = null;
        private static BlueprintSpellsTableReference TricksterDomainSpellsPerDay = null;
        public static List<BlueprintProgression> TricksterDomains = new List<BlueprintProgression>();
        public static BlueprintUnitProperty TricksterDomainRankProperty = Helpers.CreateBlueprint<BlueprintUnitProperty>(TTTContext, "TricksterTTTDomainRankProperty", bp => {
            bp.AddComponent<SimplePropertyGetter>(c => {
                c.Property = UnitProperty.MythicLevel;
                c.Settings = new PropertySettings() {
                    m_Progression = PropertySettings.Progression.MultiplyByModifier,
                    m_StepLevel = 2
                };
            });
            bp.BaseValue = 0;
        });
        public static BlueprintUnitProperty TricksterDomainDCProperty = Helpers.CreateBlueprint<BlueprintUnitProperty>(TTTContext, "TricksterTTTDomainDCProperty", bp => {
            bp.AddComponent<SimplePropertyGetter>(c => {
                c.Property = UnitProperty.MythicLevel;
                c.Settings = new PropertySettings() {
                    m_Progression = PropertySettings.Progression.MultiplyByModifier,
                    m_StepLevel = 2
                };
            });
            bp.BaseValue = 10;
        });
        private static BlueprintFeature DomainMastery => BlueprintTools.GetBlueprint<BlueprintFeature>("2de64f6a1f2baee4f9b7e52e3f046ec5");

        public static void AddTricksterDomains() {
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

            CreateDomainSpellsPerDay();
            CreateAirDomain();
            CreateAnimalDomain();
            CreateArtificeDomain();
            CreateChaosDomain();
            CreateCharmDomain();
            CreateCommunityDomain();
            CreateDarknessDomain();
            CreateDeathDomain();
            //CreateDestructionDomain();
            CreateEarthDomain();
            CreateEvilDomain();
            CreateFireDomain();
            //CreateGloryDomain();
            CreateGoodDomain();
            CreateHealingDomain();
            //CreateKnowledgeDomain();
            CreateLawDomain();
            //CreateLiberationDomain();
            CreateLuckDomain();
            //CreateMadnessDomain();
            CreateMagicDomain();
            CreateNobilityDomain();
            //CreatePlantDomain();
            //CreateProtectionDomain();
            //CreateReposeDomain();
            //CreateRuneDomain();
            CreateStrengthDomain();
            //CreateSunDomain();
            CreateTravelDomain();
            //CreateTrickeryDomain();
            CreateWarDomain();
            CreateWaterDomain();
            CreateWeatherDomain();

            void CreateDomainSpellsPerDay() {
                TricksterDomainSpellsKnown = Helpers.CreateBlueprint<BlueprintSpellsTable>(TTTContext, "TricksterTTTDomainSpellsKnown", bp => {
                    bp.Levels = new SpellsLevelEntry[] {
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
                        SpellTools.CreateSpellLevelEntry(0,1,1,1,1,1,1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(0,1,1,1,1,1,1,1,1,1)
                    };
                }).ToReference<BlueprintSpellsTableReference>();
                TricksterDomainSpellsPerDay = Helpers.CreateBlueprint<BlueprintSpellsTable>(TTTContext, "TricksterTTTDomainSpellsPerDay", bp => {
                    bp.Levels = new SpellsLevelEntry[] {
                        SpellTools.CreateSpellLevelEntry(1),
                        SpellTools.CreateSpellLevelEntry(1),
                        SpellTools.CreateSpellLevelEntry(1,1),
                        SpellTools.CreateSpellLevelEntry(1,1),
                        SpellTools.CreateSpellLevelEntry(1,1,1),
                        SpellTools.CreateSpellLevelEntry(1,1,1),
                        SpellTools.CreateSpellLevelEntry(1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(1,1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(1,1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(1,1,1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(1,1,1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(1,1,1,1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(1,1,1,1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(1,1,1,1,1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(1,1,1,1,1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(1,1,1,1,1,1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(1,1,1,1,1,1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(1,1,1,1,1,1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(1,1,1,1,1,1,1,1,1),
                        SpellTools.CreateSpellLevelEntry(1,1,1,1,1,1,1,1,1)
                    };
                }).ToReference<BlueprintSpellsTableReference>();
            }
            void CreateAirDomain() {
                var AirDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("39b0c7db785560041b436b558c9df2bb");
                var tricksterDomain = CreateTricksterDomain(AirDomainProgression);

                tricksterDomain.LevelEntries
                    .Where(entry => entry.Level == 1)
                    .ForEach(entry => {
                        entry.m_Features = new List<BlueprintFeatureBaseReference>() {
                            CreateTricksterDomainAbilityFeature(AirDomainBaseFeature)
                        };
                    });
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateAnimalDomain() {
                var AnimalDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("d577aba79b5727a4ab74627c4c6ba23c");
                var AnimalCompanionSelectionDomain = BlueprintTools.GetBlueprint<BlueprintFeatureSelection>("2ecd6c64683b59944a7fe544033bb533");
                var DomainAnimalCompanionProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("125af359f8bc9a145968b5d8fd8159b8");
                var AnimalCompanionRank = BlueprintTools.GetBlueprint<BlueprintFeature>("1670990255e4fe948a863bafd5dbda5d");
                var tricksterDomain = CreateTricksterDomain(AnimalDomainProgression);

                tricksterDomain.LevelEntries
                    .Where(entry => entry.Level == 1)
                    .ForEach(entry => {
                        entry.m_Features = new List<BlueprintFeatureBaseReference>() {
                            AnimalDomainBaseFeature.CreateCopy(TTTContext, $"TricksterTTT{AnimalDomainBaseFeature.name}", TricksterDomainMasterID, bp => {
                                ConvertContextRankConfigs(bp);
                            }).ToReference<BlueprintFeatureBaseReference>()
                        };
                    });
                tricksterDomain.LevelEntries
                    .Where(entry => entry.Level == 2)
                    .ForEach(entry => {
                        entry.m_Features = new List<BlueprintFeatureBaseReference>() {
                            AnimalCompanionSelectionDomain.CreateCopy(TTTContext, $"TricksterTTT{AnimalCompanionSelectionDomain.name}", TricksterDomainMasterID, bp => {
                                bp.RemoveComponents<AddFeatureOnApply>(c => c.m_Feature.deserializedGuid == AnimalCompanionRank.AssetGuid);
                                bp.GetComponents<AddFeatureOnApply>()
                                .Where(c => c.m_Feature.deserializedGuid == DomainAnimalCompanionProgression.AssetGuid)
                                .FirstOrDefault().m_Feature = DomainAnimalCompanionProgression.CreateCopy(TTTContext, $"TricksterTTT{DomainAnimalCompanionProgression.name}", TricksterDomainMasterID, bp => {
                                    bp.m_Classes = new BlueprintProgression.ClassWithLevel[0];
                                    bp.m_Archetypes = new BlueprintProgression.ArchetypeWithLevel[0];
                                    bp.m_FeaturesRankIncrease = new List<BlueprintFeatureReference>();
                                    ResourcesLibrary.GetRoot()
                                        .Progression
                                        .CharacterMythics
                                        .ForEach(mythic => bp.AddClass(mythic));
                                    bp.LevelEntries = new LevelEntry[] {
                                        Helpers.CreateLevelEntry(2, AnimalCompanionRank),
                                        Helpers.CreateLevelEntry(3, AnimalCompanionRank, AnimalCompanionRank),
                                        Helpers.CreateLevelEntry(4, AnimalCompanionRank, AnimalCompanionRank),
                                        Helpers.CreateLevelEntry(5, AnimalCompanionRank, AnimalCompanionRank),
                                        Helpers.CreateLevelEntry(6, AnimalCompanionRank, AnimalCompanionRank),
                                        Helpers.CreateLevelEntry(7, AnimalCompanionRank, AnimalCompanionRank),
                                        Helpers.CreateLevelEntry(8, AnimalCompanionRank, AnimalCompanionRank),
                                        Helpers.CreateLevelEntry(9, AnimalCompanionRank, AnimalCompanionRank),
                                        Helpers.CreateLevelEntry(10, AnimalCompanionRank, AnimalCompanionRank)
                                    };
                                }).ToReference<BlueprintFeatureReference>();
                            }).ToReference<BlueprintFeatureBaseReference>()
                        };
                    });
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateArtificeDomain() {
                var ArtificeDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("01025d876ac28d349ac42d69ba462059");
                var ArtificeDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("9c536f77cae0c5c46a9cf871003ebe43");
                var ArtificeDomainBaseToggleAbility = BlueprintTools.GetBlueprint<BlueprintActivatableAbility>("18fd072abe74d144a916e3501533b76b");
                var ArtificeDomainBaseBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("af772f43b1e59e043968796b6b534057");
                var ArtificeDomainBaseAura = BlueprintTools.GetBlueprint<BlueprintAbilityAreaEffect>("f042f2d62e6785d4e8612a027de1f298");
                var ArtificeDomainGreaterEffect = BlueprintTools.GetBlueprint<BlueprintBuff>("9d4a139cb5605fa409b1be3ad6e87ba9");

                var baseResource = ArtificeDomainBaseFeature.GetComponent<AddAbilityResources>()?.Resource;
                var tricksterResource = baseResource.CreateCopy(TTTContext, $"TricksterTTT{baseResource.name}", TricksterDomainMasterID, bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterMythics,
                        m_ClassDiv = new BlueprintCharacterClassReference[0],
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = baseResource.m_MaxAmount.BaseValue,
                        LevelIncrease = 2,
                        IncreasedByLevel = true,
                        IncreasedByStat = false
                    };
                    bp.m_Max = 20;
                    bp.m_UseMax = true;
                });
                var tricksterGreaterBuff = ArtificeDomainGreaterEffect.CreateCopy(TTTContext, $"TricksterTTT{ArtificeDomainGreaterEffect.name}", TricksterDomainMasterID, bp => {
                    ConvertContextRankConfigs(bp);
                });
                var tricksterBaseFeature = ArtificeDomainBaseFeature.CreateCopy(TTTContext, $"TricksterTTT{ArtificeDomainBaseFeature.name}", TricksterDomainMasterID, bp => {
                    var toggleAbility = bp.GetComponent<AddFacts>().Facts.First();
                    bp.GetComponent<AddAbilityResources>().m_Resource = tricksterResource.ToReference<BlueprintAbilityResourceReference>();
                    bp.GetComponent<AddFacts>().m_Facts = new BlueprintUnitFactReference[] {
                        ArtificeDomainBaseToggleAbility.CreateCopy(TTTContext, $"TricksterTTT{ArtificeDomainBaseToggleAbility.name}", TricksterDomainMasterID, toggleBP => {
                            toggleBP.GetComponent<ActivatableAbilityResourceLogic>().m_RequiredResource =  tricksterResource.ToReference<BlueprintAbilityResourceReference>();

                            toggleBP.m_Buff = ArtificeDomainBaseBuff.CreateCopy(TTTContext, $"TricksterTTT{ArtificeDomainBaseBuff.name}", TricksterDomainMasterID, baseBuffBP => {
                                baseBuffBP.GetComponent<AddAreaEffect>().m_AreaEffect = ArtificeDomainBaseAura.CreateCopy(TTTContext, $"TricksterTTT{ArtificeDomainBaseAura.name}", TricksterDomainMasterID, areaBP => {
                                    areaBP.FlattenAllActions().OfType<ContextActionApplyBuff>()
                                    .Where(c => c.Buff == ArtificeDomainGreaterEffect)
                                    .ForEach(c => {
                                        c.m_Buff = tricksterGreaterBuff.ToReference<BlueprintBuffReference>();
                                    });
                                }).ToReference<BlueprintAbilityAreaEffectReference>();
                            }).ToReference<BlueprintBuffReference>();

                            
                        }).ToReference<BlueprintUnitFactReference>()
                    };
                });    
                    
                var tricksterDomain = CreateTricksterDomain(ArtificeDomainProgression);

                tricksterDomain.LevelEntries
                    .Where(entry => entry.Level == 1)
                    .ForEach(entry => {
                        entry.m_Features = new List<BlueprintFeatureBaseReference>() {
                            tricksterBaseFeature.ToReference<BlueprintFeatureBaseReference>()
                        };
                    });
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateChaosDomain() {
                var ChaosDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("0c9d931180a19a646bcf4165f66bd318");
                var ChaosDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("41b647ee4591dc1448a665a62b7a7b5f");
                var tricksterDomain = CreateTricksterDomain(ChaosDomainProgression);
                
                tricksterDomain.LevelEntries
                    .Where(entry => entry.Level == 1)
                    .ForEach(entry => {
                        entry.m_Features = new List<BlueprintFeatureBaseReference>() {
                            CreateTricksterDomainAbilityFeature(ChaosDomainBaseFeature)
                        };
                    });
                tricksterDomain.LevelEntries
                    .Where(entry => entry.Level == 4)
                    .ForEach(entry => {
                        entry.m_Features = new List<BlueprintFeatureBaseReference>() {
                            CreateTricksterDomainGreaterAbilityFeature(ChaosDomainGreaterFeature)
                        };
                    });
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateCharmDomain() {
                var CharmDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("4847d450fbef9b444abcc3a82337b426");
                var CharmDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("d1fee57aa8f12b849b5abd5f2b7c4616");
                var tricksterDomain = CreateTricksterDomain(CharmDomainProgression);

                tricksterDomain.LevelEntries
                    .Where(entry => entry.Level == 1)
                    .ForEach(entry => {
                        entry.m_Features = new List<BlueprintFeatureBaseReference>() {
                            CreateTricksterDomainAbilityFeature(CharmDomainBaseFeature)
                        };
                    });
                tricksterDomain.LevelEntries
                    .Where(entry => entry.Level == 4)
                    .ForEach(entry => {
                        entry.m_Features = new List<BlueprintFeatureBaseReference>() {
                            CreateTricksterDomainAbilityFeature(CharmDomainGreaterFeature)
                        };
                    });
                BlueprintTools.GetModBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTCharmDomainGreaterResource")
                    .TemporaryContext(bp => {
                        bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                            m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterMythics,
                            m_ClassDiv = new BlueprintCharacterClassReference[0],
                            m_Archetypes = new BlueprintArchetypeReference[0],
                            m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                            BaseValue = bp.m_MaxAmount.BaseValue,
                            LevelIncrease = 2,
                            IncreasedByLevel = true,
                            IncreasedByStat = false
                        };
                    });
                    
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateCommunityDomain() {
                var CommunityDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("102d61a114786894bb2b30568943ef1f");
                var CommunityDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("4cddbb24833e1d24ea1ff0f59574284a");
                var CommunityDomainGreaterAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("76291e62d2496ad41824044aba3077ea");
                var CommunityDomainGreaterArea = BlueprintTools.GetBlueprint<BlueprintAbilityAreaEffect>("3635b48c6e8d54947bbd27c1be818677");
                var CommunityDomainGreaterBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("672b0149e68d73943ad09ac35a9f5f30");
                var tricksterDomain = CreateTricksterDomain(CommunityDomainProgression);
                var tricksterGreaterFeature = CommunityDomainGreaterFeature.CreateCopy(TTTContext, $"TricksterTTT{CommunityDomainGreaterFeature.name}", TricksterDomainMasterID, bp => {
                    bp.GetComponent<AddFacts>().m_Facts = new BlueprintUnitFactReference[] {
                        CommunityDomainGreaterAbility.CreateCopy(TTTContext, $"TricksterTTT{CommunityDomainGreaterAbility.name}", TricksterDomainMasterID, abilityBP => {
                            ConvertContextRankConfigs(abilityBP);
                            AddToDomainZealot(abilityBP);
                            abilityBP.FlattenAllActions()
                                .OfType<ContextActionSpawnAreaEffect>()
                                .First()
                                .m_AreaEffect = CommunityDomainGreaterArea.CreateCopy(TTTContext, $"TricksterTTT{CommunityDomainGreaterArea.name}", TricksterDomainMasterID, areaBP => {
                                    areaBP.GetComponent<AbilityAreaEffectBuff>().m_Buff = CommunityDomainGreaterBuff.CreateCopy(TTTContext, $"TricksterTTT{CommunityDomainGreaterBuff.name}", TricksterDomainMasterID, buffBP => {
                                        ConvertContextRankConfigs(buffBP);
                                        buffBP.GetComponent<ContextRankConfig>().m_BaseValueType = ContextRankBaseValueType.MythicLevel;
                                    }).ToReference<BlueprintBuffReference>();
                                }).ToReference<BlueprintAbilityAreaEffectReference>();
                        }).ToReference<BlueprintUnitFactReference>()
                    };
                });

                tricksterDomain.LevelEntries
                    .Where(entry => entry.Level == 1)
                    .ForEach(entry => {
                        entry.m_Features = new List<BlueprintFeatureBaseReference>() {
                            CreateTricksterDomainAbilityFeature(CommunityDomainBaseFeature)
                        };
                    });
                tricksterDomain.LevelEntries
                    .Where(entry => entry.Level == 4)
                    .ForEach(entry => {
                        entry.m_Features = new List<BlueprintFeatureBaseReference>() {
                            tricksterGreaterFeature.ToReference<BlueprintFeatureBaseReference>()
                        };
                    });

                TricksterDomains.Add(tricksterDomain);
            }
            void CreateDarknessDomain() {
                var DarknessDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("9dc5863168155854fa8daf4a780f6663");
                var DarknessDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("0653cd3fee730654eb4daa6629e07fad");
                var tricksterDomain = CreateTricksterDomain(DarknessDomainProgression);

                tricksterDomain.LevelEntries
                    .Where(entry => entry.Level == 1)
                    .ForEach(entry => {
                        entry.m_Features = new List<BlueprintFeatureBaseReference>() {
                            CreateTricksterDomainAbilityFeature(DarknessDomainBaseFeature)
                        };
                    });
                tricksterDomain.LevelEntries
                    .Where(entry => entry.Level == 4)
                    .ForEach(entry => {
                        entry.m_Features = new List<BlueprintFeatureBaseReference>() {
                            CreateTricksterDomainGreaterAbilityFeature(DarknessDomainGreaterFeature)
                        };
                    });
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateDeathDomain() {
                var DeathDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("9809efa15e5f9ad478594479af575a5d");
                var DeathDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("b0acce833384b9b428f32517163c9117");
                var tricksterDomain = CreateTricksterDomain(DeathDomainProgression);

                tricksterDomain.LevelEntries
                    .Where(entry => entry.Level == 1)
                    .ForEach(entry => {
                        entry.m_Features = new List<BlueprintFeatureBaseReference>() {
                            CreateTricksterDomainAbilityFeature(DeathDomainBaseFeature)
                        };
                    });
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateDestructionDomain(){ }
            void CreateEarthDomain() {
                var EarthDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("828d82a0e8c5a944bbdb6b12f802ff02");
                var tricksterDomain = CreateTricksterDomain(EarthDomainProgression);

                tricksterDomain.LevelEntries
                    .Where(entry => entry.Level == 1)
                    .ForEach(entry => {
                        entry.m_Features = new List<BlueprintFeatureBaseReference>() {
                            CreateTricksterDomainAbilityFeature(EarthDomainBaseFeature)
                        };
                    });
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateEvilDomain() {
                var EvilDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("80de18178ff13304e8cf27ba3ef3d77d");
                var EvilDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("3784df3083cb6404fbce7a585be24bcf");
                var tricksterDomain = CreateTricksterDomain(EvilDomainProgression);

                tricksterDomain.LevelEntries
                    .Where(entry => entry.Level == 1)
                    .ForEach(entry => {
                        entry.m_Features = new List<BlueprintFeatureBaseReference>() {
                            CreateTricksterDomainAbilityFeature(EvilDomainBaseFeature)
                        };
                    });
                tricksterDomain.LevelEntries
                    .Where(entry => entry.Level == 4)
                    .ForEach(entry => {
                        entry.m_Features = new List<BlueprintFeatureBaseReference>() {
                            CreateTricksterDomainGreaterAbilityFeature(EvilDomainGreaterFeature)
                        };
                    });
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateFireDomain() {
                var FireDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("42cc125d570c5334c89c6499b55fc0a3");
                var tricksterDomain = CreateTricksterDomain(FireDomainProgression);

                tricksterDomain.LevelEntries
                    .Where(entry => entry.Level == 1)
                    .ForEach(entry => {
                        entry.m_Features = new List<BlueprintFeatureBaseReference>() {
                            CreateTricksterDomainAbilityFeature(FireDomainBaseFeature)
                        };
                    });
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateGloryDomain(){ }
            void CreateGoodDomain() {
                var GoodDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("f27684b3b72c2f546abf3ef2fb611a05");
                var GoodDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("c90f8979927db4b4fbf6159297e01af8");
                var tricksterDomain = CreateTricksterDomain(GoodDomainProgression);

                tricksterDomain.LevelEntries
                    .Where(entry => entry.Level == 1)
                    .ForEach(entry => {
                        entry.m_Features = new List<BlueprintFeatureBaseReference>() {
                            CreateTricksterDomainAbilityFeature(GoodDomainBaseFeature, true)
                        };
                    });
                tricksterDomain.LevelEntries
                    .Where(entry => entry.Level == 4)
                    .ForEach(entry => {
                        entry.m_Features = new List<BlueprintFeatureBaseReference>() {
                            CreateTricksterDomainGreaterAbilityFeature(GoodDomainGreaterFeature)
                        };
                    });
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateHealingDomain() {
                var HealingDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("303cf1c933f343c4d91212f8f4953e3c");
                var tricksterDomain = CreateTricksterDomain(HealingDomainProgression);

                tricksterDomain.LevelEntries
                    .Where(entry => entry.Level == 1)
                    .ForEach(entry => {
                        entry.m_Features = new List<BlueprintFeatureBaseReference>() {
                            CreateTricksterDomainAbilityFeature(HealingDomainBaseFeature)
                        };
                    });
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateKnowledgeDomain() { }
            void CreateLawDomain() {
                var LawDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("9bd2d216e56a0db44be0df48ffc515af");
                var LawDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("3dc5e2b315ff07f438582a2468beb1fb");
                var tricksterDomain = CreateTricksterDomain(LawDomainProgression);

                tricksterDomain.LevelEntries
                    .Where(entry => entry.Level == 1)
                    .ForEach(entry => {
                        entry.m_Features = new List<BlueprintFeatureBaseReference>() {
                            CreateTricksterDomainAbilityFeature(LawDomainBaseFeature, true)
                        };
                    });
                tricksterDomain.LevelEntries
                    .Where(entry => entry.Level == 4)
                    .ForEach(entry => {
                        entry.m_Features = new List<BlueprintFeatureBaseReference>() {
                            CreateTricksterDomainGreaterAbilityFeature(LawDomainGreaterFeature)
                        };
                    });
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateLiberationDomain(){ }
            void CreateLuckDomain() {
                var LuckDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("2b3818bf4656c1a41b93467755662c78");
                var LuckDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("dd58b458af054e642bf845c3f01307e5");
                var tricksterDomain = CreateTricksterDomain(LuckDomainProgression);

                tricksterDomain.LevelEntries
                    .Where(entry => entry.Level == 1)
                    .ForEach(entry => {
                        entry.m_Features = new List<BlueprintFeatureBaseReference>() {
                            CreateTricksterDomainAbilityFeature(LuckDomainBaseFeature)
                        };
                    });
                tricksterDomain.LevelEntries
                    .Where(entry => entry.Level == 3)
                    .ForEach(entry => {
                        entry.m_Features = new List<BlueprintFeatureBaseReference>() {
                            CreateTricksterDomainGreaterAbilityFeature(LuckDomainGreaterFeature)
                        };
                    });
                BlueprintTools.GetModBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTLuckDomainGreaterResource").m_MaxAmount = new BlueprintAbilityResource.Amount() {
                    m_Class = new BlueprintCharacterClassReference[0],
                    m_ClassDiv = ResourcesLibrary.GetRoot().Progression.m_CharacterMythics,
                    m_Archetypes = new BlueprintArchetypeReference[0],
                    m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                    BaseValue = 0,
                    StartingLevel = 3,
                    StartingIncrease = 1,
                    LevelStep = 3,
                    PerStepIncrease = 1,
                    IncreasedByLevelStartPlusDivStep = true,
                    IncreasedByStat = false
                };
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateMadnessDomain() {
                var MadnessDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("84bf46e8086dbdc438bac875ab0e5c2f");
                var MadnessDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("9acc8ab2f313d0e49bb01e030c868e3f");
                var tricksterDomain = CreateTricksterDomain(MadnessDomainProgression);

                tricksterDomain.LevelEntries
                    .Where(entry => entry.Level == 1)
                    .ForEach(entry => {
                        entry.m_Features = new List<BlueprintFeatureBaseReference>() {
                            CreateTricksterDomainAbilityFeature(MadnessDomainBaseFeature, true)
                        };
                    });
                /*
                tricksterDomain.LevelEntries
                    .Where(entry => entry.Level == 4)
                    .ForEach(entry => {
                        entry.m_Features = new List<BlueprintFeatureBaseReference>() {
                            CreateTricksterDomainGreaterAbilityFeature(LuckDomainGreaterFeature)
                        };
                    });
                */
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateMagicDomain() {
                var MagicDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("90f939eb611ac3743b5de3dd00135e22");
                var MagicDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("cf47e96abd88c9f418f8e67f5a14381f");
                var tricksterDomain = CreateTricksterDomain(MagicDomainProgression);

                tricksterDomain.LevelEntries
                    .Where(entry => entry.Level == 1)
                    .ForEach(entry => {
                        entry.m_Features = new List<BlueprintFeatureBaseReference>() {
                            CreateTricksterDomainAbilityFeature(MagicDomainBaseFeature)
                        };
                    });
                tricksterDomain.LevelEntries
                    .Where(entry => entry.Level == 4)
                    .ForEach(entry => {
                        entry.m_Features = new List<BlueprintFeatureBaseReference>() {
                            CreateTricksterDomainGreaterAbilityFeature(MagicDomainGreaterFeature)
                        };
                    });
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateNobilityDomain() {
                var NobilityDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("a1a7f3dd904ed8e45b074232f48190d1");
                var NobilityDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("75acf3f9598248344b76f0b87ad27ac1");
                var tricksterDomain = CreateTricksterDomain(NobilityDomainProgression);

                tricksterDomain.LevelEntries
                    .Where(entry => entry.Level == 1)
                    .ForEach(entry => {
                        entry.m_Features = new List<BlueprintFeatureBaseReference>() {
                            CreateTricksterDomainAbilityFeature(NobilityDomainBaseFeature)
                        };
                    });
                tricksterDomain.LevelEntries
                    .Where(entry => entry.Level == 4)
                    .ForEach(entry => {
                        entry.m_Features = new List<BlueprintFeatureBaseReference>() {
                            CreateTricksterDomainAbilityFeature(NobilityDomainGreaterFeature)
                        };
                    });
                TricksterDomains.Add(tricksterDomain);
            }
            void CreatePlantDomain(){ }
            void CreateProtectionDomain() { }
            void CreateReposeDomain() { }
            void CreateRuneDomain() { }
            void CreateStrengthDomain(){
                var StrengthDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("526f99784e9fe4346824e7f210d46112");
                var StrengthDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("3298fd30e221ef74189a06acbf376d29");
                var tricksterDomain = CreateTricksterDomain(StrengthDomainProgression);
                var tricksterGreaterFeature = StrengthDomainGreaterFeature.CreateCopy(TTTContext, $"TricksterTTT{StrengthDomainGreaterFeature.name}", TricksterDomainMasterID, bp => {
                    ConvertContextRankConfigs(bp);
                });

                tricksterDomain.LevelEntries
                    .Where(entry => entry.Level == 1)
                    .ForEach(entry => {
                        entry.m_Features = new List<BlueprintFeatureBaseReference>() {
                            CreateTricksterDomainAbilityFeature(StrengthDomainBaseFeature, true)
                        };
                    });
                tricksterDomain.LevelEntries
                    .Where(entry => entry.Level == 4)
                    .ForEach(entry => {
                        entry.m_Features = new List<BlueprintFeatureBaseReference>() {
                            tricksterGreaterFeature.ToReference<BlueprintFeatureBaseReference>()
                        };
                    });
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateSunDomain() { }
            void CreateTravelDomain() {
                var TravelDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("3079cdfba971d614ab4f49220c6cd228");
                var TravelDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("9c4b72c847277cd4c94933a647d846cc");
                var tricksterDomain = CreateTricksterDomain(TravelDomainProgression);

                tricksterDomain.LevelEntries
                    .Where(entry => entry.Level == 1)
                    .ForEach(entry => {
                        entry.m_Features = new List<BlueprintFeatureBaseReference>() {
                            CreateTricksterDomainAbilityFeature(TravelDomainBaseFeature, true)
                        };
                    });
                tricksterDomain.LevelEntries
                    .Where(entry => entry.Level == 4)
                    .ForEach(entry => {
                        entry.m_Features = new List<BlueprintFeatureBaseReference>() {
                            CreateTricksterDomainAbilityFeature(TravelDomainGreaterFeature)
                        };
                    });
                BlueprintTools.GetModBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTTravelDomainGreaterResource").m_MaxAmount = new BlueprintAbilityResource.Amount() {
                    m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterMythics,
                    m_ClassDiv = new BlueprintCharacterClassReference[0],
                    m_Archetypes = new BlueprintArchetypeReference[0],
                    m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                    BaseValue = 0,
                    LevelIncrease = 2,
                    IncreasedByLevel = true,
                    IncreasedByStat = false
                };
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateTrickeryDomain() { }
            void CreateWarDomain() {
                var WarDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("640c20da7d6fcbc43b0d30a0a762f122");
                var tricksterDomain = CreateTricksterDomain(WarDomainProgression);

                tricksterDomain.LevelEntries
                    .Where(entry => entry.Level == 1)
                    .ForEach(entry => {
                        entry.m_Features = new List<BlueprintFeatureBaseReference>() {
                            CreateTricksterDomainAbilityFeature(WarDomainBaseFeature)
                        };
                    });
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateWaterDomain() {
                var WaterDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("4c21ad24f55f64d4fb722f40720d9ab0");
                var tricksterDomain = CreateTricksterDomain(WaterDomainProgression);

                tricksterDomain.LevelEntries
                    .Where(entry => entry.Level == 1)
                    .ForEach(entry => {
                        entry.m_Features = new List<BlueprintFeatureBaseReference>() {
                            CreateTricksterDomainAbilityFeature(WaterDomainBaseFeature)
                        };
                    });
                TricksterDomains.Add(tricksterDomain);
            }
            void CreateWeatherDomain() {
                var WeatherDomainBaseFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("1c37869ee06ca33459f16f23f4969e7d");
                var WeatherDomainGreaterFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("8e44306af595c8d44aad2f1260fd7be2");
                var tricksterDomain = CreateTricksterDomain(WeatherDomainProgression);

                tricksterDomain.LevelEntries
                    .Where(entry => entry.Level == 1)
                    .ForEach(entry => {
                        entry.m_Features = new List<BlueprintFeatureBaseReference>() {
                            CreateTricksterDomainAbilityFeature(WeatherDomainBaseFeature)
                        };
                    });
                tricksterDomain.LevelEntries
                    .Where(entry => entry.Level == 4)
                    .ForEach(entry => {
                        entry.m_Features = new List<BlueprintFeatureBaseReference>() {
                            CreateTricksterDomainAbilityFeature(WeatherDomainGreaterFeature)
                        };
                    });
                BlueprintTools.GetModBlueprint<BlueprintAbilityResource>(TTTContext, "TricksterTTTWeatherDomainGreaterResource").m_MaxAmount = new BlueprintAbilityResource.Amount() {
                    m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterMythics,
                    m_ClassDiv = new BlueprintCharacterClassReference[0],
                    m_Archetypes = new BlueprintArchetypeReference[0],
                    m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                    BaseValue = 0,
                    LevelIncrease = 2,
                    IncreasedByLevel = true,
                    IncreasedByStat = false
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
                    ResourcesLibrary.GetRoot()
                        .Progression
                        .CharacterMythics
                        .ForEach(mythic => bp.AddClass(mythic));
                    bp.LevelEntries.ForEach(entry => {
                        if (entry.Level > 1) {
                            entry.Level /= 2;
                        }
                    });
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
                            m_CustomProperty = TricksterDomainRankProperty.ToReference<BlueprintUnitPropertyReference>()
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
            static BlueprintFeatureBaseReference CreateTricksterDomainAbilityFeature(BlueprintFeature feature, bool updateBuffs = false) {
                return feature.CreateCopy(TTTContext, $"TricksterTTT{feature.name}", TricksterDomainMasterID, bp => {
                    var baseAbilities = bp.GetComponent<AddFacts>()?.Facts.OfType<BlueprintAbility>().ToArray() 
                        ?? bp.GetComponent<FactSinglify>()?.NewFacts.OfType<BlueprintAbility>().ToArray();
                    var baseResource = bp.GetComponent<AddAbilityResources>()?.Resource;
                    var tricksterResource = CreateTricksterDomainAbilityResource(baseResource);
                    var tricksterAbilities = baseAbilities.Select(ability => CreateTricksterDomainAbility(ability, tricksterResource, updateBuffs)).ToArray();

                    bp.RemoveComponents<ReplaceAbilitiesStat>();
                    if (baseResource != null) {
                        bp.GetComponent<AddAbilityResources>().m_Resource = tricksterResource;
                    } else { TTTContext.Logger.Log($"Trickster Domain Feature has no resource: {feature.name}"); }
                    if (baseAbilities != null || !baseAbilities.Any()) {
                        bp.RemoveComponents<AddFacts>(c => c.Facts.Any(ability => baseAbilities.Contains(ability)));
                        bp.RemoveComponents<FactSinglify>(c => c.NewFacts.Any(ability => baseAbilities.Contains(ability)));
                        bp.AddComponent<AddFacts>(c => {
                            c.m_Facts = tricksterAbilities;
                        });
                    } else { TTTContext.Logger.Log($"Trickster Domain Feature has no ability: {feature.name}"); }

                }).ToReference<BlueprintFeatureBaseReference>();
            }
            static BlueprintFeatureBaseReference CreateTricksterDomainGreaterAbilityFeature(BlueprintFeature feature, bool updateBuffs = false) {
                return feature.CreateCopy(TTTContext, $"TricksterTTT{feature.name}", TricksterDomainMasterID, bp => {
                    var baseAbilities = bp.GetComponent<AddFacts>()?.Facts.OfType<BlueprintAbility>().ToArray()
                        ?? bp.GetComponent<FactSinglify>()?.NewFacts.OfType<BlueprintAbility>().ToArray();
                    var baseResource = bp.GetComponent<AddAbilityResources>()?.Resource;
                    var tricksterResource = CreateTricksterDomainGreaterAbilityResource(baseResource);
                    var tricksterAbilities = baseAbilities.Select(ability => CreateTricksterDomainAbility(ability, tricksterResource, updateBuffs)).ToArray();

                    bp.RemoveComponents<ReplaceAbilitiesStat>();
                    if (baseResource != null) {
                        bp.GetComponent<AddAbilityResources>().m_Resource = tricksterResource;
                    } else { TTTContext.Logger.Log($"Trickster Domain Feature has no resource: {feature.name}"); }
                    if (baseAbilities != null || !baseAbilities.Any()) {
                        bp.RemoveComponents<AddFacts>(c => c.Facts.Any(ability => baseAbilities.Contains(ability)));
                        bp.RemoveComponents<FactSinglify>(c => c.NewFacts.Any(ability => baseAbilities.Contains(ability)));
                        bp.AddComponent<AddFacts>(c => {
                            c.m_Facts = tricksterAbilities;
                        });
                    } else { TTTContext.Logger.Log($"Trickster Domain Feature has no ability: {feature.name}"); }

                }).ToReference<BlueprintFeatureBaseReference>();
            }
            static BlueprintUnitFactReference CreateTricksterDomainAbility(BlueprintAbility ability, BlueprintAbilityResourceReference resource, bool updateBuffs) {
                if (ability == null) { return null; }
                return ability.CreateCopy(TTTContext, $"TricksterTTT{ability.name}", TricksterDomainMasterID, bp => {
                    bp.GetComponent<AbilityResourceLogic>().m_RequiredResource = resource;
                    ConvertContextRankConfigs(bp);
                    AddToDomainZealot(bp);
                    bp.AddComponent<ContextSetAbilityParams>(c => {
                        c.DC = new ContextValue() {
                            ValueType = ContextValueType.CasterCustomProperty,
                            m_CustomProperty = TricksterDomainDCProperty.ToReference<BlueprintUnitPropertyReference>()
                        };
                        c.CasterLevel = new ContextValue() {
                            ValueType = ContextValueType.CasterCustomProperty,
                            m_CustomProperty = TricksterDomainRankProperty.ToReference<BlueprintUnitPropertyReference>()
                        };
                        c.Concentration = new ContextValue() {
                            ValueType = ContextValueType.CasterCustomProperty,
                            m_CustomProperty = TricksterDomainRankProperty.ToReference<BlueprintUnitPropertyReference>()
                        };
                        c.SpellLevel = -1;
                    });
                    if (updateBuffs) {
                        var applyBuffs = bp.FlattenAllActions().OfType<ContextActionApplyBuff>().ToArray();
                        foreach (var action in applyBuffs) {
                            var buff = action.Buff;
                            var tricksterBuff = buff.CreateCopy(TTTContext, $"TricksterTTT{buff.name}", TricksterDomainMasterID, tb => {
                                ConvertContextRankConfigs(tb);
                            });
                            action.m_Buff = tricksterBuff.ToReference<BlueprintBuffReference>();
                        }
                    }
                }).ToReference<BlueprintUnitFactReference>();
            }
            static BlueprintAbilityResourceReference CreateTricksterDomainAbilityResource(BlueprintAbilityResource resource) {
                if (resource == null) { return null; }
                return resource.CreateCopy(TTTContext, $"TricksterTTT{resource.name}", TricksterDomainMasterID, bp => {
                    if (resource.m_MaxAmount.IncreasedByStat && resource.m_MaxAmount.ResourceBonusStat == StatType.Wisdom) {
                        bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                            m_Class = ResourcesLibrary.GetRoot().Progression.m_CharacterMythics,
                            m_ClassDiv = new BlueprintCharacterClassReference[0],
                            m_Archetypes = new BlueprintArchetypeReference[0],
                            m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                            BaseValue = resource.m_MaxAmount.BaseValue,
                            LevelIncrease = 1,
                            IncreasedByLevel = true,
                            IncreasedByStat = false
                        };
                        bp.m_Max = 13;
                        bp.m_UseMax = true;
                    }      
                }).ToReference<BlueprintAbilityResourceReference>();
            }
            static BlueprintAbilityResourceReference CreateTricksterDomainGreaterAbilityResource(BlueprintAbilityResource resource) {
                if (resource == null) { return null; }
                return resource.CreateCopy(TTTContext, $"TricksterTTT{resource.name}", TricksterDomainMasterID, bp => {
                    bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                        m_Class = new BlueprintCharacterClassReference[0],
                        m_ClassDiv = ResourcesLibrary.GetRoot().Progression.m_CharacterMythics,
                        m_Archetypes = new BlueprintArchetypeReference[0],
                        m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                        BaseValue = resource.m_MaxAmount.BaseValue,
                        StartingLevel = 4,
                        StartingIncrease = 1,
                        LevelStep = 2,
                        PerStepIncrease = 1,
                        IncreasedByLevelStartPlusDivStep = true,
                        IncreasedByStat = false
                    };
                }).ToReference<BlueprintAbilityResourceReference>();
            }
            static void ConvertContextRankConfigs(BlueprintScriptableObject blueprint) {
                blueprint.GetComponents<ContextRankConfig>()
                    .Where(c => { return c.m_BaseValueType == ContextRankBaseValueType.SummClassLevelWithArchetype;})
                    .ForEach(c => {
                        c.m_BaseValueType = ContextRankBaseValueType.CustomProperty;
                        c.m_CustomProperty = TricksterDomainRankProperty.ToReference<BlueprintUnitPropertyReference>();
                        c.m_FeatureList = new BlueprintFeatureReference[0];
                    });
            }
            static void AddToDomainZealot(BlueprintAbility ability) {
                var AutoMetamagic = DomainMastery.GetComponent<AutoMetamagic>();
                AutoMetamagic.Abilities.Add(ability.ToReference<BlueprintAbilityReference>());
            }
        }

        public static void AddTricksterTricks() {
            var Icon_TricksterPersausion = AssetLoader.LoadInternal(TTTContext, "TricksterTricks", "Icon_TricksterPersausion.png");

            var SongOfDiscordBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("2e1646c2449c88a4188e58043455a43a");
            var TricksterPerceptionTier1Feature = BlueprintTools.GetBlueprintReference<BlueprintUnitFactReference>("8bc2f9b88a0cf704ea72d86c2a3e2aef");
            var TricksterPerceptionTier2Feature = BlueprintTools.GetBlueprintReference<BlueprintUnitFactReference>("e9298851786c5334dba1398e9635a83d");

            var TricksterPerception3EffectBuff = Helpers.CreateBuff(TTTContext, "TricksterPerception3EffectBuff", bp => {
                bp.m_Flags = BlueprintBuff.Flags.HiddenInUi;
                bp.SetName(TTTContext, "Aeon Bane Increase Resource Feature");
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
                            condition.IfFalse = Helpers.CreateActionList(
                                Helpers.Create<ContextActionRemoveBuff>(a => {
                                    a.m_Buff = TricksterPerception3EffectBuff.ToReference<BlueprintBuffReference>();
                                })
                            );
                            condition.IfTrue = Helpers.CreateActionList();
                        }));
                    c.UnitMove = Helpers.CreateActionList();
                    c.Round = Helpers.CreateActionList();
                });
            });
            var TricksterPerception3Buff = Helpers.CreateBuff(TTTContext, "TricksterPerception3Buff", bp => {
                bp.m_Flags = BlueprintBuff.Flags.HiddenInUi;
                bp.SetName(TTTContext, "Aeon Bane Increase Resource Feature");
                bp.SetDescription(TTTContext, "");
                bp.IsClassFeature = true;
                bp.AddComponent<AddAreaEffect>(c => {
                    c.m_AreaEffect = TricksterPerception3Area.ToReference<BlueprintAbilityAreaEffectReference>();
                });
            });
            var TricksterPersuasion2Buff = Helpers.CreateBuff(TTTContext, "TricksterPersuasion2Buff", bp => {
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
                    c.m_Progression = ContextRankProgression.OnePlusDiv2;
                });
            });
            var TricksterPersuasion3Buff = SongOfDiscordBuff.CreateCopy(TTTContext, "TricksterPersuasion3Buff", bp => {
                bp.SetName(TTTContext, "Trickster Persuasion 3");
                bp.SetDescription(TTTContext, "Creature has a 50% chance to attack the nearest target each turn " +
                    "and suffers pentalities to its AC and Saves equal to 1 + half your mythic rank.");
                bp.m_Icon = Icon_TricksterPersausion;
                bp.Stacking = StackingType.Prolong;
                bp.IsClassFeature = true;
                bp.RemoveComponents<SpellDescriptorComponent>();
                bp.AddComponent<AddContextStatBonus>(c => {
                    c.Stat = StatType.AC;
                    c.Value = new ContextValue() {
                        ValueType = ContextValueType.Rank
                    };
                    c.Descriptor = ModifierDescriptor.UntypedStackable;
                    c.Multiplier = -1;
                });
                bp.AddComponent<AddContextStatBonus>(c => {
                    c.Stat = StatType.SaveFortitude;
                    c.Value = new ContextValue() {
                        ValueType = ContextValueType.Rank
                    };
                    c.Descriptor = ModifierDescriptor.UntypedStackable;
                    c.Multiplier = -1;
                });
                bp.AddComponent<AddContextStatBonus>(c => {
                    c.Stat = StatType.SaveReflex;
                    c.Value = new ContextValue() {
                        ValueType = ContextValueType.Rank
                    };
                    c.Descriptor = ModifierDescriptor.UntypedStackable;
                    c.Multiplier = -1;
                });
                bp.AddComponent<AddContextStatBonus>(c => {
                    c.Stat = StatType.SaveWill;
                    c.Value = new ContextValue() {
                        ValueType = ContextValueType.Rank
                    };
                    c.Descriptor = ModifierDescriptor.UntypedStackable;
                    c.Multiplier = -1;
                });
                bp.AddContextRankConfig(c => {
                    c.m_BaseValueType = ContextRankBaseValueType.MythicLevel;
                    c.m_Progression = ContextRankProgression.OnePlusDiv2;
                });
            });
        }
    }
}
