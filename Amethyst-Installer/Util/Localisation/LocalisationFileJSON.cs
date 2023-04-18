using Newtonsoft.Json;
using System.Collections.Generic;

namespace amethyst_installer_gui {
    public class LocalisationFileJSON {
        [JsonProperty("language")]
        public string Language;

        [JsonProperty("messages")]
        public List<LocalisedMessage> Messages;
    }

    public class LocalisedMessage {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("translation")]
        public string Translation;

        [JsonProperty("comment")]
        public string Comment;
    }
}
