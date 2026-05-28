using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float walkSpeed = 4.5f;
    [SerializeField] private float sprintSpeed = 7f;
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float jumpHeight = 1.35f;
    [SerializeField] private float mouseSensitivity = 2.2f;
    [SerializeField] private float gravity = -20f;

    private CharacterController controller;
    private float verticalVelocity;
    private float pitch;
    private float standingHeight;
    private float standingCameraHeight = 1.5f;

    public bool IsCrouching { get; private set; }
    public bool IsSprinting { get; private set; }

    public void ResetView()
    {
        verticalVelocity = 0f;
        pitch = 0f;
        IsCrouching = false;
        IsSprinting = false;

        if (playerCamera != null)
        {
            playerCamera.transform.localRotation = Quaternion.identity;
        }
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        standingHeight = controller.height;
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
        }

        if (playerCamera != null)
        {
            standingCameraHeight = playerCamera.transform.localPosition.y;
        }
    }

    private void Start()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsChoosingMode)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        if (GameManager.Instance != null && (GameManager.Instance.IsGameEnded || GameManager.Instance.IsPaused || GameManager.Instance.IsChoosingMode))
        {
            return;
        }

        Look();
        Move();
    }

    private void Look()
    {
        float activeSensitivity = GameSettings.GetMouseSensitivity(mouseSensitivity);
        float mouseX = Input.GetAxis("Mouse X") * activeSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * activeSensitivity;

        transform.Rotate(Vector3.up * mouseX);
        pitch = Mathf.Clamp(pitch - mouseY, -78f, 78f);

        if (playerCamera != null)
        {
            playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }
    }

    private void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 input = Vector3.ClampMagnitude(new Vector3(x, 0f, z), 1f);
        IsCrouching = Input.GetKey(KeyCode.C);
        IsSprinting = !IsCrouching && Input.GetKey(KeyCode.LeftShift);

        float speed = IsCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : walkSpeed;
        controller.height = Mathf.Lerp(controller.height, IsCrouching ? 1.15f : standingHeight, 12f * Time.deltaTime);
        controller.center = new Vector3(0f, controller.height * 0.45f, 0f);
        if (playerCamera != null)
        {
            Vector3 cameraPosition = playerCamera.transform.localPosition;
            cameraPosition.y = Mathf.Lerp(cameraPosition.y, IsCrouching ? 0.95f : standingCameraHeight, 12f * Time.deltaTime);
            playerCamera.transform.localPosition = cameraPosition;
        }

        Vector3 motion = transform.TransformDirection(input) * speed;

        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -1f;
        }

        if (controller.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        verticalVelocity += gravity * Time.deltaTime;
        motion.y = verticalVelocity;

        controller.Move(motion * Time.deltaTime);
    }
}
