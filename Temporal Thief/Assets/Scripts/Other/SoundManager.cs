using UnityEngine;

public class FootstepSound : MonoBehaviour
{
    [SerializeField] AudioSource footstep;

    public void PlayerFootstep()
    {
        footstep.Play();
    }
}
