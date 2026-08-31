using UnityEngine;



[RequireComponent(typeof(JointManager))]
public class PlayerManager : MonoBehaviour
{
    private JointManager _jointManager;

    [Header("Rigidbodys")]
    public Rigidbody bodyRB;
    public Rigidbody handRB;

    [Header("Shoulder Points")]
    [SerializeField] private Transform _shoulderPoint;


    // Spawning and checkpoints
    public bool isRespawning;
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
                //SpawnPlayer();
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



    public float armLength = 4.2f;

    public bool canGripFinish { get; set; }
    public bool canGripCheckpoint { get; set; }
    public bool canGripJug { get; set; }
    public bool canGripCrimp { get; set; }
    public bool canGripPocket { get; set; }
    public bool canGripBreaker { get; set; }


    public bool isGripping;

    private void Awake()
    {
        _jointManager = GetComponent<JointManager>();
    }

    private void Update()
    {
        _jointManager.JointChecking();
    }

    public void SpawnPlayer()
    {

    }
}
