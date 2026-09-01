using UnityEngine;


public class GripCollider : MonoBehaviour
{
    [SerializeField] private PlayerManager _playerMovement;

   

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
        }
        if (collider.gameObject.CompareTag("Crimp"))
        {
            _playerMovement.CanGripCrimp = true;
            //IsBreaker(collider, true, true);
        }
        if (collider.gameObject.CompareTag("Pocket"))
        {
            _playerMovement.CanGripPocket = true;
            //IsBreaker(collider, true, true);
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
            //IsBreaker(collider, true, false);
        }
        if (collider.gameObject.CompareTag("Crimp"))
        {
            _playerMovement.CanGripCrimp = false;
            //IsBreaker(collider, true, false);
        }
        if (collider.gameObject.CompareTag("Pocket"))
        {
            _playerMovement.CanGripPocket = false;
            //IsBreaker(collider, true, false);
        }
    }
}
