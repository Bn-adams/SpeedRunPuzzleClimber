using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LevelManager))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private LevelManager levelManager;

    [SerializeField] private GameObject LevelUI;

    [SerializeField] private PlayerManager _playerManager;

    [Header("Debug Level Loading (Editor Only)")]
    [SerializeField] private bool isMultiplayer = true;
    [SerializeField] private bool setLevel;
    [SerializeField] private int level;

    private void Awake()
    {
        // Singleton
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        levelManager = GetComponent<LevelManager>();

    }

    private void Update()
    {
        _playerManager = FindAnyObjectByType<PlayerManager>();

        // Debug manual level change from inspector
        if (setLevel)
        {
            setLevel = false;
            LoadLevel(level);
        }
    }

    
    public void LoadLevel(int index)
    {
        levelManager.LoadLevel(index);
    }

    public void Reloadlevel()
    {
        levelManager.ReloadLevel();
    }

    public void setCurrentLevelTime(float time)
    {
        float best = PlayerDataManager.Instance.GetSingleLevelTime(levelManager.CurrentLevelIndex);

        if (best > time || best == 0)
            PlayerDataManager.Instance.SetSingleLevelTime(levelManager.CurrentLevelIndex, time);
    }

    public float GetCurrentLevelBestTime()
    {
        return PlayerDataManager.Instance.GetSingleLevelTime(levelManager.CurrentLevelIndex);
    }
    public int GetCurrentLevel()
    {
        return levelManager.CurrentLevelIndex;
    }

    public void SetUI(bool UIEnabled)
    {
        LevelUI.SetActive(UIEnabled);
    }
}
