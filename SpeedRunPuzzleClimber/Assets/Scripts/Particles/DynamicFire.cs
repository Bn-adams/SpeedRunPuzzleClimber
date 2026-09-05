using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class DynamicFire : MonoBehaviour
{
    private PlayerManager _playerManager;

    private ParticleSystem _particle;

    [SerializeField] float min = -0.2f;
    [SerializeField] float max = -0.5f;


    private void Awake()
    {
        _playerManager = GetComponentInParent<PlayerManager>();
        _particle = GetComponent<ParticleSystem>();

    }

    private void Update()
    {
        var main = _particle.main;

        main.gravityModifier = Mathf.Lerp(min, max, _playerManager.PlayerSpeedScaler);
    }
}
