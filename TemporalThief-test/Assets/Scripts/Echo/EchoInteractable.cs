using System.Collections.Generic;
using UnityEngine;

public class EchoInteractable : MonoBehaviour
{
    static readonly Dictionary<string, EchoInteractable> registry = new Dictionary<string, EchoInteractable>();

    public string interactionId;
    public DoorController door;
    public EchoRecorder recorder;
    public Transform handle;
    public float activeTime = 3f;
    public float onAngle = -45f;
    public float offAngle;
    public float rotateSpeed = 8f;

    bool playerInRange;
    bool isActive;
    float timer;

    void Update()
    {
        if (playerInRange && Input.GetMouseButtonDown(0))
            Use();

        if (isActive)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                isActive = false;
                if (door != null)
                    door.SetOpen(false);
            }
        }

        if (handle != null)
        {
            var target = Quaternion.Euler(isActive ? onAngle : offAngle, 0f, 0f);
            handle.localRotation = Quaternion.Lerp(handle.localRotation, target, Time.deltaTime * rotateSpeed);
        }
    }

    public void Use()
    {
        Activate();
        if (recorder != null && !string.IsNullOrEmpty(interactionId))
            recorder.RecordInteraction(interactionId);
    }

    public void ReplayUse()
    {
        Activate();
    }

    void Activate()
    {
        isActive = true;
        timer = activeTime;
        if (door != null)
            door.SetOpen(true);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    void OnEnable()
    {
        if (!string.IsNullOrEmpty(interactionId))
            registry[interactionId] = this;
    }

    void OnDisable()
    {
        if (!string.IsNullOrEmpty(interactionId))
            registry.Remove(interactionId);
    }

    public static bool TryGetInteractable(string id, out EchoInteractable interactable)
    {
        if (string.IsNullOrEmpty(id))
        {
            interactable = null;
            return false;
        }

        return registry.TryGetValue(id, out interactable);
    }
}
