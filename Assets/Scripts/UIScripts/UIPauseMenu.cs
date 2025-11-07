using UnityEngine;
using UnityEngine.UI;

public class UIPauseMenu : UIBase
{
    [SerializeField] private Button buttonrResume;
    [SerializeField] private Button buttonSettings;
    [SerializeField] private Button buttonMainMenu;

    void Start()
    {
        SetUpButton();
    }

    public void SetUpButton()
    {
        buttonrResume.onClick.AddListener(() =>
        {
            UIManager.Instance.ChangeUI(UIType.GAMEPLAY);
        });
    }
    public void CloseButton()
    {
        UIManager.Instance.ChangeUI(UIType.GAMEPLAY);
    } 
}
