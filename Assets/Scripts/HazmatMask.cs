using UnityEngine;

public class HazmatMask : MonoBehaviour, Mask
{
    public int maskId = 2;

    public void ApplyAbilities(GameObject player)
    {
        player.GetComponent<PlayerController>().hazmat = true;
    }

    public bool CanApply(GameObject player)
    {
        return true;
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
        player.GetComponent<PlayerController>().hazmat = false;
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
}
