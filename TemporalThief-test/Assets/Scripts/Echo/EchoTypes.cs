using System.Collections.Generic;
using UnityEngine;

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

[System.Serializable]
public class EchoRecording
{
    public float duration;
    public List<PoseFrame> frames = new List<PoseFrame>();
    public List<InteractionEvent> interactions = new List<InteractionEvent>();

    public void EnsureCapacity(int frameCapacity = 0, int interactionCapacity = 0)
    {
        if (frameCapacity > 0 && frames.Capacity < frameCapacity)
            frames.Capacity = frameCapacity;

        if (interactionCapacity > 0 && interactions.Capacity < interactionCapacity)
            interactions.Capacity = interactionCapacity;
    }
}
