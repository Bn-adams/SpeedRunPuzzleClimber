using UnityEngine;

// Class to store player information
[System.Serializable]
public class PlayerData
{
    public string PlayerName;
    public int PlayerScore;
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

    public string GetPlayerName()
    {
        return cachedData.PlayerName;
    }

    public void SetPlayerName(string name)
    {
        cachedData.PlayerName = name;
        Save();
    }

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

    public void DeleteData()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.Save();

        cachedData = new PlayerData();

        Save();

        Debug.Log("Deleted PlayerData.");
    }
}