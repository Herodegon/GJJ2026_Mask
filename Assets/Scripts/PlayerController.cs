using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Mouse Look Settings")]
    [SerializeField] private float mouseSensitivity = 0.5f;
    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform cameraTransform;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float friction = 5f;
    [SerializeField] private float groundCheckDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float xRotation = 0f;
    
    // Cached input values
    private Vector2 lookInput;
    private Vector2 moveInput;

    private PlayerInput playerInput;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        controller = GetComponent<CharacterController>();

        playerInput = GetComponent<PlayerInput>();
        playerInput.actions.Disable();
        playerInput.actions.FindActionMap("Player").Enable();
    }

    void Update()
    {
        MouseLook();
        Movement();
    }

    // Called by PlayerInput component
    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    // Called by PlayerInput component
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void MouseLook()
    {
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    void Movement()
    {
        isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundMask);

        Vector3 targetVelocity = (playerBody.right * moveInput.x + playerBody.forward * moveInput.y) * moveSpeed;
        float currentAcceleration = moveInput.magnitude > 0 ? acceleration : friction;
        velocity = Vector3.Lerp(velocity, targetVelocity, currentAcceleration * Time.deltaTime);

        controller.Move(velocity * moveSpeed * Time.deltaTime);
    }
}