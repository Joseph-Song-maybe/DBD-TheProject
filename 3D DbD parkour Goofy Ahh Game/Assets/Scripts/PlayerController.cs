using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
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
    private bool isGrounded;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Debug.Log("PlayerController Awake called!");
    }
    
    void Start()
    {
        Debug.Log("PlayerController Start called!");
        
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
        HandleMovement();
        HandleJump();
    }
    
    private void HandleMovement()
    {
        Vector3 movement = Vector3.zero;
        
        // Use legacy Input for guaranteed compatibility
        bool anyInput = false;
        
        if (Input.GetKey(KeyCode.W))
        {
            movement += Vector3.forward;
            anyInput = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movement += Vector3.back;
            anyInput = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            movement += Vector3.left;
            anyInput = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movement += Vector3.right;
            anyInput = true;
        }
        
        if (anyInput)
        {
            Debug.Log($"Input detected! Movement: {movement}");
            
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
                Vector3 moveDirection = cameraForward * movement.z + cameraRight * movement.x;
                movement = moveDirection;
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
    
    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Debug.Log("Jump!");
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
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