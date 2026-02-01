using System;
using System.Collections.Generic;
using UnityEngine;

public class SpeedMask : MonoBehaviour, Mask
{
    public int maskId = 3;

    public bool maskCollected = false;

    public float slowdownFactor = 0.5f;

    public void ApplyAbilities(GameObject player)
    {
        Time.timeScale = slowdownFactor;
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
        return Resources.Load<Sprite>("Sprites/speed-mask");
    }

    public void RemoveAbilities(GameObject player)
    {
        Time.timeScale = 1.0f;
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

    private void OnDestroy()
    {
        RemoveAbilities(null);
    }

    public void Collect(GameObject player)
    {
        maskCollected = true;
        Debug.Log("Double Jump Mask Collected!");
    }
}
