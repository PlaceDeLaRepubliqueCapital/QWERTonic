using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    Color activeColor = new Color(0.5f, 0.83f, 0.99f, 0.5f);

    private Coroutine fadeOutCoroutine;
    private static HashSet<KeyCode> activeKeys = new HashSet<KeyCode>(); // Track currently pressed keys

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null || sr == null)
        {
            Debug.LogError($"Required components missing on {gameObject.name}");
            enabled = false;
            return;
        }

        originalColor = sr.color;
        originalVolume = audioSource.volume;

        // Configure AudioSource for better performance
        audioSource.playOnAwake = false;
        audioSource.priority = 0; // Highest priority
    }

    void Update()
    {
        HandleKeyInput();
        HandleSustainPedal();
    }

    void HandleKeyInput()
    {
        if (Input.GetKeyDown(key))
        {
            sr.color = activeColor;
            activeKeys.Add(key);

            if (fadeOutCoroutine != null)
            {
                StopCoroutine(fadeOutCoroutine);
                audioSource.volume = originalVolume;
            }

            PlaySound(Input.GetKey(KeyCode.LeftShift) ? sound2 : sound1);
        }
        else if (Input.GetKeyUp(key))
        {
            activeKeys.Remove(key);

            if (!isSustaining)
            {
                StopSound();
                sr.color = originalColor;
            }
        }
    }

    void HandleSustainPedal()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isSustaining = true;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            isSustaining = false;

            // Reset colors and stop sounds for keys that are no longer pressed
            if (!Input.GetKey(key))
            {
                sr.color = originalColor;
                if (!activeKeys.Contains(key))
                {
                    StopSound();
                }
            }
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning($"AudioClip is not assigned for {gameObject.name}");
            return;
        }

        // Always play the sound on key press, regardless of current state
        audioSource.clip = clip;
        audioSource.volume = originalVolume;
        audioSource.Play();
    }

    void StopSound()
    {
        if (audioSource.isPlaying)
        {
            fadeOutCoroutine = StartCoroutine(FadeOut(0.07f));
        }
    }

    IEnumerator FadeOut(float duration)
    {
        if (duration <= 0)
        {
            audioSource.Stop();
            audioSource.volume = originalVolume;
            yield break;
        }

        float startVolume = audioSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / duration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = originalVolume;
    }
}