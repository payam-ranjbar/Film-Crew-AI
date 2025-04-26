using System;
using DG.Tweening;
using Models;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ActorAget : MonoBehaviour
{
    [SerializeField] private float bodyRotationCheckRadius;
    [SerializeField] private Rig headRotationRig;
    [SerializeField] private Transform headTarget;
    [SerializeField] private Animator animator;
    
    private ActorAction _currentAction;
    private float _walkDuration;
    private float _lookDuration;
    private Vector3 _initHeadPos;

    private void Start()
    {
        _initHeadPos = headTarget.position;
    }

    private void LookAt(AgentTarget target)
    {
        // send the target to the location 
        
        // animate weight of rig to 1
        RealizeLookDuration();
        if(CheckForBodyRotation(target.Coordinate))
            RotateBody(target);
        headTarget.DOMove(target.Coordinate, _lookDuration);


    }

    private void ResetLook()
    {
        headTarget.DOMove(_initHeadPos, _lookDuration);
    }
    private void WalkTo(AgentTarget target)
    {
        RealizeWalkDuration();
        transform.DOMove(target.Coordinate, _walkDuration);
    }

    private bool CheckForBodyRotation(Vector3 targetPosition)
    {
        var position = transform.position;
        var dirVec = new Vector3(targetPosition.x, position.y, targetPosition.z) - position;

        var dot = Vector3.Dot(transform.forward, dirVec);

        return dot <= 0;
    }
    private void RotateBody(AgentTarget target)
    {
        transform.DOLookAt(target.Coordinate, _lookDuration);
    }

    // Talk to Actor Assistant to determine the 
    private float RealizeWalkDuration()
    {
        return 5f;
    }

    private float RealizeLookDuration()
    {
        return 1f;
    }


    public void Perform()
    {
        
    }
    
}
