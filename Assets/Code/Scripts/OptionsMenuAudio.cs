using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuAudio : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField]
    private Slider musicSlider;

    [SerializeField]
    private Slider sfxSlider;

    [Header("Optional Percentage Text")]
    [SerializeField]
    private TMP_Text musicValueText;

    [SerializeField]
    private TMP_Text sfxValueText;


    private void Start()
    {
        RefreshFromAudioManager();
    }


    private void OnEnable()
    {
        if (AudioManager.instance != null)
        {
            RefreshFromAudioManager();
        }
    }


    private void RefreshFromAudioManager()
    {
        if (AudioManager.instance == null)
            return;


        if (musicSlider != null)
        {
            musicSlider.SetValueWithoutNotify(
                AudioManager.instance.MusicVolume
            );
        }


        if (sfxSlider != null)
        {
            sfxSlider.SetValueWithoutNotify(
                AudioManager.instance.SfxVolume
            );
        }


        UpdateMusicText(
            AudioManager.instance.MusicVolume
        );

        UpdateSfxText(
            AudioManager.instance.SfxVolume
        );
    }


    public void OnMusicVolumeChanged(float value)
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.SetMusicVolume(value);
        }

        UpdateMusicText(value);
    }


    public void OnSfxVolumeChanged(float value)
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.SetSfxVolume(value);
        }

        UpdateSfxText(value);
    }


    private void UpdateMusicText(float value)
    {
        if (musicValueText != null)
        {
            musicValueText.text =
                $"{Mathf.RoundToInt(value * 100f)}%";
        }
    }


    private void UpdateSfxText(float value)
    {
        if (sfxValueText != null)
        {
            sfxValueText.text =
                $"{Mathf.RoundToInt(value * 100f)}%";
        }
    }
}