using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private bool set;

    [SerializeField] private float masterVolume;
    [SerializeField] private float soundFXVolume;
    [SerializeField] private float musicVolume;

    public float MasterVolume
    {
        get { return masterVolume; }
        set 
        { 
            masterVolume = value;
            SetMasterVolume(value);
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
    public float MusicVolume
    {
        get { return musicVolume; }
        set
        {
            soundFXVolume = value;
            SetMusicVolume(value);
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


    public void SetMasterVolume(float level)
    {
        Debug.Log("volumeSet");
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(level) * 20);
    }

    public void SetSoundFXVolume(float level)
    {
        audioMixer.SetFloat("SoundFXVolume", Mathf.Log10(level) * 20);

    }

    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(level) * 20);

    }
}
