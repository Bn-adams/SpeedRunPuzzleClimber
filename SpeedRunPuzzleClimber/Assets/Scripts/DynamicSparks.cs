using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class DynamicSparks : MonoBehaviour
{
    private PlayerManager _playerManager;

    private ParticleSystem _particle;

    [SerializeField] private float amountDiv;


    [SerializeField] private float minEmission;
    [SerializeField] private float maxEmission;

    [SerializeField] private float minSpeed;
    [SerializeField] private float maxSpeed;

    [SerializeField] private float minSize;
    [SerializeField] private float maxSize;




    private void Awake()
    {
        _playerManager = GetComponentInParent<PlayerManager>();
        _particle = GetComponent<ParticleSystem>();

    }

    private void FixedUpdate()
    {
        float playerSpeedScale = _playerManager.bodyRB.linearVelocity.magnitude / amountDiv;

        var emission = _particle.emission;
        emission.rateOverTime = Mathf.Lerp(minEmission, maxEmission, playerSpeedScale);

        var main = _particle.main;
        main.startSpeed = Mathf.Lerp(minSpeed, maxSpeed, playerSpeedScale);

        main.startSize = Mathf.Lerp(minSize, maxSize, playerSpeedScale);
    }

}
