using UnityEngine;
using UnityEngine.UI;


public enum UISettingsType
{
    MAIN,
    GRAPHICS,
    CONTROLS,
    VOLUME,
    ABOUT
}
public class UISettings : UIBase
{
    [SerializeField, ReadOnly] private UISettingsType settingsType;

    [Header("Buttons")]
    private Button selectedButton;
    [SerializeField] private Button graphicsButton;
    [SerializeField] private Button controlsButton;
    [SerializeField] private Button volumeButton;
    [SerializeField] private Button aboutButton;
    [SerializeField] private Button backButton;
    [Header("Panels")]
    [SerializeField] private AnimationController mainPanel;
    [SerializeField] private AnimationController graphicsPanel;
    [SerializeField] private AnimationController controlsPanel;
    [SerializeField] private AnimationController volumePanel;
    [SerializeField] private AnimationController aboutPanel;
    // [SerializeField] private AnimationController aboutPanel;

    void Start()
    {
        SetUpButton();
    }
    void OnEnable()
    {
        selectedButton = graphicsButton;
        SwitchPanel(UISettingsType.GRAPHICS);
    }
    private void Update()
    {
        selectedButton.Select();
    }
    private void SetUpButton()
    {
        graphicsButton.onClick.AddListener(() =>
        {
            SwitchPanel(UISettingsType.GRAPHICS);
        });
        controlsButton.onClick.AddListener(() =>
        {
            SwitchPanel(UISettingsType.CONTROLS);   
        });
        volumeButton.onClick.AddListener(() =>
        {
            SwitchPanel(UISettingsType.VOLUME);
        });
        aboutButton.onClick.AddListener(() =>
        {
            UIManager.Instance.ChangeUI(UIType.ABOUT);
        });
        backButton.onClick.AddListener(() =>
        {
            UIManager.Instance.OnEscape();
        });
    }

    private void SwitchPanel(UISettingsType type)
    {
        settingsType = type;
        switch (type)
        {
            case UISettingsType.MAIN:
                selectedButton = graphicsButton;
                mainPanel.Show();
                graphicsPanel.Hide();
                controlsPanel.Hide();
                volumePanel.Hide();
                aboutPanel.Hide();
                break;
            case UISettingsType.GRAPHICS:
                selectedButton = graphicsButton;
                graphicsPanel.Show();
                controlsPanel.Hide();
                volumePanel.Hide();
                break;
            case UISettingsType.CONTROLS:
                selectedButton = controlsButton;
                graphicsPanel.Hide();
                controlsPanel.Show();
                volumePanel.Hide();
                break;
            case UISettingsType.VOLUME:
                selectedButton = volumeButton;
                graphicsPanel.Hide();
                controlsPanel.Hide();
                volumePanel.Show();
                break;
            case UISettingsType.ABOUT:
                selectedButton = aboutButton;
                graphicsPanel.Hide();
                controlsPanel.Hide();
                volumePanel.Hide();
                break;
        }

    }
    
}
