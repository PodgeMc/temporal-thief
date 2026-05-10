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
    public Transform buttonVisual;
    public float pressedY = -0.1f;
    public float releasedY;
    public float buttonSpeed = 10f;
    public Transform leverHandle;
    public float leverOnAngle = -45f;
    public float leverOffAngle;
    public float leverSpeed = 8f;
    public float returnTime = 3f;
    public float activationThreshold = 0.01f;

    bool playerInRange;
    bool isHeld;
    int actorsInside;
    float charge;
    bool isOpen;

    void Awake()
    {
        var collider = GetComponent<Collider>();
        collider.isTrigger = true;
    }

    void Update()
    {
        bool shouldOpen = false;

        if (mode == Mode.HoldButton)
        {
            if (playerInRange && Input.GetMouseButtonDown(0))
                isHeld = true;

            if (isHeld && Input.GetMouseButtonUp(0))
                isHeld = false;

            shouldOpen = isHeld;
        }
        else
        {
            if (actorsInside > 0)
                charge = 1f;
            else
                charge = Mathf.MoveTowards(charge, 0f, Time.deltaTime / returnTime);

            shouldOpen = charge > activationThreshold;
        }

        if (door != null)
            door.SetOpen(shouldOpen);

        UpdateVisuals(shouldOpen);
    }

    void UpdateVisuals(bool active)
    {
        if (buttonVisual != null)
        {
            var pos = buttonVisual.localPosition;
            pos.y = Mathf.Lerp(pos.y, active ? pressedY : releasedY, Time.deltaTime * buttonSpeed);
            buttonVisual.localPosition = pos;
        }

        if (leverHandle != null)
        {
            var target = Quaternion.Euler(active ? leverOnAngle : leverOffAngle, 0f, 0f);
            leverHandle.localRotation = Quaternion.Lerp(leverHandle.localRotation, target, Time.deltaTime * leverSpeed);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsActivator(other))
            return;

        if (mode == Mode.HoldButton)
        {
            if (other.CompareTag("Player"))
                playerInRange = true;
        }
        else
        {
            actorsInside++;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!IsActivator(other))
            return;

        if (mode == Mode.HoldButton)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = false;
                isHeld = false;
            }
        }
        else
        {
            actorsInside = Mathf.Max(0, actorsInside - 1);
        }
    }

    bool IsActivator(Collider other)
    {
        return other.CompareTag("Player") || other.CompareTag("Echo");
    }
}
