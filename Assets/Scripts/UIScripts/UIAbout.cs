using UnityEngine;
using UnityEngine.UI;

public class UIAbout : UIBase
{
    [SerializeField] private Button backButton;
    void Start()
    {
        backButton.onClick.AddListener(() =>
        {
            UIManager.Instance.OnEscape();
        });
    }
}
