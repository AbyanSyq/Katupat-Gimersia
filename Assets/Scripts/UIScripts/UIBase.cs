using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.UI;

public class UIBase : MonoBehaviour
{
    [SerializeField] private AnimationController animationController;
    [SerializeField] private Button firstSelected;
    [ReadOnly] public bool isActive;
    
    public virtual void Show()
    {
        isActive = true;
        gameObject.SetActive(true);
        if(animationController != null) animationController.Show();
    }

    public virtual void Hide()
    {
        if (animationController != null) animationController.Hide();
        else gameObject.SetActive(false);
        
        isActive = false;
    }
}
