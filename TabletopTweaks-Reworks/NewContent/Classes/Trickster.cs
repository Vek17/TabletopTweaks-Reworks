using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.ElementsSystem;
using Kingmaker.ResourceLinks;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components.AreaEffects;
using Kingmaker.UnitLogic.Buffs.Components;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using Kingmaker.Utility;
using System.Collections.Generic;
using System.Linq;
using TabletopTweaks.Core.NewComponents;
using TabletopTweaks.Core.NewComponents.OwlcatReplacements;
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

        public static void AddTricksterDomains() {
            var DomainsSelection = BlueprintTools.GetBlueprint<BlueprintFeatureSelection>("48525e5da45c9c243a343fc6545dbdb9");
            DomainsSelection.AllFeatures
                .OfType<BlueprintProgression>()
                .ForEach(domain => GenerateTricksterDomain(TricksterDomainMasterID, domain));
            static BlueprintProgression GenerateTricksterDomain(BlueprintGuid masterID, BlueprintProgression domain) {
                return domain.CreateCopy(TTTContext, $"TricksterTTT{domain.name}", masterID, bp => {
                    var SpellList = bp.GetComponent<LearnSpellList>()?.m_SpellList;
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
                    bp.RemoveComponents<LearnSpellList>();
                    bp.RemoveComponents<Prerequisite>();
                    bp.AddComponent<AddSpellListAsAbilitiesTTT>(c => {
                        c.m_SpellList = SpellList;
                        c.m_ResourcePerSpellLevel = new BlueprintAbilityResourceReference[] {
                        CreateTricksterSpellResource(1, SpellList),
                        CreateTricksterSpellResource(2, SpellList),
                        CreateTricksterSpellResource(3, SpellList),
                        CreateTricksterSpellResource(4, SpellList),
                        CreateTricksterSpellResource(5, SpellList),
                        CreateTricksterSpellResource(6, SpellList),
                        CreateTricksterSpellResource(7, SpellList),
                        CreateTricksterSpellResource(8, SpellList),
                        CreateTricksterSpellResource(9, SpellList),
                    };
                    });
                    bp.AddComponent<RecalculateOnLevelUp>();
                });
            }
            static BlueprintAbilityResourceReference CreateTricksterSpellResource(int spellLevel, BlueprintSpellList spellList) {
                return Helpers.CreateDerivedBlueprint<BlueprintAbilityResource>(
                    modContext: TTTContext,
                    $"TricksterTTT{spellList.name}Resource{spellLevel}",
                    TricksterSpellResource[spellLevel - 1],
                    new SimpleBlueprint[] { spellList },
                    bp => {
                        bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                            m_Class = new BlueprintCharacterClassReference[0],
                            m_ClassDiv = new BlueprintCharacterClassReference[0],
                            m_Archetypes = new BlueprintArchetypeReference[0],
                            m_ArchetypesDiv = new BlueprintArchetypeReference[0],
                            BaseValue = 1,
                            IncreasedByLevel = false,
                            IncreasedByStat = false
                        };
                    }
                ).ToReference<BlueprintAbilityResourceReference>();
            }
        }   

        public static void AddTricksterTricks() {
            var TricksterPerceptionTier1Feature = BlueprintTools.GetBlueprintReference<BlueprintUnitFactReference>("8bc2f9b88a0cf704ea72d86c2a3e2aef");
            var TricksterPerceptionTier2Feature = BlueprintTools.GetBlueprintReference<BlueprintUnitFactReference>("e9298851786c5334dba1398e9635a83d");

            var TricksterPerception3EffectBuff = Helpers.CreateBuff(TTTContext, "TricksterPerception3EffectBuff", bp => {
                bp.m_Flags = Kingmaker.UnitLogic.Buffs.Blueprints.BlueprintBuff.Flags.HiddenInUi;
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
                bp.m_Flags = Kingmaker.UnitLogic.Buffs.Blueprints.BlueprintBuff.Flags.HiddenInUi;
                bp.SetName(TTTContext, "Aeon Bane Increase Resource Feature");
                bp.SetDescription(TTTContext, "");
                bp.IsClassFeature = true;
                bp.AddComponent<AddAreaEffect>(c => {
                    c.m_AreaEffect = TricksterPerception3Area.ToReference<BlueprintAbilityAreaEffectReference>();
                });
            });
        }
    }
}
