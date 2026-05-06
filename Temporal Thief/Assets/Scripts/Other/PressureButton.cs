using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PressureButton : MonoBehaviour
{
    public DoorController door;
    public Transform buttonVisual;

    public float pressedY = -0.1f;
    public float releasedY = 0f;
    public float moveSpeed = 10f;

    bool playerInRange = false;
    bool isHeld = false;

    void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void Update()
    {
        if (playerInRange && Input.GetMouseButtonDown(0))
        {
            isHeld = true;

            if (door != null)
                door.SetOpen(true);
        }

        if (isHeld && Input.GetMouseButtonUp(0))
        {
            isHeld = false;

            if (door != null)
                door.SetOpen(false);
        }

        if (buttonVisual != null)
        {
            Vector3 pos = buttonVisual.localPosition;
            float targetY = isHeld ? pressedY : releasedY;
            pos.y = Mathf.Lerp(pos.y, targetY, Time.deltaTime * moveSpeed);
            buttonVisual.localPosition = pos;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            isHeld = false;

            if (door != null)
                door.SetOpen(false);
        }
    }
}