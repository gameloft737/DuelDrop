using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Transform characterColliderObj;  

    [Header("Settings")]
    public float moveSpeed = 5f;               
    public float acceleration = 10f;           
    public float deceleration = 10f;           
    public float jumpForce = 5f;               
    public float groundedDistance = 0.1f;      
    public float gravityMultiplier = 5f;       
    public float coyoteTime = 0.2f;            
    public float jumpTimeout = 0.2f;           
    public float maxFallSpeed = -10f;          
    public LayerMask groundLayer;              

    [Header("Ground Check")]
    public Transform groundCheck;              
    public float groundCheckRadius = 0.2f;     

    private Rigidbody _rb;
    private Vector2 _inputDirection;
    private Vector3 _velocity;
    private bool _isGrounded;
    private bool _jumpRequested;
    private float _lastGroundedTime;
    private float _jumpRequestTime;
    private Camera _mainCamera;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ; 
        _mainCamera = Camera.main;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _inputDirection = context.ReadValue<Vector2>();
        if (_inputDirection.x > 0) 
        {
            characterColliderObj.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (_inputDirection.x < 0)
        {
            characterColliderObj.transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _jumpRequested = true;
            _jumpRequestTime = Time.time; 
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
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        if (_isGrounded)
        {
            _lastGroundedTime = Time.time;
        }
    }

    private void HandleMovement()
    {
        Vector3 targetVelocity = new Vector3(_inputDirection.x, 0, _inputDirection.y) * moveSpeed;

        if (_inputDirection.magnitude > 0.1f)
        {
            _velocity = Vector3.MoveTowards(_velocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            _velocity = Vector3.MoveTowards(_velocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
        }

        _rb.velocity = new Vector3(_velocity.x, _rb.velocity.y, _velocity.z);
    }

    private void HandleJump()
    {
        if (_jumpRequested && Time.time - _jumpRequestTime <= jumpTimeout)
        {
            if (_isGrounded || Time.time - _lastGroundedTime <= coyoteTime)
            {
                _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
                _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                _jumpRequested = false;
            }
        }
        else
        {
            _jumpRequested = false; 
        }
    }

    private void ApplyCustomGravity()
    {
        if (!_isGrounded)
        {
            _rb.AddForce(Vector3.down * gravityMultiplier, ForceMode.Acceleration);
            if (_rb.velocity.y < maxFallSpeed)
            {
                _rb.velocity = new Vector3(_rb.velocity.x, maxFallSpeed, _rb.velocity.z);
            }
        }
    }

    // New Knockback function
        [Header("Knockback Settings")]
        public float upliftModifier = 1f; // Uplift modifier for explosion force

        public void Knockback(Vector3 direction, float strength)
        {
            // Normalize the direction and apply force
            Vector3 knockbackForce = direction.normalized * strength;

            // Apply the knockback force to the rigidbody
            _rb.AddForce(knockbackForce, ForceMode.Impulse);

            // Draw a line in the knockback direction for visualization
            Debug.DrawLine(transform.position, transform.position + knockbackForce, Color.red, 1f); // Draw for 1 second
        }
}
