using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Transform doorTransform;
    public Vector3 openOffset = new Vector3(0f, 2.2f, 0f);
    public float speed = 2f;
    public AudioSource doorAudio;

    Vector3 closedPosition;
    Vector3 openPosition;
    Vector3 targetPosition;
    bool isOpen;

    void Start()
    {
        if (doorTransform == null)
            doorTransform = transform;

        closedPosition = doorTransform.localPosition;
        openPosition = closedPosition + openOffset;
        targetPosition = closedPosition;
    }

    void Update()
    {
        if (doorTransform.localPosition != targetPosition)
            doorTransform.localPosition = Vector3.MoveTowards(doorTransform.localPosition, targetPosition, speed * Time.deltaTime);
    }

    public void SetOpen(bool open)
    {
        if (open == isOpen)
            return;

        isOpen = open;
        targetPosition = isOpen ? openPosition : closedPosition;

        if (doorAudio != null)
            doorAudio.Play();
    }
}
