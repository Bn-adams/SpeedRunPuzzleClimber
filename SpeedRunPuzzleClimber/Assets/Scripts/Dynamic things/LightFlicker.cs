using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{
    private PlayerManager _playerManager;

    public Light2D light1;
    public Light2D light2;
    public Light2D light3;

    public float light1Min = 0.8f;
    public float light1Max = 1.2f;

    public float light2Min = 0.6f;
    public float light2Max = 1.0f;

    public float light3Min = 0.7f;
    public float light3Max = 1.4f;

    public float changeSpeed = 2f;

    private float target1;
    private float target2;
    private float target3;

    private void Awake()
    {
        _playerManager = FindAnyObjectByType<PlayerManager>();
    }

    void Start()
    {
        target1 = Random.Range(light1Min, light1Max);
        target2 = Random.Range(light2Min, light2Max);
        target3 = Random.Range(light3Min, light3Max);
    }

    void Update()
    {
        // Player speed from 0 to 1
        float speed = Mathf.Clamp01(_playerManager.PlayerSpeedScaler);

        // How quickly the lights change
        float flickerSpeed = Mathf.Lerp(0.5f, changeSpeed, speed);

        // Speed controls overall brightness
        float brightnessMultiplier = Mathf.Lerp(0.4f, 1f, speed);

        // Pick new random targets
        if (Mathf.Abs(light1.intensity - target1 * brightnessMultiplier) < 0.05f)
            target1 = Random.Range(light1Min, light1Max);

        if (Mathf.Abs(light2.intensity - target2 * brightnessMultiplier) < 0.05f)
            target2 = Random.Range(light2Min, light2Max);

        if (Mathf.Abs(light3.intensity - target3 * brightnessMultiplier) < 0.05f)
            target3 = Random.Range(light3Min, light3Max);

        // Apply brightness
        float target1Scaled = target1 * brightnessMultiplier;
        float target2Scaled = target2 * brightnessMultiplier;
        float target3Scaled = target3 * brightnessMultiplier;

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

        light3.intensity = Mathf.Lerp(
            light3.intensity,
            target3Scaled,
            flickerSpeed * Time.deltaTime
        );
    }
}
