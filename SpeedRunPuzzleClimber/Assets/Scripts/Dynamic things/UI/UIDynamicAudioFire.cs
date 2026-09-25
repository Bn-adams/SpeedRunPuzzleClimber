using UnityEngine;

public class UIDynamicAudioFire : MonoBehaviour
{
    private UIMouseManager _UIMouseManager;

    [SerializeField] AudioSource _roaringFire;
    [SerializeField] GameObject _roaringFireFadeOut;



    [SerializeField] float minVolume = 0.1f;
    [SerializeField] float maxVolume = 1f;

    [SerializeField] float minPitch = 0.85f;
    [SerializeField] float maxPitch = 1.15f;

    private float fadeOutSpeed = 0.005f;
    private void Awake()
    {
        _UIMouseManager = GetComponentInParent<UIMouseManager>();

    }

    private void Update()
    {

        _roaringFire.volume = Mathf.Lerp(minVolume, maxVolume, _UIMouseManager.speedScalar);
        _roaringFire.pitch = Mathf.Lerp(minPitch, maxPitch, _UIMouseManager.speedScalar);
        
    }
}
