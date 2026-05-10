using UnityEngine;

public class EchoInteractable : MonoBehaviour
{
    static readonly System.Collections.Generic.Dictionary<string, EchoInteractable> _registeredInteractables = new System.Collections.Generic.Dictionary<string, EchoInteractable>();

    public string interactionId;
    public DoorController door;
    public Transform handle;

    public float activeTime = 3f;
    public float onAngle = -45f;
    public float offAngle = 0f;
    public float rotateSpeed = 8f;

    public EchoRecorder recorder;

    bool playerInRange = false;
    bool isActive = false;
    float timer = 0f;

    void Update()
    {
        if (playerInRange && Input.GetMouseButtonDown(0))
        {
            PlayerUse();
        }

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
            float targetAngle = isActive ? onAngle : offAngle;
            Quaternion targetRot = Quaternion.Euler(targetAngle, 0f, 0f);

            handle.localRotation = Quaternion.Lerp(
                handle.localRotation,
                targetRot,
                Time.deltaTime * rotateSpeed
            );
        }
    }

    public void PlayerUse()
    {
        Activate();

        if (recorder != null && !string.IsNullOrEmpty(interactionId))
        {
            recorder.RecordInteraction(interactionId);
        }
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
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    void OnEnable()
    {
        if (!string.IsNullOrEmpty(interactionId))
            _registeredInteractables[interactionId] = this;
    }

    void OnDisable()
    {
        if (!string.IsNullOrEmpty(interactionId))
            _registeredInteractables.Remove(interactionId);
    }

    public static bool TryGetInteractable(string id, out EchoInteractable interactable)
    {
        interactable = null;
        return !string.IsNullOrEmpty(id) && _registeredInteractables.TryGetValue(id, out interactable);
    }
}
