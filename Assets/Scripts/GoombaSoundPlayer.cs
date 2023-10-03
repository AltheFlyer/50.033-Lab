using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoombaSoundPlayer : MonoBehaviour
{

    public AudioSource deathAudio;

    void PlayDeathSound()
    {
        deathAudio.PlayOneShot(deathAudio.clip);
    }
}
