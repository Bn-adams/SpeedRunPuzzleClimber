using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private bool set;

    [SerializeField] private float masterVolume;
    [SerializeField] private float musicVolume;
    [SerializeField] private float soundFXVolume;

    public float MasterVolume
    {
        get { return masterVolume; }
        set
        {
            masterVolume = value;
            SetMasterVolume(value);
        }
    }
    public float MusicVolume
    {
        get { return musicVolume; }
        set
        {
            musicVolume = value;
            SetMusicVolume(value);
        }
    }
    public float SoundFXVolume
    {
        get { return soundFXVolume; }
        set
        {
            soundFXVolume = value;
            SetSoundFXVolume(value);
        }
    }

    private void Update()
    {
        if (set)
        {
            set = false;
            MasterVolume = masterVolume;
            SoundFXVolume = soundFXVolume;
            MusicVolume = musicVolume;
        }
    }
    private void Start()
    {
        SetMasterVolume(PlayerDataManager.Instance.GetMasterVolume());
        SetMusicVolume(PlayerDataManager.Instance.GetMusicVolume());
        SetSoundFXVolume(PlayerDataManager.Instance.GetSoundFXVolume());
    }


    public void SetMasterVolume(float level)
    {
        level = Mathf.Clamp(level, 0.0001f, 1);
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(level) * 20);
        PlayerDataManager.Instance.SetMasterVolume(level);
    }
    public void SetMusicVolume(float level)
    {
        level = Mathf.Clamp(level, 0.0001f, 1);
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(level) * 20);
        PlayerDataManager.Instance.SetMusicVolume(level);
    }
    public void SetSoundFXVolume(float level)
    {
        level = Mathf.Clamp(level, 0.0001f, 1);
        audioMixer.SetFloat("SoundFXVolume", Mathf.Log10(level) * 20);
        PlayerDataManager.Instance.SetSoundFXVolume(level);
    }

}
