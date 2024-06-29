using HarmonyLib;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Blueprints;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TabletopTweaks.Core.Utilities;
using static TabletopTweaks.Reworks.Main;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.Utility;
using Kingmaker.UnitLogic.Mechanics;

namespace TabletopTweaks.Reworks.Patches {
    class Warpriest {
        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        static class BlueprintsCache_Init_Patch {
            static bool Initialized;

            static void Postfix() {
                if (Initialized) return;
                Initialized = true;
                TTTContext.Logger.LogHeader("Patching Warpriest");
                PatchBase();
                PatchArchetypes();
            }
            static void PatchBase() {
            }

            static void PatchArchetypes() {
                PatchMantisZealot();

                void PatchMantisZealot() {
                    if (TTTContext.Homebrew.Warpriest.Archetypes["MantisZealot"].IsDisabled("DeadlyFascination")) { return; }

                    var MantisZealotDeadlyFascinationFeature = BlueprintTools.GetBlueprint<BlueprintFeature>("823221f892e24568b1e5b111222d5b45");
                    var MantisZealotDeadlyFascinationAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("c2cd38949dad4756900b378767ca90c9");
                    var MantisZealotDeadlyFascinationBuff = BlueprintTools.GetBlueprintReference<BlueprintBuffReference>("fabd37ce1b244503a4ad7235327950f9");
                    var DazzledBuff = BlueprintTools.GetBlueprintReference<BlueprintBuffReference>("df6d1025da07524429afbae248845ecc");

                    MantisZealotDeadlyFascinationFeature.TemporaryContext(bp => {
                        bp.SetName(TTTContext, "Dazzling Bladework");
                        bp.SetDescription(TTTContext, "At 3rd level, a mantis zealot's deadly motions become dazzling. " +
                            "Whenever he kills an enemy with a sawtooth saber, " +
                            "other enemies in a 30-foot radius are dazzled for 2 rounds unless they succeed at a Will save " +
                            "(DC 10 + half the mantis zealot's class level + his Dexterity modifier; " +
                            "if he is wielding two sawtooth sabers, the DC increases by 2; if the red shroud is activated, " +
                            "the DC is increased by an additional 2).");
                    });
                    MantisZealotDeadlyFascinationAbility.TemporaryContext(bp => {
                        bp.SetName(MantisZealotDeadlyFascinationFeature.m_DisplayName);
                        bp.SetDescription(MantisZealotDeadlyFascinationFeature.m_Description);
                        bp.FlattenAllActions().OfType<ContextActionApplyBuff>().ForEach(c => {
                            c.m_Buff = DazzledBuff;
                            c.Permanent = false;
                            c.DurationValue = new ContextDurationValue() {
                                Rate = DurationRate.Rounds,
                                DiceCountValue = 0,
                                BonusValue = 2
                            };
                        });
                    });

                    TTTContext.Logger.LogPatch(MantisZealotDeadlyFascinationFeature);
                }
            }
        }
    }
}
