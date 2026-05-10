using UnityEngine;

public class EchoAnim : MonoBehaviour
{
    public Animator anim;
    public Transform groundCheck;
    public float groundDistance = 0.25f;
    public LayerMask groundMask;

    Vector3 lastPos;
    bool firstFrame = true;

    float lastSpeed = -1f;
    bool lastGrounded = false;
    bool lastFalling = false;

    void Start()
    {
        if (anim == null) anim = GetComponentInChildren<Animator>();
        lastPos = transform.position;
    }

    void Update()
    {
        float dt = Time.deltaTime;
        if (dt <= 0f || anim == null) return;

        Vector3 delta = transform.position - lastPos;
        if (firstFrame) { delta = Vector3.zero; firstFrame = false; }

        Vector3 horizontalDelta = new Vector3(delta.x, 0f, delta.z);
        float speed = horizontalDelta.sqrMagnitude <= 0f ? 0f : Mathf.Sqrt(horizontalDelta.sqrMagnitude) / dt;

        bool grounded = Physics.Raycast(groundCheck.position, Vector3.down, groundDistance, groundMask);
        bool falling = !grounded && (delta.y / dt) < -0.05f;

        if (!Mathf.Approximately(speed, lastSpeed))
        {
            anim.SetFloat("Speed", speed);
            lastSpeed = speed;
        }

        if (grounded != lastGrounded)
        {
            anim.SetBool("Islanded", grounded);   // keep your original exact name
            lastGrounded = grounded;
        }

        if (falling != lastFalling)
        {
            anim.SetBool("isFalling", falling);
            lastFalling = falling;
        }

        lastPos = transform.position;
    }
}
