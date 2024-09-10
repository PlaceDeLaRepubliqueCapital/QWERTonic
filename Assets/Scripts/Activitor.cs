using UnityEngine;
using System.Collections;

public class Activitor : MonoBehaviour
{
    SpriteRenderer sr;
    public KeyCode key;
    Color originalColor;
    public AudioClip sound1;
    public AudioClip sound2;
    AudioSource audioSource;
    bool isSustaining = false;
    float originalVolume;
    Color activeColor = new Color(0.5f, 0.83f, 0.99f, 0.5f); // Define active color here

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        audioSource = GetComponent<AudioSource>();
        originalVolume = audioSource.volume; // Store the original volume
    }

    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            sr.color = activeColor; // Set the active color for key press

            if (Input.GetKey(KeyCode.LeftShift))
            {
                // Play sound2
                PlaySound(sound2);
            }
            else
            {
                // Play sound1
                PlaySound(sound1);
            }
        }
        else if (Input.GetKeyUp(key) && !isSustaining)
        {
            // Stop the audio with fade out
            StopWithFadeOut(0.07f);
            sr.color = originalColor;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isSustaining = true;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            isSustaining = false;
            if (!Input.anyKey)
            {
                StopWithFadeOut(0.07f);
                sr.color = originalColor;
            }
        }
    }

    void PlaySound(AudioClip clip)
    {
        // Check if AudioClip is assigned before playing
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("AudioClip is not assigned.");
        }
    }

    void StopWithFadeOut(float fadeDuration)
    {
        StartCoroutine(FadeOut(fadeDuration, originalVolume)); // Pass the original volume to the coroutine
    }

    IEnumerator FadeOut(float duration, float originalVolume)
    {
        // Check for zero or negative duration to avoid issues
        if (duration <= 0)
        {
            audioSource.volume = 0;
            audioSource.Stop();
            audioSource.volume = originalVolume; // Reset volume to original after stop
            yield break;
        }

        while (audioSource.volume > 0)
        {
            audioSource.volume -= originalVolume * Time.deltaTime / duration; // Use originalVolume for volume adjustment
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = 0; // Explicitly set the volume to 0 after the fade-out

        audioSource.volume = originalVolume; // Reset the volume to its original value
    }
}
