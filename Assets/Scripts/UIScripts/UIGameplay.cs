using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIGameplay : UIBase
{
    public void PauseHandler()
    {
        UIManager.Instance.ChangeUI(UIType.PAUSEMENU);
    } 
}
