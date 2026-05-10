using UnityEngine;

public class EchoGhost : MonoBehaviour
{
    public float playbackSpeed = 1f;
    public System.Action<EchoGhost> onFinished;

    EchoRecording recording;
    int frameIndex;
    int interactionIndex;
    float timeElapsed;

    public void SetRecording(EchoRecording recording)
    {
        this.recording = recording;
        Restart();
    }

    void Restart()
    {
        frameIndex = 0;
        interactionIndex = 0;
        timeElapsed = 0f;

        if (recording != null && recording.frames.Count > 0)
        {
            transform.position = recording.frames[0].position;
            transform.rotation = recording.frames[0].rotation;
        }
    }

    void Update()
    {
        if (recording == null || recording.frames.Count == 0)
            return;

        timeElapsed += Time.deltaTime * playbackSpeed;

        while (interactionIndex < recording.interactions.Count && recording.interactions[interactionIndex].time <= timeElapsed)
        {
            TriggerInteraction(recording.interactions[interactionIndex].targetId);
            interactionIndex++;
        }

        if (timeElapsed >= recording.duration)
        {
            onFinished?.Invoke(this);
            gameObject.SetActive(false);
            return;
        }

        while (frameIndex + 1 < recording.frames.Count && recording.frames[frameIndex + 1].time <= timeElapsed)
            frameIndex++;

        var currentFrame = recording.frames[frameIndex];
        if (frameIndex + 1 < recording.frames.Count)
        {
            var nextFrame = recording.frames[frameIndex + 1];
            float segment = nextFrame.time - currentFrame.time;
            float t = segment > 0f ? (timeElapsed - currentFrame.time) / segment : 0f;
            transform.position = Vector3.Lerp(currentFrame.position, nextFrame.position, t);
            transform.rotation = Quaternion.Slerp(currentFrame.rotation, nextFrame.rotation, t);
        }
        else
        {
            transform.position = currentFrame.position;
            transform.rotation = currentFrame.rotation;
        }
    }

    void TriggerInteraction(string targetId)
    {
        if (string.IsNullOrEmpty(targetId))
            return;

        if (EchoInteractable.TryGetInteractable(targetId, out var interactable))
            interactable.ReplayUse();
    }
}
