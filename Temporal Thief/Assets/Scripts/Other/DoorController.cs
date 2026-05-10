using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Transform door;
    public Vector3 openOffset = new Vector3(0, 2.2f, 0);
    public float speed = 2f;

    public AudioSource doorAudio; // audio source attached to the door

    Vector3 closedPos;
    Vector3 openPos;
    Vector3 targetPos;
    bool isOpen = false;

    void Start()
    {
        closedPos = door.localPosition;
        openPos = closedPos + openOffset;
        targetPos = closedPos;
    }

    void Update()
    {
        if (door.localPosition != targetPos)
        {
            door.localPosition = Vector3.MoveTowards(door.localPosition, targetPos, speed * Time.deltaTime);
        }
    }

    public void OpenDoor()
    {
        if (!isOpen) // only play sound when state changes
        {
            if (doorAudio != null) doorAudio.Play(); // play opening sound
        }

        isOpen = true;
        targetPos = openPos;
    }

    public void CloseDoor()
    {
        if (isOpen) // only play sound when state changes
        {
            if (doorAudio != null) doorAudio.Play(); // play closing sound
        }

        isOpen = false;
        targetPos = closedPos;
    }

    public void SetOpen(bool open)
    {
        if (open != isOpen) // play sound only if state actually changes
        {
            if (doorAudio != null) doorAudio.Play();
        }

        isOpen = open;
        targetPos = isOpen ? openPos : closedPos;
    }
}
