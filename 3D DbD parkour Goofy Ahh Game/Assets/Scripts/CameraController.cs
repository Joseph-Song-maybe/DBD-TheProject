using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;
    
    [Header("First Person Settings")]
    public float eyeHeight = 1.6f;
    public Vector3 eyeOffset = new Vector3(0, 0, 0);
    public float followSpeed = 10f;
    
    [Header("Mouse Look Controls")]
    public float mouseSensitivity = 2f;
    public float verticalMouseSensitivity = 2f;
    public float minVerticalAngle = -90f;
    public float maxVerticalAngle = 90f;
    
    [Header("Cursor Settings")]
    public bool lockCursorOnStart = true;
    
    [Header("Constraints")]
    public bool followX = true;
    public bool followY = true;
    public bool followZ = true;
    
    // Private variables
    private float currentHorizontalAngle = 0f;
    private float currentVerticalAngle = 0f;
    private Vector3 currentVelocity;
    private Vector2 mouseDelta;
    private bool isCursorLocked = false;
    
    // Modern Input System
    private InputAction mouseLookAction;
    
    void Awake()
    {
        mouseLookAction = new InputAction("MouseLook", InputActionType.Value, "<Mouse>/delta");
        mouseLookAction.Enable();
        Debug.Log("Modern Input System initialized");
    }
    
    void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                Debug.Log("Found player target");
            }
        }
        
        Vector3 eulerAngles = transform.eulerAngles;
        currentHorizontalAngle = eulerAngles.y;
        currentVerticalAngle = eulerAngles.x;
        
        if (currentVerticalAngle > 180f)
            currentVerticalAngle -= 360f;
            
        if (lockCursorOnStart)
        {
            LockCursor();
        }
        
        Debug.Log("CameraController started. Cursor locked: " + isCursorLocked);
    }
    
    void Update()
    {
        HandleCursorToggle();
        HandleMouseInput();
    }
    
    void LateUpdate()
    {
        if (target == null) return;
        
        UpdateCameraRotation();
        FollowTarget();
        ApplyRotation();
    }
    
    private void HandleCursorToggle()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isCursorLocked)
            {
                UnlockCursor();
            }
            else
            {
                LockCursor();
            }
        }
        
        if (!isCursorLocked && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            LockCursor();
        }
    }
    
    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isCursorLocked = true;
        Debug.Log("Cursor locked to center - Modern Input System");
    }
    
    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isCursorLocked = false;
        Debug.Log("Cursor unlocked - Modern Input System");
    }
    
    private void HandleMouseInput()
    {
        if (isCursorLocked)
        {
            mouseDelta = mouseLookAction.ReadValue<Vector2>();
            
            if (mouseDelta.magnitude > 0.01f)
            {
                Debug.Log("Modern mouse input: " + mouseDelta + " (magnitude: " + mouseDelta.magnitude.ToString("F3") + ")");
            }
        }
        else
        {
            mouseDelta = Vector2.zero;
        }
    }
    
    private void UpdateCameraRotation()
    {
        if (isCursorLocked && mouseDelta != Vector2.zero)
        {
            float deltaX = mouseDelta.x * mouseSensitivity;
            float deltaY = mouseDelta.y * verticalMouseSensitivity;
            
            currentHorizontalAngle += deltaX;
            currentVerticalAngle -= deltaY;
            
            currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, minVerticalAngle, maxVerticalAngle);
            
            currentHorizontalAngle = currentHorizontalAngle % 360f;
            if (currentHorizontalAngle < 0f)
                currentHorizontalAngle += 360f;
                
            Debug.Log("Camera rotation - H: " + currentHorizontalAngle.ToString("F1") + ", V: " + currentVerticalAngle.ToString("F1"));
        }
    }
    
    private void FollowTarget()
    {
        Vector3 desiredPosition = target.position + Vector3.up * eyeHeight + eyeOffset;
        
        if (!followX) desiredPosition.x = transform.position.x;
        if (!followY) desiredPosition.y = transform.position.y;
        if (!followZ) desiredPosition.z = transform.position.z;
        
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, 1f / followSpeed);
    }
    
    private void ApplyRotation()
    {
        transform.rotation = Quaternion.Euler(currentVerticalAngle, currentHorizontalAngle, 0f);
    }
    
    void OnDestroy()
    {
        mouseLookAction?.Disable();
        UnlockCursor();
    }
    
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && isCursorLocked)
        {
            UnlockCursor();
        }
    }
    
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    
    public void SetEyeHeight(float newHeight)
    {
        eyeHeight = newHeight;
    }
    
    public void SetEyeOffset(Vector3 newOffset)
    {
        eyeOffset = newOffset;
    }
    
    public void ResetCamera()
    {
        currentHorizontalAngle = 0f;
        currentVerticalAngle = 0f;
    }
    
    public void SetMouseSensitivity(float horizontal, float vertical)
    {
        mouseSensitivity = horizontal;
        verticalMouseSensitivity = vertical;
    }
    
    public bool IsCursorLocked()
    {
        return isCursorLocked;
    }
}