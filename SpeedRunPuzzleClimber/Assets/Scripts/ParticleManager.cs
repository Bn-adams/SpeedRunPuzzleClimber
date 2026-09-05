using UnityEditor;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] private GameObject sparkHalo;

    [SerializeField] private GameObject bodySpark;


    public void InstantiateSparkHalo(Vector3 vector)
    {
        Instantiate(sparkHalo, vector, Quaternion.identity);
    }

    public void InstantiateBodySpark(Transform transform)
    {
        Instantiate(bodySpark, transform);
    }
}
