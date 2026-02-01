using System;
using System.Collections.Generic;
using UnityEngine;

public class HazmatMask : MonoBehaviour, Mask
{
    public int maskId = 2;
    public float hazmatSlowedSpeed = 10f;

    public int maskWorth = 20;
    public static event Action<int> OnMaskCollect;

    private float oldSlowedSpeed = -1.0f;

    public bool maskCollected = false;



    public void ApplyAbilities(GameObject player)
    {
        var controller = player.GetComponent<PlayerController>();
        controller.hazmat = true;
        oldSlowedSpeed = controller.CurrentMaxMoveSpeed;
        controller.CurrentMaxMoveSpeed = hazmatSlowedSpeed;
    }

    public bool CanApply(GameObject player)
    {
        return maskCollected;
    }

    public int GetMaskId()
    {
        return maskId;
    }

    public AudioClip GetMaskMusic()
    {
        return null;
    }

    public Sprite GetSprite()
    {
        return null;
    }

    public void RemoveAbilities(GameObject player)
    {
        var controller = player.GetComponent<PlayerController>();
        controller.hazmat = false;
        if (oldSlowedSpeed > 0f)
        {
            // This means if someone has increased the move speed since we preserve the speed
            controller.CurrentMaxMoveSpeed = oldSlowedSpeed + (controller.CurrentMaxMoveSpeed - hazmatSlowedSpeed);
            oldSlowedSpeed = -1.0f;
        }
    }

    public void SetMaskId(int id)
    {
        maskId = id;
    }

    public bool ShouldRemove(GameObject player)
    {
        return false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Collect(GameObject player)
    {
        OnMaskCollect?.Invoke(maskWorth);
        maskCollected = true;
        Debug.Log("Double Jump Mask Collected!");
    }
}
