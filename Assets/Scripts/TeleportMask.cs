using System;
using System.Collections.Generic;
using UnityEngine;

public class TeleportMask : MonoBehaviour, Mask
{
    public int maskId = 1;
    public int maskLevel = 1;
    public int maskWorth = 20;

    public List<float> cooldownLengths = new List<float>{ 10f, 5f, 3f };


    public bool maskCollected = false;

    public AudioClip maskMusic;
    public AudioClip GetMaskMusic() => maskMusic;
    public static event Action<int> OnMaskCollect;
    public void ApplyAbilities(GameObject player)
    {
        var controller = player.GetComponent<PlayerController>();
        controller.canTeleport = true;
        // controller.teleportCooldown = cooldownLengths[maskLevel-1];
    }

    public bool CanApply(GameObject player)
    {
        // TODO Query if we have a limited number of uses per run
        return maskCollected;
    }

    public Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/TeleportMaskSprite");
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

    public void Collect(GameObject player)
    {
        OnMaskCollect?.Invoke(maskWorth);
        maskCollected = true;
        Debug.Log("Double Jump Mask Collected!");
    }
}
