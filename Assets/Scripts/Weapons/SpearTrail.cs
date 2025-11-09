using UnityEngine;
using Redcode.Pools;
using System.Collections;

public class SpearTrail : MonoBehaviour, IPoolObject
{
    [Header("References")]
    [SerializeField] TrailRenderer trail;
    public GameObject spearTailObjReference;

    [SerializeField] float timeToLive = 5f;
    private float initialTimeToLive;

    void Awake()
    {
        initialTimeToLive = timeToLive;
    }

    public void OnCreatedInPool() { }
    public void OnGettingFromPool()
    {
        trail.Clear();
        StartCoroutine(DelayTrailEnable(0.1f));
    }

    // Update is called once per frame
    void Update()
    {
        timeToLive -= Time.deltaTime;

        //trail.material.color = new Color(trail.material.color.r, trail.material.color.g, trail.material.color.b, timeToLive / initialTimeToLive);
        
        if (timeToLive <= 0f)
        {
            Despawn();
        }
        if (spearTailObjReference != null)
        {
            transform.position = spearTailObjReference.transform.position;
            transform.rotation = spearTailObjReference.transform.rotation;
        }
    }

    public void Despawn()
    {
        // Return to pool
        timeToLive = initialTimeToLive;
        spearTailObjReference = null;
        trail.Clear();
        trail.enabled = false;

        PoolManager.Instance.TakeToPool(1, this);
    }

    IEnumerator DelayTrailEnable(float seconds = 3)
    {
        yield return new WaitForSeconds(seconds);
    
        trail.enabled = true;
    }
}
