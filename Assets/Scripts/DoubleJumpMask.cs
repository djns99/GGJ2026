using UnityEngine;

public class DoubleJumpMask : MonoBehaviour, Mask
{
    public int maskId = 0;
    public int maskLevel = 1;

    public void ApplyAbilities(GameObject player)
    {
        player.GetComponent<PlayerController>().maxJumps = maskLevel + 1;
    }

    public bool CanApply(GameObject player)
    {
        return true;
    }

    public Sprite GetSprite()
    {
        return null;
    }

    public void RemoveAbilities(GameObject player)
    {
        player.GetComponent<PlayerController>().maxJumps = 1;
    }

    public bool ShouldRemove(GameObject player)
    {
        return false;
    }

    public int GetMaskId() { return maskId; }
    public void SetMaskId(int id) { maskId = id; }
}
