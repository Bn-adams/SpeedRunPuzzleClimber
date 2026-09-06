using UnityEngine;

public class ChainManager : MonoBehaviour
{
    public Transform target;

    void LateUpdate()
    {
        transform.position = target.position;
        transform.rotation = target.rotation;
        //transform.localScale = target.localScale;
    }
}

