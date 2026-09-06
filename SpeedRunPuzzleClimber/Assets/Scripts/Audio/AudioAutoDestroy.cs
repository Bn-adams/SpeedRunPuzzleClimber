using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioAutoDestroy : MonoBehaviour
{
    private AudioSource audioSource;

    private void OnEnable()
    {
        audioSource = GetComponent<AudioSource>();

        StartCoroutine(DestroySelf());
    }

    private IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        Destroy(gameObject);
    }
}
