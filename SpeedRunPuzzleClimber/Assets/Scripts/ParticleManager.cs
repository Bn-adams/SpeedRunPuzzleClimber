using UnityEditor;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] private GameObject sparkHalo;

    [SerializeField] private GameObject bodySpark;



    [SerializeField] private ParticleSystem fireParticle;

    public void CleartAllParticles()
    {
        fireParticle.Clear();
    }

    public void InstantiateSparkHalo(Vector3 vector)
    {
        Instantiate(sparkHalo, vector, Quaternion.identity);
    }

    public void InstantiateBodySpark(Transform transform)
    {
        Instantiate(bodySpark, transform);
    }
}
