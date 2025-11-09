using UnityEngine;
using Redcode.Pools;


public class Spear : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject tailObject;
    [SerializeField] GameObject spearDummyModel;

    private Rigidbody rb;
    [SerializeField] float timeToLive = 5f;

    SpearTrail trailObj;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Attach a trail when the spear is first created
        if (tailObject != null)
        {
            trailObj = PoolManager.Instance.GetFromPool<SpearTrail>(1);
            trailObj.transform.position = tailObject.transform.position;
            trailObj.transform.rotation = tailObject.transform.rotation;
            
            if (trailObj != null)
            {
                trailObj.spearTailObjReference = tailObject;
            }
        }
    }
    
    void Update()
    {
        timeToLive -= Time.deltaTime;
        if (timeToLive <= 0f)
        {
            Despawn();
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        // Only rotate if moving
        if (rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            // Align spear's UP axis (local up) with its velocity direction
            Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, rb.linearVelocity.normalized);
            rb.MoveRotation(targetRotation);
        }
    }
    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.TryGetComponent<IDamageable>(out var damageable))
        {
            // // Stick the spear into the enemy
            // rb.isKinematic = true;
            // transform.SetParent(collision.transform);
            damageable.TakeDamage(10f);
            Despawn();
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            CreateDummyObject();
            Despawn();
        }
    }

    void Despawn()
    {
        // Return to pool
        timeToLive = 5f;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Detach the trail so it can continue its own lifetime independently of the spear.
        if (trailObj != null)
        {
            // Remove the reference to the tail object so the trail will stop following the spear
            trailObj.spearTailObjReference = null;
            // Do not StopTrail() here — allow the trail to live out its own timeToLive and return to its pool.
            trailObj = null;
        }

        PoolManager.Instance.TakeToPool(0, this);
    }

    void CreateDummyObject()
    {
        var dummy = PoolManager.Instance.GetFromPool<SpearDummy>(2);
        dummy.transform.position = transform.position;
        dummy.transform.rotation = transform.rotation;
    }

    void OnEnable()
    {
        // When a spear is reused from the pool it may be re-enabled; ensure it has a fresh trail attached.
        if (trailObj == null && tailObject != null)
        {
            var trail = PoolManager.Instance.GetFromPool<SpearTrail>(1);
            if (trail != null)
            {
                trailObj = trail;
                trailObj.transform.position = tailObject.transform.position;
                trailObj.transform.rotation = tailObject.transform.rotation;
                trailObj.spearTailObjReference = tailObject;
                trail.enabled = true;
            }
        }
    }
}
