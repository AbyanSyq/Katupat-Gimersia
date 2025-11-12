using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class UIWin : UIBase
{
    // [SerializeField] private Button restartButton;
    [SerializeField] private Button exitButton;
    // public void ZoomCamera()
    // {
    //     Camera mainCamera = Camera.main;
    //     if (mainCamera != null)
    //     {
    //         mainCamera.DOFieldOfView(30f, 1.5f).SetEase(Ease.InOutSine);
    //     }
    // }
    void Start()
    {
        // restartButton.onClick.AddListener(() =>
        // {
        //     GameManager.Instance.RestartGame();
        // });
        exitButton.onClick.AddListener(() =>
        {
            GameManager.Instance.ReturnToMainMenu();
        });
    }
}
