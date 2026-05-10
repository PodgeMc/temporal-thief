using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class PressurePlate : MonoBehaviour
{
    public DoorController door;
    public AudioSource TriggerSound;

    public TutorialManager tutorial;

    Collider plateCollider;

    // keep track of the actual things standing on the plate
    List<Collider> activators = new List<Collider>();

    bool wasPressed = false;

    void Start()
    {
        plateCollider = GetComponent<Collider>();
        plateCollider.isTrigger = true;
        TriggerSound = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsActivator(other)) return;

        if (!activators.Contains(other))
        {
            activators.Add(other);
        }

        RefreshPlate();
    }

    void OnTriggerExit(Collider other)
    {
        if (!IsActivator(other)) return;

        activators.Remove(other);
        RefreshPlate();
    }

    void LateUpdate()
    {
        if (activators.Count == 0) return;

        bool changed = false;
        for (int i = activators.Count - 1; i >= 0; i--)
        {
            Collider c = activators[i];

            if (c == null || !c.gameObject.activeInHierarchy || !IsStillOnPlate(c))
            {
                activators.RemoveAt(i);
                changed = true;
            }
        }

        if (changed)
            RefreshPlate();
    }

    void RefreshPlate()
    {
        bool isPressed = activators.Count > 0;

        if (isPressed)
        {
            door.OpenDoor();

            if (!wasPressed)
            {
                if (TriggerSound != null)
                {
                    TriggerSound.Play();
                }

                if (tutorial != null)
                {
                    tutorial.PressurePadComplete();
                }
            }
        }
        else
        {
            door.CloseDoor();

            if (wasPressed)
            {
                if (TriggerSound != null)
                {
                    TriggerSound.Play();
                }
            }
        }

        wasPressed = isPressed;
    }

    bool IsActivator(Collider other)
    {
        return other.CompareTag("Player") || other.CompareTag("Echo");
    }

    bool IsStillOnPlate(Collider other)
    {
        if (plateCollider == null || other == null) return false;

        Bounds plateBounds = plateCollider.bounds;
        Bounds otherBounds = other.bounds;

        return plateBounds.Intersects(otherBounds);
    }
}