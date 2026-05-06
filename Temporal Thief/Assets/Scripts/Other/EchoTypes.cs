// EchoTypes.cs
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct PoseFrame
{
    public float time;
    public Vector3 position;
    public Quaternion rotation;
}

[System.Serializable]
public struct InteractionEvent
{
    public float time;
    public string targetId;
}

public class EchoRecording
{
    public List<PoseFrame> frames = new List<PoseFrame>();
    public List<InteractionEvent> interactions = new List<InteractionEvent>();
    public float duration = 0f;
}