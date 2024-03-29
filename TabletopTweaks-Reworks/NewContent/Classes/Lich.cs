﻿using Kingmaker.Assets.UnitLogic.Mechanics.Properties;
using Kingmaker.UnitLogic.Mechanics.Properties;
using TabletopTweaks.Core.Utilities;
using static TabletopTweaks.Reworks.Main;

namespace TabletopTweaks.Reworks.NewContent.Classes {
    static class Lich {
        public static void AddLichFeatures() {
            var LichDCProperty = Helpers.CreateBlueprint<BlueprintUnitProperty>(TTTContext, "LichDCProperty", bp => {
                /*
                bp.AddComponent<CompositePropertyGetter>(c => {
                    c.CalculationMode = CompositePropertyGetter.Mode.Sum;
                    c.Properties = new CompositePropertyGetter.ComplexProperty[] {
                        new CompositePropertyGetter.ComplexProperty {
                            Property = UnitProperty.Level,
                            Numerator = 1,
                            Denominator = 2
                        },
                        new CompositePropertyGetter.ComplexProperty {
                            Property = UnitProperty.MythicLevel,
                            Numerator = 2,
                            Denominator = 1
                        } 
                    };
                    c.Settings = new PropertySettings() {
                        m_Progression = PropertySettings.Progression.AsIs,
                        m_CustomProgression = new PropertySettings.CustomProgressionItem[0]
                    };
                });
                */
                bp.AddComponent<SimplePropertyGetter>(c => {
                    c.Property = UnitProperty.MythicLevel;
                    c.Settings = new PropertySettings() {
                        m_Progression = PropertySettings.Progression.AsIs
                    };
                });
                bp.AddComponent<SimplePropertyGetter>(c => {
                    c.Property = UnitProperty.Level;
                    c.Settings = new PropertySettings() {
                        m_Progression = PropertySettings.Progression.Div2
                    };
                });
                bp.AddComponent<MaxAttributeBonusGetter>(c => {
                    c.Settings = new PropertySettings() {
                        m_Progression = PropertySettings.Progression.AsIs
                    };
                });
                bp.BaseValue = 10;
            });
        }
    }
}
