using DG.Tweening;
using UnityEngine;


public class DoorStart : MonoBehaviour
{

    [SerializeField] private Vector3 openPosition;
    [SerializeField] private Vector3 closedPosition;
    [SerializeField] private float openSpeed = 2f;
    [SerializeField] private Ease easeType = Ease.InOutSine;
    void Awake()
    {
        transform.localPosition = closedPosition;
    }

    void OnEnable()
    {
        Events.OnEnemyDied.Add(OnDoorOpen);
    }

    void OnDisable()
    {
        Events.OnEnemyDied.Remove(OnDoorOpen);
    }
    private void OnDoorOpen()
    {
        transform.DOLocalMove(openPosition, openSpeed).SetEase(easeType);
    }
    private void OnDoorClose()
    {
        transform.DOLocalMove(closedPosition, openSpeed).SetEase(easeType);
    }
}
