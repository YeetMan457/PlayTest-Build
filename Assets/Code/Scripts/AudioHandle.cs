using System.Collections;
using UnityEngine;

public class AudioHandle
{
    private GameObject gameObject;
    private AudioSource audioSource;
    private Coroutine returnCoroutine;

    public bool IsPlaying => audioSource != null && audioSource.isPlaying;

    public AudioHandle(GameObject gameObject, AudioSource audioSource, Coroutine returnCoroutine)
    {
        this.gameObject = gameObject;
        this.audioSource = audioSource;
        this.returnCoroutine = returnCoroutine;
    }

    public void Stop()
    {
        if (audioSource == null)
            return;

        audioSource.Stop();

        if (returnCoroutine != null)
            AudioManager.instance.StopCoroutine(returnCoroutine);

        ObjectPool.instance.ReturnObject(gameObject);

        gameObject = null;
        audioSource = null;
        returnCoroutine = null;
    }
}
