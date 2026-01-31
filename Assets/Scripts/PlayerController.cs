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
    public LayerMask obstacleLayer;
    // Useful if you want teleport to ignore certain layers (like triggers)

    [Header("Teleport Visuals")]
    public LineRenderer rangeRenderer; // Drag your LineRenderer here
    public int segments = 50;
    public GameObject teleportEffectPrefab; // Drag your prefab here

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
    void Start()
    {
        // Initialize the LineRenderer if it's assigned
        if (rangeRenderer != null)
        {
            rangeRenderer.positionCount = segments;
            rangeRenderer.useWorldSpace = true;
        }
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

        // 3. Update Teleport Range Visual
        UpdateRangeVisual();
    }
    // Update the LineRenderer to show teleport range
    private void UpdateRangeVisual()
    {
        if (rangeRenderer == null) return;

        // Only show the ring if teleporting is enabled
        rangeRenderer.enabled = canTeleport;

        if (canTeleport)
        {
            float angle = 0f;
            for (int i = 0; i < segments; i++)
            {
                // Calculate point on a circle using Sine and Cosine
                float x = Mathf.Cos(angle) * maxTeleportDistance;
                float y = Mathf.Sin(angle) * maxTeleportDistance;

                // Set the position relative to the player
                rangeRenderer.SetPosition(i, new Vector3(rb.position.x + x, rb.position.y + y, 0));

                angle += (2f * Mathf.PI) / segments;
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
        // 1. Calculate Target Position
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, Mathf.Abs(mainCamera.transform.position.z)));
        Vector2 startPos = rb.position;
        Vector2 rawTargetPos = (Vector2)worldPoint;

        // Clamp to max distance
        Vector2 direction = rawTargetPos - startPos;
        float distance = Mathf.Min(direction.magnitude, maxTeleportDistance);
        Vector2 finalTarget = startPos + (direction.normalized * distance);

        // 2. The "Trace Back" logic
        // We check if the player's shape (OverlapCircle) would hit anything at the destination
        float playerRadius = 0.4f; // Adjust to match your player's width
        int safetyBreak = 0;

        // While the target position is inside an obstacle, move it back toward the player
        while (Physics2D.OverlapCircle(finalTarget, playerRadius, obstacleLayer) && safetyBreak < 100)
        {
            // Move back by a small step (e.g., 0.1 units)
            finalTarget = Vector2.MoveTowards(finalTarget, startPos, 0.1f);
            safetyBreak++; // Prevent infinite loops
        }

        // 3. Spawn effect at the START position
        SpawnTeleportEffect(startPos);

        // 4. Move the player
        rb.position = finalTarget;
        rb.linearVelocity = Vector2.zero;

        Debug.Log($"<color=cyan>Teleported.</color> Distance adjusted by {safetyBreak * 0.1f} units to avoid collision.");
        // 5. Spawn effect at the END position
        SpawnTeleportEffect(finalTarget);
    }

    private void SpawnTeleportEffect(Vector2 position)
    {
        if (teleportEffectPrefab != null)
        {
            // Create the particles
            GameObject effect = Instantiate(teleportEffectPrefab, position, Quaternion.identity);

            // Destroy the object after 1 second so it doesn't clutter the hierarchy
            Destroy(effect, 1f);
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