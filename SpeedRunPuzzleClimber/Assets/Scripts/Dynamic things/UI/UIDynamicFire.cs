using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class UIDynamicFire : MonoBehaviour
{
    private UIMouseManager _UIMouseManager;

    private ParticleSystem _particle;

    [SerializeField] float min = -0.2f;
    [SerializeField] float max = -0.5f;


    private void Awake()
    {
        _UIMouseManager = GetComponentInParent<UIMouseManager>();

        _particle = GetComponent<ParticleSystem>();

    }

    private void Update()
    {
        var main = _particle.main;

        main.gravityModifier = Mathf.Lerp(min, max, _UIMouseManager.speedScalar);
    }
}
