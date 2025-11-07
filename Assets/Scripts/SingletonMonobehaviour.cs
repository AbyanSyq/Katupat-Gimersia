using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T: SingletonMonoBehaviour<T>
{
    public static T Instance { get; protected set; }
 
    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            Debug.LogWarning("An instance of this singleton already exists.");
        }
        else
        {
            Instance = (T)this;
        }
    }
}
