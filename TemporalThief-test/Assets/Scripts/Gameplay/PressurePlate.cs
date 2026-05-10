using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PressurePlate : MonoBehaviour
{
    public DoorController door;
    public TutorialManager tutorial;
    public float pressedY = -0.05f;
    public float releasedY;
    public float moveSpeed = 8f;

    bool active;
    int insideCount;
    Transform plateVisual;

    void Start()
    {
        plateVisual = transform;
        var collider = GetComponent<Collider>();
        collider.isTrigger = true;
    }

    void Update()
    {
        if (door != null)
            door.SetOpen(insideCount > 0);

        if (plateVisual != null)
        {
            var pos = plateVisual.localPosition;
            pos.y = Mathf.Lerp(pos.y, insideCount > 0 ? pressedY : releasedY, Time.deltaTime * moveSpeed);
            plateVisual.localPosition = pos;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Echo"))
        {
            insideCount = Mathf.Max(insideCount + 1, 1);
            if (!active && insideCount > 0)
            {
                active = true;
                tutorial?.PressurePadComplete();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Echo"))
            insideCount = Mathf.Max(0, insideCount - 1);
    }
}
