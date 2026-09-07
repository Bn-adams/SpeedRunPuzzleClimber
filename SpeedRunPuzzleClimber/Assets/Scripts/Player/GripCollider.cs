using UnityEngine;


public class GripCollider : MonoBehaviour
{
    [SerializeField] private PlayerManager _playerManager;

    private void IsBreaker(Collider collider, bool isGripping)
    {
        HoldBreaker holdBreaker = collider.gameObject.GetComponent<HoldBreaker>();
        if (holdBreaker != null)
        {
            holdBreaker.GetPlayerReference(_playerManager);
            if (isGripping)
            {
                holdBreaker.OnCollision();
            }

            else
            {
                holdBreaker.EndLGrip();
            }
        }
    }

    private void OnTriggerEnter(Collider collider)
    {




        if (collider.gameObject.CompareTag("Finish"))
        {
            _playerManager.CanGripFinish = true;
        }
        if (collider.gameObject.CompareTag("Checkpoint"))
        {
            _playerManager.CanGripCheckpoint = true;
            _playerManager.spawnManager.PotentialCheckpoint = collider.transform.position;
        }
        
        if (collider.gameObject.CompareTag("Jug"))
        {
            _playerManager.CanGripJug = true;
            IsBreaker(collider, true);
        }
        if (collider.gameObject.CompareTag("Crimp"))
        {
            _playerManager.CanGripCrimp = true;
            IsBreaker(collider, true);

        }
        if (collider.gameObject.CompareTag("Pocket"))
        {
            _playerManager.CanGripPocket = true;
            IsBreaker(collider, true);

        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (_playerManager.isGripping)
        {
            _playerManager.SetGripPoint();
            return;
        }

        if (collider.gameObject.CompareTag("Finish"))
        {
            _playerManager.CanGripFinish = false;
        }
        if (collider.gameObject.CompareTag("Checkpoint"))
        {
            _playerManager.CanGripCheckpoint = false;
        }
       
        if (collider.gameObject.CompareTag("Jug"))
        {
            _playerManager.CanGripJug = false;
            IsBreaker(collider, false);

        }
        if (collider.gameObject.CompareTag("Crimp"))
        {
            _playerManager.CanGripCrimp = false;
            IsBreaker(collider, false);

        }
        if (collider.gameObject.CompareTag("Pocket"))
        {
            _playerManager.CanGripPocket = false;
            IsBreaker(collider, false);

        }
    }
}
