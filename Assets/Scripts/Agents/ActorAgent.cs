using System.Collections;
using Agents.Utils;
using DG.Tweening;
using Models;
using TMPro;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

namespace Agents
{
    public class ActorAgent : MonoBehaviour
    {
        [SerializeField] private float bodyRotationCheckRadius;
        [SerializeField] private Rig headRotationRig;
        [SerializeField] private Transform headTarget;
        [SerializeField] private Animator animator;
        [SerializeField] private FacialExpressionProfile expressionProfile;
        [SerializeField] private Image faceImage;
        [SerializeField] private TMP_Text text;
        private ActorAction _currentAction;
        private float _walkDuration = 5;
        private float _lookDuration = 1;
        private Vector3 _initHeadPos;

        private void Start()
        {
            _initHeadPos = headTarget.position;
        }

        private IEnumerator LookAt(AgentTarget target, bool walk = false)
        {
            // send the target to the location 
        
            // animate weight of rig to 1
            RealizeLookDuration();
            if(walk || CheckForBodyRotation(target.Coordinate))
                yield return RotateBody(target);
            yield return headTarget.DOMove(target.Coordinate, _lookDuration);


        }


        private IEnumerator PlayDialogue(string line)
        {
            text.text = line;
            yield return null;
        }
        private void SetExpression(FacialExpression expression)
        {
            var sprite = expressionProfile.GetSprite(expression);
            faceImage.sprite = sprite;
        }
        private IEnumerator ResetLook()
        {
            yield return headTarget.DOMove(_initHeadPos, _lookDuration);
        }

        private bool walking;
        private IEnumerator WalkTo(AgentTarget target)
        {
            RealizeWalkDuration();
            animator.SetTrigger("walk");
            yield return LookAt(target, true);
            walking = true;
            var pos = new Vector3(target.Coordinate.x - bodyRotationCheckRadius, target.Coordinate.y,
                target.Coordinate.z - bodyRotationCheckRadius);
            yield return transform.DOMove(pos, _walkDuration).SetSpeedBased().SetEase(Ease.Linear).onComplete +=
                () =>
                {
                    walking = false;
                    StartCoroutine(LookAt(target));
                };
        }

        private bool CheckForBodyRotation(Vector3 targetPosition)
        {
            var position = transform.position;
            var dirVec = new Vector3(targetPosition.x, position.y, targetPosition.z) - position;

            var dot = Vector3.Dot(transform.forward, dirVec);

            return dot <= 0;
        }
        private IEnumerator RotateBody(AgentTarget target)
        {
            yield return transform.DOLookAt(target.Coordinate, _lookDuration);
        }

        // Talk to Actor Assistant to determine the 
        private float RealizeWalkDuration()
        {
            return 10f;
        }

        private float RealizeLookDuration()
        {
            return 1f;
        }

        private IEnumerator PlayTriggerSafe(
            string triggerName,
            int layerIndex     = 0)
        {
            if (animator == null || string.IsNullOrEmpty(triggerName))
                yield break;

            bool hasTrigger = false;
            foreach (var p in animator.parameters)
            {
                if (p.type == AnimatorControllerParameterType.Trigger &&
                    p.name == triggerName)
                {
                    hasTrigger = true;
                    break;
                }
            }

            if (!hasTrigger) triggerName = "idle";


            var stateTag = triggerName;                          
            int targetHash = Animator.StringToHash(stateTag);
            if (animator.GetCurrentAnimatorStateInfo(layerIndex).tagHash == targetHash)
                yield break;

            animator.ResetTrigger(triggerName);                     
            animator.SetTrigger(triggerName);
        }
        public IEnumerator Perform(ActorAction action)
        {
            _currentAction = action;
            if (_currentAction.PlayDialogue)
                yield return PlayDialogue(_currentAction.DialogueLine);
            SetExpression(_currentAction.FacialExpression);

            if (_currentAction.Method.ToLower() == "walk_to")
            {
                yield return WalkTo(_currentAction.Target);
            }
        
            if (_currentAction.Method.ToLower() == "look_at")
            {
                yield return LookAt(_currentAction.Target);
            }

            yield return new WaitWhile((() => walking));
            yield return PlayTriggerSafe(_currentAction.Mood);


        }
    
    }
}
