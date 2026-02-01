using UnityEngine;

public interface Mask
{
    abstract bool CanApply(GameObject player);

    abstract void ApplyAbilities(GameObject player);

    abstract bool ShouldRemove(GameObject player);

    abstract void RemoveAbilities(GameObject player);

    abstract Sprite GetSprite();

    abstract int GetMaskId();
    abstract void SetMaskId(int id);

    abstract AudioClip GetMaskMusic();

    abstract void Collect(GameObject player);
}
