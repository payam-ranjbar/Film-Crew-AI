using System.Collections.Generic;
using System.Linq;
using System.Text;
using Agents;
using Agents.Utils;
using Models;
using UnityEngine;
using Random = UnityEngine.Random;

// ActorAction, AgentTarget, FacialExpression

namespace Test
{
    public class DebugLayout : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private ActorAgent      actor;         // your in-scene actor agent
        [SerializeField] private Material      defualtMat;         // your in-scene actor agent
        [SerializeField] private Material     highlightedMat;         // your in-scene actor agent
        [SerializeField] private GameObject[]   dummyTargets;  // targets to walk/look at

        private string _lastJson = "(click the button)";

        private ActorAction current;
        /*───────────────────────────────────────────────────────────────
     *  Mood lookup tables  (older pair-initializer syntax)
     *───────────────────────────────────────────────────────────────*/
        private readonly Dictionary<string, List<string>> _moodLines =
            new Dictionary<string, List<string>>
            {
                { "idle",  new List<string>{ "Just another day…", "Hmm.", "Alright then." } },
                { "happy", new List<string>{ "Great job!", "This is awesome!", "Love it!" } },
                { "angry", new List<string>{ "What were you thinking?!", "Unbelievable!", "Move it!" } },
                { "agree", new List<string>{ "Sounds good to me.", "I’m in.", "Absolutely." } }
            };

        private readonly Dictionary<string, FacialExpression[]> _moodToFace =
            new Dictionary<string, FacialExpression[]>
            {
                { "idle",  new[]{ FacialExpression.Neutral, FacialExpression.Bored } },
                { "happy", new[]{ FacialExpression.Happy,   FacialExpression.Excited } },
                { "angry", new[]{ FacialExpression.Angry,   FacialExpression.Cocky } },
                { "agree", new[]{ FacialExpression.Happy,   FacialExpression.Curious } }
            };

        private readonly Dictionary<string, string[]> _moodToMethod =
            new Dictionary<string, string[]>
            {
                { "idle",  new[]{ "walk_to", "look_at" } },
                { "happy", new[]{ "walk_to", "look_at"  } },
                { "angry", new[]{ "walk_to", "look_at" } },
                { "agree", new[]{ "look_at", "walk_to" } }
            };

        private void OnGUI()
        {
            GUIStyle _btnStyle = new GUIStyle(GUI.skin.button);
            GUIStyle _boxStyle = new GUIStyle(GUI.skin.box);
            // one-time style init
            if (_btnStyle.fontSize == 0)
            {
                int f = Mathf.RoundToInt(Screen.height * 0.04f);      // ~30 px @1080p
                _btnStyle.fontSize = f;
                _boxStyle.fontSize = Mathf.RoundToInt(f * 0.65f);
                _boxStyle.alignment = TextAnchor.UpperLeft;
                _boxStyle.wordWrap  = true;
            }

            float margin  = Screen.height * 0.02f;                     // 2 % border
            float btnW    = Screen.width  * 0.18f;                     // 18 % width
            float btnH    = Screen.height * 0.06f;                     // 6 % height

            if (GUI.Button(new Rect(margin, margin, btnW, btnH),
                "Random Action", _btnStyle))
            {
                RunRandom();
            }

            float boxY  = margin + btnH + Screen.height * 0.015f;      // small gap
            float boxH  = Screen.height * 0.12f;                       // 12 % of height
            float boxW  = Screen.width  * 0.28f;                       // 28 % width
            if(current != null)

                GUI.Box(new Rect(margin, boxY, boxW, boxH),  ToReadableString(current), _boxStyle);
        }

        public string ToReadableString(ActorAction a)
        {
            var sb = new StringBuilder();

            // Mood / expression
            sb.Append($"[{a.Mood ?? "neutral"} | {a.FacialExpression}]  ");

            // Method & target
            sb.Append(a.Method switch
            {
                "walk_to" => $"walk to {a.Target?.Name}",
                "look_at" => $"look at {a.Target?.Name}",
                _         => "(no move)"
            });

            // Dialogue
            if (a.PlayDialogue && !string.IsNullOrWhiteSpace(a.DialogueLine))
                sb.Append($", say \"{a.DialogueLine}\"");

            return sb.ToString();
    
        }
        /*───────────────────────────────────────────────────────────────
     *  Driver
     *───────────────────────────────────────────────────────────────*/
        private ActorAction RunRandom()
        {
            if (actor == null)
            {
                Debug.LogWarning("DebugLayout: ‘actor’ reference not set.");
                return null;
            }

            if (dummyTargets == null || dummyTargets.Length == 0)
            {
                Debug.LogWarning("DebugLayout: assign at least one GameObject to ‘dummyTargets’.");
                return null;
            }

            var action   = GenerateCoherentAction();
            // _lastJson    = JsonConvert.SerializeObject(action, Formatting.Indented);

            StopAllCoroutines();                 // stop any previous test action
            StartCoroutine(actor.Perform(action));
            return action;
        }

        private ActorAction GenerateCoherentAction()
        {
            /* 1️⃣ mood */
            var moodKeys = _moodLines.Keys.ToArray();             // ["idle","happy","angry","agree"]
            var mood     = moodKeys[Random.Range(0, moodKeys.Length)];

            /* 2️⃣ expression matching mood */
            var exprPool = _moodToFace[mood];
            var expr     = exprPool[Random.Range(0, exprPool.Length)];

            /* 3️⃣ method matching mood */
            var methodPool = _moodToMethod[mood];
            var method     = methodPool[Random.Range(0, methodPool.Length)];

            /* 4️⃣ target only if method != none */
            AgentTarget tgt = null;
            if (method != "none")
            {
                var go = dummyTargets[Random.Range(0, dummyTargets.Length)];
                foreach (var dummyTarget in dummyTargets)
                {
                    var mat = dummyTarget.GetComponentInChildren<SkinnedMeshRenderer>();
                    if(mat is null) continue;
                    mat.material = defualtMat;
                }
                var mat2 = go.GetComponentInChildren<SkinnedMeshRenderer>();
                if(mat2 != null) 
                    mat2.material = highlightedMat;           
                tgt = new AgentTarget
                {
                    Name       = go.name,
                    Transform = go.transform
                };
            }

            /* 5️⃣ dialogue likelihood by mood */
            bool mustSpeak    = mood is "happy" or "agree";
            bool playDialogue = mustSpeak ||
                                (mood == "angry" ? Random.value > 0.1f
                                    : Random.value > 0.2f);

            var line = playDialogue
                ? _moodLines[mood][Random.Range(0, _moodLines[mood].Count)]
                : "";

            /* 6️⃣ assemble */
            current =  new ActorAction
            {
                PlayDialogue     = playDialogue,
                FacialExpression = expr,
                Mood             = mood,
                Method           = method,
                Target           = tgt,
                DialogueLine     = line
            };

        
            return current;
        }
    }
}
