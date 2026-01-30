using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Tooltip("Horizontal move speed (units/sec)")]
    public float moveSpeed = 5f;

    [Header("Jumping")]
    [Tooltip("Initial jump velocity (units/sec)")]
    public float jumpVelocity = 12f;
    [Tooltip("Maximum number of jumps allowed (0 = no jumping, 1 = single jump, 2 = double jump, etc.)")]
    public int maxJumps = 2;

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

    // track which colliders currently count as ground contacts
    private readonly HashSet<Collider2D> groundContacts = new HashSet<Collider2D>();

    // jump state
    private int jumpsRemaining;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        if (rb2d == null)
            Debug.LogWarning("PlayerController: no Rigidbody2D found — falling back to Transform movement.");

        // initialize jumps available
        jumpsRemaining = Mathf.Max(0, maxJumps);
    }

    void Update()
    {
        // update grounded state from collision-driven contacts
        wasGrounded = isGrounded;
        isGrounded = groundContacts.Count > 0;

        // Reset jumps only on landing (transition from air -> ground)
        if (!wasGrounded && isGrounded)
        {
            jumpsRemaining = Mathf.Max(0, maxJumps);
            Debug.Log($"Landed: reset jumps to {jumpsRemaining}");
        }

        // log when we leave the ground (transition grounded -> not grounded)
        if (wasGrounded && !isGrounded)
        {
            if (rb2d != null)
                Debug.Log($"Left ground: airborne. linearVelocity={rb2d.linearVelocity}");
            else
                Debug.Log("Left ground: airborne (no Rigidbody2D)");
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

    // Multi-jump: immediate jump when jumpsRemaining > 0
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
            if (maxJumps <= 0)
            {
                Debug.Log("Jump input ignored: maxJumps <= 0 (jumping disabled).");
                return;
            }

            if (jumpsRemaining > 0)
            {
                if (rb2d != null)
                {
                    rb2d.linearVelocity = new Vector2(rb2d.linearVelocity.x, jumpVelocity);
                    jumpsRemaining--;
                    Debug.Log($"Jumped: velocity set to {jumpVelocity}. jumpsRemaining={jumpsRemaining}/{maxJumps}");
                }
                else
                {
                    // simple transform-based fallback: small upward translation
                    transform.Translate(Vector3.up * 0.2f, Space.World);
                    jumpsRemaining = Mathf.Max(0, jumpsRemaining - 1);
                    Debug.Log($"Jumped (transform fallback). jumpsRemaining={jumpsRemaining}/{maxJumps}");
                }
            }
            else
            {
                Debug.Log("Jump pressed but no jumps remaining.");
            }
        }
        else
        {
            // button release; no variable jump height in simple mode
            Debug.Log("Jump released.");
        }
    }

    // Collision-driven ground detection
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsOnGroundLayer(collision.gameObject))
            return;

        // check contacts for upward-facing normal (we hit ground from above)
        foreach (var contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                if (groundContacts.Add(collision.collider))
                {
                    Debug.Log($"OnCollisionEnter2D: added ground contact '{collision.collider.name}'. totalGroundContacts={groundContacts.Count}");
                }
                break;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (groundContacts.Remove(collision.collider))
        {
            Debug.Log($"OnCollisionExit2D: removed ground contact '{collision.collider.name}'. totalGroundContacts={groundContacts.Count}");
        }
    }

    // Optional: support trigger-based ground (if you use trigger colliders for ground)
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsOnGroundLayer(other.gameObject))
            return;

        // crude trigger handling: add trigger as ground contact
        if (groundContacts.Add(other))
            Debug.Log($"OnTriggerEnter2D: added ground trigger '{other.name}'. totalGroundContacts={groundContacts.Count}");
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (groundContacts.Remove(other))
            Debug.Log($"OnTriggerExit2D: removed ground trigger '{other.name}'. totalGroundContacts={groundContacts.Count}");
    }

    private bool IsOnGroundLayer(GameObject go)
    {
        return (groundLayer.value & (1 << go.layer)) != 0;
    }

    // Draw ground check sphere in editor (kept for visual aid if you still want it)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 pos = groundCheck != null ? groundCheck.position : transform.position;
        Gizmos.DrawWireSphere(pos, groundCheckRadius);
    }
}