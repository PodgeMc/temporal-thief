using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SlowReturnLever : MonoBehaviour
{
    public DoorController door; // door this lever controls

    public float returnTime = 3f; // how long before it switches off
    public Transform leverHandle; // optional visual handle

    public float onAngle = -45f; // angle when ON
    public float offAngle = 0f; // angle when OFF
    public float rotateSpeed = 8f; // visual smoothness

    int insideCount = 0; // how many things are holding the lever
    float charge = 0f; // 0 = off, 1 = on

    void Awake()
    {
        GetComponent<Collider>().isTrigger = true; // trigger zone
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsActivator(other)) return;
        insideCount++; // something entered
    }

    void OnTriggerExit(Collider other)
    {
        if (!IsActivator(other)) return;
        insideCount--; // something left
        if (insideCount < 0) insideCount = 0; // safety
    }

    void Update()
    {
        bool held = insideCount > 0; // is someone holding it?

        if (held) charge = 1f; // fully on
        else charge = Mathf.MoveTowards(charge, 0f, Time.deltaTime / returnTime); // slowly off

        if (door) door.SetOpen(charge > 0.01f); // door open while charged

        if (leverHandle)
        {
            float angle = Mathf.Lerp(offAngle, onAngle, charge);
            Quaternion target = Quaternion.Euler(angle, 0f, 0f);
            leverHandle.localRotation = Quaternion.Lerp(
                leverHandle.localRotation,
                target,
                Time.deltaTime * rotateSpeed
            );
        }
    }

    bool IsActivator(Collider other)
    {
        return other.CompareTag("Player") || other.CompareTag("Echo");
    }
}
