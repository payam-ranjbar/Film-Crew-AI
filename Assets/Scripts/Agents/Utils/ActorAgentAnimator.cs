using UnityEngine;

namespace Agents.Utils
{
    public class ActorAgentAnimator : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        
        public void SetTrigger(string triggerName, int layerIndex = 0)
        {
            if (animator == null || string.IsNullOrEmpty(triggerName))
                return;

            var hasTrigger = HasTrigger(triggerName);

            if (!hasTrigger) triggerName = "idle";
            
            if (IsAnimPlaying(triggerName, layerIndex)) return;

            animator.ResetTrigger(triggerName);                     
            animator.SetTrigger(triggerName);
        }

        private bool IsAnimPlaying(string triggerName, int layerIndex)
        {
            var stateTag = triggerName;
            int targetHash = Animator.StringToHash(stateTag);
            if (animator.GetCurrentAnimatorStateInfo(layerIndex).tagHash == targetHash)
                return true;
            return false;
        }

        private bool HasTrigger(string triggerName)
        {
            for (var i = 0; i < animator.parameters.Length; i++)
            {
                var p = animator.parameters[i];
                if (p.type == AnimatorControllerParameterType.Trigger &&
                    p.name == triggerName)
                {
                    return true;
                }
            }

            return false;
        }
    }
}