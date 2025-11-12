using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneType
{
    MAINMENU,
    STAGE1
}

[Serializable]
public struct SceneConfig
{
    [SerializeField, HideInInspector] public string name;
    public SceneType sceneType;
    public UIType initialUI;
    public SceneField scene;
    public TransitionEffect transitionEffect;
}

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public bool isGamePaused = false;
    public SceneConfig[] sceneConfigs;
    public SceneConfig currentSceneConfig;

#if UNITY_EDITOR
    private void OnValidate()
    {
        var sceneTypes = (SceneType[])Enum.GetValues(typeof(SceneType));

        // Pastikan panjang array sama dengan jumlah enum
        if (sceneConfigs == null || sceneConfigs.Length != sceneTypes.Length)
        {
            Array.Resize(ref sceneConfigs, sceneTypes.Length);
        }

        // Sinkronkan tiap SceneConfig dengan enum
        for (int i = 0; i < sceneTypes.Length; i++)
        {
            sceneConfigs[i].sceneType = sceneTypes[i];
            sceneConfigs[i].name = sceneTypes[i].ToString(); // ⬅️ isi nama otomatis

            if (sceneConfigs[i].scene == null)
            {
                Debug.LogWarning(
                    $"[GameManager] Scene for {sceneTypes[i]} is not assigned in sceneConfigs[{i}].",
                    this
                );
            }
        }

        // Pastikan currentSceneConfig valid
        bool found = false;
        for (int i = 0; i < sceneConfigs.Length; i++)
        {
            if (currentSceneConfig.sceneType == sceneConfigs[i].sceneType)
            {
                found = true;
                break;
            }
        }

        if (!found && sceneConfigs.Length > 0)
        {
            currentSceneConfig = sceneConfigs[0];
        }
    }
#endif


    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
    public void StartGame()
    {
        SceneManager.LoadScene("MainScene");
    }
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
    public void RestartGame()
    {
        LoadScene(currentSceneConfig.sceneType);
    }

    public void LoadScene(SceneType sceneType)
    {
        SceneConfig? configToLoad = null;
        foreach (var config in sceneConfigs)
        {
            if (config.sceneType == sceneType)
            {
                configToLoad = config;
                break;
            }
        }

        if (configToLoad.HasValue)
        {
            currentSceneConfig = configToLoad.Value;
            SceneTransitionManager.Instance.LoadScene(
                currentSceneConfig.scene.SceneName,
                currentSceneConfig.transitionEffect
            );
        }
        else
        {
            Debug.LogError($"[GameManager] SceneConfig for {sceneType} not found.");
        }
    }


    public void PauseGame(bool pause)
    {
        isGamePaused = pause;
        Time.timeScale = pause ? 0f : 1f;
        AudioManager.Instance.SetVolume(Sound.BGM, pause ? 0.2f : 1f);
    }

    // public void ResumeGame(bool pause)
    // {
    //     isGamePaused = !pause;
    //     Time.timeScale = pause ? 0f : 1f;

    // }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SetCursorVisibility(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
