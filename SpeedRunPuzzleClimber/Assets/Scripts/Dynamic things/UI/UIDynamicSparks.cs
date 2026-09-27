using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class UIDynamicSparks : MonoBehaviour
{
    private UIMouseManager _UIMouseManager;

    private ParticleSystem _particle;

    [SerializeField] private float amountDiv;


    [SerializeField] private float minEmission;
    [SerializeField] private float maxEmission;

    [SerializeField] private float minSpeed;
    [SerializeField] private float maxSpeed;

    //[SerializeField] private float minSize;
    //[SerializeField] private float maxSize;

    float playerSpeedScale = 0.5f;


    private void Awake()
    {
        _UIMouseManager = GetComponentInParent<UIMouseManager>();
        _particle = GetComponent<ParticleSystem>();

    }

    private void FixedUpdate()
    {
        playerSpeedScale = _UIMouseManager.speedScalar;

        var emission = _particle.emission;
        emission.rateOverTime = Mathf.Lerp(minEmission, maxEmission, playerSpeedScale);

        var main = _particle.main;
        //main.startSpeed = Mathf.Lerp(minSpeed, maxSpeed, playerSpeedScale);

        main.startLifetime = Mathf.Lerp(minSpeed, maxSpeed, playerSpeedScale);

        

        //main.startSize = Mathf.Lerp(minSize, maxSize, playerSpeedScale);
    }

}
