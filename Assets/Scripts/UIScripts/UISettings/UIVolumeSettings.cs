using UnityEngine;
using UnityEngine.UI;
using Ami.BroAudio; 

public class UIVolumeSettings : MonoBehaviour
{
    [Header("UI Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    // Use OnEnable so this updates every time the Menu is opened
    private void OnEnable()
    {
        // 1. Get current values from PlayerPrefs (or default 1f)
        float masterVol = PlayerPrefs.GetFloat(AudioManager.Instance.MasterKey, 1f);
        float musicVol = PlayerPrefs.GetFloat(AudioManager.Instance.MusicKey, 1f);
        float sfxVol = PlayerPrefs.GetFloat(AudioManager.Instance.SFXKey, 1f);

        // 2. Set Slider Visuals (without triggering events if possible)
        if (masterSlider) 
        {
            masterSlider.SetValueWithoutNotify(masterVol);
            masterSlider.onValueChanged.RemoveListener(SetMasterVolume); // Safety remove
            masterSlider.onValueChanged.AddListener(SetMasterVolume);    // Add fresh listener
        }

        if (musicSlider)
        {
            musicSlider.SetValueWithoutNotify(musicVol);
            musicSlider.onValueChanged.RemoveListener(SetMusicVolume);
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sfxSlider)
        {
            sfxSlider.SetValueWithoutNotify(sfxVol);
            sfxSlider.onValueChanged.RemoveListener(SetSFXVolume);
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }
    }

    public void SetMasterVolume(float value)
    {
        BroAudio.SetVolume(BroAudioType.All, value);
        PlayerPrefs.SetFloat(AudioManager.Instance.MasterKey, value);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float value)
    {
        BroAudio.SetVolume(BroAudioType.Music, value);
        PlayerPrefs.SetFloat(AudioManager.Instance.MusicKey, value);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        BroAudio.SetVolume(BroAudioType.SFX, value);
        PlayerPrefs.SetFloat(AudioManager.Instance.SFXKey, value);
        PlayerPrefs.Save();
    }
}