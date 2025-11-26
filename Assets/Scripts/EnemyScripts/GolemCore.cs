using DG.Tweening;
using UnityEngine;

public class GolemCore : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Transform spawnPoint; // Titik awal garis (misal: tangan/dada golem)
    [SerializeField] Transform targetLineTransform; // Titik tujuan
    [SerializeField] Ease easeType = Ease.OutBack;

    public void Update()
    {
        // PENTING: Pastikan spawnPoint tidak null untuk menghindari error
        if (spawnPoint != null && targetLineTransform != null) 
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, targetLineTransform.position);
        }
    }

    public void Spawn(float duration)
    {
        transform.DOLocalMove(new Vector3(0, 0, 0), duration).SetEase(easeType);
    }
}