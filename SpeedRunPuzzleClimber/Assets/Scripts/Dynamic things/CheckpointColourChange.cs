using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CheckpointColourChange : MonoBehaviour
{
    [SerializeField] private Light2D _light;
    [SerializeField] private Color _currentColour
        ;
    [SerializeField] private Color _targetColour;

    [SerializeField] private float _duration = 1f;


    public bool set;

    private void Update()
    {
        if (set)
        {
            ChangeColour();
            set = false;
        }
    }
    private void Start()
    {
        _currentColour = _light.color;
    }

    public void ChangeColour()
    {
        
        StartCoroutine(LerpColour());
    }

    private IEnumerator LerpColour()
    {
        Color startColour = _light.color;
        float elapsed = 0f;

        while (elapsed < _duration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / _duration;

            _light.color = Color.Lerp(startColour, _targetColour, t);

            yield return null;
        }

        
        elapsed = 0f;

        while (elapsed < _duration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / _duration;

            _light.color = Color.Lerp(_targetColour, startColour, t);

            yield return null;
        }
    }
}
