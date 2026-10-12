using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance = null;

    public List<AudioClip> mainMenuMusic;
    public List<AudioClip> gameMusic;
    public List<AudioClip> ambienceList;
    public AudioSource previousAmbience;
    float nextAmbienceTime = 0;
    public float minAmbienceInterval;
    public float maxAmbienceInterval;


    private List<AudioClip> currentMusicList;

    public AudioSource currentMusic;

    [SerializeField]
    private GameObject audioSourcePrefab;

    private int currentTrack = 0;

    // =========================================================
    // VOLUME SETTINGS
    // =========================================================

    private const string MusicVolumeKey = "MusicVolume";
    private const string SfxVolumeKey = "SfxVolume";
    private float baseMusicVolume = 1f;

    [Range(0f, 1f)]
    [SerializeField]
    private float musicVolume = 1f;

    [Range(0f, 1f)]
    [SerializeField]
    private float sfxVolume = 1f;

    public float MusicVolume => musicVolume;
    public float SfxVolume => sfxVolume;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (currentMusic != null)
        {
            baseMusicVolume = currentMusic.volume;
        }

        LoadVolumeSettings();

        nextAmbienceTime = Random.Range(minAmbienceInterval, maxAmbienceInterval);

    }


    private void Start()
    {
        ApplyMusicVolume();

        ChangeMusic(
            SceneManager.GetActiveScene()
        );

        
    }


    private void Update()
    {
        if (!applicationFocused)
            return;

        if (currentMusic == null ||
            currentMusicList == null ||
            currentMusicList.Count == 0)
        {
            return;
        }

        if (!currentMusic.isPlaying)
        {
            currentTrack++;

            if (currentTrack >= currentMusicList.Count)
                currentTrack = 0;

            PlayMusic(currentTrack);
        }

        if (SceneManager.GetActiveScene().name == "Game" &&  Time.time >= nextAmbienceTime)
        {
            PlayAmbience();
        }
    }

    private void PlayAmbience()
    {
        AudioClip ambience = ambienceList[Random.Range(0, ambienceList.Count - 1)];
        if (previousAmbience != null && ambience == previousAmbience)
        {
            PlayAmbience();
            return;
        }
        
        previousAmbience.clip = ambience;
        previousAmbience.Play();
        nextAmbienceTime = Time.time + ambience.length + Random.Range(minAmbienceInterval, maxAmbienceInterval);
        Debug.Log($"playing ambience {ambience.name}, length of {ambience.length}, next ambience playing at {nextAmbienceTime}");
    }


    // =========================================================
    // MUSIC
    // =========================================================

    public void ChangeMusic(Scene scene)
    {
        if (scene.buildIndex == 0)
        {
            currentMusicList = mainMenuMusic;
            PlayMusic(0);
        }
        else if (scene.name == "Game" ||
                 scene.name == "Gallery")
        {
            currentMusicList = gameMusic;
            PlayMusic(0);
        }
    }


    public void PlayMusic(int index)
    {
        if (currentMusic == null ||
            currentMusicList == null ||
            currentMusicList.Count == 0)
        {
            return;
        }

        currentMusic.clip =
            currentMusicList[index];

        currentTrack = index;

        ApplyMusicVolume();

        currentMusic.Play();
    }


    public void SetMusicVolume(float volume)
    {
        musicVolume =
            Mathf.Clamp01(volume);

        ApplyMusicVolume();

        PlayerPrefs.SetFloat(
            MusicVolumeKey,
            musicVolume
        );

        PlayerPrefs.Save();
    }


    private void ApplyMusicVolume()
    {
        if (currentMusic != null)
        {
            currentMusic.volume =
                baseMusicVolume * musicVolume;
        }
    }


    // =========================================================
    // SOUND EFFECTS
    // =========================================================

    public void SetSfxVolume(float volume)
    {
        sfxVolume =
            Mathf.Clamp01(volume);

        PlayerPrefs.SetFloat(
            SfxVolumeKey,
            sfxVolume
        );

        PlayerPrefs.Save();
    }


    public AudioHandle PlaySound(
        AudioClip clip,
        Vector3 position,
        float volume = 1f)
    {
        if (clip == null)
            return null;

        GameObject pooledGO =
            ObjectPool.instance.GetObject(
                audioSourcePrefab
            );

        pooledGO.transform.position =
            position;

        AudioSource aSource =
            pooledGO.GetComponent<AudioSource>();

        aSource.clip =
            clip;

        // The individual sound's intended volume
        // is multiplied by the player's SFX setting.
        aSource.volume =
            volume * sfxVolume;

        aSource.Play();

        Coroutine returnCoroutine =
            StartCoroutine(
                ReturnToPoolAfterPlay(
                    pooledGO,
                    clip.length
                )
            );

        return new AudioHandle(
            pooledGO,
            aSource,
            returnCoroutine
        );
    }


    // =========================================================
    // SAVE / LOAD SETTINGS
    // =========================================================

    private void LoadVolumeSettings()
    {
        musicVolume =
            PlayerPrefs.GetFloat(
                MusicVolumeKey,
                1f
            );

        sfxVolume =
            PlayerPrefs.GetFloat(
                SfxVolumeKey,
                1f
            );
    }


    private IEnumerator ReturnToPoolAfterPlay(
        GameObject go,
        float delay)
    {
        yield return new WaitForSeconds(delay);

        ObjectPool.instance.ReturnObject(go);
    }


    // =========================================================
    // APPLICATION FOCUS
    // =========================================================

    private bool applicationFocused = true;


    private void OnApplicationFocus(
        bool hasFocus)
    {
        applicationFocused =
            hasFocus;

        if (currentMusic == null)
            return;

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