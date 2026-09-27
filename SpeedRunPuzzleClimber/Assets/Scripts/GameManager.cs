using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LevelManager))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private LevelManager levelManager;

    [SerializeField] private GameObject LevelUI;

    public GameObject _player;
    private PlayerManager _playerManager;
    public GameObject UILightsParticle;

    [Header("Debug Level Loading (Editor Only)")]
    [SerializeField] private bool isMultiplayer = true;
    [SerializeField] private bool setLevel;
    [SerializeField] private int level;

    [SerializeField] private bool deleteData;


    private void Awake()
    {
        // Singleton
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        levelManager = GetComponent<LevelManager>();

    }

    private void Update()
    {

        // Debug manual level change from inspector
        if (setLevel)
        {
            setLevel = false;
            LoadLevel(level);
        }
        if (deleteData)
        {
            deleteData = false;
            DeleteData();
        }
    }

    
    public void LoadLevel(int index)
    {
        levelManager.LoadLevel(index);

        if (_player != null)
        {
            _player.SetActive(true);
            _playerManager = FindAnyObjectByType<PlayerManager>();
            if (_playerManager != null) _playerManager.spawnManager.SpawnPlayer();
            else Debug.LogError("no player :(");
            if (UILightsParticle != null) UILightsParticle.SetActive(false);
        }
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
    public void DeleteData()
    {
        PlayerDataManager.Instance.DeleteData();
    }
}
