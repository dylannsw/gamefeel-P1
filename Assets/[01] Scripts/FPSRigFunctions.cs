using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSRigFunctions : MonoBehaviour
{
    public float PitchVariation = 0.05f;
    public void PlaySoundWithRandomPitch(AudioClip clip)
    {
        GetComponent<AudioSource>().pitch = 1 + Random.Range(-PitchVariation, PitchVariation);
        GetComponent<AudioSource>().PlayOneShot(clip);
    }
}