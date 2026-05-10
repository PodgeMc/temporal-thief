using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour, IPlayerOnly
{
    public Camera playerCamera;
    public float walkSpeed = 3f;
    public float runSpeed = 5f;
    public float sprintSpeed = 10f;
    public float crouchSpeed = 2f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;
    public float mouseSensitivity = 2f;
    public float doubleTapTime = 0.25f;

    CharacterController controller;
    float verticalVelocity;
    float cameraPitch;
    float lastWPressTime = -1f;
    bool runMode;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (playerCamera != null)
            playerCamera.transform.localRotation = Quaternion.identity;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleLook();
        HandleRunInput();
        HandleMovement();
    }

    void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(0f, mouseX, 0f);

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -80f, 80f);

        if (playerCamera != null)
            playerCamera.transform.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
    }

    void HandleRunInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (Time.time - lastWPressTime < doubleTapTime)
                runMode = true;

            lastWPressTime = Time.time;
        }

        if (!Input.GetKey(KeyCode.W))
            runMode = false;
    }

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

    void HandleMovement()
    {
        float speed = GetSpeed();
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        move *= speed;

        if (controller.isGrounded)
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
