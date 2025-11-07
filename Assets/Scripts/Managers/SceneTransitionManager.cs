using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class SceneTransitionManager : SingletonMonoBehaviour<SceneTransitionManager>
{
    [SerializeField] private Slider progressBar;
    [SerializeField] private GameObject transitionsContainer;

    [SerializeField] private AnimationController[] transitions;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string sceneName, string transitionName)
    {
        StartCoroutine(LoadSceneAsync(sceneName, transitionName));
    }

    private IEnumerator LoadSceneAsync(string sceneName, string transitionName)
    {
        AnimationController transition = transitions.First(t => t.name == transitionName);
        AsyncOperation scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;
        
        transition.Show();
        yield return new WaitForSeconds(transition.Duration);

        progressBar.gameObject.SetActive(true);

        do
        {
            progressBar.value = scene.progress;
            yield return null;
        } while (scene.progress < 0.9f);

        yield return new WaitForSeconds(1f);

        scene.allowSceneActivation = true;

        progressBar.gameObject.SetActive(false);

        transition.Hide();
        yield return new WaitForSeconds(transition.Duration);
    }

    [SerializeField] private SceneField targetScene;
    [SerializeField] private string transitionEffect;
    [ContextMenu("Load Target Scene")]
    public void LoadTargetScene()
    {
        LoadScene(targetScene.SceneName, transitionEffect);
    }
}
