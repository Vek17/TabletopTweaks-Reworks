using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.ResourceLinks;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.UnitLogic.Mechanics.Properties;
using TabletopTweaks.Core.NewComponents;
using TabletopTweaks.Core.NewComponents.OwlcatReplacements;
using TabletopTweaks.Core.NewComponents.Properties;
using TabletopTweaks.Core.Utilities;
using static TabletopTweaks.Reworks.Main;

namespace TabletopTweaks.Reworks.NewContent.Classes {
    internal class Azata {
        public static void AddAzataFeatures() {
            var NaturalArmor12 = BlueprintTools.GetBlueprintReference<BlueprintUnitFactReference>("0b2d92c6aac8093489dfdadf1e448280");
            var TailType = BlueprintTools.GetBlueprintReference<BlueprintWeaponTypeReference>("4ce435468ebd4364997da8cbd0c66133");
            var HeroismBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("87ab2fed7feaaff47b62a3320a57ad8d");
            var ZippyMagicFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("30b4200f897ba25419ba3a292aed4053");
            var DragonAzataArchetype = BlueprintTools.GetBlueprintReference<BlueprintArchetypeReference>("6e6135c91c2f84e46b7bb49f2158a9ce");
            var DragonClass = BlueprintTools.GetBlueprintReference<BlueprintCharacterClassReference>("01a754e7c1b7c5946ba895a5ff0faffc");

            var DragonAzataWeaponOverrides = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "DragonAzataWeaponOverrides", bp => {
                bp.SetName(TTTContext, "Dragon Weapon Overrides");
                bp.SetDescription(TTTContext, "");
                bp.ReapplyOnLevelUp = true;
                bp.IsClassFeature = true;
                bp.HideInUI = true;
                bp.AddComponent<BonusStatDamageMultiplerReplacement>(c => {
                    c.CheckWeaponCategory = true;
                    c.WeaponCategory = WeaponCategory.Bite;
                    c.Multiplier = 1.5f;
                });
                bp.AddComponent<BonusStatDamageMultiplerReplacement>(c => {
                    c.CheckWeaponCategory = true;
                    c.WeaponCategory = WeaponCategory.Claw;
                    c.Multiplier = 1.0f;
                });
                bp.AddComponent<BonusStatDamageMultiplerReplacement>(c => {
                    c.CheckWeaponCategory = true;
                    c.WeaponCategory = WeaponCategory.Wing;
                    c.Multiplier = 0.5f;
                });
                bp.AddComponent<BonusStatDamageMultiplerReplacement>(c => {
                    c.CheckWeaponCategory = true;
                    c.WeaponCategory = WeaponCategory.Tail;
                    c.Multiplier = 1.5f;
                });
                bp.AddComponent<SecondaryWeaponOverride>(c => {
                    c.CheckWeaponCategory = true;
                    c.WeaponCategory = WeaponCategory.Tail;
                    c.IsSecondary = false;
                });
            });
            var DragonAzataStatGrowth = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "DragonAzataStatGrowth", bp => {
                bp.SetName(TTTContext, "Mythic Draconic Growth");
                bp.SetDescription(TTTContext, "Unlike normal animals of its kind, Aivu's Hit Dice, abilities, " +
                    "skills, and feats advance as the Azata gains mythic ranks.");
                bp.ReapplyOnLevelUp = true;
                bp.IsClassFeature = true;
                bp.AddComponent<RemoveFeatureOnApply>(c => {
                    c.m_Feature = NaturalArmor12;
                });
                bp.AddComponent<AddContextStatBonus>(c => {
                    c.Stat = StatType.AC;
                    c.Descriptor = ModifierDescriptor.NaturalArmor;
                    c.Value = new ContextValue() {
                        ValueType = ContextValueType.Rank,
                        ValueRank = AbilityRankType.DamageBonus
                    };
                });
                bp.AddComponent<AddContextStatBonus>(c => {
                    c.Stat = StatType.Strength;
                    c.Descriptor = ModifierDescriptor.Racial;
                    c.Value = new ContextValue() {
                        ValueType = ContextValueType.Rank,
                        ValueRank = AbilityRankType.DamageDice
                    };
                });
                bp.AddComponent<AddContextStatBonus>(c => {
                    c.Stat = StatType.Dexterity;
                    c.Descriptor = ModifierDescriptor.Racial;
                    c.Value = new ContextValue() {
                        ValueType = ContextValueType.Rank,
                        ValueRank = AbilityRankType.DamageDiceAlternative
                    };
                });
                bp.AddComponent<AddContextStatBonus>(c => {
                    c.Stat = StatType.Constitution;
                    c.Descriptor = ModifierDescriptor.Racial;
                    c.Value = new ContextValue() {
                        ValueType = ContextValueType.Rank,
                        ValueRank = AbilityRankType.Default
                    };
                });
                bp.AddComponent<AddContextStatBonus>(c => {
                    c.Stat = StatType.Intelligence;
                    c.Descriptor = ModifierDescriptor.Racial;
                    c.Value = new ContextValue() {
                        ValueType = ContextValueType.Rank,
                        ValueRank = AbilityRankType.ProjectilesCount
                    };
                });
                bp.AddComponent<AddContextStatBonus>(c => {
                    c.Stat = StatType.Wisdom;
                    c.Descriptor = ModifierDescriptor.Racial;
                    c.Value = new ContextValue() {
                        ValueType = ContextValueType.Rank,
                        ValueRank = AbilityRankType.SpeedBonus
                    };
                });
                bp.AddComponent<AddContextStatBonus>(c => {
                    c.Stat = StatType.Charisma;
                    c.Descriptor = ModifierDescriptor.Racial;
                    c.Value = new ContextValue() {
                        ValueType = ContextValueType.Rank,
                        ValueRank = AbilityRankType.StatBonus
                    };
                });
                //AbilityRankType.DamageBonus = Natural Armor
                bp.AddContextRankConfig(c => {
                    c.m_Type = AbilityRankType.DamageBonus;
                    c.m_BaseValueType = ContextRankBaseValueType.MasterMythicLevel;
                    c.m_Progression = ContextRankProgression.Custom;
                    c.m_CustomProgression = new ContextRankConfig.CustomProgressionItem[] {
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 1,
                            ProgressionValue = 12
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 2,
                            ProgressionValue = 12
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 3,
                            ProgressionValue = 12
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 4,
                            ProgressionValue = 18
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 5,
                            ProgressionValue = 20
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 6,
                            ProgressionValue = 26
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 7,
                            ProgressionValue = 28
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 8,
                            ProgressionValue = 31
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 9,
                            ProgressionValue = 34
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 10,
                            ProgressionValue = 37
                        }
                    };
                });
                //AbilityRankType.DamageDice = Strength
                bp.AddContextRankConfig(c => {
                    c.m_Type = AbilityRankType.DamageDice;
                    c.m_BaseValueType = ContextRankBaseValueType.MasterMythicLevel;
                    c.m_Progression = ContextRankProgression.Custom;
                    c.m_CustomProgression = new ContextRankConfig.CustomProgressionItem[] {
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 1,
                            ProgressionValue = 0
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 2,
                            ProgressionValue = 0
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 3,
                            ProgressionValue = 0
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 4,
                            ProgressionValue = 4
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 5,
                            ProgressionValue = 6
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 6,
                            ProgressionValue = 10
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 7,
                            ProgressionValue = 12
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 8,
                            ProgressionValue = 14
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 9,
                            ProgressionValue = 16
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 10,
                            ProgressionValue = 18
                        }
                    };
                });
                //AbilityRankType.DamageDiceAlternative = Dexterity
                bp.AddContextRankConfig(c => {
                    c.m_Type = AbilityRankType.DamageDiceAlternative;
                    c.m_BaseValueType = ContextRankBaseValueType.MasterMythicLevel;
                    c.m_Progression = ContextRankProgression.Custom;
                    c.m_CustomProgression = new ContextRankConfig.CustomProgressionItem[] {
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 1,
                            ProgressionValue = 0
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 2,
                            ProgressionValue = 0
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 3,
                            ProgressionValue = 0
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 4,
                            ProgressionValue = -2
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 5,
                            ProgressionValue = -2
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 6,
                            ProgressionValue = -4
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 7,
                            ProgressionValue = -4
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 8,
                            ProgressionValue = -4
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 9,
                            ProgressionValue = -6
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 10,
                            ProgressionValue = -6
                        }
                    };
                });
                //AbilityRankType.Default = Constitution
                bp.AddContextRankConfig(c => {
                    c.m_Type = AbilityRankType.Default;
                    c.m_BaseValueType = ContextRankBaseValueType.MasterMythicLevel;
                    c.m_Progression = ContextRankProgression.Custom;
                    c.m_CustomProgression = new ContextRankConfig.CustomProgressionItem[] {
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 1,
                            ProgressionValue = 0
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 2,
                            ProgressionValue = 0
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 3,
                            ProgressionValue = 0
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 4,
                            ProgressionValue = 2
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 5,
                            ProgressionValue = 4
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 6,
                            ProgressionValue = 6
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 7,
                            ProgressionValue = 6
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 8,
                            ProgressionValue = 8
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 9,
                            ProgressionValue = 8
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 10,
                            ProgressionValue = 10
                        }
                    };
                });
                //AbilityRankType.ProjectilesCount = Intelligence
                bp.AddContextRankConfig(c => {
                    c.m_Type = AbilityRankType.ProjectilesCount;
                    c.m_BaseValueType = ContextRankBaseValueType.MasterMythicLevel;
                    c.m_Progression = ContextRankProgression.Custom;
                    c.m_CustomProgression = new ContextRankConfig.CustomProgressionItem[] {
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 1,
                            ProgressionValue = 0
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 2,
                            ProgressionValue = 0
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 3,
                            ProgressionValue = 0
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 4,
                            ProgressionValue = 2
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 5,
                            ProgressionValue = 4
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 6,
                            ProgressionValue = 6
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 7,
                            ProgressionValue = 6
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 8,
                            ProgressionValue = 8
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 9,
                            ProgressionValue = 8
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 10,
                            ProgressionValue = 10
                        }
                    };
                });
                //AbilityRankType.SpeedBonus = Wisdom
                bp.AddContextRankConfig(c => {
                    c.m_Type = AbilityRankType.SpeedBonus;
                    c.m_BaseValueType = ContextRankBaseValueType.MasterMythicLevel;
                    c.m_Progression = ContextRankProgression.Custom;
                    c.m_CustomProgression = new ContextRankConfig.CustomProgressionItem[] {
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 1,
                            ProgressionValue = 0
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 2,
                            ProgressionValue = 0
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 3,
                            ProgressionValue = 0
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 4,
                            ProgressionValue = 2
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 5,
                            ProgressionValue = 4
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 6,
                            ProgressionValue = 6
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 7,
                            ProgressionValue = 6
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 8,
                            ProgressionValue = 8
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 9,
                            ProgressionValue = 8
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 10,
                            ProgressionValue = 10
                        }
                    };
                });
                //AbilityRankType.StatBonus = Charisma
                bp.AddContextRankConfig(c => {
                    c.m_Type = AbilityRankType.StatBonus;
                    c.m_BaseValueType = ContextRankBaseValueType.MasterMythicLevel;
                    c.m_Progression = ContextRankProgression.Custom;
                    c.m_CustomProgression = new ContextRankConfig.CustomProgressionItem[] {
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 1,
                            ProgressionValue = 0
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 2,
                            ProgressionValue = 0
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 3,
                            ProgressionValue = 0
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 4,
                            ProgressionValue = 1
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 5,
                            ProgressionValue = 1
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 6,
                            ProgressionValue = 2
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 7,
                            ProgressionValue = 1
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 8,
                            ProgressionValue = 3
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 9,
                            ProgressionValue = 2
                        },
                        new ContextRankConfig.CustomProgressionItem() {
                            BaseValue = 10,
                            ProgressionValue = 4
                        }
                    };
                });
            });
            var DragonAzataTailSweep = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "DragonAzataTailSweep", bp => {
                bp.SetName(TTTContext, "Tail Sweep");
                bp.SetDescription(TTTContext, "When you make an attack with your tail, you can make a trip attempt as a free action that does not provoke attacks of opportunity.");
                bp.IsClassFeature = true;
                bp.ReapplyOnLevelUp = true;
                bp.AddComponent<ManeuverOnAttack>(c => {
                    c.Maneuver = CombatManeuver.Trip;
                    c.Category = WeaponCategory.Tail;
                });
            });
            var DragonAzataDeadlyTail = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "DragonAzataDeadlyTail", bp => {
                bp.SetName(TTTContext, "Deadly Tail");
                bp.SetDescription(TTTContext, "The dragon’s tail slap critical multiplier becomes ×3.");
                bp.IsClassFeature = true;
                bp.ReapplyOnLevelUp = true;
                bp.AddComponent<WeaponTypeCriticalMultiplierIncrease>(c => {
                    c.m_WeaponType = TailType;
                    c.AdditionalMultiplier = 1;
                });
            });
            var DragonAzataDeliriumBreath = Helpers.CreateBlueprint<BlueprintFeature>(TTTContext, "DragonAzataDeliriumBreath", bp => {
                bp.SetName(TTTContext, "Delirium Breath");
                bp.SetDescription(TTTContext, "A creature that fails its save against the dragon’s breath weapon becomes confused for 1 round.");
                bp.IsClassFeature = true;
                bp.ReapplyOnLevelUp = true;
            });
            var DragonAzataHeroismEffect = HeroismBuff.CreateCopy(TTTContext, "DragonAzataHeroismEffect", bp => {
                bp.FxOnStart = new PrefabLink();
            });

            var ZippyMagicBuff = Helpers.CreateBlueprint<BlueprintBuff>(TTTContext, "ZippyMagicBuff", bp => {
                bp.SetName(TTTContext, "Zippy Magic");
                bp.SetDescription(TTTContext, "Whenever Azata casts a spell, spell-like ability, or supernatural ability that targets individual creatures, " +
                        "not areas, it affects one additional creature within a 30-foot range from the target. " +
                        "If the target is an enemy, the spell affects one additional enemy; " +
                        "if the target is an ally, the spell affects one additional ally. " +
                        "Additionally, spells that target enemies damage them for 2d6 + mythic rank.");
                bp.m_Icon = ZippyMagicFeature.Icon;
                bp.AddComponent<AzataZippyMagicTTT>(c => {
                    c.CheckAbilityType = true;
                    c.Types = new AbilityType[] {
                            AbilityType.Spell,
                            AbilityType.SpellLike,
                            AbilityType.Supernatural
                        };
                    c.AllowDescriptorOverride = true;
                    c.Descriptors = SpellDescriptor.Hex;
                    c.ZippyDamageDice = new DiceFormula(2, DiceType.D6);
                    c.ZippyDamageBonus = new ContextValue() {
                        ValueType = ContextValueType.CasterProperty,
                        Property = UnitProperty.MythicLevel
                    };
                });
            });
            var ZippyMagicToggleAbility = Helpers.CreateBlueprint<BlueprintActivatableAbility>(TTTContext, "ZippyMagicToggleAbility", bp => {
                bp.SetName(TTTContext, "Zippy Magic");
                bp.SetDescription(ZippyMagicBuff.m_Description);
                bp.m_Icon = ZippyMagicFeature.Icon;
                bp.m_Buff = ZippyMagicBuff.ToReference<BlueprintBuffReference>();
                bp.IsOnByDefault = true;
                bp.DoNotTurnOffOnRest = true;
                bp.DeactivateImmediately = true;
            });

            var DragonAzataBreathWeaponDCProperty = Helpers.CreateBlueprint<BlueprintUnitProperty>(TTTContext, "DragonAzataBreathWeaponDCProperty", bp => {
                bp.BaseValue = 10;
                bp.AddComponent<CompositeCustomPropertyGetter>(c => {
                    c.CalculationMode = CompositeCustomPropertyGetter.Mode.Sum;
                    c.Properties = new CompositeCustomPropertyGetter.ComplexCustomProperty[] {
                        new CompositeCustomPropertyGetter.ComplexCustomProperty() {
                            Property = new SimplePropertyGetter() {
                                Property = UnitProperty.Level,
                                Settings = new PropertySettings() {
                                    m_Progression = PropertySettings.Progression.Div2
                                }
                            }
                        },
                        new CompositeCustomPropertyGetter.ComplexCustomProperty() {
                            Property = new SimplePropertyGetter() {
                                Property = UnitProperty.StatBonusConstitution,
                                Settings = new PropertySettings() {
                                    m_Progression = PropertySettings.Progression.AsIs
                                }
                            }
                        },
                        new CompositeCustomPropertyGetter.ComplexCustomProperty() {
                            Property = new MasterSimplePropertyGetter() {
                                Property = UnitProperty.MythicLevel,
                                Settings = new PropertySettings() {
                                    m_Progression = PropertySettings.Progression.AsIs
                                }
                            }
                        }
                    };
                });
            });

            var DragonAzataBreathWeaponCasterlevelProperty = Helpers.CreateBlueprint<BlueprintUnitProperty>(TTTContext, "DragonAzataBreathWeaponCasterlevelProperty", bp => {
                bp.AddComponent<SimplePropertyGetter>(c => {
                    c.Property = UnitProperty.Level;
                });
            });
        }
    }
}
