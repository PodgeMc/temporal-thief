using UnityEngine;

public class SimplePlayerController : MonoBehaviour
{
    // Movement speeds
    public float walkSpeed = 3f;
    public float runSpeed = 5f;      // double-tap W
    public float sprintSpeed = 10f;  // hold Shift
    public float crouchSpeed = 2f;   // hold Ctrl

    // Jump settings
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    // Mouse look settings
    public float mouseSensitivity = 2f;
    public Camera playerCamera;
    Transform cameraTransform;

    // How fast the second W press must happen to count as a double tap
    public float doubleTapTime = 0.25f;

    // Reference to the CharacterController component
    private CharacterController controller;

    // Vertical movement (jumping / falling)
    private float verticalVelocity;

    // Camera up/down rotation
    private float cameraPitch;

    // Used for detecting double-tap W
    private float lastWPressTime = -1f;
    private bool runMode = false;

    void Start()
    {
        // Get the CharacterController attached to this object
        controller = GetComponent<CharacterController>();
        cameraTransform = playerCamera != null ? playerCamera.transform : null;

        // Lock the mouse cursor to the screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Look();               // Rotate player and camera
        CheckRunDoubleTap();  // Check if player double-tapped W
        Move();               // Move the character
    }

    // Rotates the player left/right and camera up/down
    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotate the whole player left/right
        transform.Rotate(0f, mouseX, 0f);

        // Rotate camera up/down
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -80f, 80f);

        if (cameraTransform != null)
            cameraTransform.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
    }

    // Detects double-tapping W to activate running
    void CheckRunDoubleTap()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            // If the time between presses is small, enable run
            if (Time.time - lastWPressTime < doubleTapTime)
                runMode = true;

            lastWPressTime = Time.time;
        }

        // If player stops holding W, stop running
        if (!Input.GetKey(KeyCode.W))
            runMode = false;
    }

    // Decides what movement speed should be used
    float GetSpeed()
    {
        if (Input.GetKey(KeyCode.LeftControl))
            return crouchSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
            return sprintSpeed;

        if (runMode)
            return runSpeed;

        return walkSpeed;
    }

    // Moves the player using CharacterController
    void Move()
    {
        float speed = GetSpeed();

        float x = Input.GetAxis("Horizontal"); // A/D
        float z = Input.GetAxis("Vertical");   // W/S

        Vector3 move = (transform.right * x + transform.forward * z) * speed;

        bool grounded = controller.isGrounded;

        if (grounded)
        {
            verticalVelocity = -1f;

            if (Input.GetButtonDown("Jump"))
                verticalVelocity = Mathf.Sqrt(-2f * gravity * jumpHeight);
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        move.y = verticalVelocity;
        controller.Move(move * Time.deltaTime);
    }
}
