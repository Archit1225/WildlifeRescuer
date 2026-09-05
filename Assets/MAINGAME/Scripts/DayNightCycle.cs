using UnityEngine;
using UnityEngine.SceneManagement;

public class DayNightCycle : MonoBehaviour
{
    [Header("Cycle Settings")]
    public float dayLengthInSeconds = 120f;
    [Range(0f, 1f)] public float timeOfDay = 0.25f; // 0 = midnight, 0.5 = noon

    [Header("References")]
    public Light sunLight;
    public Material skyboxMaterial;
    public string scoreScene = "ScoreScene";

    [Header("Sun Intensity/Color")]
    public AnimationCurve sunIntensityCurve;
    public Gradient sunColorGradient;
    public float maxSunIntensity = 1.2f;

    [Header("Ambient Light")]
    public AnimationCurve ambientIntensityCurve;
    public Gradient ambientColorGradient;

    [Header("Fog")]
    public bool controlFog = true;
    public Gradient fogColorGradient;

    private bool isNightMusicPlaying = false;

    private void Awake()
    {
        // 1. Wipe the static score bus clean the moment the scene starts
        // This guarantees a fresh slate when hitting "Retry" from the End Screen
        GameScoreData.ResetData();
    }

    private void Start()
    {
        if (BGMManager.Instance != null) BGMManager.Instance.PlayDayMusic();
    }

    private void Update()
    {
        timeOfDay += Time.deltaTime / dayLengthInSeconds;

        if (timeOfDay >= 0.75f && !isNightMusicPlaying)
        {
            isNightMusicPlaying = true;
            if (BGMManager.Instance != null) BGMManager.Instance.PlayNightMusic();
        }

        if (timeOfDay >= 1f)
        {
            timeOfDay -= 1f;
            TriggerEndOfDay();
        }

        UpdateSun();
        UpdateAmbient();
        if (controlFog) UpdateFog();
    }

    private void UpdateSun()
    {
        float sunAngle = timeOfDay * 360f - 90f;
        sunLight.transform.rotation = Quaternion.Euler(sunAngle, 170f, 0f);

        float intensityMultiplier = sunIntensityCurve.Evaluate(timeOfDay);
        sunLight.intensity = intensityMultiplier * maxSunIntensity;
        sunLight.color = sunColorGradient.Evaluate(timeOfDay);

        if (skyboxMaterial != null && skyboxMaterial.HasProperty("_SunSize"))
        {
            skyboxMaterial.SetFloat("_Exposure", Mathf.Lerp(0.3f, 1.3f, intensityMultiplier));
        }
    }

    private void UpdateAmbient()
    {
        RenderSettings.ambientIntensity = ambientIntensityCurve.Evaluate(timeOfDay);
        RenderSettings.ambientLight = ambientColorGradient.Evaluate(timeOfDay);
    }

    private void UpdateFog()
    {
        RenderSettings.fogColor = fogColorGradient.Evaluate(timeOfDay);
    }

    private void TriggerEndOfDay()
    {
        Debug.Log("Midnight reached! Loading End Screen...");

        // 2. Load the scene. Unity automatically garbage collects all active scene objects.
        SceneManager.LoadScene(scoreScene);
    }
}