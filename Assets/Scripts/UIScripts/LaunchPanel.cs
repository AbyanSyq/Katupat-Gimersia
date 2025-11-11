using System.Collections;
using UnityEngine;

public class LaunchPanel : MonoBehaviour
{
    [Header("Wait load all Settings")]
    [SerializeField] private AnimationController panelLaunchGame;

    private static bool hasLaunched = false;
    void Awake()
    {
        if (hasLaunched)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator Start()
    {
        hasLaunched = true;

        yield return null;
        yield return null;
        yield return null;
        yield return new WaitForEndOfFrame();

        panelLaunchGame.Hide();
    }
}
