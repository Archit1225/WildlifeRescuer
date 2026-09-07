using UnityEngine;
using UnityEngine.SceneManagement;

public class DayNightCycle : MonoBehaviour
{
    [Header("Cycle Settings")]
    public float dayLengthInSeconds = 120f;
    [Range(0f, 1f)] public float timeOfDay = 0.25f; // 0 = midnight, 0.5 = noon

    [Header("References")]
    public Light sunLight;
    public Material skyboxMaterial; // Keep your single Boxophobic material assigned here
    public string scoreScene = "ScoreScene";

    [Header("Sun Intensity/Color")]
    public AnimationCurve sunIntensityCurve;
    public Gradient sunColorGradient;
    public float maxSunIntensity = 1.2f;

    [Header("Skybox Settings (Boxophobic)")]
    [Tooltip("Pure white at noon, deep purple/blue at sunset, dark blue/black at midnight.")]
    public Gradient skyboxColorGradient;
    [Tooltip("Maintains brightness (1.0) during day, drops lower (e.g., 0.2) at night.")]
    public AnimationCurve skyboxExposureCurve;

    [Header("Ambient Light")]
    public AnimationCurve ambientIntensityCurve;
    public Gradient ambientColorGradient;

    [Header("Fog")]
    public bool controlFog = true;
    public Gradient fogColorGradient;

    private bool isNightMusicPlaying = false;

    // Cached property IDs to eliminate string lookups in Update (Crucial for mobile VR performance)
    private int tintColorPropID;
    private int exposurePropID;

    private void Awake()
    {
        // 1. Wipe the static score bus clean the moment the scene starts
        GameScoreData.ResetData();

        // Cache Boxophobic's internal shader property IDs
        tintColorPropID = Shader.PropertyToID("_CubemapTintColor");
        exposurePropID = Shader.PropertyToID("_CubemapExposure");
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
        UpdateSkybox(); // Handles the Tint and Exposure of your single Boxophobic material
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
    }

    private void UpdateSkybox()
    {
        if (skyboxMaterial == null) return;

        // Smoothly shift the sky tint based on the color gradient
        Color targetSkyColor = skyboxColorGradient.Evaluate(timeOfDay);
        skyboxMaterial.SetColor(tintColorPropID, targetSkyColor);

        // Smoothly adjust shader exposure based on your curve profile
        float targetExposure = skyboxExposureCurve.Evaluate(timeOfDay);
        skyboxMaterial.SetFloat(exposurePropID, targetExposure);
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
        SceneManager.LoadScene(scoreScene);
    }
}
