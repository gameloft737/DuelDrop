using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 5f;               // Maximum movement speed
    public float acceleration = 10f;           // Rate of acceleration
    public float deceleration = 10f;           // Rate of deceleration
    public float jumpForce = 5f;               // Jump force
    public float groundedDistance = 0.1f;      // Distance to check if grounded
    public float gravityMultiplier = 5f;       // Custom gravity multiplier (increased for less float)
    public float coyoteTime = 0.2f;            // Time after leaving ground the player can still jump
    public float maxFallSpeed = -10f;          // Maximum falling speed
    public LayerMask groundLayer;              // Layer to consider as ground

    [Header("Ground Check")]
    public Transform groundCheck;              // Ground check position
    public float groundCheckRadius = 0.2f;     // Radius of ground check

    private Rigidbody _rb;
    private Vector2 _inputDirection;
    private Vector3 _velocity;
    private bool _isGrounded;
    private bool _jumpRequested;
    private float _lastGroundedTime;
    private Camera _mainCamera;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeRotation; // Freeze rotation to keep movement in the XY plane
        _mainCamera = Camera.main;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _inputDirection = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _jumpRequested = true;
        }
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        HandleMovement();
        HandleJump();
        ApplyCustomGravity();
    }

    private void CheckGrounded()
    {
        // Check if the player is grounded by casting a sphere below the player
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        // Update last grounded time
        if (_isGrounded)
        {
            _lastGroundedTime = Time.time;
        }
    }

    private void HandleMovement()
    {
        Vector3 targetVelocity = new Vector3(_inputDirection.x, 0, _inputDirection.y) * moveSpeed;

        // Acceleration
        if (_inputDirection.magnitude > 0.1f)
        {
            _velocity = Vector3.MoveTowards(_velocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        }
        // Deceleration
        else
        {
            _velocity = Vector3.MoveTowards(_velocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
        }

        // Apply movement
        _rb.velocity = new Vector3(_velocity.x, _rb.velocity.y, _velocity.z);
    }

    private void HandleJump()
    {
        // Allow jump if grounded or within coyote time
        if (_jumpRequested && (_isGrounded || Time.time - _lastGroundedTime <= coyoteTime))
        {
            _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z); // Reset Y velocity before jumping
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            _jumpRequested = false;
        }
    }

    private void ApplyCustomGravity()
    {
        // Apply additional gravity when the player is in the air
        if (!_isGrounded)
        {
            // Apply custom gravity
            _rb.AddForce(Vector3.down * gravityMultiplier, ForceMode.Acceleration);
            
            // Limit maximum fall speed
            if (_rb.velocity.y < maxFallSpeed)
            {
                _rb.velocity = new Vector3(_rb.velocity.x, maxFallSpeed, _rb.velocity.z);
            }
        }
    }
}
