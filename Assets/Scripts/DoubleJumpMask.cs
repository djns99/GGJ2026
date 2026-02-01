using System;
using UnityEngine;

public class DoubleJumpMask : MonoBehaviour, Mask
{
    public int maskId = 0;
    public int maskLevel = 1;
    public int maskWorth = 20;

    public bool maskCollected = false;

    public AudioClip maskMusic;
    public AudioClip GetMaskMusic() => maskMusic;

    public static event Action<int> OnMaskCollect;
    public void ApplyAbilities(GameObject player)
    {
        var controller = player.GetComponent<PlayerController>();
        controller.maxJumps = maskLevel + 1;
        controller.jumpsRemaining = controller.jumpsRemaining + maskLevel;
    }

    public bool CanApply(GameObject player)
    {
        return maskCollected;
    }

    public Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/DoubleJumpMaskSprite");
    }

    public void RemoveAbilities(GameObject player)
    {
        var controller = player.GetComponent<PlayerController>();
        controller.maxJumps = 1;
        controller.jumpsRemaining = 0;
    }

    public bool ShouldRemove(GameObject player)
    {
        return false;
    }

    public int GetMaskId() { return maskId; }
    public void SetMaskId(int id) { maskId = id; }

    public void Collect(GameObject player)
    {
        OnMaskCollect?.Invoke(maskWorth);
        maskCollected = true;
        Debug.Log("Double Jump Mask Collected!");
    }
}
