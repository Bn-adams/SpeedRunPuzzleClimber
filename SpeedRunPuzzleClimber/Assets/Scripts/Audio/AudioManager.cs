using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private List<GameObject> soundPrefabs = new List<GameObject>();

    [SerializeField] private int maxSounds = 7;
    private GameObject currentMusicPrefab;
    public float musicFadeRate = 0.1f;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // Use this to spawn in a sound prefab
    public void SpawnAudio(GameObject soundPrefab)
    {
        if (soundPrefabs.Count < maxSounds)
        {

        }
        GameObject sound = Instantiate(soundPrefab, transform.position, Quaternion.identity);

        soundPrefabs.Add(sound);
    }

    // Use this if you want to keep track of the spawned sound object
    public void SpawnAudioWithOut(GameObject soundPrefab, out GameObject spawnedSound)
    {
        spawnedSound = Instantiate(soundPrefab, transform.position, Quaternion.identity);

        soundPrefabs.Add(spawnedSound);
    }

    public void RemoveAudio(GameObject soundPrefab)
    {
        if (soundPrefabs.Contains(soundPrefab))
        {
            soundPrefabs.Remove(soundPrefab);
        }
    }

    // Use this to spanwn a random prefab from an array
    public void SpawnRandomAudio(GameObject[] randomSounds)
    {
        if (soundPrefabs.Count < maxSounds)
        {
            int randIndex = Random.Range(0, randomSounds.Length - 1);
            GameObject sound = Instantiate(randomSounds[randIndex], transform.position, Quaternion.identity);

            soundPrefabs.Add(sound);
        }
    }

    public void SpawnMusic(GameObject musicPrefab)
    {
        if (currentMusicPrefab == null)
        {
            currentMusicPrefab = Instantiate(musicPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            AudioSource musicSource = currentMusicPrefab.GetComponent<AudioSource>();
            StartCoroutine(MusicFadeWait(musicPrefab, musicSource));

        }

    }

    private IEnumerator MusicFadeWait(GameObject musicPrefab, AudioSource musicSource)
    {
        if (musicSource != null)
        {
            while (musicSource.volume > 0f)
            {
                musicSource.volume -= musicFadeRate * Time.deltaTime;
                yield return null;
            }

            musicSource.volume = 0f;

            Destroy(currentMusicPrefab);
            currentMusicPrefab = Instantiate(musicPrefab, transform.position, Quaternion.identity);
        }
    }
}
