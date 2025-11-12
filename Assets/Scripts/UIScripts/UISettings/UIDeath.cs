using UnityEngine;
using DG.Tweening;
public class UIDeath : UIBase
{

    public void ZoomCamera(){
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.DOFieldOfView(30f, 1.5f).SetEase(Ease.InOutSine);
        }
    }
}
