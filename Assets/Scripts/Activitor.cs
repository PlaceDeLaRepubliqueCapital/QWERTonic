using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;


public class Activitor : MonoBehaviour
{
    SpriteRenderer sr;
    public KeyCode key;
    Color originalColor;
    public AudioClip sound1;
    public AudioClip sound2;
    AudioSource audioSource;
    bool isSustaining = false;
    //bool keyDown = false;
    //int keyPressCount = 0;
    float originalVolume;
    //[DllImport("user32.dll")]
    //public static extern short GetKeyState(int keyCode);
    //bool isCapsLockOn = false;

    /*void Start()
    {
        isCapsLockOn = (((ushort)GetKeyState(0x14)) & 0xffff) != 0;//init stat
    }*/
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        audioSource = GetComponent<AudioSource>();
        originalVolume = audioSource.volume; // Store the original volume
    }

    // ... (existing code)

    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                // Play sound2
                PlaySound(sound2);
                sr.color = new Color(0.5f, 0.83f, 0.99f, 0.5f); // Use normalized float values for color
            }
            else
            {
                // Play sound1
                //keyPressCount++;
                PlaySound(sound1);
                sr.color = new Color(0.5f, 0.83f, 0.99f, 0.5f); // Use normalized float values for color
            }
        }
        else if (Input.GetKeyUp(key) && !isSustaining)
        {
            // Stop the audio
            StopWithFadeOut(0.07f);
            sr.color = originalColor;
            //keyPressCount = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isSustaining = true;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            isSustaining = false;
            if (!Input.anyKey) // Use Input.anyKey for efficient key state checking
            {
                StopWithFadeOut(0.07f);
                sr.color = originalColor;
                //keyPressCount = 0;
            }
        }

        /*
        if (isCapsLockOn)
        {
            isSustaining = true;
        }
        else if (!isCapsLockOn)
        {
            isSustaining = false;
            if (!Input.anyKey) // Use Input.anyKey for efficient key state checking
            {
                StopWithFadeOut(0.07f);
                sr.color = originalColor;
                //keyPressCount = 0;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.CapsLock))
        {
            isCapsLockOn = !isCapsLockOn;
            //......
        }

        if (Input.GetKeyDown(key))
        {
            keyDown = true;
            // Rest of your code
        }
        else if (Input.GetKeyUp(key))
        {
            keyDown = false;
            // Rest of your code
        }*/
    }

    // ... (existing code)

    void PlaySound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

    void StopWithFadeOut(float fadeDuration)
    {
        StartCoroutine(FadeOut(fadeDuration, originalVolume)); // Pass the original volume to the coroutine
    }

    IEnumerator FadeOut(float duration, float originalVolume)
    {
        //float startVolume = audioSource.volume;

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
