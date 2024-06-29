using System.Collections.Generic;
using System;
using TabletopTweaks.Core.Config;
using Kingmaker.Utility;

namespace TabletopTweaks.Reworks.Config {
    public class Homebrew : IUpdatableSettings {
        public bool NewSettingsOffByDefault = false;
        public MythicReworkGroup MythicReworks = new MythicReworkGroup();
        public ClassGroup Warpriest = new ClassGroup();
        public SettingGroup Feats = new SettingGroup();
        public SettingGroup MythicAbilities = new SettingGroup();
        public SettingGroup MythicFeats = new SettingGroup();

        public void Init() {
            MythicReworks.Init();
            Warpriest.SetParents();
        }

        public void OverrideSettings(IUpdatableSettings userSettings) {
            var loadedSettings = userSettings as Homebrew;
            NewSettingsOffByDefault = loadedSettings.NewSettingsOffByDefault;
            MythicReworks.LoadMythicReworkGroup(loadedSettings.MythicReworks, NewSettingsOffByDefault);
            MythicAbilities.LoadSettingGroup(loadedSettings.MythicAbilities, NewSettingsOffByDefault);
            MythicFeats.LoadSettingGroup(loadedSettings.MythicFeats, NewSettingsOffByDefault);
            Feats.LoadSettingGroup(loadedSettings.Feats, NewSettingsOffByDefault);

            Warpriest.LoadClassGroup(loadedSettings.Warpriest, NewSettingsOffByDefault);
        }
    }
    public class MythicReworkGroup : IDisableableGroup, ICollapseableGroup {
        public bool IsExpanded = true;
        ref bool ICollapseableGroup.IsExpanded() => ref IsExpanded;
        public void SetExpanded(bool value) => IsExpanded = value;
        public bool DisableAll = false;
        public bool GroupIsDisabled() => DisableAll;
        public void SetGroupDisabled(bool value) => DisableAll = value;
        public NestedSettingGroup Aeon;
        public NestedSettingGroup Angel;
        public NestedSettingGroup Azata;
        public NestedSettingGroup Demon;
        public NestedSettingGroup Lich;
        public NestedSettingGroup Trickster;
        public NestedSettingGroup Devil;
        public NestedSettingGroup GoldDragon;
        public NestedSettingGroup Legend;
        public NestedSettingGroup Swarm;

        public MythicReworkGroup() {
            Aeon = new NestedSettingGroup(this);
            Angel = new NestedSettingGroup(this);
            Azata = new NestedSettingGroup(this);
            Demon = new NestedSettingGroup(this);
            Lich = new NestedSettingGroup(this);
            Trickster = new NestedSettingGroup(this);
            Devil = new NestedSettingGroup(this);
            GoldDragon = new NestedSettingGroup(this);
            Legend = new NestedSettingGroup(this);
            Swarm = new NestedSettingGroup(this);
        }

        public void Init() {
            Aeon.Parent = this;
            Angel.Parent = this;
            Azata.Parent = this;
            Demon.Parent = this;
            Lich.Parent = this;
            Trickster.Parent = this;
            Devil.Parent = this;
            GoldDragon.Parent = this;
            Legend.Parent = this;
            Swarm.Parent = this;
        }

        public void LoadMythicReworkGroup(MythicReworkGroup group, bool frozen) {
            DisableAll = group.DisableAll;
            Aeon.LoadSettingGroup(group.Aeon, frozen);
            Angel.LoadSettingGroup(group.Angel, frozen);
            Azata.LoadSettingGroup(group.Azata, frozen);
            Demon.LoadSettingGroup(group.Demon, frozen);
            Lich.LoadSettingGroup(group.Lich, frozen);
            Trickster.LoadSettingGroup(group.Trickster, frozen);
            Devil.LoadSettingGroup(group.Devil, frozen);
            GoldDragon.LoadSettingGroup(group.GoldDragon, frozen);
            Legend.LoadSettingGroup(group.Legend, frozen);
            Swarm.LoadSettingGroup(group.Swarm, frozen);
        }
    }

    public class ClassGroup : IDisableableGroup, ICollapseableGroup {
        private bool IsExpanded = true;
        public bool DisableAll = false;
        public bool GroupIsDisabled() => DisableAll;
        public void SetGroupDisabled(bool value) => DisableAll = value;
        public NestedSettingGroup Base;
        public SortedDictionary<string, NestedSettingGroup> Archetypes = new SortedDictionary<string, NestedSettingGroup>(StringComparer.InvariantCulture);

        public ClassGroup() {
            Base = new NestedSettingGroup(this);
        }

        public void SetParents() {
            Base.Parent = this;
            Archetypes.ForEach(entry => entry.Value.Parent = this);
        }

        public void LoadClassGroup(ClassGroup group, bool frozen) {
            DisableAll = group.DisableAll;
            Base.LoadSettingGroup(group.Base, frozen);
            group.Archetypes.ForEach(entry => {
                if (Archetypes.ContainsKey(entry.Key)) {
                    Archetypes[entry.Key].LoadSettingGroup(entry.Value, frozen);
                }
            });
        }

        ref bool ICollapseableGroup.IsExpanded() {
            return ref IsExpanded;
        }

        public void SetExpanded(bool value) {
            IsExpanded = value;
        }
    }
}
