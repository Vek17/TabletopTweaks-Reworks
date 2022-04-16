using Kingmaker.Blueprints;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TabletopTweaks.Core.Utilities;

namespace TabletopTweaks.Reworks.Config.LootTables {
    internal class LootTable {
        [JsonProperty("Items")]
        public Dictionary<string, Guid> m_Items = new Dictionary<string, Guid>();
        [JsonIgnore]
        public BlueprintItemEquipmentReference[] Items {
            get {
                return m_Items.Select(entry => BlueprintTools.GetBlueprintReference<BlueprintItemEquipmentReference>(new BlueprintGuid(entry.Value))).ToArray();
            }
        }
        private static JsonSerializerSettings cachedSettings;
        private static JsonSerializerSettings SerializerSettings {
            get {
                if (cachedSettings == null) {
                    cachedSettings = new JsonSerializerSettings {
                        CheckAdditionalContent = false,
                        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                        DefaultValueHandling = DefaultValueHandling.Include,
                        FloatParseHandling = FloatParseHandling.Double,
                        Formatting = Formatting.Indented,
                        MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        NullValueHandling = NullValueHandling.Include,
                        ObjectCreationHandling = ObjectCreationHandling.Replace,
                        StringEscapeHandling = StringEscapeHandling.Default,
                    };
                }
                return cachedSettings;
            }
        }

        internal static LootTable LoadTable(string fileName, string path) {
            JsonSerializer serializer = JsonSerializer.Create(SerializerSettings);
            var assembly = Main.TTTContext.ModEntry.Assembly;
            var resourcePath = $"{path}.{fileName}";
            LootTable result = null;

            using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
            using (StreamReader streamReader = new StreamReader(stream))
            using (JsonReader jsonReader = new JsonTextReader(streamReader)) {
                result = serializer.Deserialize<LootTable>(jsonReader);
            }
            return result;
        }
    }
}
