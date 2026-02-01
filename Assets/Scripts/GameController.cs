using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private int progressAmount;
    public Slider progressSlider;

    private List<Mask> collectedMasks = new List<Mask>();
    private float score = 0f;
    private float distanceTraveled = 0f;
    private Vector2 lastPlayerPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        score = 0f;
        progressAmount = 0;
        DoubleJumpMask.OnMaskCollect += IncreaseProgressAmount;
    }

    void IncreaseProgressAmount(int amount)
    {
        progressAmount += amount;
        progressSlider.value = progressAmount;

        Debug.Log("Progress increased by " + amount + ". Total progress: " + progressAmount);
        if(progressAmount >= 100)
        {
            Debug.Log("Level Complete!");
            // Handle level completion logic here
        }
    }

    // Update is called once per frame
    void Update()
    {
        TrackDistance();
    }

    public void CollectMask(Mask mask)
    {
        if (!collectedMasks.Contains(mask))
        {
            collectedMasks.Add(mask);
            Debug.Log("Collected mask: " + mask.GetMaskId());
        }
    }

    public bool HasMask(int maskId)
    {
        foreach (var mask in collectedMasks)
        {
            if (mask.GetMaskId() == maskId)
            {
                return true;
            }
        }
        return false;
    }

    private void TrackDistance()  {
        Transform playerTransform = GameObject.FindWithTag("Player").transform;
        if (playerTransform != null)
        {
            float distanceSinceLastFrame = Vector2.Distance(lastPlayerPosition, playerTransform.position);
            distanceTraveled += distanceSinceLastFrame;
            lastPlayerPosition = playerTransform.position;
        }
    }

    public void ResetGame()
    {
        collectedMasks.Clear();
        score = 0f;
        distanceTraveled = 0f;
        lastPlayerPosition = Vector2.zero;
    }
}
