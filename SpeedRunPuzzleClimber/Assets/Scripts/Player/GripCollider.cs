using UnityEngine;


public class GripCollider : MonoBehaviour
{
    [SerializeField] private PlayerManager _playerMovement;

   

    private void OnTriggerEnter(Collider collider)
    {




        if (collider.gameObject.CompareTag("Finish"))
        {
            _playerMovement.canGripFinish = true;
        }
        if (collider.gameObject.CompareTag("Checkpoint"))
        {
            _playerMovement.canGripCheckpoint = true;
            _playerMovement.PotentialCheckpoint = collider.transform.position;
        }
        
        if (collider.gameObject.CompareTag("Jug"))
        {
            _playerMovement.canGripJug = true;
        }
        if (collider.gameObject.CompareTag("Crimp"))
        {
            _playerMovement.canGripCrimp = true;
            //IsBreaker(collider, true, true);
        }
        if (collider.gameObject.CompareTag("Pocket"))
        {
            _playerMovement.canGripPocket = true;
            //IsBreaker(collider, true, true);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Finish"))
        {
            _playerMovement.canGripFinish = false;
        }
        if (collider.gameObject.CompareTag("Checkpoint"))
        {
            _playerMovement.canGripCheckpoint = false;
        }
       
        if (collider.gameObject.CompareTag("Jug"))
        {
            _playerMovement.canGripJug = false;
            //IsBreaker(collider, true, false);
        }
        if (collider.gameObject.CompareTag("Crimp"))
        {
            _playerMovement.canGripCrimp = false;
            //IsBreaker(collider, true, false);
        }
        if (collider.gameObject.CompareTag("Pocket"))
        {
            _playerMovement.canGripPocket = false;
            //IsBreaker(collider, true, false);
        }
    }
}
