using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Level Prefabs (Order Matters)")]
    public GameObject[] levelPrefabs;

    [Header("Breaker Level Settings")]
    public bool[] isBreakerLevel;

    private GameObject currentLevelInstance;
    [SerializeField] private int currentLevelIndex = -1;

    public int CurrentLevelIndex => currentLevelIndex;

    public void LoadLevel(int index)
    {
        Debug.Log("attempt load");

        // Validation
        if (index < 0 || index >= levelPrefabs.Length)
        {
            Debug.LogError("LevelManager: Invalid level index!");
            return;
        }

        //if (index == currentLevelIndex)
        //    return;

        // Remove current level and load new
        UnloadCurrentLevel();

        currentLevelInstance = Instantiate(levelPrefabs[index]);
        currentLevelIndex = index;

        Debug.Log($"[LevelManager] Loaded Level {index}");
    }

    public void UnloadCurrentLevel()
    {
        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance);
            currentLevelInstance = null;
        }

        currentLevelIndex = -1;
    }

    public void ReloadLevel()
    {
        if (currentLevelIndex != -1)
            LoadLevel(currentLevelIndex);
    }
}
