using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailRendererController : MonoBehaviour
{
    [Header("Trail Settings")]
    public bool trailEnabled = true; // Toggle trail on/off
    public float trailTime = 2f; // Duration of the trail (set to 0 for unlimited)
    public float trailWidth = 0.1f; // Constant width of the trail
    public Color trailColor = Color.white; // Base color of the trail
    public bool colorChangesOverTime = false; // Toggle color change over time

    private TrailRenderer trailRenderer;
    private float timeSinceStart = 0f;

    void Start()
    {
        // Add a TrailRenderer component if it doesn't exist
        if (!TryGetComponent(out trailRenderer))
        {
            trailRenderer = gameObject.AddComponent<TrailRenderer>();
            trailRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        // Initialize the trail settings
        UpdateTrailSettings();
    }

    void Update()
    {
        // Toggle the trail on/off
        trailRenderer.enabled = trailEnabled;

        // Update the trail's color over time if enabled
        if (colorChangesOverTime)
        {
            UpdateRainbowColorOverTime();
        }
    }

    void UpdateTrailSettings()
    {
        // Set the trail's time (0 means unlimited)
        trailRenderer.time = trailTime;

        // Set the trail's width
        trailRenderer.startWidth = trailWidth;
        trailRenderer.endWidth = trailWidth;

        // Set the trail's initial color
        trailRenderer.material = new Material(Shader.Find("Standard"));
        trailRenderer.material.color = trailColor;
    }

    void UpdateRainbowColorOverTime()
    {
        // Calculate a normalized time value (0 to 1) based on the trail's duration
        float normalizedTime = timeSinceStart / trailTime;

        // Generate a rainbow color based on the normalized time
        Color rainbowColor = GetRainbowColor(normalizedTime);

        // Apply the color to the trail
        trailRenderer.material.color = rainbowColor;

        // Increment the time since start
        timeSinceStart += Time.deltaTime;

        // Reset the timer if it exceeds the trail time
        if (timeSinceStart > trailTime)
        {
            timeSinceStart = 0f;
        }
    }

    Color GetRainbowColor(float time)
    {
        // Map the time value to a hue in the HSV color space (0 to 1)
        float hue = time % 1f; // Ensure the value stays within 0-1
        return Color.HSVToRGB(hue, 1f, 1f); // Convert HSV to RGB
    }

    // Public method to toggle the trail on/off
    public void ToggleTrail(bool enabled)
    {
        trailEnabled = enabled;
    }

    // Public method to set the trail time
    public void SetTrailTime(float time)
    {
        trailTime = time;
        UpdateTrailSettings();
    }

    // Public method to set the trail width
    public void SetTrailWidth(float width)
    {
        trailWidth = width;
        UpdateTrailSettings();
    }

    // Public method to set the trail color
    public void SetTrailColor(Color color)
    {
        trailColor = color;
        UpdateTrailSettings();
    }
}