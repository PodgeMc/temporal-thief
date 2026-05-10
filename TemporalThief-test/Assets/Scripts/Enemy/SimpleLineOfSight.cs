using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SimpleLineOfSight : MonoBehaviour
{
    public float viewDistance = 10f;
    public float viewAngle = 90f;
    public float delay = 0.25f;
    public float paradoxDuration = 2f;
    public Transform eye;
    public Light visionLight;
    public Color idleColor = Color.yellow;
    public Color seenColor = Color.red;
    public TutorialManager tutorial;

    Transform player;
    bool caught;
    float cosHalfAngleSqr;

    void Start()
    {
        if (tutorial == null)
            tutorial = FindFirstObjectByType<TutorialManager>();

        UpdateCosineCache();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (visionLight != null)
            visionLight.color = idleColor;
    }

    void Update()
    {
        if (caught)
            return;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
            return;

        if (CanSeePlayer())
        {
            caught = true;
            StartCoroutine(CaughtAndReset());
        }
    }

    void UpdateCosineCache()
    {
        float halfAngleRad = viewAngle * 0.5f * Mathf.Deg2Rad;
        float cosHalfAngle = Mathf.Cos(halfAngleRad);
        cosHalfAngleSqr = cosHalfAngle * cosHalfAngle;
    }

    Vector3 GetOrigin()
    {
        return eye != null ? eye.position : transform.position + Vector3.up * 1.6f;
    }

    bool CanSeePlayer()
    {
        if (visionLight != null)
            visionLight.color = idleColor;

        Vector3 origin = GetOrigin();
        Vector3 target = player.position + Vector3.up * 1.2f;
        Vector3 direction = target - origin;
        float distSqr = direction.sqrMagnitude;
        if (distSqr > viewDistance * viewDistance)
            return false;

        float forwardDot = Vector3.Dot(transform.forward, direction);
        if (forwardDot < 0f)
            return false;

        if (forwardDot * forwardDot < cosHalfAngleSqr * distSqr)
            return false;

        if (Physics.Raycast(origin, direction.normalized, out var hit, viewDistance) && hit.collider.CompareTag("Player"))
        {
            if (visionLight != null)
                visionLight.color = seenColor;
            return true;
        }

        return false;
    }

    IEnumerator CaughtAndReset()
    {
        yield return new WaitForSeconds(delay);
        tutorial?.EchoCaught();
        yield return new WaitForSeconds(paradoxDuration);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
