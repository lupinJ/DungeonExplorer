using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SoundComponent : MonoBehaviour, IPoolable
{
    AudioSource audioSource;
    CancellationTokenSource cts;

    public bool IsPlaying => audioSource.isPlaying;
    public string Name => audioSource.clip.name;

    public float Volume
    {
        get {  return audioSource.volume; }
        set {  audioSource.volume = value; }
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnDisable()
    {
        cts?.Cancel();
        cts?.Dispose();
        cts = null;
    }

    public void OnDespawn()
    {
        
    }

    public void OnSpawn()
    {
        cts?.Cancel();
        cts?.Dispose();
        cts = null;
    }

    public void Play(AudioClip clip, float volume, bool loop = false)
    {
        if (audioSource.isPlaying && audioSource.clip == clip)
        {
            return;
        }
        
        audioSource.clip = clip;
        audioSource.loop = loop;  
        audioSource.volume = volume;
        audioSource.Play();

        if (!loop)
        {
            cts?.Cancel();
            cts?.Dispose();
            cts = new CancellationTokenSource();
            WaitStopAsync(cts.Token).Forget();
        }
    }

    public void Stop()
    {
        audioSource.Stop();
    }

    private async UniTaskVoid WaitStopAsync(CancellationToken ct)
    {
        await UniTask.WaitWhile(() => audioSource.isPlaying, PlayerLoopTiming.Update, ct);
        SoundManager.Instance.Stop(gameObject);
    }
  
}
