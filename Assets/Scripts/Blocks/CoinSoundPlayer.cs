using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayCoinSounds : MonoBehaviour
{

    private AudioSource audioSource;
    public AudioClip coinSpawnSound;
    public AudioClip coinCollectSound;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (!audioSource)
        {
            throw new System.Exception("audio source not set on block!");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayCoinSpawnSound()
    {
        audioSource.PlayOneShot(coinSpawnSound);
    }

    public void PlayCoinCollectSound()
    {
        audioSource.PlayOneShot(coinCollectSound);
    }
}
