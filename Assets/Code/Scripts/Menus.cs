using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using LitMotion.Animation;

public class Menus : MonoBehaviour
{
    public static Menus instance;

    [Header("MAIN MENUS")]
    public GameObject mainMenu;
    public GameObject helpMenu;
    public GameObject audioMenu;
    public GameObject gameMenu;
    public GameObject restartMenu;
    public GameObject creditsMenu;
    public LitMotionAnimation creditsAnimation;
    public GameStartTransition transitionEffect;

    [Header("SCENES")]
    public string gameScene = "Game";
    public string mainMenuScene = "MainMenu";
    public string galleryScene = "Gallery";

    int currentSceneIndex;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        Time.timeScale = 1f;
    
        HideAllMenus();

        if (mainMenu != null)
            mainMenu.SetActive(true);
    }

    void Update()
    {
        HandleEscapeKey();

    }

    // =========================================================
    // ESCAPE KEY
    // =========================================================

    void HandleEscapeKey()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
            return;

        // Help closes first
        if (helpMenu.activeSelf)
        {
            CloseHelpMenu();
            return;
        }

        // Options closes second
        if (audioMenu.activeSelf)
        {
            CloseAudioMenu();
            return;
        }
    }

    // =========================================================
    // MAIN MENU
    // =========================================================

    public void StartGame()
    {
        if (transitionEffect.transitionStarted)
            return;
        Time.timeScale = 1f;

        StopLitMotionAnimations();
        transitionEffect.Animate();
        
    }

    public void StartGallery()
    {
        if (transitionEffect.transitionStarted)
            return;

        Time.timeScale = 1f;

        StopLitMotionAnimations();

        SceneManager.LoadScene(galleryScene);
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    // =========================================================
    // Game MENU
    // =========================================================

    public void ToggleGameMenu()
    {
        if (gameMenu.activeSelf)
            gameMenu.SetActive(false);
        else
            gameMenu.SetActive(true);
    }

    // =========================================================
    // HELP MENU
    // =========================================================

    public void OpenHelpMenu()
    {
        if (helpMenu == null)
            return;

        if (!helpMenu.activeSelf)
        {
            helpMenu.SetActive(true);

            if (GameManager.instance != null)
            {
                GameManager.instance.menuOpen = true;
            }
        }
    }

    public void CloseHelpMenu()
    {
        if (helpMenu == null)
            return;

        helpMenu.SetActive(false);

        if (GameManager.instance != null)
        {
            GameManager.instance.menuOpen = false;
        }
    }

    // =========================================================
    // RESTART MENU
    // =========================================================

    public void OpenRestartMenu()
    {
        if (!restartMenu.activeSelf)
        {
            restartMenu.SetActive(true);
            GameManager.instance.menuOpen = true;
        }
        
    }

    public void CloseRestartMenu()
    {
        restartMenu.SetActive(false);
        GameManager.instance.menuOpen = false;
    }

    // =========================================================
    // OPTIONS
    // =========================================================

    public void OpenAudioMenu()
    {
        if (!audioMenu.activeSelf)
        {
            audioMenu.SetActive(true);
        }
        
    }

    public void CloseAudioMenu()
    {
        audioMenu.SetActive(false);
    }

    // =========================================================
    // CREDITS MENU
    // =========================================================

    public void OpenCreditsMenu()
    {
        if (transitionEffect.transitionStarted)
            return;

        creditsMenu.SetActive(true);
        creditsAnimation.Restart();
    }

    public void CloseCreditsMenu()
    {
        creditsMenu.SetActive(false);
    }

    // =========================================================
    // UTIL
    // =========================================================

    public void RestartGame()
    {
        Time.timeScale = 1f;

        StopLitMotionAnimations();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;

        StopLitMotionAnimations();

        SceneManager.LoadScene(mainMenuScene);
    }

    void HideAllMenus()
    {
        if (mainMenu != null) mainMenu.SetActive(false);
        if (helpMenu != null) helpMenu.SetActive(false);
        if (audioMenu != null) audioMenu.SetActive(false);
        if (creditsMenu != null) creditsMenu.SetActive(false);
    }

    void OnDestroy()
    {
        Time.timeScale = 1f;
    }

    private void StopLitMotionAnimations()
    {
        LitMotionAnimation[] animations =
            FindObjectsByType<LitMotionAnimation>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (LitMotionAnimation animation in animations)
        {
            if (animation != null && animation.IsPlaying)
            {
                animation.Stop();
            }
        }
    }
}