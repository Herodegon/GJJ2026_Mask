using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Mouse Look Settings")]
    [SerializeField] private float mouseSensitivity = 0.5f;
    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private GameObject interactPrompt;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float friction = 5f;
    [SerializeField] private float groundCheckDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;
    private bool isGrounded;
    
    // Cached input values
    private Vector2 lookInput;
    private Vector2 moveInput;

    private PlayerInput playerInput;

    private ObjNPC focusedObject;
    private bool isTalking = false; // Flag to check if the player is in a conversation

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
        if (isTalking)
        {
            if (interactPrompt.activeSelf)
            {
                focusedObject = null;
                interactPrompt.gameObject.SetActive(false);
            }
            return; // Skip movement and looking when talking
        }
        CheckForFocusTarget();
        MouseLook();
        Movement();
    }

    // This method is automatically called by PlayerInput component
    public void OnInteract(InputValue value)
    {
        Debug.Log($"OnInteract called! Value: {value.isPressed}");
        
        if (focusedObject != null)
        {
            Debug.Log("Starting conversation with NPC.");
            isTalking = true;
            focusedObject.StartConversation(() => {isTalking = false;});
        }
    }

    // Called by PlayerInput component
    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    public void LookAt(Transform target)
    {
        Vector3 direction = (target.position - cameraTransform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        cameraTransform.rotation = Quaternion.Euler(lookRotation.eulerAngles.x, lookRotation.eulerAngles.y, 0f);
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

    private void CheckForFocusTarget()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, 3f))
        {
            if (hit.transform.CompareTag("NPC"))
            {
                ObjNPC npc = hit.transform.parent.GetComponent<ObjNPC>();
                if (!interactPrompt.activeSelf && npc.IsWaitingAtCounter())
                {
                    focusedObject = npc;
                    interactPrompt.gameObject.SetActive(true);
                }
                return;
            }
        }
        if (interactPrompt.activeSelf)
        {
            focusedObject = null;
            interactPrompt.gameObject.SetActive(false);
        }
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