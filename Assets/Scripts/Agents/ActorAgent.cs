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
        [SerializeField] private ActorAgentAnimator animator;
        [SerializeField] private ActorAgentExpression expression;
        [SerializeField] private ActorAgentMotor motor;
        [SerializeField] private TMP_Text text;
        private ActorAction _currentAction;

        
        private IEnumerator PlayDialogue(string line)
        {
            text.text = line;
            yield return null;
        }

        
        public IEnumerator Perform(ActorAction action)
        {
            _currentAction = action;
            if (_currentAction.PlayDialogue)
                yield return PlayDialogue(_currentAction.DialogueLine);
            
            expression.SetExpression(_currentAction.FacialExpression);

            if (_currentAction.Method.ToLower() == "walk_to")
            {
                yield return motor.WalkTo(_currentAction.Target, animator);
            }
        
            if (_currentAction.Method.ToLower() == "look_at")
            {
                yield return animator.LookAt(_currentAction.Target);
            }

            yield return new WaitWhile((() => motor.IsWalking));
            
            animator.SetTrigger(_currentAction.Mood);


        }
    
    }
}
