using UnityEngine;

public class EchoGhost : MonoBehaviour
{
    public float playbackSpeed = 1f;

    EchoRecording rec;
    int index = 0;
    int interactionIndex = 0;
    float t = 0;

    public System.Action<EchoGhost> onFinished;

    public void SetRecording(EchoRecording r)
    {
        rec = r;
        Restart();
    }

    public void Restart()
    {
        index = 0;
        interactionIndex = 0;
        t = 0;

        if (rec != null && rec.frames.Count > 0)
        {
            transform.position = rec.frames[0].position;
            transform.rotation = rec.frames[0].rotation;
        }
    }

    void Update()
    {
        if (rec == null || rec.frames.Count == 0) return;

        t += Time.deltaTime * playbackSpeed;

        while (rec.interactions != null &&
               interactionIndex < rec.interactions.Count &&
               rec.interactions[interactionIndex].time <= t)
        {
            TriggerInteraction(rec.interactions[interactionIndex].targetId);
            interactionIndex++;
        }

        if (t >= rec.duration)
        {
            onFinished?.Invoke(this);
            Destroy(gameObject);
            return;
        }

        while (index + 1 < rec.frames.Count && rec.frames[index + 1].time <= t)
            index++;

        var a = rec.frames[index];

        if (index + 1 < rec.frames.Count)
        {
            var b = rec.frames[index + 1];
            float dt = b.time - a.time;
            if (dt <= 0) dt = 0.0001f;

            float f = (t - a.time) / dt;
            transform.position = Vector3.Lerp(a.position, b.position, f);
            transform.rotation = Quaternion.Slerp(a.rotation, b.rotation, f);
        }
        else
        {
            transform.position = a.position;
            transform.rotation = a.rotation;
        }
    }

    void TriggerInteraction(string targetId)
    {
        if (string.IsNullOrEmpty(targetId)) return;

        if (EchoInteractable.TryGetInteractable(targetId, out var interactable))
        {
            interactable.ReplayUse();
        }
    }
}