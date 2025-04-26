using Newtonsoft.Json;

namespace Models
{
    public class AgentAction
    {
        [JsonProperty("playDialogue")]
        public int ID { get; set; }
    }
}