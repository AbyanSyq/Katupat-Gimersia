using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIControlsSettings : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown sensitivityDropdown;
    [SerializeField] private Slider mouseSensitivitySlider;
    [SerializeField] private TMP_Text mouseSensitivityValueText;

    private float mouseSensitivity = 1.0f;

    void Start()
    {
        if (mouseSensitivitySlider != null)
        {
            mouseSensitivitySlider.onValueChanged.AddListener(SetMouseSensitivity);
            mouseSensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 1.0f);
            SetMouseSensitivity(mouseSensitivitySlider.value);
        }
    }

    public void SetMouseSensitivity(float value)
    {
        mouseSensitivity = value;
        PlayerPrefs.SetFloat("MouseSensitivity", value);
        if (mouseSensitivityValueText)
            mouseSensitivityValueText.text = $"{value:F2}";
    }

    // Optional: implement keybinding UI here
    public void ResetBindings()
    {
        Debug.Log("Resetting controls to default...");
        // Implement rebinding logic with Unity Input System if needed
    }
}
