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

    void Update()
    {
        timeOfDay += Time.deltaTime / dayLengthInSeconds;
        
        if (timeOfDay >= 1f) 
        {
            timeOfDay -= 1f;
            TriggerEndOfDay(); 
        }

        UpdateSun();
        UpdateAmbient();
        if (controlFog) UpdateFog();
    }

    void UpdateSun()
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

    void UpdateAmbient()
    {
        RenderSettings.ambientIntensity = ambientIntensityCurve.Evaluate(timeOfDay);
        RenderSettings.ambientLight = ambientColorGradient.Evaluate(timeOfDay);
    }

    void UpdateFog()
    {
        RenderSettings.fogColor = fogColorGradient.Evaluate(timeOfDay);
    }

    private void TriggerEndOfDay()
    {
        Debug.Log("Midnight reached! Loading End Screen...");

        // Make sure "EndScreen" matches the exact name of your End Screen scene in Build Settings!
        SceneManager.LoadScene("EndScreen");
    }
}