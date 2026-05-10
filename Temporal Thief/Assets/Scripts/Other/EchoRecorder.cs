using UnityEngine;

public class EchoRecorder : MonoBehaviour
{
    public Transform target;
    public float sampleRate = 20f;
    public float maxDuration = 60f;
    public int maxFrames = 1000;

    public bool IsRecording { get; private set; }

    EchoRecording current;
    float t = 0f, timer = 0f;

    public float CurrentTime => t;

    public void StartRecording()
    {
        if (IsRecording) return;

        current = new EchoRecording();
        int estimatedFrameCount = Mathf.Clamp(Mathf.CeilToInt(maxDuration * sampleRate) + 1, 1, maxFrames);
        current.EnsureCapacity(estimatedFrameCount, 16);
        t = 0;
        timer = 0;
        IsRecording = true;
    }

    public EchoRecording StopAndGetRecording()
    {
        if (!IsRecording) return null;

        IsRecording = false;

        if (current == null)
            current = new EchoRecording();

        current.EnsureCapacity();

        if (current.frames.Count == 0 && target != null)
        {
            current.frames.Add(new PoseFrame
            {
                time = t,
                position = target.position,
                rotation = target.rotation
            });
        }

        current.duration = t;
        return current;
    }

    void Update()
    {
        if (!IsRecording || target == null) return;

        t += Time.deltaTime;
        timer += Time.deltaTime;

        float step = 1f / sampleRate;

        while (timer >= step)
        {
            timer -= step;

            current.frames.Add(new PoseFrame
            {
                time = t,
                position = target.position,
                rotation = target.rotation
            });

            if (current.frames.Count >= maxFrames)
            {
                StopAndGetRecording();
                return;
            }
        }

        if (t >= maxDuration)
            StopAndGetRecording();
    }

    public void RecordInteraction(string targetId)
    {
        if (!IsRecording || current == null || string.IsNullOrEmpty(targetId)) return;

        current.interactions.Add(new InteractionEvent
        {
            time = t,
            targetId = targetId
        });

        Debug.Log("Recorded interaction: " + targetId + " at time " + t);
    }
}