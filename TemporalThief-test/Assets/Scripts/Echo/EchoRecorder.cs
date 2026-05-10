using UnityEngine;

public class EchoRecorder : MonoBehaviour
{
    public Transform target;
    public float sampleRate = 20f;
    public float maxDuration = 60f;
    public int maxFrames = 1000;

    public bool IsRecording { get; private set; }

    EchoRecording currentRecording;
    float timeElapsed;
    float timer;

    public void StartRecording()
    {
        if (IsRecording)
            return;

        currentRecording = new EchoRecording();
        currentRecording.EnsureCapacity(Mathf.Clamp(Mathf.CeilToInt(maxDuration * sampleRate) + 1, 1, maxFrames), 8);
        timeElapsed = 0f;
        timer = 0f;
        IsRecording = true;
    }

    public EchoRecording StopAndGetRecording()
    {
        if (!IsRecording)
            return null;

        IsRecording = false;
        if (currentRecording == null)
            currentRecording = new EchoRecording();

        if (currentRecording.frames.Count == 0 && target != null)
        {
            currentRecording.frames.Add(new PoseFrame
            {
                time = timeElapsed,
                position = target.position,
                rotation = target.rotation
            });
        }

        currentRecording.duration = timeElapsed;
        return currentRecording;
    }

    void Update()
    {
        if (!IsRecording || target == null)
            return;

        timeElapsed += Time.deltaTime;
        timer += Time.deltaTime;

        float step = 1f / sampleRate;
        while (timer >= step)
        {
            timer -= step;
            currentRecording.frames.Add(new PoseFrame
            {
                time = timeElapsed,
                position = target.position,
                rotation = target.rotation
            });

            if (currentRecording.frames.Count >= maxFrames)
            {
                StopAndGetRecording();
                return;
            }
        }

        if (timeElapsed >= maxDuration)
            StopAndGetRecording();
    }

    public void RecordInteraction(string targetId)
    {
        if (!IsRecording || string.IsNullOrEmpty(targetId) || currentRecording == null)
            return;

        currentRecording.interactions.Add(new InteractionEvent
        {
            time = timeElapsed,
            targetId = targetId
        });
    }
}
