using HarmonyLib;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.Enums.Damage;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Buffs.Components;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using Kingmaker.UnitLogic.Parts;
using Kingmaker.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TabletopTweaks.Core.MechanicsChanges;
using TabletopTweaks.Core.NewComponents;
using TabletopTweaks.Core.NewComponents.AbilitySpecific;
using TabletopTweaks.Core.Utilities;
using static TabletopTweaks.Reworks.Main;

namespace TabletopTweaks.Reworks.Reworks {
    static class Aeon {
        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        static class BlueprintsCache_Init_Patch {
            static bool Initialized;

            static void Postfix() {
                if (Initialized) return;
                Initialized = true;
                TTTContext.Logger.LogHeader("Aeon Rework");

                PatchAeonBaneActions();
                PatchAeonBaneDamage();
                PatchAeonBaneIcon();
                PatchAeonBaneBuffNames();
                PatchAeonBaneSpellResistance();
                PatchAeonBaneUses();
                PatchPatchAeonImprovedBaneDamage();
                PatchAeonImprovedBaneDispelLimit();
                PatchAeonGreaterBaneDamage();
                PatchAeonGreaterBaneDispel();
                PatchAeonGazeAction();
                PatchAeonGazeIcons();
            }

            static void PatchAeonBaneActions() {
                if (TTTContext.Homebrew.MythicReworks.Aeon.IsDisabled("AeonBaneAction")) { return; }

                var AeonBaneAbility = BlueprintTools.GetBlueprint<BlueprintActivatableAbility>("67fb31f553f2bb14bbfae0b1040169f1");
                AeonBaneAbility.m_ActivateWithUnitCommand = UnitCommand.CommandType.Free;
                TTTContext.Logger.LogPatch("Patched", AeonBaneAbility);
            }
            static void PatchAeonBaneDamage() {
                if (TTTContext.Homebrew.MythicReworks.Aeon.IsDisabled("AeonBaneDamage")) { return; }

                var AeonBaneBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("345160619fc2ddc44b8ad98c94dde448");
                var InquisitorBaneBuff = BlueprintTools.GetBlueprintReference<BlueprintUnitFactReference>("be190d2dd5433dd41a4aa00e1abc9a5b");
                var InquisitorBaneNormalFeatureAdd = BlueprintTools.GetBlueprintReference<BlueprintUnitFactReference>("7ddf7fbeecbe78342b83171d888028cf");

                AeonBaneBuff.TemporaryContext(bp => {
                    bp.FlattenAllActions()
                        .OfType<Conditional>()
                        .SelectMany(conditional => conditional.ConditionsChecker.Conditions)
                        .OfType<ContextConditionCasterHasFact>()
                        .Where(c => c.m_Fact == InquisitorBaneBuff)
                        .ForEach(c => c.m_Fact = InquisitorBaneNormalFeatureAdd);
                    bp.AddComponent<AddAdditionalWeaponDamage>(c => {
                        c.Value = new ContextDiceValue() {
                            DiceType = DiceType.D6,
                            DiceCountValue = 2,
                            BonusValue = 0
                        };
                        c.DamageType = new DamageTypeDescription() {
                            Type = DamageType.Force
                        };
                    });
                    bp.AddComponent<AddStatBonus>(c => {
                        c.Value = 1;
                        c.Stat = StatType.AdditionalAttackBonus;
                        c.Descriptor = ModifierDescriptor.UntypedStackable;
                    });
                    bp.AddComponent<AddStatBonus>(c => {
                        c.Value = 1;
                        c.Stat = StatType.AdditionalDamage;
                        c.Descriptor = ModifierDescriptor.UntypedStackable;
                    });
                });
                
                TTTContext.Logger.LogPatch("Patched", AeonBaneBuff);
            }
            static void PatchAeonBaneIcon() {
                if (TTTContext.Homebrew.MythicReworks.Aeon.IsDisabled("AeonBaneIcon")) { return; }

                var Icon_AeonBane = AssetLoader.LoadInternal(TTTContext, "Abilities", "Icon_AeonBane.png");
                var AeonBaneFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("0b25e8d8b0488c84c9b5714e9ca0a204");
                var AeonBaneAbility = BlueprintTools.GetBlueprint<BlueprintActivatableAbility>("67fb31f553f2bb14bbfae0b1040169f1");
                var AeonBaneBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("345160619fc2ddc44b8ad98c94dde448");
                AeonBaneFeature.m_Icon = Icon_AeonBane;
                AeonBaneAbility.m_Icon = Icon_AeonBane;
                AeonBaneBuff.m_Icon = Icon_AeonBane;
                TTTContext.Logger.LogPatch("Patched", AeonBaneFeature);
                TTTContext.Logger.LogPatch("Patched", AeonBaneAbility);
            }
            static void PatchAeonBaneBuffNames() {
                if (TTTContext.Homebrew.MythicReworks.Aeon.IsDisabled("AeonBaneBuffNames")) { return; }

                var AeonBaneFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("0b25e8d8b0488c84c9b5714e9ca0a204");
                var AeonBaneBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("345160619fc2ddc44b8ad98c94dde448");
                var AeonImprovedBaneFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("bbf227fe76f1eaa418a95e6095145ce8");
                var AeonImprovedBaneBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("e903d04113681fc4ba5603672b05540e");
                var AeonGreaterBaneFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("0a7482fd7b0479c44b51d7cd6c33e5c4");
                var AeonGreaterBaneBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("cdcc13884252b2c4d8dac57cb5f46555");

                AeonBaneBuff.m_DisplayName = AeonBaneFeature.m_DisplayName;
                AeonImprovedBaneBuff.m_DisplayName = AeonImprovedBaneFeature.m_DisplayName;
                AeonGreaterBaneBuff.m_DisplayName = AeonGreaterBaneFeature.m_DisplayName;

                TTTContext.Logger.LogPatch(AeonBaneBuff);
                TTTContext.Logger.LogPatch(AeonImprovedBaneBuff);
                TTTContext.Logger.LogPatch(AeonGreaterBaneBuff);
            }
            static void PatchAeonBaneSpellResistance() {
                if (TTTContext.Homebrew.MythicReworks.Aeon.IsDisabled("AeonBaneSpellResistance")) { return; }

                var AeonBaneBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("345160619fc2ddc44b8ad98c94dde448");
                AeonBaneBuff.RemoveComponents<ModifyD20>();
                AeonBaneBuff.AddComponent<SpellPenetrationBonus>(c => {
                    c.Value = new ContextValue() {
                        ValueType = ContextValueType.Rank
                    };
                    c.Descriptor = Kingmaker.Enums.ModifierDescriptor.UntypedStackable;
                });
                TTTContext.Logger.LogPatch("Patched", AeonBaneBuff);
            }
            static void PatchAeonBaneUses() {
                if (TTTContext.Homebrew.MythicReworks.Aeon.IsDisabled("AeonBaneUses")) { return; }
                var AeonClass = BlueprintTools.GetBlueprint<BlueprintCharacterClass>("15a85e67b7d69554cab9ed5830d0268e");
                var AeonBaneFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("0b25e8d8b0488c84c9b5714e9ca0a204");
                var AeonBaneIncreaseResourceFeature = BlueprintTools.GetModBlueprint<BlueprintFeature>(modContext: TTTContext, "AeonBaneIncreaseResourceFeature");
                AeonBaneFeature.AddComponent(Helpers.Create<AddFeatureOnApply>(c => {
                    c.m_Feature = AeonBaneIncreaseResourceFeature.ToReference<BlueprintFeatureReference>();
                }));
                AeonBaneFeature.SetDescription(TTTContext, "At 4th rank, Aeon gains an ability to make his weapons and spells especially deadly " +
                    "against his enemies. For a number of rounds per day equal to twice your mythic rank plus your character level, " +
                    "you can make any weapon you wield count as having Bane quality, your spells gain a bonus equal to your mythic rank " +
                    "to caster level checks to overcome spell resistance, and every hit you make with weapons or spells dispel effects " +
                    "from the target, as per the dispel magic spell.\nAdditionally, if you have inquisitor's bane ability, you gain the " +
                    "same bonuses while using inquisitor's bane ability.");
                TTTContext.Logger.LogPatch("Patched", AeonBaneFeature);
            }
            static void PatchPatchAeonImprovedBaneDamage() {
                if (TTTContext.Homebrew.MythicReworks.Aeon.IsDisabled("AeonImprovedBaneDamage")) { return; }
                var AeonImprovedBaneBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("e903d04113681fc4ba5603672b05540e");
                AeonImprovedBaneBuff.TemporaryContext(bp => {
                    bp.RemoveComponents<AddInitiatorAttackWithWeaponTrigger>(c => c.Action.Actions.OfType<ContextActionDealDamage>().Any());
                    bp.RemoveComponents<AdditionalDiceOnAttack>();
                    bp.RemoveComponents<AddStatBonus>();
                    bp.AddComponent<AddAdditionalWeaponDamage>(c => {
                        c.Value = new ContextDiceValue() {
                            DiceType = DiceType.D6,
                            DiceCountValue = 2,
                            BonusValue = 0
                        };
                        c.DamageType = new DamageTypeDescription() {
                            Type = DamageType.Force
                        };
                    });
                    bp.AddComponent<AddStatBonus>(c => {
                        c.Value = 1;
                        c.Stat = StatType.AdditionalAttackBonus;
                        c.Descriptor = ModifierDescriptor.UntypedStackable;
                    });
                    bp.AddComponent<AddStatBonus>(c => {
                        c.Value = 1;
                        c.Stat = StatType.AdditionalDamage;
                        c.Descriptor = ModifierDescriptor.UntypedStackable;
                    });
                });
                TTTContext.Logger.LogPatch("Patched", AeonImprovedBaneBuff);
            }
            static void PatchAeonImprovedBaneDispelLimit() {
                if (TTTContext.Homebrew.MythicReworks.Aeon.IsDisabled("AeonImprovedBaneDispelLimit")) { return; }

                var AeonBaneBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("345160619fc2ddc44b8ad98c94dde448");
                AeonBaneBuff.GetComponent<AddInitiatorAttackRollTrigger>()
                    .Action
                    .Actions
                    .OfType<Conditional>()
                    .ForEach(conditional => {
                        conditional.IfTrue.Actions
                            .OfType<ContextActionDispelMagic>()
                            .ForEach(a => {
                                a.OneRollForAll = true;
                            });
                    });
                AeonBaneBuff.GetComponent<AddInitiatorAttackRollTrigger>()
                    .Action
                    .Actions
                    .OfType<Conditional>()
                    .ForEach(conditional => {
                        conditional.IfFalse.Actions
                            .OfType<ContextActionDispelMagic>()
                            .ForEach(a => {
                                a.OneRollForAll = true;
                            });
                    });
                /*
                AeonBaneBuff.GetComponent<AddAbilityUseTrigger>()
                    .Action
                    .Actions
                    .OfType<Conditional>()
                    .ForEach(conditional => {
                        conditional.IfTrue.Actions
                            .OfType<ContextActionDispelMagic>()
                            .ForEach(a => {
                                a.m_StopAfterCountRemoved = true;
                                a.m_CountToRemove = new ContextValue() {
                                    ValueType = ContextValueType.Shared,
                                    ValueShared = AbilitySharedValue.StatBonus
                                };
                            });
                    });
                */
                AeonBaneBuff.AddComponent<ContextCalculateSharedValue>(c => {
                    c.ValueType = AbilitySharedValue.StatBonus;
                    c.Value = new ContextDiceValue() {
                        DiceType = DiceType.One,
                        DiceCountValue = new ContextValue() {
                            ValueType = ContextValueType.Rank
                        },
                        BonusValue = new ContextValue() {
                            ValueType = ContextValueType.Rank,
                            ValueRank = Kingmaker.Enums.AbilityRankType.DamageDice
                        },
                    };
                    c.Modifier = 0.25;
                });
                TTTContext.Logger.LogPatch("Patched", AeonBaneBuff);
            }
            static void PatchAeonGreaterBaneDamage() {
                if (TTTContext.Homebrew.MythicReworks.Aeon.IsDisabled("AeonGreaterBaneDamage")) { return; }
                var AeonGreaterBaneBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("cdcc13884252b2c4d8dac57cb5f46555");
                AeonGreaterBaneBuff.TemporaryContext(bp => {
                    bp.RemoveComponents<AddInitiatorAttackWithWeaponTrigger>(c => c.Action.Actions.OfType<ContextActionDealDamage>().Any());
                    bp.RemoveComponents<AdditionalDiceOnAttack>();
                    bp.RemoveComponents<AddStatBonus>();
                    bp.AddComponent<AddAdditionalWeaponDamage>(c => {
                        c.Value = new ContextDiceValue() {
                            DiceType = DiceType.D6,
                            DiceCountValue = 2,
                            BonusValue = 0
                        };
                        c.DamageType = new DamageTypeDescription() {
                            Type = DamageType.Force
                        };
                    });
                    bp.AddComponent<AddStatBonus>(c => {
                        c.Value = 1;
                        c.Stat = StatType.AdditionalAttackBonus;
                        c.Descriptor = ModifierDescriptor.UntypedStackable;
                    });
                    bp.AddComponent<AddStatBonus>(c => {
                        c.Value = 1;
                        c.Stat = StatType.AdditionalDamage;
                        c.Descriptor = ModifierDescriptor.UntypedStackable;
                    });
                });
                TTTContext.Logger.LogPatch("Patched", AeonGreaterBaneBuff);
            }
            static void PatchAeonGreaterBaneDispel() {
                if (TTTContext.Homebrew.MythicReworks.Aeon.IsDisabled("PatchAeonGreaterBaneDispel")) { return; }

                var AeonGreaterBaneBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("cdcc13884252b2c4d8dac57cb5f46555");
                AeonGreaterBaneBuff.GetComponents<AddInitiatorAttackWithWeaponTrigger>()
                    .Where(action => action.Action.Actions.OfType<ContextActionDispelMagic>().Any())
                    .First().OnlyOnFirstHit = true;
                AeonGreaterBaneBuff.FlattenAllActions()
                    .OfType<ContextActionDispelMagic>()
                    .ForEach(c => c.m_BuffType = ContextActionDispelMagic.BuffType.FromSpells);
                TTTContext.Logger.LogPatch("Patched", AeonGreaterBaneBuff);
            }
            static void PatchAeonGazeAction() {
                if (TTTContext.Homebrew.MythicReworks.Aeon.IsDisabled("AeonGazeActionSystem")) { return; }
                var AeonGazeThirdSelection = BlueprintTools.GetBlueprint<BlueprintFeatureSelection>("0bec49f67ecb49a5826fcfefb9408a35");
                AeonGazeThirdSelection.AllFeatures
                    .SelectMany(feature => feature.GetComponent<AddFacts>()?.m_Facts)
                    .Where(gaze => gaze != null)
                    .Select(gaze => gaze.Get() as BlueprintActivatableAbility)
                    .Where(gaze => gaze != null)
                    .ForEach(gaze => {
                        gaze.m_ActivateWithUnitCommand = UnitCommand.CommandType.Swift;
                        var spendLogic = gaze.GetComponent<ActivatableAbilityResourceLogic>();
                        if (spendLogic != null) {
                            spendLogic.SpendType = ActivatableAbilitySpendLogic.StandardSpendType.AeonGaze.ToResourceType();
                            TTTContext.Logger.LogPatch("Patched", gaze);
                        }
                        var gazeBuff = gaze.Buff;
                        if (gazeBuff != null) {
                            gazeBuff.AddComponent<AeonGazeResouceLogic>();
                            TTTContext.Logger.LogPatch("Patched", gazeBuff);
                        }
                    });
            }
            static void PatchAeonGazeIcons() {
                if (TTTContext.Homebrew.MythicReworks.Aeon.IsDisabled("AeonGazeIcons")) { return; }
                var AeonGazeThirdSelection = BlueprintTools.GetBlueprint<BlueprintFeatureSelection>("0bec49f67ecb49a5826fcfefb9408a35");
                AeonGazeThirdSelection.AllFeatures
                    .SelectMany(feature => feature.GetComponent<AddFacts>()?.m_Facts)
                    .Where(gaze => gaze != null)
                    .Select(gaze => gaze.Get() as BlueprintActivatableAbility)
                    .Where(gaze => gaze != null)
                    .ForEach(gaze => {
                        var gazeBuff = gaze.Buff;
                        if (gazeBuff != null) {
                            gazeBuff.m_Icon = gaze.Icon;
                            TTTContext.Logger.LogPatch("Patched", gazeBuff);
                            gazeBuff.GetComponent<AddAreaEffect>().TemporaryContext(areaEffect => {
                                areaEffect.AreaEffect
                                .FlattenAllActions()
                                .OfType<ContextActionApplyBuff>()
                                .ForEach(component => {
                                    component.Buff.TemporaryContext(buff => {
                                        buff.m_Flags &= ~BlueprintBuff.Flags.HiddenInUi;
                                        buff.m_Icon = gaze.Icon;
                                    });
                                });
                            });
                        }
                    });
            }
        }
        [HarmonyPatch(typeof(AbilityData), nameof(AbilityData.RuntimeActionType), MethodType.Getter)]
        static class AbilityData_GetRuntimeActionType_MythicMoveAction_Patch {
            static readonly BlueprintBuff AeonGreaterBaneBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("cdcc13884252b2c4d8dac57cb5f46555");
            static void Postfix(AbilityData __instance, ref UnitCommand.CommandType __result) {
                if (TTTContext.Homebrew.MythicReworks.Aeon.IsDisabled("AeonGreaterBaneActionBoost")) { return; }
                UnitCommand.CommandType commandType = __instance.ActionType;
                if (commandType == UnitCommand.CommandType.Swift) {
                    UnitDescriptor caster = __instance.Caster;
                    if (((caster != null) ? caster.State.Features.MythicAbilitiesAsMoveAction : null)
                        && caster.Buffs.HasFact(AeonGreaterBaneBuff)
                        && __instance.Blueprint.Type != AbilityType.Physical
                        && caster.Unit.CombatState.HasCooldownForCommand(UnitCommand.CommandType.Swift)
                        && !caster.Unit.CombatState.HasCooldownForCommand(UnitCommand.CommandType.Move)) {
                        __result = UnitCommand.CommandType.Move;
                    }
                }
            }
        }
        [HarmonyPatch(typeof(ActivatableAbility), nameof(ActivatableAbility.IsActivateWithSameCommand), new Type[] { typeof(ActivatableAbility) })]
        static class ActivatableAbility_IsActivateWithSameCommand_Aeon_Patch {
            static void Postfix(ActivatableAbility __instance, ActivatableAbility other, ref bool __result) {
                if (TTTContext.Homebrew.MythicReworks.Aeon.IsDisabled("AeonGazeActionSystem")) { return; }
                if (__instance.Blueprint.Group == ActivatableAbilityGroup.AeonGaze && __instance.Blueprint.Group == other.Blueprint.Group) {
                    __result = true;
                }
            }
        }
        [HarmonyPatch(typeof(UnitPartActivatableAbility), nameof(UnitPartActivatableAbility.OnActivatedWithCommand))]
        static class UnitPartActivatableAbility_OnActivatedWithCommand_Aeon_Patch {
            static void Postfix(UnitPartActivatableAbility __instance, ActivatableAbility ability) {
                if (TTTContext.Homebrew.MythicReworks.Aeon.IsDisabled("AeonGazeActionSystem")) { return; }
                int groupSize = __instance.GetGroupSize(ability.Blueprint.Group);
                if (groupSize >= 2 || ability.Blueprint.Group != ActivatableAbilityGroup.Judgment) {
                    UnitPartActivatableAbility.ActivatedWithCommandData activatedWithCommandData =
                        __instance.m_ActivatedWithCommand.FirstItem((UnitPartActivatableAbility.ActivatedWithCommandData i) => i.Ability == ability);
                    if (activatedWithCommandData == null) {
                        activatedWithCommandData = new UnitPartActivatableAbility.ActivatedWithCommandData {
                            Ability = ability
                        };
                        __instance.m_ActivatedWithCommand.Add(activatedWithCommandData);
                    }
                    activatedWithCommandData.Time = Game.Instance.TimeController.GameTime;
                    HashSet<ActivatableAbility> activated = activatedWithCommandData.Activated;
                    if (activated != null) {
                        activated.Clear();
                    }
                    activatedWithCommandData.GroupSize = groupSize;
                }
            }
        }
    }
}
