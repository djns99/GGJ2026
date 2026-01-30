using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Tooltip("Horizontal move speed (units/sec)")]
    public float moveSpeed = 5f;

    [Header("Jumping")]
    [Tooltip("Initial jump velocity (units/sec)")]
    public float jumpVelocity = 12f;

    [Tooltip("Transform used to check for ground. If null, player's position will be used.")]
    public Transform groundCheck;
    [Tooltip("Radius for ground check")]
    public float groundCheckRadius = 1.5f;
    [Tooltip("Layers considered ground")]
    public LayerMask groundLayer;

    // horizontal input (-1..1)
    private float moveX;

    private Rigidbody2D rb2d;

    // grounding helpers
    private bool wasGrounded;
    private bool isGrounded;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        if (rb2d == null)
            Debug.LogWarning("PlayerController: no Rigidbody2D found — falling back to Transform movement.");
    }

    void Update()
    {
        // Ground check (use groundCheck if assigned)
        Vector2 checkPos = groundCheck != null ? (Vector2)groundCheck.position : (Vector2)transform.position;

        // perform the overlap and capture the returned collider (if any)
        Collider2D hit = Physics2D.OverlapCircle(checkPos, groundCheckRadius, groundLayer);
        bool groundedNow = hit != null;

        // update state
        wasGrounded = isGrounded;
        isGrounded = groundedNow;

        // Detailed debug to help diagnose why ground detection may fail.
        // This logs the check position, radius, the LayerMask value, and the collider found (if any).
        if (!groundedNow)
        {
            Debug.Log($"GroundCheck: NO HIT at pos={checkPos}, radius={groundCheckRadius}, layerMask={groundLayer.value}. " +
                      $"Make sure ground has a Collider2D (e.g. BoxCollider2D), is on a layer included in the mask, and groundCheck is at the feet.");
        }
        else
        {
            Debug.Log($"GroundCheck: HIT '{hit.name}' (layer={hit.gameObject.layer}) at pos={checkPos}, radius={groundCheckRadius}");
        }

        // log when we leave the ground (transition grounded -> not grounded)
        if (wasGrounded && !isGrounded)
        {
            if (rb2d != null)
                Debug.Log($"Left ground: airborne. linearVelocity={rb2d.linearVelocity}");
            else
                Debug.Log("Left ground: airborne (no Rigidbody2D)");
        }

        // optional: log landing
        if (!wasGrounded && isGrounded)
        {
            Debug.Log("Landed: back on ground.");
        }
    }

    // Physics update for consistent movement when using Rigidbody2D
    void FixedUpdate()
    {
        if (rb2d != null)
        {
            // preserve vertical velocity (for gravity/jumps)
            rb2d.linearVelocity = new Vector2(moveX * moveSpeed, rb2d.linearVelocity.y);
        }
        else
        {
            // fallback: move transform if no Rigidbody2D
            transform.Translate(new Vector3(moveX * moveSpeed * Time.fixedDeltaTime, 0f, 0f), Space.World);
        }
    }

    // Called by Unity's PlayerInput when a "Move" action is performed (PlayerInput behaviour: Send Messages)
    // Make sure your Input Action has a Vector2 binding (e.g., A/D or left stick).
    public void OnMove(InputValue value)
    {
        Vector2 v = value.Get<Vector2>();
        moveX = Mathf.Clamp(v.x, -1f, 1f);

        // Log the vector plus which WASD keys are down (Input System 1.18)
        var kb = Keyboard.current;
        if (kb != null)
        {
            string pressed = "";
            if (kb.aKey.isPressed) pressed += "A ";
            if (kb.dKey.isPressed) pressed += "D ";

            if (!string.IsNullOrEmpty(pressed))
                Debug.Log($"OnMove: {v} - WASD pressed: {pressed.Trim()}");
            else
                Debug.Log($"OnMove: {v} - no WASD keys pressed (could be arrows/gamepad)");
        }
        else
        {
            Debug.Log($"OnMove: {v} - Keyboard.current is null");
        }
    }

    // Optional: handle when action is canceled to ensure input resets
    public void OnMoveCanceled(InputValue value)
    {
        moveX = 0f;
        Debug.Log("OnMoveCanceled: move input canceled");
    }

    // Simple jump: immediate jump when grounded
    // Called by PlayerInput when Jump action is performed (button press)
    public void OnJump(InputValue value)
    {
        float v = 0f;
        try
        {
            v = value.Get<float>();
        }
        catch
        {
            v = 1f;
        }

        if (v > 0.5f)
        {
            if (isGrounded)
            {
                if (rb2d != null)
                {
                    rb2d.linearVelocity = new Vector2(rb2d.linearVelocity.x, jumpVelocity);
                    Debug.Log($"Jumped: velocity set to {jumpVelocity}");
                }
                else
                {
                    // simple transform-based fallback: small upward translation
                    transform.Translate(Vector3.up * 0.2f, Space.World);
                    Debug.Log("Jumped (transform fallback)");
                }
            }
            else
            {
                Debug.Log("Jump pressed but not grounded (no buffer/coyote).");
            }
        }
        else
        {
            // button release; no variable jump height in simple mode
            Debug.Log("Jump released.");
        }
    }

    // Draw ground check sphere in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 pos = groundCheck != null ? groundCheck.position : transform.position;
        Gizmos.DrawWireSphere(pos, groundCheckRadius);
    }
}
