using Kingmaker.Blueprints.Items.Ecnchantments;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.Enums.Damage;
using Kingmaker.RuleSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TabletopTweaks.Core.Utilities;
using static TabletopTweaks.Reworks.Main;

namespace TabletopTweaks.Reworks.NewContent.MythicAbilities {
    static class SpellCastersOnslaught {
        public static void AddSpellCastersOnslaught() {
            var Shock = BlueprintTools.GetBlueprint<BlueprintWeaponEnchantment>("7bda5277d36ad114f9f9fd21d0dab658");
            var Flaming = BlueprintTools.GetBlueprint<BlueprintWeaponEnchantment>("30f90becaaac51f41bf56641966c4121");
            var Frost = BlueprintTools.GetBlueprint<BlueprintWeaponEnchantment>("421e54078b7719d40915ce0672511d0b");
            var Corrosive = BlueprintTools.GetBlueprint<BlueprintWeaponEnchantment>("633b38ff1d11de64a91d490c683ab1c8");
            var Thundering = BlueprintTools.GetBlueprint<BlueprintWeaponEnchantment>("690e762f7704e1f4aa1ac69ef0ce6a96");
            var Holy = BlueprintTools.GetBlueprint<BlueprintWeaponEnchantment>("28a9964d81fedae44bae3ca45710c140");

            var SpellCastersOnslaughtFire = Helpers.CreateBlueprint<BlueprintWeaponEnchantment>(TTTContext, "SpellCastersOnslaughtFire", bp => {
                bp.m_EnchantmentCost = 2;
                bp.SetName(TTTContext, "Spell Casters Onslaught — Fire");
                bp.SetDescription(TTTContext, "Your weapon deals an extra 2d6 points of fire damage on a successful hit. " +
                    "This weapon deals an additional 1d10 fire damage on a sccessful crit. " +
                    "If this weapon's critical multiplier is x3 add an extra 2d10 points of fire damage instead, " +
                    "and if the multiplier is x4, add an extra 3d10 points.");
                bp.SetPrefix(TTTContext, "");
                bp.SetSuffix(TTTContext, "");
                bp.WeaponFxPrefab = Flaming.WeaponFxPrefab;
                bp.AddComponent<WeaponEnergyDamageDice>(c => {
                    c.Element = DamageEnergyType.Fire;
                    c.EnergyDamageDice = new DiceFormula() { 
                        m_Rolls = 2,
                        m_Dice = DiceType.D6
                    };
                });
                bp.AddComponent<WeaponEnergyBurst>(c => {
                    c.Element = DamageEnergyType.Fire;
                    c.Dice = DiceType.D10;
                });
            });
            var SpellCastersOnslaughtCold = Helpers.CreateBlueprint<BlueprintWeaponEnchantment>(TTTContext, "SpellCastersOnslaughtCold", bp => {
                bp.m_EnchantmentCost = 2;
                bp.SetName(TTTContext, "Spell Casters Onslaught — Cold");
                bp.SetDescription(TTTContext, "Your weapon deals an extra 2d6 points of cold damage on a successful hit. " +
                    "This weapon deals an additional 1d10 cold damage on a sccessful crit. " +
                    "If this weapon's critical multiplier is x3 add an extra 2d10 points of cold damage instead, " +
                    "and if the multiplier is x4, add an extra 3d10 points.");
                bp.SetPrefix(TTTContext, "");
                bp.SetSuffix(TTTContext, "");
                bp.WeaponFxPrefab = Frost.WeaponFxPrefab;
                bp.AddComponent<WeaponEnergyDamageDice>(c => {
                    c.Element = DamageEnergyType.Cold;
                    c.EnergyDamageDice = new DiceFormula() {
                        m_Rolls = 2,
                        m_Dice = DiceType.D6
                    };
                });
                bp.AddComponent<WeaponEnergyBurst>(c => {
                    c.Element = DamageEnergyType.Cold;
                    c.Dice = DiceType.D10;
                });
            });
            var SpellCastersOnslaughtAcid = Helpers.CreateBlueprint<BlueprintWeaponEnchantment>(TTTContext, "SpellCastersOnslaughtAcid", bp => {
                bp.m_EnchantmentCost = 2;
                bp.SetName(TTTContext, "Spell Casters Onslaught — Acid");
                bp.SetDescription(TTTContext, "Your weapon deals an extra 2d6 points of acid damage on a successful hit. " +
                    "This weapon deals an additional 1d10 acid damage on a sccessful crit. " +
                    "If this weapon's critical multiplier is x3 add an extra 2d10 points of acid damage instead, " +
                    "and if the multiplier is x4, add an extra 3d10 points.");
                bp.SetPrefix(TTTContext, "");
                bp.SetSuffix(TTTContext, "");
                bp.WeaponFxPrefab = Corrosive.WeaponFxPrefab;
                bp.AddComponent<WeaponEnergyDamageDice>(c => {
                    c.Element = DamageEnergyType.Acid;
                    c.EnergyDamageDice = new DiceFormula() {
                        m_Rolls = 2,
                        m_Dice = DiceType.D6
                    };
                });
                bp.AddComponent<WeaponEnergyBurst>(c => {
                    c.Element = DamageEnergyType.Acid;
                    c.Dice = DiceType.D10;
                });
            });
            var SpellCastersOnslaughtElectricity = Helpers.CreateBlueprint<BlueprintWeaponEnchantment>(TTTContext, "SpellCastersOnslaughtElectricity", bp => {
                bp.m_EnchantmentCost = 2;
                bp.SetName(TTTContext, "Spell Casters Onslaught — Electricity");
                bp.SetDescription(TTTContext, "Your weapon deals an extra 2d6 points of electricity damage on a successful hit. " +
                    "This weapon deals an additional 1d10 electricity damage on a sccessful crit. " +
                    "If this weapon's critical multiplier is x3 add an extra 2d10 points of electricity damage instead, " +
                    "and if the multiplier is x4, add an extra 3d10 points.");
                bp.SetPrefix(TTTContext, "");
                bp.SetSuffix(TTTContext, "");
                bp.WeaponFxPrefab = Shock.WeaponFxPrefab;
                bp.AddComponent<WeaponEnergyDamageDice>(c => {
                    c.Element = DamageEnergyType.Electricity;
                    c.EnergyDamageDice = new DiceFormula() {
                        m_Rolls = 2,
                        m_Dice = DiceType.D6
                    };
                });
                bp.AddComponent<WeaponEnergyBurst>(c => {
                    c.Element = DamageEnergyType.Electricity;
                    c.Dice = DiceType.D10;
                });
            });
            var SpellCastersOnslaughtSonic = Helpers.CreateBlueprint<BlueprintWeaponEnchantment>(TTTContext, "SpellCastersOnslaughtSonic", bp => {
                bp.m_EnchantmentCost = 2;
                bp.SetName(TTTContext, "Spell Casters Onslaught — Sonic");
                bp.SetDescription(TTTContext, "Your weapon deals an extra 2d6 points of sonic damage on a successful hit. " +
                    "This weapon deals an additional 1d10 sonic damage on a sccessful crit. " +
                    "If this weapon's critical multiplier is x3 add an extra 2d10 points of sonic damage instead, " +
                    "and if the multiplier is x4, add an extra 3d10 points.");
                bp.SetPrefix(TTTContext, "");
                bp.SetSuffix(TTTContext, "");
                bp.WeaponFxPrefab = Thundering.WeaponFxPrefab;
                bp.AddComponent<WeaponEnergyDamageDice>(c => {
                    c.Element = DamageEnergyType.Sonic;
                    c.EnergyDamageDice = new DiceFormula() {
                        m_Rolls = 2,
                        m_Dice = DiceType.D6
                    };
                });
                bp.AddComponent<WeaponEnergyBurst>(c => {
                    c.Element = DamageEnergyType.Sonic;
                    c.Dice = DiceType.D10;
                });
            });
            var SpellCastersOnslaughtDivine = Helpers.CreateBlueprint<BlueprintWeaponEnchantment>(TTTContext, "SpellCastersOnslaughtDivine", bp => {
                bp.m_EnchantmentCost = 2;
                bp.SetName(TTTContext, "Spell Casters Onslaught — Divine");
                bp.SetDescription(TTTContext, "Your weapon deals an extra 1d6 points of divine damage on a successful hit. " +
                    "This weapon deals an additional 1d8 divine damage on a sccessful crit. " +
                    "If this weapon's critical multiplier is x3 add an extra 2d8 points of divine damage instead, " +
                    "and if the multiplier is x4, add an extra 3d8 points.");
                bp.SetPrefix(TTTContext, "");
                bp.SetSuffix(TTTContext, "");
                bp.WeaponFxPrefab = Holy.WeaponFxPrefab;
                bp.AddComponent<WeaponEnergyDamageDice>(c => {
                    c.Element = DamageEnergyType.Divine;
                    c.EnergyDamageDice = new DiceFormula() {
                        m_Rolls = 1,
                        m_Dice = DiceType.D6
                    };
                });
                bp.AddComponent<WeaponEnergyBurst>(c => {
                    c.Element = DamageEnergyType.Divine;
                    c.Dice = DiceType.D8;
                });
            });
        }
    }
}
