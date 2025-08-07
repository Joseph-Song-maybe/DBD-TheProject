using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerInputController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.3f;
    public LayerMask groundMask = 1;
    
    [Header("Camera Reference")]
    public Transform cameraTransform;
    
    // Private variables
    private Rigidbody rb;
    private Vector2 moveInput;
    private bool isGrounded;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Debug.Log("PlayerInputController Awake called!");
    }
    
    void Start()
    {
        Debug.Log("PlayerInputController Start called!");
        
        // Find the camera if not assigned
        if (cameraTransform == null)
        {
            GameObject cameraObj = GameObject.Find("Main Camera");
            if (cameraObj != null)
            {
                cameraTransform = cameraObj.transform;
                Debug.Log("Found Main Camera!");
            }
            else
            {
                Debug.LogWarning("Could not find Main Camera!");
            }
        }
        
        // Create ground check if it doesn't exist
        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.SetParent(transform);
            groundCheckObj.transform.localPosition = new Vector3(0, -1f, 0);
            groundCheck = groundCheckObj.transform;
            Debug.Log("Created GroundCheck!");
        }
    }
    
    void Update()
    {
        CheckGrounded();
    }
    
    void FixedUpdate()
    {
        HandleMovement();
    }
    
    // Called by Unity's PlayerInput component when Move action is triggered
    public void OnMove(InputValue inputValue)
    {
        moveInput = inputValue.Get<Vector2>();
        Debug.Log($"Move input received: {moveInput}");
    }
    
    // Called by Unity's PlayerInput component when Jump action is triggered
    public void OnJump(InputValue inputValue)
    {
        if (inputValue.isPressed && isGrounded)
        {
            Debug.Log("Jump input received!");
            Jump();
        }
    }
    
    private void HandleMovement()
    {
        if (moveInput != Vector2.zero)
        {
            Debug.Log($"Processing movement: {moveInput}");
            
            Vector3 movement;
            
            if (cameraTransform != null)
            {
                // Transform movement relative to camera
                Vector3 cameraForward = cameraTransform.forward;
                Vector3 cameraRight = cameraTransform.right;
                
                // Keep movement horizontal
                cameraForward.y = 0f;
                cameraRight.y = 0f;
                cameraForward.Normalize();
                cameraRight.Normalize();
                
                // Convert input to camera-relative movement
                movement = cameraForward * moveInput.y + cameraRight * moveInput.x;
            }
            else
            {
                // Fallback to world-space movement
                movement = new Vector3(moveInput.x, 0, moveInput.y);
            }
            
            // Apply movement
            Vector3 newVelocity = movement * moveSpeed;
            newVelocity.y = rb.linearVelocity.y; // Preserve Y velocity
            rb.linearVelocity = newVelocity;
            
            Debug.Log($"Applied velocity: {newVelocity}");
        }
        else
        {
            // Stop horizontal movement
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }
    
    private void Jump()
    {
        Debug.Log("Jumping!");
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
    
    private void CheckGrounded()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}