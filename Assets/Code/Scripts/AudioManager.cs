using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance = null;
    public List<AudioClip> mainMenuMusic;
    public List<AudioClip> gameMusic;
    List<AudioClip> currentMusicList;
    public AudioSource currentMusic;
    [SerializeField] GameObject audioSourcePrefab;
    int currentTrack = 0;
    void Awake()
    {

        if (instance == null)

            instance = this;

        else if (instance != this)

            Destroy(gameObject);


    }

    private void Update()
    {
        if (!applicationFocused)
            return;

        if (currentMusic == null || currentMusicList == null)
            return;

        if (!currentMusic.isPlaying)
        {
            currentTrack++;

            if (currentTrack >= currentMusicList.Count)
                currentTrack = 0;

            PlayMusic(currentTrack);
        }
    }

    public void ChangeMusic(Scene scene)
    {
        if (scene.buildIndex == 0)
        {
            currentMusicList = mainMenuMusic;
            PlayMusic(0);
        }
        else if (scene.buildIndex == 2)
        {
            currentMusicList = gameMusic;
            PlayMusic(0);
        }
    }

    public void PlayMusic(int index)
    {
        currentMusic.clip = currentMusicList[index];
        currentTrack = index;
        currentMusic.Play();
    }


    public AudioHandle PlaySound(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null)
            return null;

        GameObject pooledGO =
            ObjectPool.instance.GetObject(audioSourcePrefab);

        pooledGO.transform.position = position;

        AudioSource aSource =
            pooledGO.GetComponent<AudioSource>();

        aSource.clip = clip;
        aSource.volume = volume;
        aSource.Play();

        Coroutine returnCoroutine = StartCoroutine(ReturnToPoolAfterPlay(pooledGO, clip.length));

        return new AudioHandle(pooledGO, aSource, returnCoroutine);
    }

    private IEnumerator ReturnToPoolAfterPlay(GameObject go,float delay)
    {
        yield return new WaitForSeconds(delay);

        ObjectPool.instance.ReturnObject(go);
    }
    private bool applicationFocused = true;
    private void OnApplicationFocus(bool hasFocus)
    {
        applicationFocused = hasFocus;
        if (hasFocus)
        {
            currentMusic.UnPause();
        }
        else
        {
            currentMusic.Pause();
        }
    }

}
