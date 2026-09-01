using UnityEngine;


public class GripCollider : MonoBehaviour
{
    [SerializeField] private PlayerManager _playerMovement;

    private void IsBreaker(Collider collider, bool isGripping)
    {
        HoldBreaker holdBreaker = collider.gameObject.GetComponent<HoldBreaker>();
        if (holdBreaker != null)
        {
            holdBreaker.GetPlayerReference(_playerMovement);
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
            _playerMovement.CanGripFinish = true;
        }
        if (collider.gameObject.CompareTag("Checkpoint"))
        {
            _playerMovement.CanGripCheckpoint = true;
            _playerMovement.spawnManager.PotentialCheckpoint = collider.transform.position;
        }
        
        if (collider.gameObject.CompareTag("Jug"))
        {
            _playerMovement.CanGripJug = true;
            IsBreaker(collider, true);
        }
        if (collider.gameObject.CompareTag("Crimp"))
        {
            _playerMovement.CanGripCrimp = true;
            IsBreaker(collider, true);

        }
        if (collider.gameObject.CompareTag("Pocket"))
        {
            _playerMovement.CanGripPocket = true;
            IsBreaker(collider, true);

        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Finish"))
        {
            _playerMovement.CanGripFinish = false;
        }
        if (collider.gameObject.CompareTag("Checkpoint"))
        {
            _playerMovement.CanGripCheckpoint = false;
        }
       
        if (collider.gameObject.CompareTag("Jug"))
        {
            _playerMovement.CanGripJug = false;
            IsBreaker(collider, false);

        }
        if (collider.gameObject.CompareTag("Crimp"))
        {
            _playerMovement.CanGripCrimp = false;
            IsBreaker(collider, false);

        }
        if (collider.gameObject.CompareTag("Pocket"))
        {
            _playerMovement.CanGripPocket = false;
            IsBreaker(collider, false);

        }
    }
}
