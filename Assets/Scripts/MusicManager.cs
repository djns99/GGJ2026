using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public AudioSource sourceA;
    public AudioSource sourceB;
    public float fadeDuration = 1.0f;
    [Range(0, 1)] public float maxVolume = 0.5f;

    private Coroutine activeFadeRoutine;
    private bool isSourceAPlaying = true;

    public void SwapMusic(AudioClip newClip)
    {
        if (newClip == null) return;

        AudioSource activeSource = isSourceAPlaying ? sourceA : sourceB;
        AudioSource newSource = isSourceAPlaying ? sourceB : sourceA;

        // If the new clip is already playing on the 'newSource' or 'activeSource', skip
        if (activeSource.clip == newClip) return;

        // 1. Stop the old fade if it's still running
        if (activeFadeRoutine != null)
        {
            StopCoroutine(activeFadeRoutine);
        }

        // 2. Prepare the new source
        newSource.clip = newClip;
        newSource.Play();

        // 3. Start the new fade
        activeFadeRoutine = StartCoroutine(FadeRoutine(activeSource, newSource));

        // 4. Switch the toggle
        isSourceAPlaying = !isSourceAPlaying;
    }

    private IEnumerator FadeRoutine(AudioSource fadeOut, AudioSource fadeIn)
    {
        float timer = 0;
        float startVolumeOut = fadeOut.volume;
        float startVolumeIn = fadeIn.volume;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float percent = timer / fadeDuration;

            // Smoothly transition from whatever the current volume is
            fadeOut.volume = Mathf.Lerp(startVolumeOut, 0, percent);
            fadeIn.volume = Mathf.Lerp(startVolumeIn, maxVolume, percent);
            yield return null;
        }

        fadeOut.volume = 0;
        fadeOut.Stop();
        fadeIn.volume = maxVolume;
        activeFadeRoutine = null;
    }
}