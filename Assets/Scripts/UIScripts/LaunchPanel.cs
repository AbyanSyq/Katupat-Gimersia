using System.Collections;
using UnityEngine;

public class LaunchPanel : MonoBehaviour
{
    [Header("Wait load all Settings")]
    [SerializeField] private AnimationController panelLaunchGame;

    private IEnumerator Start()
    {
        yield return null;
        yield return null;
        yield return null;
        yield return new WaitForSecondsRealtime(0.2f);
        yield return new WaitForEndOfFrame();

        panelLaunchGame.Hide();
    }
    public void ShowLaunchPanel()
    {
        panelLaunchGame.Show();
    }
}
