using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UIElements;

[RequireComponent(typeof(PlayerManager))]
public class SpawnManager : MonoBehaviour
{
    private PlayerManager _playerManager;
    private HUDManager _hudManager;
    public GameObject HUD;


    // Spawning and checkpoints
    public bool IsRespawning
    {
        get { return _playerManager.isRespawning; }
        set { _playerManager.isRespawning = value; }
    }

    private Vector2 spawnPoint;
    public Vector2 SpawnPoint
    {
        get => spawnPoint;
        set
        {
            if (spawnPoint != value)
            {
                spawnPoint = value;
                currentCheckpoint = Vector2.zero;
                SpawnPlayer();
            }
        }
    }
    private Vector2 potentialCheckPoint;
    public Vector2 PotentialCheckpoint
    {
        get => potentialCheckPoint;
        set
        {
            if (potentialCheckPoint != value)
            {
                potentialCheckPoint = value;
            }
        }
    }
    public Vector2 currentCheckpoint;

    private void Awake()
    {
        _playerManager = GetComponent<PlayerManager>();
    }

    public void SpawnPlayer()
    {
        IsRespawning = true;
        _playerManager.ResetGrips();


        _playerManager.particleManager.CleartAllParticles();

        _hudManager = FindAnyObjectByType<HUDManager>();
        if (_hudManager != null) _hudManager.ResetHUD();


        _playerManager.bodyRB.linearVelocity = Vector3.zero;
        _playerManager.bodyRB.constraints = RigidbodyConstraints.FreezeAll;
        _playerManager.hasFinished = false;

        // if checkpoint is zero then restart level
        if (currentCheckpoint == Vector2.zero)
        {
            _playerManager.bodyRB.transform.position = new Vector2(spawnPoint.x, spawnPoint.y - _playerManager.armLength);
            _playerManager.handRB.transform.position = _playerManager.bodyRB.transform.position;


            //cineCam.OnTargetObjectWarped(this.transform, new Vector3(currentCheckpoint.x, currentCheckpoint.y, cineCam.transform.position.z) - transform.position);
            _playerManager.timerManager.timeElapsed = 0;
            _playerManager.timerManager.isTimerRunning = false;
            _playerManager.playerInput.shouldRestartTimer = true;

        }
        else
        {
            _playerManager.bodyRB.transform.position = new Vector2(currentCheckpoint.x, currentCheckpoint.y - _playerManager.armLength);
            _playerManager.handRB.transform.position = _playerManager.bodyRB.transform.position;

            //cineCam.OnTargetObjectWarped(this.transform, new Vector3(currentCheckpoint.x, currentCheckpoint.y, cineCam.transform.position.z) - transform.position);
        }
    }

    public void SetCheckPoint()
    {
        if (currentCheckpoint != potentialCheckPoint)
        {
            currentCheckpoint = potentialCheckPoint;
        }
    }
}
