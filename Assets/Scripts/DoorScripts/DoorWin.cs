using System.Collections;
using DG.Tweening;
using UnityEngine;

public class DoorWin : MonoBehaviour
{

    [SerializeField] private Vector3 openPosition;
    [SerializeField] private Vector3 closedPosition;
    [SerializeField] private float openSpeed = 2f;
    [SerializeField] private float openDelay = 3f;
    [SerializeField] private Ease easeType = Ease.InOutSine;
    void Awake()
    {
        closedPosition = transform.localPosition;
    }

    void OnEnable()
    {
        Events.OnEnemyDied.Add(OnEnemyDied);
    }

    void OnDisable()
    {
        Events.OnEnemyDied.Remove(OnEnemyDied);
    }
    private void OnEnemyDied()
    {
        StartCoroutine(DelayOpenDoor());
    }
    private void OnDoorClose()
    {
        transform.DOLocalMove(closedPosition, openSpeed).SetEase(easeType);
    }

    IEnumerator DelayOpenDoor()
    {
        yield return new WaitForSeconds(openDelay);
        OpenDoor();
    }
    void OpenDoor()
    {
        transform.DOLocalMove(openPosition, openSpeed).SetEase(easeType);
    }
}
