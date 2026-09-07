using UnityEngine;

[RequireComponent(typeof(PlayerManager))]
public class Dyno : MonoBehaviour
{
    private PlayerManager _playerManager;

    [SerializeField] private float playerVelocity;
    [SerializeField] private float playerScaledVelocity;

    [SerializeField] public bool dynoed = false;
    [SerializeField] private float dynoEnterAmount = 0.5f;
    [SerializeField] private float dynoExitAmount = 0.2f;

    [SerializeField] private GameObject dynoSoundPrefab;

    public GameObject shineParticlePrefab;
    private GameObject _shineParticle;

    public GameObject dynoRingParticlePrefab;


    private void Awake()
    {
        _playerManager = GetComponent<PlayerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        playerVelocity = _playerManager.bodyRB.linearVelocity.magnitude;
        playerScaledVelocity = _playerManager.bodyRB.linearVelocity.magnitude / _playerManager.divideScaler;

        EndDyno();
    }
    public void StartDyno()
    {
        if (playerScaledVelocity > dynoEnterAmount && !dynoed)
        {
            dynoed = true;
            _shineParticle = Instantiate(shineParticlePrefab, _playerManager.bodyRB.transform);
            _playerManager.cameraShake.Shake();
            Instantiate(dynoRingParticlePrefab, _playerManager.bodyRB.transform);
            Instantiate(dynoSoundPrefab);
        }
    }

    public void EndDyno()
    {
        if (playerScaledVelocity < dynoExitAmount && dynoed)
        {
            dynoed = false;
            _shineParticle.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
    public void ForceEndDyno()
    {
        dynoed = false;
        if (_shineParticle != null)
        {
            _shineParticle.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}
