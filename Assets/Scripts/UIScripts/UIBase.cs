using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIBase : MonoBehaviour
{
    [SerializeField] private AnimationController animationController;
    [SerializeField] private Button firstSelected;
    [ReadOnly] public bool isActive;

    void OnEnable()
    {
        if(firstSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(firstSelected.gameObject);
        }
    }
    
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
