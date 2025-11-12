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
            print("clicked yes");
            GameManager.Instance.ExitGame();
        });
        noButton.onClick.AddListener(() =>
        {
            print("clicked no");
            UIManager.Instance.OnEscape();
        });
    }
    public void CloseButton()
    {
        UIManager.Instance.ChangeUI(UIType.GAMEPLAY);
    } 
}
