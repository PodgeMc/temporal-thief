using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DoorTrigger : MonoBehaviour
{
    public enum Mode
    {
        HoldButton,
        TimedLever
    }

    public Mode mode = Mode.HoldButton;
    public DoorController door;

    [Header("Button Settings")]
    public Transform buttonVisual;
    public float pressedY = -0.1f;
    public float releasedY = 0f;
    public float buttonMoveSpeed = 10f;

    [Header("Lever Settings")]
    public Transform leverHandle;
    public float onAngle = -45f;
    public float offAngle = 0f;
    public float leverRotateSpeed = 8f;
    public float returnTime = 3f;

    [Header("Common Settings")]
    public float activationThreshold = 0.01f;

    bool playerInRange = false;
    bool isHeld = false;
    int insideCount = 0;
    float charge = 0f;
    bool isOpen = false;

    void Awake()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    void Update()
    {
        bool shouldOpen = false;

        if (mode == Mode.HoldButton)
        {
            if (playerInRange && Input.GetMouseButtonDown(0))
            {
                isHeld = true;
            }

            if (isHeld && Input.GetMouseButtonUp(0))
            {
                isHeld = false;
            }

            shouldOpen = isHeld;
        }
        else // TimedLever
        {
            bool held = insideCount > 0;
            if (held)
            {
                charge = 1f;
            }
            else
            {
                charge = Mathf.MoveTowards(charge, 0f, Time.deltaTime / returnTime);
            }

            shouldOpen = charge > activationThreshold;
        }

        SetDoorState(shouldOpen);
        UpdateVisuals(shouldOpen);
    }

    void SetDoorState(bool open)
    {
        if (door == null) return;

        if (open != isOpen)
        {
            door.SetOpen(open);
            isOpen = open;
        }
    }

    void UpdateVisuals(bool isActive)
    {
        if (buttonVisual != null)
        {
            Vector3 pos = buttonVisual.localPosition;
            float targetY = isActive ? pressedY : releasedY;
            float newY = Mathf.Lerp(pos.y, targetY, Time.deltaTime * buttonMoveSpeed);

            if (Mathf.Abs(newY - pos.y) > 0.0001f)
            {
                pos.y = newY;
                buttonVisual.localPosition = pos;
            }
            else if (Mathf.Abs(pos.y - targetY) > 0.0001f)
            {
                pos.y = targetY;
                buttonVisual.localPosition = pos;
            }
        }

        if (leverHandle != null)
        {
            float targetAngle = isActive ? onAngle : offAngle;
            Quaternion targetRot = Quaternion.Euler(targetAngle, 0f, 0f);
            leverHandle.localRotation = Quaternion.Lerp(
                leverHandle.localRotation,
                targetRot,
                Time.deltaTime * leverRotateSpeed
            );
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsActivator(other)) return;

        if (mode == Mode.HoldButton)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = true;
            }
        }
        else
        {
            insideCount++;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!IsActivator(other)) return;

        if (mode == Mode.HoldButton)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = false;
                isHeld = false;
                SetDoorState(false);
            }
        }
        else
        {
            insideCount--;
            if (insideCount < 0) insideCount = 0;
        }
    }

    bool IsActivator(Collider other)
    {
        return other.CompareTag("Player") || other.CompareTag("Echo");
    }
}
