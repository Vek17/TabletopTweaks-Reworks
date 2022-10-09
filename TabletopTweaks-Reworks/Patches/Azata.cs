using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Designers.Mechanics.Buffs;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.Enums.Damage;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using Kingmaker.UnitLogic.Mechanics.Properties;
using Kingmaker.Utility;
using System.Linq;
using TabletopTweaks.Core.NewComponents;
using TabletopTweaks.Core.NewComponents.OwlcatReplacements;
using TabletopTweaks.Core.Utilities;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static TabletopTweaks.Reworks.Main;

namespace TabletopTweaks.Reworks.Reworks {
    class Azata {
        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        static class BlueprintsCache_Init_Patch {
            static bool Initialized;

            static void Postfix() {
                if (Initialized) return;
                Initialized = true;
                TTTContext.Logger.LogHeader("Azata Rework");

                PatchAzataPerformanceResource();
                PatchAzataSpellList();
                PatchAzataSpells();
                PatchAzataSongActions();
                PatchAzataSongToggles();
                PatchFavorableMagic();
                PatchIncredibleMight();
                PatchLifeBondingFriendship();
                PatchSupersonicSpeed();
                PatchZippyMagicFeature();
            }

            static void PatchAzataPerformanceResource() {
                if (TTTContext.Homebrew.MythicReworks.Azata.IsDisabled("AzataPerformanceResource")) { return; }
                var AzataPerformanceResource = BlueprintTools.GetBlueprint<BlueprintAbilityResource>("83f8a1c45ed205a4a989b7826f5c0687");

                BlueprintCharacterClassReference[] characterClasses = ResourcesLibrary
                    .GetRoot()
                    .Progression
                    .CharacterClasses
                    .Where(c => c != null)
                    .Select(c => c.ToReference<BlueprintCharacterClassReference>())
                    .ToArray();
                AzataPerformanceResource.m_MaxAmount.m_Class = characterClasses;
                TTTContext.Logger.LogPatch("Patched", AzataPerformanceResource);
            }
            static void PatchAzataSpellList() {
                if (TTTContext.Homebrew.MythicReworks.Azata.IsDisabled("AzataSpellList")) { return; }

                var AzataForSpellsCollateralFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("a7a4ae18dc57b8c4791221323812899a");
                var AzataForSpellsDevilFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("6d330ba4e39fdb647bd34df9810d0a4c");
                var AzataForSpellsGoodFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("8155b3b3692a2b04089a19349579f8af");
                // Collateral Bonus Spells
                var WaterPush = BlueprintTools.GetBlueprint<BlueprintAbility>("17712729faf427f4fa0463bc919a0ff4");
                var RepulsiveNature = BlueprintTools.GetBlueprint<BlueprintAbility>("1b148617dac4f0341a9b0dec00b11b3a");
                var BurstOfSonicEnergy = BlueprintTools.GetBlueprint<BlueprintAbility>("b5a2d0e400dd38e428c953f8a2be5f0b");
                var WindsOfTheFall = BlueprintTools.GetBlueprint<BlueprintAbility>("af2ed41c7894b934c9a9ca5048af3f58");
                var Revolt = BlueprintTools.GetBlueprint<BlueprintAbility>("319c5a450b01f834599389853753d0f0");
                // Good Bonus Spells
                var BelieveInYourself = BlueprintTools.GetBlueprint<BlueprintAbility>("3ed3cef7c267cb847bfd44ed4708b726");
                var ElusiveNature = BlueprintTools.GetBlueprint<BlueprintAbility>("2acdc2aeba36d8e45871ec6105c16509");
                var MoralSupport = BlueprintTools.GetBlueprint<BlueprintAbility>("92ca26ebceec30a4babc1b0582e34ce6");
                var SongsOfSteel = BlueprintTools.GetBlueprint<BlueprintAbility>("f7b476555f96afe4b9ba6e7d56d3272a");
                var UnbreakableBond = BlueprintTools.GetBlueprint<BlueprintAbility>("947a929f3347d3e458a524424fbceccb");

                AzataForSpellsCollateralFeature.SetComponents();
                AzataForSpellsDevilFeature.SetComponents();
                AzataForSpellsGoodFeature.SetComponents();

                SpellTools.AddToSpellList(WaterPush, SpellTools.SpellList.AzataMythicSpellsSpelllist, 3);
                SpellTools.AddToSpellList(RepulsiveNature, SpellTools.SpellList.AzataMythicSpellsSpelllist, 4);
                SpellTools.AddToSpellList(BurstOfSonicEnergy, SpellTools.SpellList.AzataMythicSpellsSpelllist, 5);
                SpellTools.AddToSpellList(WindsOfTheFall, SpellTools.SpellList.AzataMythicSpellsSpelllist, 6);
                SpellTools.AddToSpellList(Revolt, SpellTools.SpellList.AzataMythicSpellsSpelllist, 7);

                SpellTools.AddToSpellList(BelieveInYourself, SpellTools.SpellList.AzataMythicSpellsSpelllist, 3);
                SpellTools.AddToSpellList(ElusiveNature, SpellTools.SpellList.AzataMythicSpellsSpelllist, 4);
                SpellTools.AddToSpellList(MoralSupport, SpellTools.SpellList.AzataMythicSpellsSpelllist, 5);
                SpellTools.AddToSpellList(SongsOfSteel, SpellTools.SpellList.AzataMythicSpellsSpelllist, 6);
                SpellTools.AddToSpellList(UnbreakableBond, SpellTools.SpellList.AzataMythicSpellsSpelllist, 7);

                TTTContext.Logger.Log("Updated Azata Spelllist");
            }
            static void PatchAzataSpells() {
                PatchRainbowArrows();
                PatchSongsOfSteel();

                void PatchRainbowArrows() {
                    var RainbowArrows = BlueprintTools.GetBlueprint<BlueprintAbility>("88bb6f877c8185543850656a5f14e802");
                    var BlindnessBuff = BlueprintTools.GetBlueprintReference<BlueprintBuffReference>("187f88d96a0ef464280706b63635f2af");
                    var Daze = BlueprintTools.GetBlueprintReference<BlueprintBuffReference>("d2e35b870e4ac574d9873b36402487e5");
                    var Nauseated = BlueprintTools.GetBlueprintReference<BlueprintBuffReference>("956331dba5125ef48afe41875a00ca0e");
                    var Slowed = BlueprintTools.GetBlueprintReference<BlueprintBuffReference>("488e53ede2802ff4da9372c6a494fb66");
                    var Stunned = BlueprintTools.GetBlueprintReference<BlueprintBuffReference>("09d39b38bb7c6014394b6daced9bacd3");
                    var Staggered = BlueprintTools.GetBlueprintReference<BlueprintBuffReference>("df3950af5a783bd4d91ab73eb8fa0fd3");

                    RainbowArrows.TemporaryContext(bp => {
                        bp.SetDescription(TTTContext, "Azata creates a rainbow arrow that jumps from target to target up to one time per 4 caster levels, " +
                            "dealing 1d12 per 2 caster levels damage with one energy randomly chosen between acid, cold, electricity, fire, or sound. " +
                            "Additionally if the enemy fails a will saving throw they are randomly blinded, dazed, nauseated, slowed, or stunned for 3 rounds.");
                        bp.SpellResistance = false;
                        bp.SetLocalizedSavingThrow(TTTContext, "Will partial");
                        bp.GetComponent<AbilityEffectRunAction>().TemporaryContext(c => {
                            c.SavingThrowType = SavingThrowType.Will;
                            c.Actions = Helpers.CreateActionList(
                                new ContextActionRandomize() {
                                    m_Actions = new ContextActionRandomize.ActionWrapper[] {
                                    CreateDamageAction(DamageEnergyType.Acid),
                                    CreateDamageAction(DamageEnergyType.Cold),
                                    CreateDamageAction(DamageEnergyType.Electricity),
                                    CreateDamageAction(DamageEnergyType.Fire),
                                    CreateDamageAction(DamageEnergyType.Sonic)
                                    }
                                },
                                new ContextActionRandomize() {
                                    m_Actions = new ContextActionRandomize.ActionWrapper[] {
                                    CreateDebuffAction(BlindnessBuff, 3),
                                    CreateDebuffAction(Daze, 3),
                                    CreateDebuffAction(Nauseated, 3),
                                    CreateDebuffAction(Slowed, 3),
                                    CreateDebuffAction(Stunned, 3)
                                    }
                                }
                            );
                        });
                        bp.GetComponent<SpellDescriptorComponent>().TemporaryContext(c => {
                            c.Descriptor = SpellDescriptor.None;
                        });
                    });

                    TTTContext.Logger.LogPatch(RainbowArrows);

                    ContextActionRandomize.ActionWrapper CreateDamageAction(DamageEnergyType energyType) {
                        return new ContextActionRandomize.ActionWrapper() {
                            Action = Helpers.CreateActionList(
                                new ContextActionDealDamage() {
                                    DamageType = new DamageTypeDescription() {
                                        Type = DamageType.Energy,
                                        Energy = energyType,
                                    },
                                    Duration = new ContextDurationValue() {
                                        DiceCountValue = new ContextValue(),
                                        BonusValue = new ContextValue()
                                    },
                                    Value = new ContextDiceValue() {
                                        DiceType = DiceType.D12,
                                        DiceCountValue = new ContextValue() {
                                            ValueType = ContextValueType.Rank
                                        },
                                        BonusValue = 0
                                    }
                                }
                            )
                        };
                    }
                    ContextActionRandomize.ActionWrapper CreateDebuffAction(BlueprintBuffReference debuff, int rounds = 1, bool permanant = false) {
                        return new ContextActionRandomize.ActionWrapper() {
                            Action = Helpers.CreateActionList(
                                new ContextActionConditionalSaved() {
                                    Succeed = Helpers.CreateActionList(),
                                    Failed = Helpers.CreateActionList(
                                        new ContextActionApplyBuff() {
                                            m_Buff = debuff,
                                            Permanent = permanant,
                                            DurationValue = new ContextDurationValue() {
                                                Rate = DurationRate.Rounds,
                                                DiceCountValue = 0,
                                                BonusValue = rounds
                                            }
                                        }
                                    )
                                }
                            )
                        };
                    }
                }
                void PatchSongsOfSteel() {
                    var SongsOfSteel = BlueprintTools.GetBlueprint<BlueprintAbility>("f7b476555f96afe4b9ba6e7d56d3272a");
                    var SongsOfSteelBuff = BlueprintTools.GetBlueprintReference<BlueprintBuffReference>("6867deda1eda183499ae61813c2f5ebb");

                    SongsOfSteel.TemporaryContext(bp => {
                        bp.SetDescription(TTTContext, "This spell grants party members additional (2d6 + Azata's caster level) sonic damage " +
                            "for their first weapon attack during a round, a +2 bonus to caster level checks made to overcome spell resistance, " +
                            "a +2 bonus to all concentration checks, and rises the DC of the saving throws against all their spells and abilities by 2.");
                        bp.SetLocalizedDuration(TTTContext, "10 minutes/level");
                        bp.SpellResistance = false;
                        bp.Range = AbilityRange.Personal;
                        bp.GetComponent<AbilityEffectRunAction>().TemporaryContext(c => {
                            c.Actions = Helpers.CreateActionList(
                                new ContextActionApplyBuff() { 
                                    m_Buff = SongsOfSteelBuff,
                                    DurationValue = new ContextDurationValue() { 
                                        Rate = DurationRate.TenMinutes,
                                        DiceCountValue = 0,
                                        BonusValue = new ContextValue() { 
                                            ValueType = ContextValueType.Rank
                                        }
                                    }
                                }
                            );
                        });
                        bp.GetComponent<SpellComponent>().TemporaryContext(c => {
                            c.School = SpellSchool.Enchantment;
                        });
                        bp.AddComponent<AbilityTargetsAround>(c => {
                            c.m_Radius = 60.Feet();
                            c.m_TargetType = TargetType.Ally;
                            c.m_Condition = new ConditionsChecker() { 
                                Conditions = new Condition[] { 
                                    new ContextConditionIsPartyMember()
                                }
                            };
                        });
                    });
                    SongsOfSteelBuff.Get().TemporaryContext(bp => {
                        bp.SetDescription(SongsOfSteel.m_Description);
                    });
                }
            }
            static void PatchAzataSongActions() {
                if (Main.TTTContext.Homebrew.MythicReworks.Azata.IsDisabled("AzataSongActions")) { return; }

                var SongOfHeroicResolveToggleAbility = BlueprintTools.GetBlueprint<BlueprintActivatableAbility>("a95449d0ea0714a4ea5cffc83fc7624f");
                var SongOfBrokenChainsToggleAbility = BlueprintTools.GetBlueprint<BlueprintActivatableAbility>("ac08e4d23e2928148a7b4109e9485e6a");
                var SongOfDefianceToggleAbility = BlueprintTools.GetBlueprint<BlueprintActivatableAbility>("661ad9ab9c8af2e4c86a7cfa4c2be3f2");
                var SongOfCourageousDefenderToggleAbility = BlueprintTools.GetBlueprint<BlueprintActivatableAbility>("66864464f529c264f8c08ec2f4bf1cb5");

                SongOfHeroicResolveToggleAbility.m_ActivateWithUnitCommand = CommandType.Move;
                SongOfBrokenChainsToggleAbility.m_ActivateWithUnitCommand = CommandType.Move;
                SongOfDefianceToggleAbility.m_ActivateWithUnitCommand = CommandType.Move;
                SongOfCourageousDefenderToggleAbility.m_ActivateWithUnitCommand = CommandType.Move;

                TTTContext.Logger.LogPatch("Patched", SongOfHeroicResolveToggleAbility);
                TTTContext.Logger.LogPatch("Patched", SongOfBrokenChainsToggleAbility);
                TTTContext.Logger.LogPatch("Patched", SongOfDefianceToggleAbility);
                TTTContext.Logger.LogPatch("Patched", SongOfCourageousDefenderToggleAbility);
            }
            static void PatchAzataSongToggles() {
                if (Main.TTTContext.Homebrew.MythicReworks.Azata.IsDisabled("AzataSongToggles")) { return; }

                var SongOfHeroicResolveToggleAbility = BlueprintTools.GetBlueprint<BlueprintActivatableAbility>("a95449d0ea0714a4ea5cffc83fc7624f");
                var SongOfBrokenChainsToggleAbility = BlueprintTools.GetBlueprint<BlueprintActivatableAbility>("ac08e4d23e2928148a7b4109e9485e6a");
                var SongOfDefianceToggleAbility = BlueprintTools.GetBlueprint<BlueprintActivatableAbility>("661ad9ab9c8af2e4c86a7cfa4c2be3f2");
                var SongOfCourageousDefenderToggleAbility = BlueprintTools.GetBlueprint<BlueprintActivatableAbility>("66864464f529c264f8c08ec2f4bf1cb5");

                SongOfHeroicResolveToggleAbility.DeactivateImmediately = false;
                SongOfBrokenChainsToggleAbility.DeactivateImmediately = false;
                SongOfDefianceToggleAbility.DeactivateImmediately = false;
                SongOfCourageousDefenderToggleAbility.DeactivateImmediately = false;

                TTTContext.Logger.LogPatch("Patched", SongOfHeroicResolveToggleAbility);
                TTTContext.Logger.LogPatch("Patched", SongOfBrokenChainsToggleAbility);
                TTTContext.Logger.LogPatch("Patched", SongOfDefianceToggleAbility);
                TTTContext.Logger.LogPatch("Patched", SongOfCourageousDefenderToggleAbility);
            }
            static void PatchFavorableMagic() {
                if (Main.TTTContext.Homebrew.MythicReworks.Azata.IsDisabled("FavorableMagic")) { return; }
                var FavorableMagicFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("afcee6925a6eadf43820d12e0d966ebe");

                FavorableMagicFeature.SetComponents(
                    Helpers.Create<AzataFavorableMagicTTT>(c => {
                        c.OnlySpells = true;
                    })
                );
                TTTContext.Logger.LogPatch("Patched", FavorableMagicFeature);
            }
            static void PatchIncredibleMight() {
                if (Main.TTTContext.Homebrew.MythicReworks.Azata.IsDisabled("IncredibleMight")) { return; }
                var IncredibleMightAllyBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("8e041bd9d786d934892d892d179fc1e8");
                var IncredibleMightMainBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("9a86d073d91f599439c8d4588cdb1fc8");
                var IncredibleMightFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("eef8d23a7e4acfe4d834a5de844c8c7c");
                var IncredibleMightAbility = BlueprintTools.GetBlueprint<BlueprintActivatableAbility>("f81b4910d05399a4aaf5fcc8c4d713eb");

                IncredibleMightFeature.SetDescription(TTTContext, IncredibleMightFeature.Description.Replace("morale ", ""));
                IncredibleMightMainBuff.m_Description = IncredibleMightFeature.m_Description;

                IncredibleMightMainBuff
                    .GetComponents<AddContextStatBonus>()
                    .ForEach(c => {
                        c.Descriptor = ModifierDescriptor.UntypedStackable;
                    });
                IncredibleMightAllyBuff
                    .GetComponents<AddContextStatBonus>()
                    .ForEach(c => {
                        c.Descriptor = ModifierDescriptor.UntypedStackable;
                    });
                TTTContext.Logger.LogPatch("Patched", IncredibleMightAllyBuff);
                TTTContext.Logger.LogPatch("Patched", IncredibleMightMainBuff);
                TTTContext.Logger.LogPatch("Patched", IncredibleMightFeature);
            }
            static void PatchLifeBondingFriendship() {
                if (Main.TTTContext.Homebrew.MythicReworks.Azata.IsDisabled("LifeBondingFriendship")) { return; }
                var LifeBondingFriendshipBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("0e72a67b5784cdb4ba5d4f8e12e73040");
                var LifeBondingFriendshipEffectBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("33655686030a333438c1d407ec158a93");
                var LifeBondingFriendshipEffectBuff1 = BlueprintTools.GetBlueprint<BlueprintBuff>("3cfbba65a0f64573b116511fb43db5c4");
                var LifeBondingFriendshipFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("5b63a6690ebbaa4459df3e04fe094cbd");
                var LifeBondingFriendshipFeature1 = BlueprintTools.GetBlueprint<BlueprintFeature>("e09d87a6493949609bc49d2a8a0c382f");

                var LifeBondingFriendshipProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("6c85301c50c6621409b42b83ce9cc6d9");
                var LifeBondingFriendshipSelection = BlueprintTools.GetBlueprint<BlueprintFeatureSelection>("cfad18f581584ac4ba066df067956477");
                var LifeBondingFriendshipSelection1 = BlueprintTools.GetBlueprint<BlueprintFeatureSelection>("69a33d6ced23446e819667149d088898");

                LifeBondingFriendshipFeature.TemporaryContext(bp => { 
                    bp.SetDescription(TTTContext, "Azata chooses one teamwork feat during this and next mythic rank up. " +
                        "She shares the effects of all teamwork feats she has with allies within a 50-foot area. Azata's Charisma inspires her allies so " +
                        "much that even when their hit points drop below 0 they continue to fight, even though they are still considered dead (or unconscious) " +
                        "for all effects and can be cured by breath of life or similar spells as per spell description. " +
                        "After the encounter or upon receiving a total number of attacks equal to Azata's Mythic Rank shared among all allies, " +
                        "the allies in this state fall dead (or unconscious). If all of the Azata's allies die that way, she becomes fully drained and dies.");
                    bp.GetComponent<KeepAlliesAlive>().TemporaryContext(c => {
                        c.m_MaxAttacksCount.Property = UnitProperty.MythicLevel;
                    });
                });
                LifeBondingFriendshipBuff.SetDescription(LifeBondingFriendshipFeature.m_Description);
                LifeBondingFriendshipEffectBuff.SetDescription(LifeBondingFriendshipFeature.m_Description);
                LifeBondingFriendshipEffectBuff1.SetDescription(LifeBondingFriendshipFeature.m_Description);
                LifeBondingFriendshipFeature1.SetDescription(LifeBondingFriendshipFeature.m_Description);
                LifeBondingFriendshipProgression.SetDescription(LifeBondingFriendshipFeature.m_Description);
                LifeBondingFriendshipSelection.SetDescription(LifeBondingFriendshipFeature.m_Description);
                LifeBondingFriendshipSelection1.SetDescription(LifeBondingFriendshipFeature.m_Description);

                TTTContext.Logger.LogPatch(LifeBondingFriendshipFeature);
            }
            static void PatchSupersonicSpeed() {
                if (Main.TTTContext.Homebrew.MythicReworks.Azata.IsDisabled("SupersonicSpeed")) { return; }
                var SupersonicSpeedHasteBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("89b097012ac11d74db2a54aa5d28c150");
                var SupersonicSpeedFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("505456aa17dd18a4e8bd8172811a4fdc");

                SupersonicSpeedFeature.TemporaryContext(bp => { 
                    bp.SetDescription(TTTContext, "Azata is able to make one additional attack an is always under the effect of haste spell, as long as she is engaged in combat. " +
                        "All melee and ranged weapon attacks against her have a 20% miss chance. All targeted spells aimed at her have a 10% miss chance. " +
                        "All the damage from area attacks against her is halved, even if previously reduced by saving throw." +
                        "\nNote: When character have miss chance and concealment at the same time, then only best bonus applied.");
                    bp.AddComponent<BuffExtraAttack>(c => {
                        c.Number = 1;
                        c.Haste = false;
                    });
                });
                    
                TTTContext.Logger.LogPatch("Patched", SupersonicSpeedHasteBuff);
                TTTContext.Logger.LogPatch("Patched", SupersonicSpeedFeature);
            }
            static void PatchZippyMagicFeature() {
                if (Main.TTTContext.Homebrew.MythicReworks.Azata.IsDisabled("ZippyMagic")) { return; }
                var ZippyMagicFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("30b4200f897ba25419ba3a292aed4053");

                ZippyMagicFeature.RemoveComponents<DublicateSpellComponent>();
                ZippyMagicFeature.AddComponent<AzataZippyMagicTTT>();
                TTTContext.Logger.LogPatch("Patched", ZippyMagicFeature);
                PatchCureWoundsDamage();
                PatchInflictWoundsDamage();

                void PatchCureWoundsDamage() {
                    BlueprintAbility[] cureSpells = new BlueprintAbility[] {
                        BlueprintTools.GetBlueprint<BlueprintAbility>("1edd1e201a608a24fa1de33d57502244"), // CureLightWoundsDamage
                        BlueprintTools.GetBlueprint<BlueprintAbility>("148673963b23fae4f9fcdcc5d67a91cc"), // CureModerateWoundsDamage
                        BlueprintTools.GetBlueprint<BlueprintAbility>("dd5d65e25a4e8b54a87d976c0a80f5b6"), // CureSeriousWoundsDamage
                        BlueprintTools.GetBlueprint<BlueprintAbility>("7d626a2c5eee30b47bbf7fee36d05309"), // CureCriticalWoundsDamage
                        BlueprintTools.GetBlueprint<BlueprintAbility>("fb7e5fe8b5750f9408398d9659b0f98f"), // CureLightWoundsMassDamage
                        BlueprintTools.GetBlueprint<BlueprintAbility>("638363b5afb817d4684c021d36279904"), // CureModerateWoundsMassDamage
                        BlueprintTools.GetBlueprint<BlueprintAbility>("21d02c685b2e64b4f852b3efcb0b5ca6"), // CureSeriousWoundsMassDamage
                        BlueprintTools.GetBlueprint<BlueprintAbility>("0cce61a5e5108114092f9773572c78b8"), // CureCriticalWoundsMassDamage
                        BlueprintTools.GetBlueprint<BlueprintAbility>("6ecd2657cb645274cbc167d667ac521d"), // HealDamage
                        BlueprintTools.GetBlueprint<BlueprintAbility>("7df289eaaf1233248b7be754f894de2e")  // HealMassDamage
                    };
                    cureSpells.ForEach(spell => BlockSpellDuplication(spell));
                }
                void PatchInflictWoundsDamage() {
                    BlueprintAbility[] inflictSpells = new BlueprintAbility[] {
                        BlueprintTools.GetBlueprint<BlueprintAbility>("f6ff156188dc4e44c850179fb19afaf5"), // InflictLightWoundsDamage
                        BlueprintTools.GetBlueprint<BlueprintAbility>("e55f5a1b875a5f242be5b92cf027b69a"), // InflictModerateWoundsDamage
                        BlueprintTools.GetBlueprint<BlueprintAbility>("095eaa846e2a8c343a54e927816e00af"), // InflictSeriousWoundsDamage
                        BlueprintTools.GetBlueprint<BlueprintAbility>("2737152467af53b4f9800e7a60644bb6"), // InflictCriticalWoundsDamage
                        BlueprintTools.GetBlueprint<BlueprintAbility>("b70d903464a738148a19bed630b91f8c"), // InflictLightWoundsMassDamage
                        BlueprintTools.GetBlueprint<BlueprintAbility>("89ddb1b4dafc5f541a3dacafbf9ea2dd"), // InflictModerateWoundsMassDamage
                        BlueprintTools.GetBlueprint<BlueprintAbility>("aba480ce9381684408290f5434402a32"), // InflictSeriousWoundsMassDamage
                        BlueprintTools.GetBlueprint<BlueprintAbility>("e05c263048e835043bb2784601dca339"), // InflictCriticalWoundsMassDamage
                        BlueprintTools.GetBlueprint<BlueprintAbility>("3da67f8b941308348b7101e7ef418f52")  // HarmDamage
                    };
                    inflictSpells.ForEach(spell => BlockSpellDuplication(spell));
                }
                void BlockSpellDuplication(BlueprintAbility blueprint) {
                    blueprint.AddComponent<BlockSpellDuplicationComponent>();
                    TTTContext.Logger.LogPatch("Blocked Duplication", blueprint);
                }
            }
        }
    }
}
