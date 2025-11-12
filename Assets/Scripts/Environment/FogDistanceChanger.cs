using System.Collections;
using UnityEngine;

public class FogDistanceChanger : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] float fogStartDistAtEnding;
    [SerializeField] float fogEndDistAtEnding;

    [Header("Original values")]
    [SerializeField, ReadOnly] Color originalColor;
    [SerializeField, ReadOnly] FogMode originalMode;
    [SerializeField, ReadOnly] float originalDensity;
    [SerializeField, ReadOnly] float originalStartDistance;
    [SerializeField, ReadOnly] float originalEndDistance;
    
    [Header("Transition")]
    [SerializeField] float transitionDuration = 1f;
    [SerializeField] bool smooth = true;

    private Coroutine fogCoroutine;

    void Start()
    {
        if (!RenderSettings.fog)
        {
            // Enable fog and set sensible defaults
            RenderSettings.fog = true;
            RenderSettings.fogColor = Color.blue;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogDensity = 0.01f;
            RenderSettings.fogStartDistance = 10f;
            RenderSettings.fogEndDistance = 50f;
        }

        // Capture current settings as the "original" values (used for Reset)
        originalColor = RenderSettings.fogColor;
        originalMode = RenderSettings.fogMode;
        originalDensity = RenderSettings.fogDensity;
        originalStartDistance = RenderSettings.fogStartDistance;
        originalEndDistance = RenderSettings.fogEndDistance;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ChangeFogDistance(fogStartDistAtEnding, fogEndDistAtEnding, transitionDuration);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ResetFogSettings();
        }
    }

    void ResetFogSettings()
    {
        // Smoothly reset to original settings
        ChangeFogDistance(originalStartDistance, originalEndDistance, transitionDuration);
        RenderSettings.fogColor = originalColor;
        RenderSettings.fogMode = originalMode;
        RenderSettings.fogDensity = originalDensity;
    }

    void ChangeFogDistance(float startDistance, float endDistance, float duration = 1f)
    {
        // Stop any previous transition
        if (fogCoroutine != null)
            StopCoroutine(fogCoroutine);

        if (duration <= 0f)
        {
            RenderSettings.fogStartDistance = startDistance;
            RenderSettings.fogEndDistance = endDistance;
        }
        else
        {
            fogCoroutine = StartCoroutine(ChangeFogDistanceSmooth(startDistance, endDistance, duration));
        }
    }

    private IEnumerator ChangeFogDistanceSmooth(float targetStart, float targetEnd, float duration)
    {
        float startStart = RenderSettings.fogStartDistance;
        float startEnd = RenderSettings.fogEndDistance;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            if (smooth) t = Mathf.SmoothStep(0f, 1f, t);
            RenderSettings.fogStartDistance = Mathf.Lerp(startStart, targetStart, t);
            RenderSettings.fogEndDistance = Mathf.Lerp(startEnd, targetEnd, t);
            yield return null;
        }

        RenderSettings.fogStartDistance = targetStart;
        RenderSettings.fogEndDistance = targetEnd;
        fogCoroutine = null;
    }
}
