using UnityEngine;
using Redcode.Pools;
using System.Collections;


public class Spear : MonoBehaviour, IPoolObject
{
    [Header("References")]
    [SerializeField] GameObject tailObject;
    [SerializeField] GameObject spearModel;

    [Header("Inputs")]
    [SerializeField] LayerMask spearObstructorLayer;

    private Rigidbody rb;
    [SerializeField] float timeToLive = 5f;

    SpearTrail trailObj;

    public void OnCreatedInPool()
    {
        spearModel.GetComponent<MeshRenderer>().enabled = false;
    }

    public void OnGettingFromPool()
    {
        StartCoroutine(DelayEnableMeshRenderer(3));
    }

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
        else if (((1 << collision.gameObject.layer) & spearObstructorLayer.value) != 0) // convert the layer to bitmask first and check
        {
            CreateDummyObject();
            Despawn();
        }

        //Debug.Log("Spear collided with " + collision.gameObject.name);
    }
    void OnTriggerEnter(Collider collider)
    {

        if (collider.gameObject.TryGetComponent<IDamageable>(out var damageable))
        {
            // // Stick the spear into the enemy
            // rb.isKinematic = true;
            // transform.SetParent(collision.transform);
            damageable.TakeDamage(10f);
            Despawn();
        }
        else if (((1 << collider.gameObject.layer) & spearObstructorLayer.value) != 0) // convert the layer to bitmask first and check
        {
            CreateDummyObject();
            Despawn();
        }

        //Debug.Log("Spear collided with " + collider.gameObject.name);
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
            trailObj = null;
        }

        PoolManager.Instance.TakeToPool(0, this);

        spearModel.GetComponent<MeshRenderer>().enabled = false;
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

    IEnumerator DelayEnableMeshRenderer(int frame)
    {
        for (int i = 0; i < frame; i++)
            yield return new WaitForEndOfFrame();
            
        spearModel.GetComponent<MeshRenderer>().enabled = true;
    }
}
