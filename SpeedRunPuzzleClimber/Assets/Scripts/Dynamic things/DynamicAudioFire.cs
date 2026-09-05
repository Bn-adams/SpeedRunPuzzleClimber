using UnityEngine;

public class DynamicAudioFire : MonoBehaviour
{
    private PlayerManager _playerManager;

    [SerializeField] AudioSource _roaringFire;

    [SerializeField] float min = 0.1f;
    [SerializeField] float max = 1f;

    private void Awake()
    {
        _playerManager = FindAnyObjectByType<PlayerManager>();
    }

    private void Update()
    {
        _roaringFire.volume = Mathf.Lerp(min, max, _playerManager.PlayerSpeedScaler);
    }
}
