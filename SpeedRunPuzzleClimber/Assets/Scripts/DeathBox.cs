using UnityEngine;

public class DeathBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        PlayerManager _playerManager = collision.gameObject.transform.parent.gameObject.GetComponent<PlayerManager>();
        
        if (_playerManager != null)
        {
            _playerManager.spawnManager.SpawnPlayer();
        }
    }
}
