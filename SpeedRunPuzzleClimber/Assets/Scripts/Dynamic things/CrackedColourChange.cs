using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CrackedColourChange : MonoBehaviour
{
    private GameObject parent;
    [SerializeField] private Light2D _light;
    [SerializeField] private float _currentIntensity
        ;
    [SerializeField] private Color _targetColour;

    [SerializeField] private float _duration = 0.2f;


    public bool set;

    //private void Update()
    //{
    //    if (set)
    //    {
    //        ChangeColour();
    //        set = false;
    //    }
    //}
    private void Start()
    {

        _currentIntensity = _light.intensity;
    }

    public void DimLights(float breakTime, float disabledDuration)
    {
        
        StartCoroutine(LerpColour(breakTime, disabledDuration));
    }

    private IEnumerator LerpColour(float breakTime, float disabledDuration)
    {
        float elapsed = 0f;

        while (elapsed < breakTime)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / breakTime;

            _light.intensity = Mathf.Lerp(_currentIntensity, 0, t);

            yield return null;
        }

        yield return new WaitForSeconds(disabledDuration);
        
        elapsed = 0f;

        while (elapsed < _duration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / _duration;

            _light.intensity = Mathf.Lerp(0, _currentIntensity, t);

            yield return null;
        }
    }
}
