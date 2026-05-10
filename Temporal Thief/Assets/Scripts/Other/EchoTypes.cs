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
    public List<PoseFrame> frames;
    public List<InteractionEvent> interactions;
    public float duration = 0f;

    public void EnsureCapacity(int frameCapacity = 0, int interactionCapacity = 0)
    {
        if (frames == null)
            frames = frameCapacity > 0 ? new List<PoseFrame>(frameCapacity) : new List<PoseFrame>();

        if (interactions == null)
            interactions = interactionCapacity > 0 ? new List<InteractionEvent>(interactionCapacity) : new List<InteractionEvent>();
    }
}