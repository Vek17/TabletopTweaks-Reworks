using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Controllers;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.Enums.Damage;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.UnitLogic.Mechanics.Properties;
using Kingmaker.Utility;
using TabletopTweaks.Core.NewActions;
using TabletopTweaks.Core.NewComponents.OwlcatReplacements;
using TabletopTweaks.Core.Utilities;
using static TabletopTweaks.Reworks.Main;

namespace TabletopTweaks.Reworks.Reworks {
    class MythicAbilities {
        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        static class BlueprintsCache_Init_Patch {
            static bool Initialized;

            static void Postfix() {
                if (Initialized) return;
                Initialized = true;
                TTTContext.Logger.LogHeader("Reworking Mythic Abilities");
                PatchElementalBarrage();
                PatchDimensionalRetribution();
                PatchGreaterEnduringSpells();
                PatchAbundantCasting();
            }
            static void PatchElementalBarrage() {
                if (TTTContext.Homebrew.MythicAbilities.IsDisabled("ElementalBarrage")) { return; }
                var ElementalBarrage = BlueprintTools.GetBlueprint<BlueprintFeature>("da56a1b21032a374783fdf46e1a92adb");
                var ElementalBarrageAcidBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("823d33bdb23e7c64d9cc1cce9b78fdea");
                var ElementalBarrageColdBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("c5e9031099d3e8d4788d3e51f7ffb8a0");
                var ElementalBarrageElectricityBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("0b8ed343b989bbb4c8d059366a7c2d01");
                var ElementalBarrageFireBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("7db8ad7b035c2f244951cbef3c9909df");

                ElementalBarrage.SetDescription(TTTContext, "You've mastered the art of raining elemental spells on your foes, " +
                    "and found a way to empower them by combining different elements.\n" +
                    "Benefit: Every time you deal elemental damage to a creature with a spell, you apply an elemental mark to it. " +
                    "If during the next three rounds the marked target takes elemental damage from any source " +
                    "with a different element, the target takes additional Divine damage and consume the mark. " +
                    "The damage is 1d6 per mythic rank of your character.");
                ElementalBarrage.GetComponents<AddOutgoingDamageTrigger>().ForEach(c => {
                    c.CheckAbilityType = true;
                    c.m_AbilityType = AbilityType.Spell;
                });
                ElementalBarrage.SetComponents();
                AddOutgoingDamageTrigger(ElementalBarrage, ElementalBarrageAcidBuff, DamageEnergyType.Acid);
                AddOutgoingDamageTrigger(ElementalBarrage, ElementalBarrageColdBuff, DamageEnergyType.Cold);
                AddOutgoingDamageTrigger(ElementalBarrage, ElementalBarrageElectricityBuff, DamageEnergyType.Electricity);
                AddOutgoingDamageTrigger(ElementalBarrage, ElementalBarrageFireBuff, DamageEnergyType.Fire);

                AddIncomingDamageTriggers(ElementalBarrageAcidBuff, DamageEnergyType.Cold, DamageEnergyType.Electricity, DamageEnergyType.Fire);
                AddIncomingDamageTriggers(ElementalBarrageColdBuff, DamageEnergyType.Acid, DamageEnergyType.Electricity, DamageEnergyType.Fire);
                AddIncomingDamageTriggers(ElementalBarrageElectricityBuff, DamageEnergyType.Acid, DamageEnergyType.Cold, DamageEnergyType.Fire);
                AddIncomingDamageTriggers(ElementalBarrageFireBuff, DamageEnergyType.Acid, DamageEnergyType.Cold, DamageEnergyType.Electricity);

                UpdateBuffVisability(ElementalBarrageAcidBuff, "acid");
                UpdateBuffVisability(ElementalBarrageColdBuff, "cold");
                UpdateBuffVisability(ElementalBarrageElectricityBuff, "electricity");
                UpdateBuffVisability(ElementalBarrageFireBuff, "fire");

                TTTContext.Logger.LogPatch("Patched", ElementalBarrage);
                void AddOutgoingDamageTrigger(BlueprintFeature barrage, BlueprintBuff barrageBuff, DamageEnergyType trigger) {
                    barrage.AddComponent<AddOutgoingDamageTriggerTTT>(c => {
                        c.IgnoreDamageFromThisFact = true;
                        c.CheckEnergyDamageType = true;
                        c.EnergyType = trigger;
                        c.CheckAbilityType = true;
                        c.ApplyToAreaEffectDamage = true;
                        c.m_AbilityType = AbilityType.Spell;
                        c.Actions = Helpers.CreateActionList(
                            Helpers.Create<ContextActionApplyBuff>(a => {
                                a.m_Buff = barrageBuff.ToReference<BlueprintBuffReference>();
                                a.DurationValue = new ContextDurationValue() {
                                    m_IsExtendable = true,
                                    DiceCountValue = new ContextValue(),
                                    BonusValue = 3
                                };
                                a.IsNotDispelable = true;
                                a.AsChild = false;
                            })
                        );
                    });
                }
                void UpdateBuffVisability(BlueprintBuff barrageBuff, string element) {
                    barrageBuff.SetDescription(TTTContext, $"If this creature takes elemental damage from a " +
                        $"type other than {element} it will take additional damage and consume the mark.");
                    barrageBuff.m_Flags = 0;
                }
                void AddIncomingDamageTriggers(BlueprintBuff barrageBuff, params DamageEnergyType[] triggers) {
                    foreach (var trigger in triggers) {
                        barrageBuff.AddComponent<AddIncomingDamageTrigger>(c => {
                            c.IgnoreDamageFromThisFact = true;
                            c.CheckEnergyDamageType = true;
                            c.EnergyType = trigger;
                            c.Actions = Helpers.CreateActionList(
                                Helpers.Create<ContextActionDealDamageTTT>(a => {
                                    a.DamageType = new DamageTypeDescription() {
                                        Type = DamageType.Energy,
                                        Energy = DamageEnergyType.Divine
                                    };
                                    a.Duration = new ContextDurationValue() {
                                        DiceCountValue = new ContextValue(),
                                        BonusValue = new ContextValue()
                                    };
                                    a.Value = new ContextDiceValue() {
                                        DiceType = DiceType.D6,
                                        DiceCountValue = new ContextValue() {
                                            ValueType = ContextValueType.CasterProperty,
                                            Property = UnitProperty.MythicLevel
                                        },
                                        BonusValue = 0
                                    };
                                    a.IgnoreCritical = true;
                                    a.SetFactAsReason = true;
                                    a.IgnoreWeapon = true;
                                }),
                                Helpers.Create<ContextActionRemoveSelf>()
                            );
                        });
                    }
                }
            }
            static void PatchDimensionalRetribution() {
                if (Main.TTTContext.Homebrew.MythicAbilities.IsDisabled("DimensionalRetribution")) { return; }
                var DimensionalRetribution = BlueprintTools.GetBlueprint<BlueprintFeature>("939f49ad995ee8d4fad03ad0c7f655d1");
                var DimensionalRetributionTTTToggleAbility = BlueprintTools.GetModBlueprintReference<BlueprintUnitFactReference>(modContext: TTTContext, "DimensionalRetributionTTTToggleAbility");

                DimensionalRetribution.SetDescription(TTTContext, "You leave a mystical link with enemy spellcasters that lets you instantly move to them." +
                    "Benefit: Every time you are hit by an enemy spell, you may teleport to the " +
                    "spellcaster as an immediate action and make an attack of opportunity.");
                DimensionalRetribution.SetComponents();
                DimensionalRetribution.AddComponent<AddFacts>(c => {
                    c.m_Facts = new BlueprintUnitFactReference[] {
                        DimensionalRetributionTTTToggleAbility
                    };
                });
                TTTContext.Logger.LogPatch("Patched", DimensionalRetribution);
            }
            static void PatchGreaterEnduringSpells() {
                if (Main.TTTContext.Homebrew.MythicAbilities.IsDisabled("GreaterEnduringSpells")) { return; }
                var EnduringSpells = BlueprintTools.GetBlueprint<BlueprintFeature>("2f206e6d292bdfb4d981e99dcf08153f");
                var EnduringSpellsGreater = BlueprintTools.GetBlueprint<BlueprintFeature>("13f9269b3b48ae94c896f0371ce5e23c");

                EnduringSpells.RemoveComponents<EnduringSpells>();
                EnduringSpells.AddComponent<EnduringSpellsTTT>(c => {
                    c.m_Greater = EnduringSpellsGreater.ToReference<BlueprintUnitFactReference>();
                    c.EnduringTime = 60.Minutes();
                    c.GreaterTime = 10.Minutes();
                });

                EnduringSpellsGreater.SetDescription(TTTContext, "You've mastered a way to prolong your beneficial spells.\n" +
                    "Benefit: Effects of your spells on your allies that should last longer than 10 minutes " +
                    "but shorter than 24 hours now last 24 hours.");

                TTTContext.Logger.LogPatch("Patched", EnduringSpells);
                TTTContext.Logger.LogPatch("Patched", EnduringSpellsGreater);
            }
            static void PatchAbundantCasting() {
                if (Main.TTTContext.Homebrew.MythicAbilities.IsDisabled("AbundantCasting")) { return; }
                var AbundantCasting = BlueprintTools.GetBlueprint<BlueprintFeature>("cf594fa8871332a4ba861c6002480ec2");
                var AbundantCastingImproved = BlueprintTools.GetBlueprint<BlueprintFeature>("37046a54739ed4844b8e8307bbbeece2");
                var AbundantCastingGreater = BlueprintTools.GetBlueprint<BlueprintFeature>("db5be78901afbfa4e8ea5b399a88b92d");

                AbundantCasting.GetComponent<AddSpellsPerDay>().Amount = 2;
                AbundantCastingImproved.GetComponent<AddSpellsPerDay>().Amount = 2;
                AbundantCastingGreater.GetComponent<AddSpellsPerDay>().Amount = 2;

                AbundantCasting.SetDescription(TTTContext, "You've learned a way to increase the number of spells you can cast per day.\n" +
                    "Benefit: You can cast two more spells per day of 1st, 2nd, and 3rd ranks each. This ability does not affect mythic spellbooks.");
                AbundantCastingImproved.SetDescription(TTTContext, "You have studied a way to increase the number of spells you can cast per day.\n" +
                    "Benefit: You can cast two more spells per day of 4th, 5th, and 6th levels each. This ability does not affect mythic spellbooks.");
                AbundantCastingGreater.SetDescription(TTTContext, "You've mastered a way to increase the number of spells you can cast per day.\n" +
                    "Benefit: You can cast four more spells per day of 7th, 8th, and 9th ranks each. This ability does not affect mythic spellbooks.");

                TTTContext.Logger.LogPatch("Patched", AbundantCasting);
                TTTContext.Logger.LogPatch("Patched", AbundantCastingImproved);
                TTTContext.Logger.LogPatch("Patched", AbundantCastingGreater);
            }
        }
    }
}
