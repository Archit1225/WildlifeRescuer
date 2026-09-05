using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;

    [Header("Assign Your Tracks Here")]
    public AudioClip startSceneMusic;
    public AudioClip dayMusic;
    public AudioClip nightMusic;
    public AudioClip endSceneMusic;

    private AudioSource audioSource;
    private Coroutine fadeRoutine;
    private float maxVolume = 0.3f; // Set your desired max volume here

    private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
            audioSource.volume = maxVolume;
            audioSource.loop = true;
            audioSource.spatialBlend = 0f;
        } 
        else 
        {
            Destroy(gameObject);
        }
    }

    // Call these from your existing scripts
    public void PlayStartMusic() => TransitionTo(startSceneMusic);
    public void PlayDayMusic() => TransitionTo(dayMusic);
    public void PlayNightMusic() => TransitionTo(nightMusic);
    public void PlayEndMusic() => TransitionTo(endSceneMusic);

    private void TransitionTo(AudioClip nextClip)
    {
        // Don't restart if it's already playing
        if (audioSource.clip == nextClip) return; 
        
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeMusic(nextClip));
    }

    private IEnumerator FadeMusic(AudioClip nextClip)
    {
        float fadeSpeed = 1f; // 1 second total fade time

        // Fade out
        while (audioSource.volume > 0) 
        {
            audioSource.volume -= maxVolume * (Time.deltaTime / fadeSpeed);
            yield return null;
        }

        audioSource.clip = nextClip;
        if (nextClip != null) audioSource.Play();

        // Fade in
        while (audioSource.volume < maxVolume) 
        {
            audioSource.volume += maxVolume * (Time.deltaTime / fadeSpeed);
            yield return null;
        }
    }
}