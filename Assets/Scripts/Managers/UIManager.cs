#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;
using System.Linq;

public static partial class Events
{
    public static readonly GameEvent<UIType> OnUIChanged = new GameEvent<UIType>();
}
[System.Serializable]
public class UIEntry
{
    [SerializeField, HideInInspector] public string name;
    public UIType type;
    public UIManager.UILayer layer = UIManager.UILayer.MAIN;
    public UIBase prefab;
    public bool pauseGame = false;
    public bool enablePlayerInput = true;
    public bool cursorVisibility = true;
}
public enum UIType
{
    MAINMENU,
    GAMEPLAY,
    PAUSEMENU,
    SETTINGS,
    ABOUT,
    EXIT,
    GAMEOVER,
    WIN
}
public class UIManager : SingletonMonoBehaviour<UIManager>
{
    public enum UILayer
    {
        MAIN,
        POPUP
    }

    [Header("Information")]
    [SerializeField] private UIType currentUI = UIType.MAINMENU;
    [SerializeField] private UIType previousUI;
    public UIType CurrentUI { get => currentUI; }
    public UIType PreviousUI { get => previousUI; }

    [Header("Atribut[Need To Set]")]
    [SerializeField] private Transform parent;
    [SerializeField] public List<UIEntry> uiEntries;
#if UNITY_EDITOR
    private void OnValidate()
    {
        var enumValues = System.Enum.GetValues(typeof(UIType));
        if (uiEntries.Count != enumValues.Length)
        {
            Debug.LogWarning("UIEntries count does not match UIType enum count.");
        }
        else
        {
            for (int i = 0; i < uiEntries.Count; i++)
            {
                if (uiEntries[i].type != (UIType)enumValues.GetValue(i))
                {
                    Debug.LogWarning($"UIEntry at index {i} ({uiEntries[i].type}) does not match UIType enum order ({(UIType)enumValues.GetValue(i)}).");
                }
            }
        }
        for (int i = 0; i < uiEntries.Count; i++)
        {
            uiEntries[i].name = uiEntries[i].type.ToString();
        }
    }
#endif
    private Dictionary<UIType, UIBase> uiInstances = new();
    private Dictionary<UIType, UIEntry> uiConfigs = new();

    public event Action<UIType> OnUIChanged;
    private PlayerInputAction inputActions;

    public bool IsUIActive(UIType type)
    {
        if (uiInstances.TryGetValue(type, out var ui))
        {
            return ui.isActive;
        }
        return false;
    }
    protected override void Awake()
    {
        base.Awake();
        InitUI();

        inputActions = new PlayerInputAction();
    }
    void Start()
    {
        // ShowUI(currentUI, true);
        StartCoroutine(LateShowUI());
    }

    void OnEnable()
    {
        inputActions.UI.Enable();
        inputActions.UI.Escape.performed += ctx => OnEscape();
    }

    void OnDisable()
    {
        inputActions.UI.Disable();
        inputActions.UI.Escape.performed -= ctx => OnEscape();
    }
    void OnDestroy()
    {
        inputActions.UI.Disable();
        inputActions.UI.Escape.performed -= ctx => OnEscape();
    }
    public IEnumerator LateShowUI()
    {
        yield return null;
        yield return null;
        yield return null;

        ShowUI(currentUI, true);
    }
    public void InitUI()
    {
        foreach (UILayer layer in Enum.GetValues(typeof(UILayer)))
        {
            foreach (var entry in uiEntries.Where(e => e.layer == layer))
            {
                if (uiConfigs.ContainsKey(entry.type))
                {
                    Debug.LogWarning($"Duplicate UI type: {entry.type}");
                    continue;
                }
                if (entry.prefab == null) continue;

                UIBase instance = Instantiate(entry.prefab, parent);
                if (currentUI != entry.type) instance.Hide();

                uiInstances[entry.type] = instance;
                uiConfigs[entry.type] = entry;
            }
        }
    }
    public void ChangeUI(UIType toUI)
    {
        Debug.Log($"Change ui to {toUI}");
        ShowUI(toUI);
        if (currentUI != toUI)
        {
            HideUI(currentUI, toUI);
            previousUI = currentUI;
        }

        currentUI = toUI;
        OnUIChanged?.Invoke(currentUI);
    }

    public void ShowUI(UIType toUI, bool forceShow = false)
    {
        if (!uiInstances.TryGetValue(toUI, out var ui))
        {
            Debug.LogError($"UI not found: {toUI}");
            return;
        }

        var config = uiConfigs[toUI];
        GameManager.Instance.PauseGame(config.pauseGame);
        GameManager.Instance.SetCursorVisibility(config.cursorVisibility);
        if(GameplayManager.Instance != null) GameplayManager.Instance.SetInput(config.enablePlayerInput);
        
        if (!ui.isActive || forceShow)
            ui.Show();
    }

    public void HideUI(UIType currentUI, UIType toUI)
    {
        if (uiInstances.TryGetValue(currentUI, out var instance))
        {
            if (uiConfigs[toUI].layer == UILayer.MAIN)
                instance.Hide();
            if (uiConfigs[toUI].layer == UILayer.POPUP && uiConfigs[currentUI].layer == UILayer.POPUP)
                instance.Hide();
        }
    }
    public void OnEscape()
    {
        if (currentUI == UIType.GAMEOVER)
        {
            return;
        }
        if (currentUI == UIType.GAMEPLAY)
        {
            ChangeUI(UIType.PAUSEMENU);
        }
        else if (currentUI == UIType.PAUSEMENU)
        {
            ChangeUI(UIType.GAMEPLAY);
        }
        else
        {
            ChangeUI(previousUI);
        }
    }
    private void OnApplicationFocus(bool hasFocus)
    {
        GameManager.Instance.SetCursorVisibility(uiConfigs[currentUI].cursorVisibility);
	}
}


