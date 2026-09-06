using UnityEngine;

public class DynamicAudioFire : MonoBehaviour
{
    private PlayerManager _playerManager;

    [SerializeField] AudioSource _roaringFire;

    [SerializeField] bool isDyno = false;


    [SerializeField] float minVolume = 0.1f;
    [SerializeField] float maxVolume = 1f;

    [SerializeField] float minPitch = 0.85f;
    [SerializeField] float maxPitch = 1.15f;


    private void Awake()
    {
        _playerManager = FindAnyObjectByType<PlayerManager>();
    }

    private void Update()
    {
        if (!isDyno)
        {
            _roaringFire.volume = Mathf.Lerp(minVolume, maxVolume, _playerManager.PlayerSpeedScaler);
            _roaringFire.pitch = Mathf.Lerp(minPitch, maxPitch, _playerManager.PlayerSpeedScaler);
        }
        if (isDyno)
        {
            if (_playerManager.dyno.dynoed)
            {
                _roaringFire.volume = Mathf.Lerp(minVolume, maxVolume, _playerManager.PlayerSpeedScaler);
                _roaringFire.pitch = Mathf.Lerp(minPitch, maxPitch, _playerManager.PlayerSpeedScaler);
            }
            else
            {
                _roaringFire.volume = 0;

            }
        }
        
    }
}
