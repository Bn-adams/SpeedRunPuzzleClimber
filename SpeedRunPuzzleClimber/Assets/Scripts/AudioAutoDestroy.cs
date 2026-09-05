using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioAutoDestroy : MonoBehaviour
{
    private AudioSource audioSource;

    private void OnEnable()
    {
        audioSource = GetComponent<AudioSource>();

        StartCoroutine(DestroySelf(audioSource.clip.length));
    }

    private IEnumerator DestroySelf(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
