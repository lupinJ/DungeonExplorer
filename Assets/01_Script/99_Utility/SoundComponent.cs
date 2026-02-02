using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundComponent : MonoBehaviour
{
    AudioSource audioSource;
    
    public void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void Play(AudioClip clip, float volume, bool loop = false)
    {
        audioSource.clip = clip;
        audioSource.loop = loop;  
        audioSource.volume = volume;
        audioSource.Play();
    }

    private void Update()
    {
        if(audioSource.isPlaying == false)
        {
            SoundManager.Instance.Stop(gameObject);
        }
    }
}
