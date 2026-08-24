using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Cycle Settings")]
    public float dayLengthInSeconds = 120f;
    [Range(0f, 1f)] public float timeOfDay = 0.25f; // 0 = midnight, 0.5 = noon

    [Header("References")]
    public Light sunLight;
    public Material skyboxMaterial; // optional, e.g. procedural skybox

    [Header("Sun Intensity/Color")]
    public AnimationCurve sunIntensityCurve; // x: 0-1 timeOfDay, y: 0-1 intensity multiplier
    public Gradient sunColorGradient;
    public float maxSunIntensity = 1.2f;

    [Header("Ambient Light")]
    public AnimationCurve ambientIntensityCurve;
    public Gradient ambientColorGradient;

    [Header("Fog")]
    public bool controlFog = true;
    public Gradient fogColorGradient;

    void Update()
    {
        timeOfDay += Time.deltaTime / dayLengthInSeconds;
        if (timeOfDay > 1f) timeOfDay -= 1f;

        UpdateSun();
        UpdateAmbient();
        if (controlFog) UpdateFog();
    }

    void UpdateSun()
    {
        // Rotate: sunrise at 0.25, noon at 0.5, sunset at 0.75, midnight at 0 or 1
        float sunAngle = timeOfDay * 360f - 90f;
        sunLight.transform.rotation = Quaternion.Euler(sunAngle, 170f, 0f);

        float intensityMultiplier = sunIntensityCurve.Evaluate(timeOfDay);
        sunLight.intensity = intensityMultiplier * maxSunIntensity;
        sunLight.color = sunColorGradient.Evaluate(timeOfDay);

        // Optional: drive a procedural skybox's sun position/exposure
        if (skyboxMaterial != null && skyboxMaterial.HasProperty("_SunSize"))
        {
            skyboxMaterial.SetFloat("_Exposure", Mathf.Lerp(0.3f, 1.3f, intensityMultiplier));
        }
    }

    void UpdateAmbient()
    {
        RenderSettings.ambientIntensity = ambientIntensityCurve.Evaluate(timeOfDay);
        RenderSettings.ambientLight = ambientColorGradient.Evaluate(timeOfDay);
    }

    void UpdateFog()
    {
        RenderSettings.fogColor = fogColorGradient.Evaluate(timeOfDay);
    }
}