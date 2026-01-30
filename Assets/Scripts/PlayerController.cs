using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float acceleration = 50f;

    [Header("Jumping")]
    public float jumpForce = 12f;
    public int maxJumps = 2;
    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.15f;

    [Header("Detection")]
    public Transform groundCheck;
    public Vector2 boxSize = new Vector2(0.5f, 0.1f);
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private float moveInput;
    private int jumpsRemaining;
    private float coyoteCounter;
    private float jumpBufferCounter;
    private bool isGrounded;
    private bool wasGrounded;

    void Awake() => rb = GetComponent<Rigidbody2D>();

    void Update()
    {
        // 1. Ground Detection
        wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapBox(groundCheck.position, boxSize, 0, groundLayer);

        // 2. State Change Logs & Jump Reset
        if (isGrounded)
        {
            if (!wasGrounded)
            {
                jumpsRemaining = maxJumps;
                Debug.Log($"<color=green>Landed.</color> Jumps reset to: {jumpsRemaining}");
            }
            coyoteCounter = coyoteTime;
        }
        else
        {
            if (wasGrounded) Debug.Log("<color=yellow>Left Ground.</color>");
            coyoteCounter -= Time.deltaTime;
        }

        // 3. Jump Buffer Handling
        if (jumpBufferCounter > 0)
        {
            jumpBufferCounter -= Time.deltaTime;

            // Check if we can jump (either grounded/coyote or have multi-jumps left)
            if (coyoteCounter > 0 || jumpsRemaining > 0)
            {
                ExecuteJump();
            }
        }
    }

    void FixedUpdate()
    {
        float targetSpeed = moveInput * moveSpeed;
        float speedDif = targetSpeed - rb.linearVelocity.x;
        float movement = speedDif * acceleration;
        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>().x;

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            jumpBufferCounter = jumpBufferTime;
            Debug.Log($"Jump Pressed. Buffer started. Jumps remaining: {jumpsRemaining}");
        }
    }

    private void ExecuteJump()
    {
        Debug.Log($"<color=cyan>Executing Jump.</color> Jumps before: {jumpsRemaining}");

        // Reset Y velocity for consistent height (essential for double jumps)
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        jumpsRemaining--;
        jumpBufferCounter = 0;
        coyoteCounter = 0; // Consume coyote time so we don't jump again instantly
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireCube(groundCheck.position, boxSize);
        }
    }
}