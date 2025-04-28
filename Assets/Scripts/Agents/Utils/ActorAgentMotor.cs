using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Agents.Utils
{
    public class ActorAgentMotor : MonoBehaviour
    {
        [SerializeField] private float bodyRotationCheckRadius;
        [SerializeField] private Transform actorTransform;

        private float _walkSpeed = 5;
        private bool _isWalking;

        public bool IsWalking => _isWalking;

        // Talk to Actor Assistant to determine the 
        private float RealizeWalkSpeed()
        {
            return 10f;
        }

        public IEnumerator WalkTo(AgentTarget target, ActorAgentAnimator animator)
        {
            RealizeWalkSpeed();
            animator.SetTrigger("walk");
            yield return animator.LookAt(target, true);
            _isWalking = true;
            var pos = new Vector3(target.Coordinate.x - bodyRotationCheckRadius, target.Coordinate.y,
                target.Coordinate.z - bodyRotationCheckRadius);
            yield return actorTransform.DOMove(pos, _walkSpeed).SetSpeedBased().SetEase(Ease.Linear).onComplete +=
                () =>
                {
                    _isWalking = false;
                    StartCoroutine(animator.LookAt(target));
                };
        }

    }
}