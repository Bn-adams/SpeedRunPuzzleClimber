using UnityEngine;

public class ParticleRotation : MonoBehaviour
{
    private PlayerManager _playerManager;

    private Vector3 _lastFramePos;


    private void Awake()
    {
        _playerManager = GetComponentInParent<PlayerManager>();
    }

    private void FixedUpdate()
    {
        Vector2 direction =  _lastFramePos - _playerManager.bodyRB.transform.position;



        transform.rotation = Quaternion.LookRotation(direction);

        _lastFramePos = _playerManager.bodyRB.transform.position;
    }
}
