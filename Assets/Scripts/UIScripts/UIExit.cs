using UnityEngine;
using UnityEngine.UI;

public class UIExit : UIBase
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
            GameManager.Instance.LoadScene(SceneType.MAINMENU);
        });
        noButton.onClick.AddListener(() =>
        {
            UIManager.Instance.ChangeUI(UIType.PAUSEMENU);
        });
    }
    public void CloseButton()
    {
        UIManager.Instance.ChangeUI(UIType.GAMEPLAY);
    } 
}
