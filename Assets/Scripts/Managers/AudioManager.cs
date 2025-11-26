using System;
using Ami.BroAudio;
using UnityEngine;

[Serializable]
public struct SoundList
{
    [HideInInspector] public string name; // auto-filled from enum
    public SoundID soundData;             // one SoundID per enum entry
}

public enum Sound
{
    BGMMainMenu,
    BGMGolem,
    ButtonClick,
    ButtonHover,
}

public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
    public string MasterKey => masterKey;
    public string MusicKey => musicKey;
    public string SFXKey => sFXKey;
    private const string masterKey = "BroAudioVol_Master";
    private const string musicKey = "BroAudioVol_Music";
    private const string sFXKey = "BroAudioVol_SFX";

    [Header("Sound Database")]
    public SoundList[] soundDataList;  // switched to array (syncs with enum count)

    
    private void Start()
    {
        // 1. Initialize Volumes FIRST
        InitVolumes();

        // 2. Then Play BGM
        PlaySound(Sound.BGMMainMenu).AsBGM();
    }

    // --- NEW: Load saved data and apply to BroAudio ---
    private void InitVolumes()
    {
        float masterVol = PlayerPrefs.GetFloat(masterKey, 1f);
        float musicVol = PlayerPrefs.GetFloat(musicKey, 1f);
        float sfxVol = PlayerPrefs.GetFloat(sFXKey, 1f);

        BroAudio.SetVolume(BroAudioType.All, masterVol);
        BroAudio.SetVolume(BroAudioType.Music, musicVol);
        BroAudio.SetVolume(BroAudioType.SFX, sfxVol);
    }
    
    // ------------------- 🔊 Helpers -------------------
    public SoundID GetSoundID(Sound sound)
    {
        if (soundDataList == null || soundDataList.Length <= (int)sound)
        {
            Debug.LogWarning($"No SoundID assigned for {sound}");
            return default;
        }
        return soundDataList[(int)sound].soundData;
    }

    // ------------------- 🔊 Play -------------------
    public IAudioPlayer PlaySound(SoundID soundID)
    {
        return BroAudio.Play(soundID);
    }

    public IAudioPlayer PlaySound(Sound sound)
    {
        return PlaySound(GetSoundID(sound));
    }

    public IMusicPlayer PlayBGM(SoundID soundID)
    {
        return BroAudio.Play(soundID).AsBGM();
    }

    public IMusicPlayer PlayBGM(Sound sound)
    {
        return PlayBGM(GetSoundID(sound));
    }

    public IAudioPlayer PlaySoundWithFade(SoundID soundID, float fadeDuration)
    {
        return BroAudio.Play(soundID)
            .SetVolume(0f)
            .OnStart(p => p.SetVolume(1f, fadeDuration));
    }

    public IAudioPlayer PlaySoundWithFade(Sound sound, float fadeDuration)
    {
        return PlaySoundWithFade(GetSoundID(sound), fadeDuration);
    }

    

    // ------------------- 🔊 UnPause -------------------
    
    public void UnPauseSound(SoundID soundID)
    {
        BroAudio.UnPause(soundID);
    }
    public void UnPauseSoundWithFade(SoundID soundID, float fadeDuration)
    {
        BroAudio.UnPause(soundID, fadeDuration);
    }

    public void UnPauseAllSounds()
    {
        foreach (Sound sound in Enum.GetValues(typeof(Sound)))
        {
            UnPauseSound(GetSoundID(sound));
        }
    }

    // ------------------- 🔊 Pause -------------------
    public void PauseSound(SoundID soundID)
    {
        BroAudio.Pause(soundID);
    }
    public void PauseSoundWithFade(SoundID soundID, float fadeDuration)
    {
        BroAudio.Pause(soundID, fadeDuration);
    }

    public void PauseAllSounds()
    {
        foreach (Sound sound in Enum.GetValues(typeof(Sound)))
        {
            PauseSound(GetSoundID(sound));
        }
    }
    

    // ------------------- 🔊 Stop -------------------
    public void StopSound(SoundID soundID)
    {
        BroAudio.Stop(soundID);
    }

    public void StopSound(Sound sound)
    {
        StopSound(GetSoundID(sound));
    }

    public void StopSoundWithFade(SoundID soundID, float fadeDuration)
    {
        BroAudio.Stop(soundID, fadeDuration);
    }

    public void StopSoundWithFade(Sound sound, float fadeDuration)
    {
        StopSoundWithFade(GetSoundID(sound), fadeDuration);
    }

    // ------------------- 🔊 Set Volume -------------------
    public void SetVolume(Sound sound, float amount)
    {
        BroAudio.SetVolume(GetSoundID(sound), amount);
    }
    public void SetVolume(BroAudioType audioType, float amount)
    {
        BroAudio.SetVolume(audioType, amount);
    }
    

    

    // ------------------- 🔊 OnValidate -------------------

#if UNITY_EDITOR
    private void OnValidate()
    {
        string[] names = Enum.GetNames(typeof(Sound));

        // Ensure array is initialized
        if (soundDataList == null)
        {
            soundDataList = new SoundList[names.Length];
        }

        // Grow array if needed (but never shrink)
        if (soundDataList.Length < names.Length)
        {
            Array.Resize(ref soundDataList, names.Length);
        }

        // Assign enum names to slots (non-destructive: only updates empty or matching slots)
        for (int i = 0; i < names.Length; i++)
        {
            if (string.IsNullOrEmpty(soundDataList[i].name))
            {
                soundDataList[i].name = names[i];
            }
            else if (soundDataList[i].name != names[i])
            {
                // keep old name (to preserve inspector data), 
                // but you could optionally add a debug log to warn mismatch
                // Debug.LogWarning($"SoundList[{i}] name '{soundDataList[i].name}' doesn't match enum '{names[i]}'");
            }
        }
    }
#endif
}
