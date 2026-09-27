using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{
    private PlayerManager _playerManager;

    public Light2D light1;
    public Light2D light2;

    public float light1Min = 0.8f;
    public float light1Max = 1.2f;

    public float light2Min = 0.6f;
    public float light2Max = 1.0f;

    public float minChangeSpeed = 0.5f;
    public float maxChangeSpeed = 2f;

    public float minBrightnessMultiplier = 0.2f;
    public float maxBrightnessMultiplier = 1f;

    private float target1;
    private float target2;


    [SerializeField] private float speed;

    private void Awake()
    {
        _playerManager = FindAnyObjectByType<PlayerManager>();
    }

    void Start()
    {
        target1 = Random.Range(light1Min, light1Max);
        target2 = Random.Range(light2Min, light2Max);
    }

    void Update()
    {
        if (_playerManager != null) speed = Mathf.Clamp01(_playerManager.PlayerSpeedScaler);
        else speed = 1;    

            // How quickly the lights change
        float flickerSpeed = Mathf.Lerp(minChangeSpeed, maxChangeSpeed, speed);

        // Speed controls overall brightness
        float brightnessMultiplier = Mathf.Lerp(minBrightnessMultiplier, maxBrightnessMultiplier, speed);

        // Pick new random targets
        if (Mathf.Abs(light1.intensity - target1 * brightnessMultiplier) < 0.05f)
            target1 = Random.Range(light1Min, light1Max);

        if (Mathf.Abs(light2.intensity - target2 * brightnessMultiplier) < 0.05f)
            target2 = Random.Range(light2Min, light2Max);

        // Apply brightness
        float target1Scaled = target1 * brightnessMultiplier;
        float target2Scaled = target2 * brightnessMultiplier;

        // Smoothly move towards targets
        light1.intensity = Mathf.Lerp(
            light1.intensity,
            target1Scaled,
            flickerSpeed * Time.deltaTime
        );

        light2.intensity = Mathf.Lerp(
            light2.intensity,
            target2Scaled,
            flickerSpeed * Time.deltaTime
        );
    }
}
