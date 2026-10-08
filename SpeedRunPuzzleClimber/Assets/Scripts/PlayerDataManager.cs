using UnityEngine;

// Class to store player information
[System.Serializable]
public class PlayerData
{
    public bool ControllerVibration;
    public bool FlickeringLights;
    public bool Background;

    public float MasterVolume;
    public float MusicVolume;
    public float SoundFXVolume;
    public float[] BestLevelTimes = new float[13];
}

// Manager to handle saving and loading player data
public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;

    private const string SAVE_KEY = "PlayerData";

    private PlayerData cachedData;

    void Awake()
    {
        // Singleton setup
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadOrCreate();
    }

    // Load existing data or create new data
    private void LoadOrCreate()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            string json = PlayerPrefs.GetString(SAVE_KEY);
            cachedData = JsonUtility.FromJson<PlayerData>(json);

            Debug.Log("Loaded PlayerData.");
        }
        else
        {
            Debug.Log("No save found, creating new data.");

            cachedData = new PlayerData();
            Save();
        }
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(cachedData);

        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();

        Debug.Log("Saved PlayerData.");
    }

    // Getters and Setters

    // Contorller vibration
    public bool GetControllerVibration()
    {
        return cachedData.ControllerVibration;
    }

    public void SetControllerViberation(bool vibration)
    {
        cachedData.ControllerVibration = vibration;
        Save();
    }


    // Flickering Lights
    public bool GetFlickeringLights()
    {
        return cachedData.FlickeringLights;
    }
    public void SetFlickeringLights(bool flickeringLights)
    {
        cachedData.FlickeringLights = flickeringLights;
        Save();
    }


    // Background
    public bool GetBackground()
    {
        return cachedData.Background;
    }
    public void SetBackground(bool background)
    {
        cachedData.FlickeringLights = background;
        Save();
    }

    // Master Volume
    public float GetMasterVolume()
    {
        return cachedData.MasterVolume;
    }
    public void SetMasterVolume(float volume)
    {
        cachedData.MasterVolume = volume;
        Save();
    }

    // Music Volume
    public float GetMusicVolume()
    {
        return cachedData.MusicVolume;
    }
    public void SetMusicVolume(float volume)
    {
        cachedData.MusicVolume = volume;
        Save();
    }

    // SFX Volume
    public float GetSoundFXVolume()
    {
        return cachedData.SoundFXVolume;
    }
    public void SetSoundFXVolume(float volume)
    {
        cachedData.SoundFXVolume = volume;
        Save();
    }

    // Level times
    public float[] GetBestLevelTimes()
    {
        return cachedData.BestLevelTimes;
    }

    public float GetSingleLevelTime(int level)
    {
        if (level < 0 || level >= cachedData.BestLevelTimes.Length)
            return -1f;

        return cachedData.BestLevelTimes[level];
    }

    public void SetSingleLevelTime(int level, float time)
    {
        if (level >= cachedData.BestLevelTimes.Length)
        {
            float[] newArray = new float[level + 1];

            cachedData.BestLevelTimes.CopyTo(newArray, 0);

            cachedData.BestLevelTimes = newArray;
        }

        cachedData.BestLevelTimes[level] = time;

        Save();
    }

    // Delete Saved Data
    public void DeleteData()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.Save();

        cachedData = new PlayerData();

        Save();

        Debug.Log("Deleted PlayerData.");
    }
}