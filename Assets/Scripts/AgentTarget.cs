using System;
using UnityEngine;

[Serializable]
public class AgentTarget
{
    [SerializeField] private string name;
    [SerializeField] private Transform transform;

    public string Name
    {
        get => name;
        set => name = value;
    }

    public Vector3 Coordinate => transform.position;

    public Transform Transform
    {
        get => transform;
        set => transform = value;
    }
}