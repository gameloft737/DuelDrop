using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class CharacterController2D : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f; // Maximum speed
    public float jumpForce = 5f; // Jump force
    public float gravity = -9.81f; // Gravity value
    public float acceleration = 10f; // Acceleration rate
    public float deceleration = 10f; // Deceleration rate

    [Header("Ground Check")]
    public Transform groundCheck; // Ground check position
    public float groundCheckRadius = 0.2f; // Radius of ground check sphere
    public LayerMask groundLayer; // Layer for the ground

    private CharacterController _controller; // Character controller reference
    private Vector3 _velocity; // Velocity vector
    private bool _isGrounded; // Is the character grounded

    private Vector2 _inputMovement; // Input movement vector
    private bool _jumpInput; // Is jump input received
    private Vector3 _currentVelocity; // Current velocity for smooth acceleration

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleMovement();
        HandleGravity();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _inputMovement = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && _isGrounded)
        {
            _jumpInput = true;
        }
    }

    private void HandleMovement()
    {
        // Calculate target velocity based on input
        Vector3 targetVelocity = new Vector3(_inputMovement.x, 0, _inputMovement.y) * moveSpeed;

        // Smoothly accelerate or decelerate to the target velocity
        if (_inputMovement.magnitude > 0)
        {
            _currentVelocity = Vector3.MoveTowards(_currentVelocity, targetVelocity, acceleration * Time.deltaTime);
        }
        else
        {
            _currentVelocity = Vector3.MoveTowards(_currentVelocity, Vector3.zero, deceleration * Time.deltaTime);
        }

        // Move the character
        _controller.Move(_currentVelocity * Time.deltaTime);

        // Handle jumping
        if (_jumpInput)
        {
            _velocity.y = jumpForce;
            _jumpInput = false;
        }

        // Apply vertical movement
        _controller.Move(_velocity * Time.deltaTime);
    }

    private void HandleGravity()
    {
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f; // A small negative value to ensure the character stays grounded
        }
        else
        {
            _velocity.y += gravity * Time.deltaTime;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
