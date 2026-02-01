using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Image progressBar;
    public Image playerAvatar;
    public Image chaserAvatar;
    public GameObject player;
    public GameObject chaser;

    private float maxProgress;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxProgress = player.GetComponent<PlayerController>().targetDistance;
        progressBar = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (progressBar == null)
        {
            Debug.LogWarning("[ProgressBar] progressBar is null. Assign your Canvas Image in the Inspector or place this script on the Image GameObject.");
            return;
        }
        if (playerAvatar == null)
        {
            Debug.LogWarning("[ProgressBar] playerAvatar is null. Assign the player avatar Image in the Inspector.");
            return;
        }
        if (chaserAvatar == null)
        {
            Debug.LogWarning("[ProgressBar] chaserAvatar is null. Assign the chaser avatar Image in the Inspector.");
            return;
        }
        if (player == null)
        {
            Debug.LogWarning("[ProgressBar] player GameObject is null. Assign it or tag the player with \"Player\".");
            return;
        }
        if (chaser == null)
        {
            Debug.LogWarning("[ProgressBar] chaser GameObject is null. Assign it or tag/name the chaser accordingly.");
            return;
        }
        if (player == null || chaser == null) return;
        
        // Get x position
        float playerX = player.transform.position.x;
        float chaserX = chaser.transform.position.x;

        PositionAvatars(playerX, chaserX);
    }

    void PositionAvatars(float playerX, float chaserX)
    {
        // Normalize positions to 0-1 range based on maxProgress
        float playerProgress = Mathf.Clamp01(playerX / maxProgress); // % of the way to maxProgress
        float chaserProgress = Mathf.Clamp01(chaserX / maxProgress); // % of the way to maxProgress    

        // Convert to screen position on the progress bar
        float barWidth = progressBar.rectTransform.rect.width;

        float playerXPosOnBar = (playerProgress * barWidth);
        float chaserXPosOnBar = (chaserProgress * barWidth);
        Debug.Log("playerXPosOnBar:" + playerXPosOnBar);
     
        playerAvatar.rectTransform.anchoredPosition = new Vector2(playerXPosOnBar, playerAvatar.rectTransform.anchoredPosition.y);
        chaserAvatar.rectTransform.anchoredPosition = new Vector2(chaserXPosOnBar, chaserAvatar.rectTransform.anchoredPosition.y);
    }
}
