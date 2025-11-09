using UnityEngine;
using Redcode.Pools;

public class SpearDummy : MonoBehaviour
{
    [SerializeField] float timeToLive = 10f;
    float initialTimeToLive;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialTimeToLive = timeToLive;
    }

    // Update is called once per frame
    void Update()
    {
        timeToLive -= Time.deltaTime;
        if (timeToLive <= 0f)
        {
            Despawn();
        }
    }
    public void Despawn()
    {
        timeToLive = initialTimeToLive;
        PoolManager.Instance.TakeToPool(2, this);
    }
}
