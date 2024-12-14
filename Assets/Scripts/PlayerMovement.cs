using System;
using System.Collections;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public Animator animator;
    private float animHorizontal;
    private float animVertical;
    [SerializeField] float animSmoothingSpeed;
    public Transform characterColliderObj;  
    public bool canMove = true;
    public bool isFrozen = false;

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

    public Rigidbody rb;
    private Vector2 _inputDirection;
    private Vector3 _velocity;
    [SerializeField]private bool _isGrounded;
    private bool _jumpRequested;
    private float _lastGroundedTime;
    private float _jumpRequestTime;
    private Camera _mainCamera;
    [SerializeField] private float stunTime = 0.1f;
    public bool isRight = true;
    public void SetState(bool frozen){
        canMove = !frozen;
        isFrozen = frozen;
        if (frozen)
        {
            // Clear knockback forces
            currentKnockbackForce = Vector3.zero;
            rb.velocity = new Vector3(0, rb.velocity.y, 0); // Retain vertical velocity if necessary
        }

    }
    private void Awake()
    {
        SetState(false);
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ; 
        _mainCamera = Camera.main;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _inputDirection = context.ReadValue<Vector2>();
        if(isFrozen){return;}
        if (_inputDirection.x > 0) 
        {
            characterColliderObj.transform.localScale = new Vector3(1, 1, 1);
            isRight = true;
        }
        else if (_inputDirection.x < 0)
        {
            characterColliderObj.transform.localScale = new Vector3(-1, 1, 1);
            isRight = false;
        }
        
    }
    private void Animate(){
        if(animator != null)
        {
            float targetHorizontal = Math.Abs(_inputDirection.x);
            if (Mathf.Abs(animHorizontal - targetHorizontal) < 0.05f)
            {
                animHorizontal = targetHorizontal;
            }
            if (animHorizontal < targetHorizontal)
            {
                animHorizontal += Time.deltaTime * animSmoothingSpeed;
            }
            else if (animHorizontal > targetHorizontal)
            {
                animHorizontal -= Time.deltaTime * animSmoothingSpeed;
            }
            animator.SetFloat("horizontal", animHorizontal);
            bool isAnimGrounded = Physics.Raycast(groundCheck.position, Vector3.down, groundCheckRadius * 7f, groundLayer);
            animator.SetBool("isGrounded", isAnimGrounded || _isGrounded);
            animator.SetBool("isMoving", _inputDirection.x != 0);
            
            
        }
        if(isFrozen){
            animator.SetFloat("horizontal", 0);
            animator.SetBool("isGrounded", true);
            animator.SetBool("isMoving", false);
        }

    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && !isFrozen)
        {
            _jumpRequested = true;
            _jumpRequestTime = Time.time; 
        }
    }
    private void Update(){
        if(animator != null){      
            Animate();
        }   
    }

    private void FixedUpdate()
    {
        if(canMove)
        { 
            HandleJump();
        }
        if(rb.useGravity){
            ApplyCustomGravity();
        }
        CheckGrounded(); 
        if(!isFrozen){
            SmoothKnockback();
            
        }
        HandleMovement();

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

        if (_inputDirection.magnitude > 0.1f && canMove && !isFrozen)
        {
            _velocity = Vector3.MoveTowards(_velocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            _velocity = Vector3.MoveTowards(_velocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
        }

        rb.velocity = new Vector3(_velocity.x, rb.velocity.y, _velocity.z);
    }

    private void HandleJump()
    {
        if (_jumpRequested && Time.time - _jumpRequestTime <= jumpTimeout)
        {
            if (_isGrounded || Time.time - _lastGroundedTime <= coyoteTime)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);  
                if(animator != null){animator.SetTrigger("jump");}
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
            rb.AddForce(Vector3.down * gravityMultiplier, ForceMode.Acceleration);
            if (rb.velocity.y < maxFallSpeed)
            {
                rb.velocity = new Vector3(rb.velocity.x, maxFallSpeed, rb.velocity.z);
            }
        }
    }

    [Header("Knockback Settings")]
    public float smoothFactor = 0.1f; // How quickly the knockback smooths out
    private Vector3 currentKnockbackForce; // Current force being applied

    public void Knockback(Vector3 position, float strength)
    {
        if(isFrozen){return;}
        AudioManager.instance.Play("Hit");
        // Calculate the direction from the player to the explosion position
        Vector3 direction = (new Vector3(transform.position.x, 0, 0) - new Vector3(position.x, 0, 0)).normalized;
        
        // Calculate the explosion force
        currentKnockbackForce = direction * strength;
        
        
        // Apply the explosion force with a smooth transition
        rb.AddForce(currentKnockbackForce, ForceMode.Impulse);
        canMove = false;
        StartCoroutine(SetStun(stunTime));
        
        // Draw a line in the knockback direction for visualization
        Debug.DrawLine(transform.position, transform.position + currentKnockbackForce, Color.red, 1f); // Draw for 1 second
    }
    private IEnumerator SetStun(float length){
        yield return new WaitForSeconds(length);
        canMove = true;
    }
    public void Knockup(float strength)
    {
        rb.AddForce(Vector3.up * strength, ForceMode.Impulse);
    }
    private void SmoothKnockback()
    {
        if (isFrozen) return;

        if (currentKnockbackForce.magnitude > 0.1f)
        {
            currentKnockbackForce = Vector3.Lerp(currentKnockbackForce, Vector3.zero, smoothFactor);
            rb.AddForce(currentKnockbackForce, ForceMode.Impulse);
        }
    }

    public void Floor()
    {
        rb.AddForce(Vector3.down * jumpForce, ForceMode.Impulse);
    }

}
