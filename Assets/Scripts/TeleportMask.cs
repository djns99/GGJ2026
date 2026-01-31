using System.Collections.Generic;
using UnityEngine;

public class TeleportMask : MonoBehaviour, Mask
{
    public int maskId = 1;
    public int maskLevel = 1;
    public List<float> cooldownLengths = new List<float>{ 10f, 5f, 3f };

    public AudioClip maskMusic;
    public AudioClip GetMaskMusic() => maskMusic;
    public void ApplyAbilities(GameObject player)
    {
        var controller = player.GetComponent<PlayerController>();
        controller.canTeleport = true;
        // controller.teleportCooldown = cooldownLengths[maskLevel-1];
    }

    public bool CanApply(GameObject player)
    {
        // TODO Query if we have a limited number of uses per run
        return true;
    }

    public Sprite GetSprite()
    {
        return Resources.Load<Sprite>("TeleportMaskSprite");
    }

    public void RemoveAbilities(GameObject player)
    {
        player.GetComponent<PlayerController>().canTeleport = false;
    }

    public bool ShouldRemove(GameObject player)
    {
        return false;
    }

    public int GetMaskId() { return maskId; }
    public void SetMaskId(int id) { maskId = id; }
}
