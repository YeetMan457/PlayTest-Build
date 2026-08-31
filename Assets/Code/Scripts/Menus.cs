using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class Menus : MonoBehaviour
{
    public static Menus instance;

    [Header("MAIN MENUS")]
    public GameObject mainMenu;
    public GameObject helpMenu;
    public GameObject optionsMenu;

    [Header("SCENES")]
    public string gameScene = "Game";
    public string mainMenuScene = "MainMenu";
    public string tutorialScene = "Tutorial";

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
        if (optionsMenu.activeSelf)
        {
            CloseOptionsMenu();
            return;
        }
    }

    // =========================================================
    // MAIN MENU
    // =========================================================

    public void StartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(gameScene);
    }

    public void StartTutorial()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(tutorialScene);
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // =========================================================
    // HELP MENU
    // =========================================================

    public void OpenHelpMenu()
    {
        helpMenu.SetActive(true);
    }

    public void CloseHelpMenu()
    {
        helpMenu.SetActive(false);
    }

    // =========================================================
    // OPTIONS
    // =========================================================

    public void OpenOptionsMenu()
    {
        optionsMenu.SetActive(true);
    }

    public void CloseOptionsMenu()
    {
        optionsMenu.SetActive(false);
    }

    // =========================================================
    // UTIL
    // =========================================================

    public void RestartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(mainMenuScene);
    }

    void HideAllMenus()
    {
        if (mainMenu != null) mainMenu.SetActive(false);
        if (helpMenu != null) helpMenu.SetActive(false);
        if (optionsMenu != null) optionsMenu.SetActive(false);
    }

    void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}