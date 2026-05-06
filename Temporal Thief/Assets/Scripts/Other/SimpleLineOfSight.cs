using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // <-- needed for reloading the scene

public class SimpleLineOfSight : MonoBehaviour
{
    public EchoLoopManager loopManager;

    public float viewDistance = 10f;
    public float viewAngle = 90f;

    // Optional: small reaction delay right after the player is first spotted
    public float delay = 0.25f;

    // Total time to wait (rewind/paradox duration) before resetting the scene.
    // Increase this to give yourself more time for animations, VFX, SFX, UI fades, etc.
    public float paradoxDuration = 2f;

    public Transform eye;

    public bool drawDebug = true;

    // Drag your Spot Light here (the Light component)
    public Light visionLight;

    // Colors for the light
    public Color idleColor = Color.yellow;
    public Color seenColor = Color.red;

    Transform player;
    bool triggered = false;

    public TutorialManager tutorial;

    void Start()
    {
        if (!loopManager)
        {
            loopManager = FindObjectOfType<EchoLoopManager>();
        }

        if (tutorial == null)
        {
            tutorial = FindObjectOfType<TutorialManager>();
        }

        FindPlayer();

        // set starting light color
        if (visionLight != null)
        {
            visionLight.color = idleColor;
        }
    }

    void Update()
    {
        if (triggered) return;

        if (!player)
        {
            FindPlayer();
            return;
        }

        // draw forward debug line
        if (drawDebug)
        {
            Debug.DrawRay(GetOrigin(), transform.forward * viewDistance, Color.yellow);
        }

        bool canSee = CanSeePlayer();

        // change the spotlight color
        if (visionLight != null)
        {
            visionLight.color = canSee ? seenColor : idleColor;
        }

        if (canSee)
        {
            triggered = true;
            StartCoroutine(CaughtAndReset());
        }
    }

    void FindPlayer()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;
    }

    Vector3 GetOrigin()
    {
        if (eye) return eye.position;
        return transform.position + Vector3.up * 1.6f;
    }

    bool CanSeePlayer()
    {
        Vector3 origin = GetOrigin();
        Vector3 target = player.position + Vector3.up * 1.2f;
        Vector3 direction = target - origin;

        // distance check
        if (direction.magnitude > viewDistance)
            return false;

        // angle check
        if (Vector3.Angle(transform.forward, direction) > viewAngle / 2f)
            return false;

        // debug line to player
        if (drawDebug)
        {
            Debug.DrawLine(origin, target, Color.green);
        }

        // raycast check
        RaycastHit hit;
        if (Physics.Raycast(origin, direction.normalized, out hit, viewDistance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }

    IEnumerator CaughtAndReset()
    {
        // Optional: short delay after detection (enemy "reaction time")
        yield return new WaitForSeconds(delay);

        // Optional fairness check: if the player escaped before the reaction finished, don't reset.
        // Uncomment if you want the player to only be caught if still visible after 'delay'.
        // if (!player || !CanSeePlayer()) { triggered = false; yield break; }

        Debug.Log("You've been caught");

        // ============================================
        // TELL TUTORIAL THIS STEP IS COMPLETE
        // ============================================
        if (tutorial == null)
        {
            tutorial = FindObjectOfType<TutorialManager>();
        }

        if (tutorial != null)
        {
            tutorial.EchoCaught();
            Debug.Log("Tutorial step completed: EchoCaught()");
        }
        else
        {
            Debug.LogWarning("SimpleLineOfSight: TutorialManager not found.");
        }

        // ============================================
        // PARADOX / REWIND EFFECTS START HERE
        // ============================================

        // rewind animation here (enemy, player, or camera)
        // animator.SetTrigger("ParadoxRewind");

        // VFX here (particles, post-processing, screen glitch, etc.)
        // paradoxVFX.Play();

        // SFX here (rewind sound)
        // audioSource.PlayOneShot(rewindClip);

        // disable player controls here so they can't move during rewind
        //var controller = player.GetComponent<PlayerController>();
        //if (controller) controller.enabled = false;

        // Wait for however long you want your paradox/rewind sequence to run
        yield return new WaitForSeconds(paradoxDuration);

        // ============================================
        // RESET SCENE
        // ============================================
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }

    void OnDrawGizmos()
    {
        Vector3 origin = eye ? eye.position : transform.position + Vector3.up * 1.6f;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, viewDistance);

        Vector3 left = Quaternion.Euler(0, -viewAngle / 2f, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, viewAngle / 2f, 0) * transform.forward;

        Gizmos.DrawRay(origin, left * viewDistance);
        Gizmos.DrawRay(origin, right * viewDistance);
    }
}