using UnityEngine;

public class DynamicCamera : MonoBehaviour
{
    private PlayerManager _playerManager;

    [SerializeField] Camera _camera;

    [SerializeField] float minZoom = 60f;
    [SerializeField] float maxZoom = 80f;

    [SerializeField] float zoomSpeed = 5f;

    private void Awake()
    {
        _playerManager = FindAnyObjectByType<PlayerManager>();
    }

    private void Update()
    {
        float targetZoom = Mathf.Lerp(minZoom, maxZoom, _playerManager.PlayerSpeedScaler);

        _camera.orthographicSize = Mathf.Lerp(
           _camera.orthographicSize,
           targetZoom,
           zoomSpeed * Time.deltaTime
       );
    }
}
