using Unity.VisualScripting;
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
    public bool canTeleport = false;
    public float maxTeleportDistance = 5f;
    public LayerMask obstacleLayer;
    public float teleportCooldown = 3f;
    public float teleportTimer = 0f;
    // Useful if you want teleport to ignore certain layers (like triggers)

    [Header("Teleport Visuals")]
    public LineRenderer rangeRenderer; // Drag your LineRenderer here
    public int segments = 50;
    public GameObject teleportEffectPrefab; // Drag your prefab here

    private Animator anim;
    private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private Camera mainCamera;
    private float moveInput;
    [DoNotSerialize]
    public int jumpsRemaining;
    private float coyoteCounter;
    private float jumpBufferCounter;
    private bool isGrounded;
    private bool wasGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        anim = GetComponent<Animator>(); // Get the Animator
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer
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
        // Cooldown Timer Logic
        if (teleportTimer > 0)
        {
            teleportTimer -= Time.deltaTime;
        }
        UpdateRangeVisual();

        // 4. Animation and Sprite Flipping
        // We use Mathf.Abs so that moving left (-1) still counts as "Speed 1"
        anim.SetFloat("Speed", Mathf.Abs(moveInput));

        // Check if we need to flip the character
        if (moveInput > 0 && transform.localScale.x < 0)
        {
            Flip();
        }
        else if (moveInput < 0 && transform.localScale.x > 0)
        {
            Flip();
        }
    }
    // Update the LineRenderer to show teleport range

    private void Flip()
    {
        // Multiply the player's x local scale by -1.
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
    private void UpdateRangeVisual()
    {
        if (rangeRenderer == null) return;

        // Only show the ring if teleporting is enabled
        rangeRenderer.enabled = canTeleport;

        if (canTeleport)
        {
            // 1. Determine the color based on the cooldown timer
            // We use a custom orange or Unity's built-in Color.orange
            Color currentStateColor = (teleportTimer > 0) ? new Color(1f, 0.5f, 0f) : Color.cyan;

            // 2. Apply the color to the LineRenderer
            rangeRenderer.startColor = currentStateColor;
            rangeRenderer.endColor = currentStateColor;
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
        // 1. Only trigger on the "down" press
        if (!value.isPressed) return;

        // 2. Standard cooldown and state checks
        if (!canTeleport || teleportTimer > 0) return;

        // 3. Get the position from the Pointer (Works for Mouse AND Touch)
        if (Pointer.current != null)
        {
            Vector2 screenPos = Pointer.current.position.ReadValue();
            ExecuteTeleport(screenPos);
        }
    }

    private void ExecuteTeleport(Vector2 inputScreenPos)
    {
        // Convert the screen tap/click to a world position
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(new Vector3(
            inputScreenPos.x,
            inputScreenPos.y,
            Mathf.Abs(mainCamera.transform.position.z)
        ));

        Vector2 startPos = rb.position;
        Vector2 finalTarget = (Vector2)worldPoint;

        // Clamp distance logic
        Vector2 direction = finalTarget - startPos;
        float distance = Mathf.Min(direction.magnitude, maxTeleportDistance);
        finalTarget = startPos + (direction.normalized * distance);

        // Collision safety check (Trace back logic)
        float playerRadius = 0.4f;
        int safetyBreak = 0;
        while (Physics2D.OverlapCircle(finalTarget, playerRadius, obstacleLayer) && safetyBreak < 100)
        {
            finalTarget = Vector2.MoveTowards(finalTarget, startPos, 0.1f);
            safetyBreak++;
        }

        // Apply movement
        SpawnTeleportEffect(startPos);
        rb.position = finalTarget;
        rb.linearVelocity = Vector2.zero;
        SpawnTeleportEffect(finalTarget);

        teleportTimer = teleportCooldown;
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

        // 2. Visualize the actual Player Collider (The "Physical" body)
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            Gizmos.color = Color.yellow;
            // This draws a box exactly where the physics engine thinks your body is
            Vector3 colliderPos = transform.TransformPoint(collider.offset);
            Gizmos.DrawWireCube(colliderPos, collider.size);
        }
    }
}