using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Agents.Utils
{
    public class ActorAgentAnimator : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private float bodyRotationCheckRadius;
        [SerializeField] private Rig headRotationRig;
        [SerializeField] private Transform headTarget;
        private float _lookDuration = 1;
        private Vector3 _initHeadPos;


        private void Start()
        {
            _initHeadPos = headTarget.position;
        }
        public IEnumerator LookAt(AgentTarget target, bool walk = false)
        {
            // send the target to the location 
        
            // animate weight of rig to 1
            RealizeLookDuration();
            if(walk || CheckForBodyRotation(target.Coordinate))
                yield return RotateBody(target);
            yield return headTarget.DOMove(target.Coordinate, _lookDuration);


        }

        public IEnumerator RotateBody(AgentTarget target)
        {
            yield return transform.DOLookAt(target.Coordinate, _lookDuration);
        }

        private IEnumerator ResetLook()
        {
            yield return headTarget.DOMove(_initHeadPos, _lookDuration);
        }

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

        private bool CheckForBodyRotation(Vector3 targetPosition)
        {
            var position = transform.position;
            var dirVec = new Vector3(targetPosition.x, position.y, targetPosition.z) - position;

            var dot = Vector3.Dot(transform.forward, dirVec);

            return dot <= 0;
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
        
        private float RealizeLookDuration()
        {
            return 1f;
        }
    }
}