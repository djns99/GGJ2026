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

    [Header("Teleportation")]
    public bool canTeleport = true;
    public float maxTeleportDistance = 5f;
    // Useful if you want teleport to ignore certain layers (like triggers)
    public LayerMask obstacleLayer;

    private Rigidbody2D rb;
    private Camera mainCamera;
    private float moveInput;
    private int jumpsRemaining;
    private float coyoteCounter;
    private float jumpBufferCounter;
    private bool isGrounded;
    private bool wasGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        // 1. Ground Detection
        wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapBox(groundCheck.position, boxSize, 0, groundLayer);

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
            coyoteCounter -= Time.deltaTime;
        }

        // 2. Jump Buffer Handling
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
    // This method is called by the Player Input component
    public void OnTeleport(InputValue value)
    {
        // Only trigger if the bool is on and the button was just pressed
        if (!canTeleport || !value.isPressed) return;
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Debug.Log($"<color=cyan>Teleporting!</color> Mouse Screen Pos: {mousePos}");
        ExecuteTeleport();
    }

    private void ExecuteTeleport()
    {
        // 1. Get Mouse position accurately
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, Mathf.Abs(mainCamera.transform.position.z)));
        Vector2 targetPos = (Vector2)worldPoint;

        // 2. Calculate direction and clamp to max distance
        Vector2 startPos = rb.position;
        Vector2 direction = targetPos - startPos;
        float distance = Mathf.Min(direction.magnitude, maxTeleportDistance);
        direction.Normalize();
        Debug.DrawLine(startPos, rb.position, Color.magenta, 2f);
        // 3. Raycast to find obstacles (The "Trace Back")
        // We use a small radius (CircleCast) instead of a thin line (Raycast) 
        // to ensure the player's whole body fits in the destination.
        float playerSize = 0.4f; // Adjust based on your character's width
        RaycastHit2D hit = Physics2D.CircleCast(startPos, playerSize, direction, distance, obstacleLayer);

        if (hit.collider != null)
        {
            // If we hit a wall, stop just before the hit point
            rb.position = startPos + (direction * (hit.distance - 0.1f));
        }
        else
        {
            // Clear path, move the full allowed distance
            rb.position = startPos + (direction * distance);
        }

        // Kill momentum to prevent clipping through things after arrival
        rb.linearVelocity = Vector2.zero;
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
        coyoteCounter = 0;
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireCube(groundCheck.position, boxSize);
        }

        // Visualizing the teleport range in the editor
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, maxTeleportDistance);
    }
}