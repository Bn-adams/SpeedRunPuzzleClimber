using UnityEngine;

[RequireComponent(typeof(PlayerManager))]
public class DecelerationDector : MonoBehaviour
{
    private PlayerManager _playerManager;


    [Header("Detection")]
    public float decelerationThreshold = 10f;

    private Vector3 previousVelocity;

    private bool decelerationDetected = false;

    void Awake()
    {
        _playerManager = GetComponent<PlayerManager>();
        
        previousVelocity = _playerManager.bodyRB.linearVelocity;
    }

    void FixedUpdate()
    {
        Vector3 currentVelocity = _playerManager.bodyRB.linearVelocity;

        float deceleration =
            (previousVelocity.magnitude - currentVelocity.magnitude)
            / Time.fixedDeltaTime;

        // Detect sudden deceleration
        if (deceleration >= decelerationThreshold && !decelerationDetected)
        {
            decelerationDetected = true;
            SuddenDeceleration();
        }

        // Reset once the sudden deceleration has ended
        if (deceleration < decelerationThreshold)
        {
            decelerationDetected = false;
        }

        previousVelocity = currentVelocity;
    }

    void SuddenDeceleration()
    {
        Debug.Log("Sudden deceleration detected!");
        _playerManager.particleManager.InstantiateBodySpark(_playerManager.bodyRB.transform);
        //_playerManager.camerashackthatbooty.shakethegeez();
    }
}
