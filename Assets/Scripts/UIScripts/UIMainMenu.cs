using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenu : UIBase
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button exitButton;
    void Start()
    {
        SetUpButton();
    }
    public void SetUpButton()
    {
        startButton.onClick.AddListener(() =>
        {
            GameManager.Instance.StartTutorial();
        });
        optionsButton.onClick.AddListener(() =>
        {
            UIManager.Instance.ChangeUI(UIType.SETTINGS);
        });
        exitButton.onClick.AddListener(() =>
        {
            GameManager.Instance.ExitGame();
        });
    }
}
