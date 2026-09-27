using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;


public class SplashScreen : MonoBehaviour
{
    [SerializeField] private GameObject _mouseTracker;
    [SerializeField] private Light2D globalLight;
    [SerializeField] private float globalLightIntensity;

    [SerializeField] private UIDocument uiDoc;
    private VisualElement root;
    private VisualElement element1;

    [SerializeField] private GameObject _splashSuddenSparks;
    [SerializeField] private GameObject _fireLightingSFX;

    [SerializeField] private GameObject _music;
    [SerializeField] private GameObject _emberParticlePrefab;


    [SerializeField] private float phase1 = 3f;
    [SerializeField] private float phase2 = 6.55f;

    private void Start()
    {
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;

        root = uiDoc.rootVisualElement;
        element1 = root.Q<VisualElement>("1");

        element1.style.opacity = 0;
        StartCoroutine(StartSplashScreen());

        
    }

    IEnumerator StartSplashScreen()
    {
        yield return new WaitForSeconds(phase1);
        _music.SetActive(true);
        _emberParticlePrefab.SetActive(true);


        yield return new WaitForSeconds(phase2);

        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        _mouseTracker.SetActive(true);
        Instantiate(_splashSuddenSparks, _mouseTracker.transform);
        Instantiate(_fireLightingSFX, _mouseTracker.transform);


        StartCoroutine(FadeElement(element1, 1, 2));
        StartCoroutine(FadeLight(globalLight, globalLightIntensity, 2));
        yield return null;

    }


    IEnumerator FadeElement(VisualElement element, float targetOpacity, float duration)
    {
        float startOpacity = element.resolvedStyle.opacity;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = time / duration;
            element.style.opacity = Mathf.Lerp(startOpacity, targetOpacity, t);

            yield return null;
        }

        element.style.opacity = targetOpacity;
    }

    IEnumerator FadeLight(Light2D light, float targetIntensity, float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = time / duration;
            light.intensity = Mathf.Lerp(0, targetIntensity, t);

            yield return null;
        }

        light.intensity = targetIntensity;
    }
}
