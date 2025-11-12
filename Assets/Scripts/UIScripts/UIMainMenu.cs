using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenu : UIBase
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button aboutButton;
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
        aboutButton.onClick.AddListener(() =>
        {
            UIManager.Instance.ChangeUI(UIType.ABOUT);
        });
        exitButton.onClick.AddListener(() =>
        {
            UIManager.Instance.ChangeUI(UIType.EXIT);
        });
    }
}
