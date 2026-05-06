using UnityEngine;

public class EchoAnim : MonoBehaviour
{
    public Animator anim;
    public Transform groundCheck;
    public float groundDistance = 0.25f;
    public LayerMask groundMask;

    Vector3 lastPos;
    bool firstFrame = true;

    void Start()
    {
        if (anim == null) anim = GetComponentInChildren<Animator>();
        lastPos = transform.position;
    }

    void Update()
    {
        float dt = Time.deltaTime;
        if (dt <= 0f) return;

        Vector3 delta = transform.position - lastPos;
        if (firstFrame) { delta = Vector3.zero; firstFrame = false; }

        float speed = new Vector3(delta.x, 0f, delta.z).magnitude / dt;

        bool grounded = Physics.Raycast(groundCheck.position, Vector3.down, groundDistance, groundMask);
        bool falling = !grounded && (delta.y / dt) < -0.05f;

        anim.SetFloat("Speed", speed);
        anim.SetBool("Islanded", grounded);   // keep your original exact name
        anim.SetBool("isFalling", falling);

        lastPos = transform.position;
    }
}
