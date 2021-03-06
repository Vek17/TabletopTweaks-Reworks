using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.Utility;
using System.Linq;
using TabletopTweaks.Core.NewComponents;
using TabletopTweaks.Core.NewComponents.OwlcatReplacements;
using TabletopTweaks.Core.Utilities;
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
                PatchAzataSongToggles();
                PatchFavorableMagic();
                PatchIncredibleMight();
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
                var fixedComponent = new AzataFavorableMagicTTT();

                FavorableMagicFeature.SetComponents(
                    Helpers.Create<AzataFavorableMagicTTT>()
                //Helpers.Create<AzataFavorableMagic>()
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
