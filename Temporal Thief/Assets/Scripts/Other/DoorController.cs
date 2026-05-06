using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Transform door;
    public Vector3 openOffset = new Vector3(0, 2.2f, 0);
    public float speed = 2f;

    public AudioSource doorAudio; // audio source attached to the door

    Vector3 closedPos;
    Vector3 openPos;
    bool isOpen = false;

    void Start()
    {
        closedPos = door.localPosition;
        openPos = closedPos + openOffset;
    }

    void Update()
    {
        if (isOpen)
        {
            door.localPosition = Vector3.MoveTowards(door.localPosition, openPos, speed * Time.deltaTime);
        }
        else
        {
            door.localPosition = Vector3.MoveTowards(door.localPosition, closedPos, speed * Time.deltaTime);
        }
    }

    public void OpenDoor()
    {
        if (!isOpen) // only play sound when state changes
        {
            if (doorAudio != null) doorAudio.Play(); // play opening sound
        }

        isOpen = true;
    }

    public void CloseDoor()
    {
        if (isOpen) // only play sound when state changes
        {
            if (doorAudio != null) doorAudio.Play(); // play closing sound
        }

        isOpen = false;
    }

    public void SetOpen(bool open)
    {
        if (open != isOpen) // play sound only if state actually changes
        {
            if (doorAudio != null) doorAudio.Play();
        }

        isOpen = open;
    }
}
