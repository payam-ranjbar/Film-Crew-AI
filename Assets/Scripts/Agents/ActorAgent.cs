using System.Collections;
using Agents.Utils;
using DG.Tweening;
using Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Agents
{
    public class ActorAgent : MonoBehaviour
    {
        [SerializeField] private float bodyRotationCheckRadius;
        [SerializeField] private ActorAgentAnimator animator;
        [SerializeField] private FacialExpressionProfile expressionProfile;
        [SerializeField] private Image faceImage;
        [SerializeField] private TMP_Text text;
        private ActorAction _currentAction;
        private float _walkDuration = 5;
        private bool _isWalking;
        
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
        
        private IEnumerator WalkTo(AgentTarget target)
        {
            RealizeWalkDuration();
            animator.SetTrigger("walk");
            yield return animator.LookAt(target, true);
            _isWalking = true;
            var pos = new Vector3(target.Coordinate.x - bodyRotationCheckRadius, target.Coordinate.y,
                target.Coordinate.z - bodyRotationCheckRadius);
            yield return transform.DOMove(pos, _walkDuration).SetSpeedBased().SetEase(Ease.Linear).onComplete +=
                () =>
                {
                    _isWalking = false;
                    StartCoroutine(animator.LookAt(target));
                };
        }

        // Talk to Actor Assistant to determine the 
        private float RealizeWalkDuration()
        {
            return 10f;
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
                yield return animator.LookAt(_currentAction.Target);
            }

            yield return new WaitWhile((() => _isWalking));
            
            animator.SetTrigger(_currentAction.Mood);


        }
    
    }
}
