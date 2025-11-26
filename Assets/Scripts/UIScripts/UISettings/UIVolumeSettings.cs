using UnityEngine;
using UnityEngine.UI;
using Ami.BroAudio; 

public class UIVolumeSettings : MonoBehaviour
{
    [Header("UI Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    // Keys for saving data

    private void Awake()
    {

        if (masterSlider) masterSlider.onValueChanged.AddListener(SetMasterVolume);
        if (musicSlider) musicSlider.onValueChanged.AddListener(SetMusicVolume);
        if (sfxSlider) sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }



    // ------------------- 🔊 Slider Event Listeners -------------------

    public void SetMasterVolume(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetVolume(BroAudioType.All, value);
        }
        
        PlayerPrefs.SetFloat(AudioManager.Instance.MasterKey, value);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetVolume(BroAudioType.Music, value);
        }

        PlayerPrefs.SetFloat(AudioManager.Instance.MusicKey, value);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetVolume(BroAudioType.SFX, value);
        }

        PlayerPrefs.SetFloat(AudioManager.Instance.SFXKey, value);
        PlayerPrefs.Save();
    }
}