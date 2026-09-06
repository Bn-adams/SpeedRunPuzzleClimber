using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem.HID;
[RequireComponent(typeof(JointManager))]



[RequireComponent(typeof(SpawnManager))]
[RequireComponent(typeof(TimerManager))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Dyno))]
public class PlayerManager : MonoBehaviour
{
    [SerializeField] private HUDManager _hudManager;


    public ParticleManager particleManager;
    //[SerializeField] private float divideScaler = 30;

    [SerializeField] public float divideScaler = 30;

    public float PlayerSpeedScaler
    {
        get { return bodyRB.linearVelocity.magnitude / divideScaler; }
    }

    [SerializeField] public float playerVelocity;
    [SerializeField] public float playerScaledVelocity;



    public JointManager jointManager;
    public SpawnManager spawnManager;
    public TimerManager timerManager;
    public PlayerInput playerInput;
    public CameraShake cameraShake;
    public Dyno dyno;

    [Header("Rigidbodys")]
    public Rigidbody bodyRB;
    public Rigidbody handRB;

    [Header("Shoulder Points")]
    [SerializeField] private Transform _shoulderPoint;


    // Spawning and checkpoints
    public bool isRespawning;

    public bool hasFinished;
    



    public float armLength = 4.2f;

    // Gripping
    public bool isGripping;

    // Grip types
    public bool CanGripFinish { get; set; }
    public bool CanGripCheckpoint { get; set; }
    public bool CanGripJug { get; set; }
    public bool CanGripCrimp { get; set; }
    public bool CanGripPocket { get; set; }
    public bool CanGripBreaker { get; set; }



    private void Awake()
    {
        jointManager = GetComponent<JointManager>();
        spawnManager = GetComponent<SpawnManager>();
        timerManager = GetComponent<TimerManager>();
        playerInput = GetComponent<PlayerInput>();
        particleManager = FindAnyObjectByType<ParticleManager>();
        cameraShake = FindAnyObjectByType<CameraShake>();
        dyno = GetComponent<Dyno>();
    }
    private void Start()
    {
        spawnManager.SpawnPlayer();
    }

    private void Update()
    {
        jointManager.JointChecking();

        playerVelocity = bodyRB.linearVelocity.magnitude;
        playerScaledVelocity = bodyRB.linearVelocity.magnitude / divideScaler;
    }

    public void ResetGrips()
    {
        isGripping = false;
        CanGripFinish = false;
        CanGripCheckpoint = false;
        CanGripJug = false;
        CanGripCrimp = false;
        CanGripPocket = false;
        CanGripBreaker = false;

    }
    public void Finish()
    {
        if (!hasFinished)
        {
            ResetGrips();
            hasFinished = true;
            timerManager.isTimerRunning = false;
            float timeDif = timerManager.timeElapsed - GameManager.Instance.GetCurrentLevelBestTime();
            _hudManager = FindAnyObjectByType<HUDManager>();
            if (_hudManager != null) _hudManager.OnFinish(timeDif, GameManager.Instance.GetCurrentLevelBestTime());


            Debug.Log("Best Time: " + GameManager.Instance.GetCurrentLevelBestTime());
            GameManager.Instance.setCurrentLevelTime(timerManager.timeElapsed);
            Debug.Log("Time: " + timerManager.timeElapsed + ((timeDif > 0) ? " Time difference from best: +" : " Time difference from best: ") + timeDif);
        }
    }

    public void OpenMenu()
    {
        spawnManager.currentCheckpoint = Vector2.zero;
        spawnManager.SpawnPlayer();
        GameManager.Instance.SetUI(true);
        _hudManager.gameObject.SetActive(false);
    }
}
