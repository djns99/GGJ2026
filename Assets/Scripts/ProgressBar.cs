using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public TextMeshProUGUI progressText;
    public Image progressBar;

    float progress,maxProgress = 100;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        progress = 0;
    }

    // Update is called once per frame
    void Update()
    {
        progressText.text = "Progress: " + Mathf.RoundToInt(progress) + " / " + Mathf.RoundToInt(maxProgress);
        if (progress > maxProgress)
            progress = maxProgress;

        ProgressBarFiller();
        //progressBar.fillAmount = progress / maxProgress;

    }

    void ProgressBarFiller()
    {
        progressBar.fillAmount = progress / maxProgress;
    }

    public void IncreaseProgress(float amount)
    {
        Debug.Log("Increasing progress by " + amount);
        if (progress < maxProgress)
        {
            progress += amount;
        }
    }

    public void OnClick() {
    Debug.Log("Clicked on Progress Bar. Current progress: " + progress);
    }

}
