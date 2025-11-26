using UnityEngine;
using UnityEngine.UI;

public class UIQuit : UIBase
{
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    void Start()
    {
        SetUpButton();
    }

    public void SetUpButton()
    {
        yesButton.onClick.AddListener(() =>
        {
            GameManager.Instance.ExitGame();
        });
        noButton.onClick.AddListener(() =>
        {
            UIManager.Instance.ChangeUI(UIType.MAINMENU);
        });
    }
    public void CloseButton()
    {
        UIManager.Instance.ChangeUI(UIType.GAMEPLAY);
    } 
}
