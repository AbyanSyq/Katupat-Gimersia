using UnityEngine;
using UnityEngine.UI;

public class UIPauseMenu : UIBase
{
    [SerializeField] private Button buttonResume;
    [SerializeField] private Button buttonSettings;
    [SerializeField] private Button buttonMainMenu;

    void Start()
    {
        SetUpButton();
    }

    public void SetUpButton()
    {
        buttonResume.onClick.AddListener(() =>
        {
            print("clicked pause");
            UIManager.Instance.ChangeUI(UIType.GAMEPLAY);
        });
        buttonSettings.onClick.AddListener(() =>
        {
            print("clicked settings");
            UIManager.Instance.ChangeUI(UIType.SETTINGS);
        });
        buttonMainMenu.onClick.AddListener(() =>
        {
            print("clicked main menu");
            GameManager.Instance.ReturnToMainMenu();
        });
    }
    public void CloseButton()
    {
        UIManager.Instance.ChangeUI(UIType.GAMEPLAY);
    } 
}
