using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIGraphicsSettings : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private TMP_Dropdown displayModeDropdown;

    private readonly Resolution[] availableResolutions = new Resolution[]
    {
        new Resolution { width = 1920, height = 1080 },
        new Resolution { width = 1280, height = 720 },
        new Resolution { width = 800, height = 600 }
    };

    private readonly string[] qualityLevels = { "High", "Medium", "Low" };

    private enum DisplayMode
    {
        Fullscreen,
        Borderless,
        Windowed
    }

    private void Start()
    {
        InitResolutions();
        InitQuality();
        InitDisplayModes();
    }

    private void InitResolutions()
    {
        resolutionDropdown.ClearOptions();
        var options = new System.Collections.Generic.List<string>();

        int currentIndex = 0;
        for (int i = 0; i < availableResolutions.Length; i++)
        {
            var res = availableResolutions[i];
            string option = $"{res.width} x {res.height}";
            options.Add(option);

            if (res.width == Screen.currentResolution.width &&
                res.height == Screen.currentResolution.height)
            {
                currentIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentIndex;
        resolutionDropdown.RefreshShownValue();
    }

    private void InitQuality()
    {
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new System.Collections.Generic.List<string>(qualityLevels));
        qualityDropdown.value = 0; // Default: High
        qualityDropdown.RefreshShownValue();
    }

    private void InitDisplayModes()
    {
        displayModeDropdown.ClearOptions();
        displayModeDropdown.AddOptions(new System.Collections.Generic.List<string> {
            "Fullscreen",
            "Borderless",
            "Windowed"
        });

        switch (Screen.fullScreenMode)
        {
            case FullScreenMode.FullScreenWindow:
                displayModeDropdown.value = (int)DisplayMode.Fullscreen;
                break;
            case FullScreenMode.MaximizedWindow:
                displayModeDropdown.value = (int)DisplayMode.Borderless;
                break;
            case FullScreenMode.Windowed:
                displayModeDropdown.value = (int)DisplayMode.Windowed;
                break;
            default:
                displayModeDropdown.value = (int)DisplayMode.Fullscreen;
                break;
        }

        displayModeDropdown.RefreshShownValue();
    }

    // Called by Dropdown OnValueChanged events
    public void SetResolution(int index)
    {
        var res = availableResolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreenMode);
    }

    public void SetQuality(int index)
    {
        // Map manually: 0=High, 1=Medium, 2=Low
        switch (index)
        {
            case 0: QualitySettings.SetQualityLevel(2, true); break; // High
            case 1: QualitySettings.SetQualityLevel(1, true); break; // Medium
            case 2: QualitySettings.SetQualityLevel(0, true); break; // Low
        }
    }

    public void SetDisplayMode(int index)
    {
        switch ((DisplayMode)index)
        {
            case DisplayMode.Fullscreen:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case DisplayMode.Borderless:
                Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
                break;
            case DisplayMode.Windowed:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
        }

        var res = Screen.currentResolution;
        Screen.SetResolution(res.width, res.height, Screen.fullScreenMode != FullScreenMode.Windowed);
    }
}
