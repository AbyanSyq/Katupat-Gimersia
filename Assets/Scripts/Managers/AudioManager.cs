using System;
using System.Collections.Generic;
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
    BGM,
    BGMShift,
    BGMMainMenu,
    ButtonClick,
    ButtonHover
}

public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
    public string MasterKey => masterKey;
    public string MusicKey => musicKey;
    public string SFXKey => sFXKey;
    private const string masterKey = "BroAudio_MasterVol";
    private const string musicKey = "BroAudio_MusicVol";
    private const string sFXKey = "BroAudio_SFXVol";

    [Header("Sound Database")]
    public SoundList[] soundDataList;  // switched to array (syncs with enum count)

    protected override void Awake()
    {
        base.Awake();
        float savedMaster = PlayerPrefs.GetFloat(MasterKey, 1.0f);
        float savedMusic = PlayerPrefs.GetFloat(MusicKey, 1.0f);
        float savedSFX = PlayerPrefs.GetFloat(SFXKey, 1.0f);

        ApplyVolumes(savedMaster, savedMusic, savedSFX);
    }
    private void Start()
    {
        PlaySound(Sound.BGM).AsBGM();
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
    

        private void ApplyVolumes(float master, float music, float sfx)
    {

        SetVolume(BroAudioType.All, master);
        SetVolume(BroAudioType.Music, music);
        SetVolume(BroAudioType.SFX, sfx);
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
