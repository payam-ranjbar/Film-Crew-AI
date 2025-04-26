using Newtonsoft.Json;

namespace Models
{
     
    public class ActorAction : AgentAction
    {
        [JsonProperty("playDialogue")]
        public bool PlayDialogue { get; set; }
        
        [JsonProperty("facialExpression")]
        public string FacialExpression { get; set; }   // null = leave unchanged
        
        [JsonProperty("mood")]
        public string Mood { get; set; }               // optional high-level tag
        
        /// <summary> "walk_to", "look_at", or "none" </summary>
        [JsonProperty("method")]
        public string Method { get; set; }
        
        /// <summary> Name of prop / actor this action refers to (may be null) </summary>
        [JsonProperty("target")]
        public string Target { get; set; }
        
        [JsonProperty("dialogueLine")]
        public string DialogueLine { get; set; }
    }
}