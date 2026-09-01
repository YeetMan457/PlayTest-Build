using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Unity.Services.Core;
using Unity.Services.Analytics;
using UnityEngine.UnityConsent;

public class PlaytestAnalytics : MonoBehaviour
{
    public static PlaytestAnalytics Instance { get; private set; }

    [Header("Playtest")]
    [SerializeField]
    private string buildVersion = "Play Test 1";

    [Header("Consent")]
    [Tooltip(
        "ONLY enable this if analytics consent has already been obtained " +
        "before the Unity application begins."
    )]
    [SerializeField]
    private bool consentAlreadyObtained = false;

    private string playtestSession;
    private float sessionStartTime;
    private int actionIndex;

    private bool servicesInitialized;
    private bool consentGranted;

    private int lastTrackedSceneBuildIndex = int.MinValue;

    private readonly Queue<PendingAction> pendingActions = new();

    private Coroutine buttonDiscoveryCoroutine;
    private Coroutine flushCoroutine;


    private struct PendingAction
    {
        public int ActionIndex;
        public float ElapsedSeconds;
        public string SceneName;
        public string ActionName;
    }


    // =========================================================
    // INITIALISATION
    // =========================================================

    private async void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        playtestSession = Guid.NewGuid().ToString("N");

        sessionStartTime = Time.realtimeSinceStartup;
        actionIndex = 0;

        SceneManager.sceneLoaded += OnSceneLoaded;

        RecordAction("Session Start");

        try
        {
            await UnityServices.InitializeAsync();

            servicesInitialized = true;

            Debug.Log(
                $"[PLAYTEST ANALYTICS] Unity Services initialized. " +
                $"Session: {playtestSession}"
            );

            if (consentAlreadyObtained)
            {
                GrantAnalyticsConsent();
            }
        }
        catch (Exception exception)
        {
            Debug.LogError(
                "[PLAYTEST ANALYTICS] Unity Services failed to initialize."
            );

            Debug.LogException(exception);
        }
    }


    private IEnumerator Start()
    {
        // Wait one frame so the current scene has finished creating its UI.
        yield return null;

        TrackScene(SceneManager.GetActiveScene());

        ScanForButtons();

        buttonDiscoveryCoroutine =
            StartCoroutine(ButtonDiscoveryLoop());
    }


    private void Update()
    {
        // These are meaningful non-UI controls in the current prototype.

        if (Input.GetMouseButtonDown(1))
        {
            RecordAction("Input: Right Click");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            RecordAction("Input: Escape");
        }
    }


    // =========================================================
    // CONSENT
    // =========================================================

    public void GrantAnalyticsConsent()
    {
        EndUserConsent.SetConsentState(
            new ConsentState
            {
                AnalyticsIntent = ConsentStatus.Granted,
                AdsIntent = ConsentStatus.Denied
            }
        );

        consentGranted = true;

        Debug.Log(
            "[PLAYTEST ANALYTICS] Analytics consent granted."
        );

        TrySendPendingActions();
    }


    // =========================================================
    // PUBLIC TRACKING API
    // =========================================================

    /// <summary>
    /// Simple static entry point for the rest of the game.
    ///
    /// Any new gameplay system only needs:
    ///
    /// PlaytestAnalytics.Track("Whatever happened");
    /// </summary>
    public static void Track(string actionName)
    {
        if (Instance == null)
        {
            Debug.LogWarning(
                $"[PLAYTEST ANALYTICS] Tracker does not exist. " +
                $"Could not record: {actionName}"
            );

            return;
        }

        Instance.RecordAction(actionName);
    }


    public void RecordAction(string actionName)
    {
        if (string.IsNullOrWhiteSpace(actionName))
        {
            return;
        }

        actionIndex++;

        PendingAction action = new PendingAction
        {
            ActionIndex = actionIndex,

            ElapsedSeconds =
                Time.realtimeSinceStartup - sessionStartTime,

            SceneName =
                SceneManager.GetActiveScene().name,

            ActionName =
                actionName
        };

        pendingActions.Enqueue(action);

        TrySendPendingActions();
    }


    // =========================================================
    // SCENE TRACKING
    // =========================================================

    private void OnSceneLoaded(
        Scene scene,
        LoadSceneMode loadSceneMode)
    {
        TrackScene(scene);

        // Some UI is created during Awake/Start,
        // so the repeating scanner will catch anything
        // that does not exist yet.
        ScanForButtons();
    }


    private void TrackScene(Scene scene)
    {
        if (scene.buildIndex == lastTrackedSceneBuildIndex)
        {
            return;
        }

        lastTrackedSceneBuildIndex = scene.buildIndex;

        RecordAction(
            $"Scene Enter: {scene.name}"
        );
    }


    // =========================================================
    // AUTOMATIC BUTTON TRACKING
    // =========================================================

    private IEnumerator ButtonDiscoveryLoop()
    {
        WaitForSecondsRealtime wait =
            new WaitForSecondsRealtime(0.25f);

        while (true)
        {
            ScanForButtons();

            yield return wait;
        }
    }


    private void ScanForButtons()
    {
        Button[] buttons =
            FindObjectsByType<Button>(
                FindObjectsInactive.Include
            );

        foreach (Button button in buttons)
        {
            if (button.GetComponent<PlaytestButtonTracker>() == null)
            {
                button.gameObject.AddComponent<PlaytestButtonTracker>();
            }
        }
    }

    public static string GetButtonLabel(Button button)
    {
        // =========================================================
        // SEMANTIC GAME BUTTONS
        // =========================================================

        // Dynamically-generated Material buttons.
        MaterialButton materialButton =
            button.GetComponent<MaterialButton>();

        if (materialButton == null)
        {
            materialButton =
                button.GetComponentInChildren<MaterialButton>(true);
        }

        if (materialButton != null &&
            materialButton.material != null &&
            !string.IsNullOrWhiteSpace(materialButton.material.Name))
        {
            return materialButton.material.Name;
        }


        // Dynamically-generated Action buttons.
        ActionButton actionButton =
            button.GetComponent<ActionButton>();

        if (actionButton == null)
        {
            actionButton =
                button.GetComponentInChildren<ActionButton>(true);
        }

        if (actionButton != null &&
            actionButton.action != null &&
            !string.IsNullOrWhiteSpace(actionButton.action.Name))
        {
            return actionButton.action.Name;
        }


        // Final-form / final-item buttons.
        FinalFormButton finalFormButton =
            button.GetComponent<FinalFormButton>();

        if (finalFormButton == null)
        {
            finalFormButton =
                button.GetComponentInChildren<FinalFormButton>(true);
        }

        if (finalFormButton != null &&
            finalFormButton.mapObject != null &&
            !string.IsNullOrWhiteSpace(finalFormButton.mapObject.Name))
        {
            return finalFormButton.mapObject.Name;
        }


        // Object-choice buttons.
        ObjectSelectItem objectSelectItem =
            button.GetComponent<ObjectSelectItem>();

        if (objectSelectItem == null)
        {
            objectSelectItem =
                button.GetComponentInChildren<ObjectSelectItem>(true);
        }

        if (objectSelectItem != null &&
            !string.IsNullOrWhiteSpace(objectSelectItem.mapObject))
        {
            return objectSelectItem.mapObject;
        }


        // =========================================================
        // NORMAL UI TEXT
        // =========================================================

        TMP_Text[] tmpTexts =
            button.GetComponentsInChildren<TMP_Text>(true);

        foreach (TMP_Text tmpText in tmpTexts)
        {
            if (tmpText == null ||
                string.IsNullOrWhiteSpace(tmpText.text))
            {
                continue;
            }

            string label =
                tmpText.text
                    .Replace("\r", "")
                    .Replace("\n", " ")
                    .Trim();

            // "Icon" is an implementation/UI label in your current prefabs,
            // not a useful playtest action.
            if (string.Equals(
                label,
                "Icon",
                StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            return label;
        }


        // Legacy Unity UI text fallback.
        Text[] legacyTexts =
            button.GetComponentsInChildren<Text>(true);

        foreach (Text legacyText in legacyTexts)
        {
            if (legacyText != null &&
                !string.IsNullOrWhiteSpace(legacyText.text))
            {
                return legacyText.text.Trim();
            }
        }


        // Last resort.
        return button.gameObject.name;
    }


    // =========================================================
    // EVENT SENDING
    // =========================================================

    private void TrySendPendingActions()
    {
        if (!servicesInitialized || !consentGranted)
        {
            return;
        }

        while (pendingActions.Count > 0)
        {
            PendingAction action =
                pendingActions.Dequeue();

            SendAction(action);
        }
    }


    private void SendAction(PendingAction action)
    {
        PlaytestActionEvent analyticsEvent =
            new PlaytestActionEvent
            {
                PlaytestSession = playtestSession,
                ActionIndex = action.ActionIndex,
                ElapsedSeconds = action.ElapsedSeconds,
                SceneName = action.SceneName,
                ActionName = action.ActionName,
                BuildVersion = buildVersion,
                RuntimePlatform =
                    Application.platform.ToString()
            };

        AnalyticsService.Instance.RecordEvent(
            analyticsEvent
        );

        Debug.Log(
            $"[PLAYTEST] " +
            $"{FormatElapsedTime(action.ElapsedSeconds)} | " +
            $"{action.SceneName} -> {action.ActionName} | " +
            $"#{action.ActionIndex} | " +
            $"Session: {playtestSession}"
        );

        RequestFlush();
    }


    // Rather than calling Flush() repeatedly if the player clicks
    // several things quickly, combine nearby events into one upload.
    private void RequestFlush()
    {
        if (flushCoroutine == null)
        {
            flushCoroutine =
                StartCoroutine(FlushSoon());
        }
    }


    private IEnumerator FlushSoon()
    {
        yield return new WaitForSecondsRealtime(1f);

        if (servicesInitialized && consentGranted)
        {
            AnalyticsService.Instance.Flush();
        }

        flushCoroutine = null;
    }


    private string FormatElapsedTime(
        float elapsedSeconds)
    {
        int totalSeconds =
            Mathf.FloorToInt(elapsedSeconds);

        int minutes =
            totalSeconds / 60;

        int seconds =
            totalSeconds % 60;

        return $"{minutes:00}:{seconds:00}";
    }


    private void OnDestroy()
    {
        if (Instance != this)
        {
            return;
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}


// =============================================================
// AUTOMATIC BUTTON HOOK
// =============================================================

public class PlaytestButtonTracker :
    MonoBehaviour,
    IPointerDownHandler
{
    private Button button;


    private void Awake()
    {
        button = GetComponent<Button>();
    }


    public void OnPointerDown(
        PointerEventData eventData)
    {
        if (button == null ||
            !button.interactable)
        {
            return;
        }

        string label =
            PlaytestAnalytics.GetButtonLabel(button);

        PlaytestAnalytics.Track(
            $"Button: {label}"
        );
    }
}


// =============================================================
// UNITY ANALYTICS EVENT
// =============================================================

public class PlaytestActionEvent :
    Unity.Services.Analytics.Event
{
    public PlaytestActionEvent()
        : base("playtestAction")
    {
    }


    public string PlaytestSession
    {
        set => SetParameter(
            "playtestSession",
            value
        );
    }


    public int ActionIndex
    {
        set => SetParameter(
            "actionIndex",
            value
        );
    }


    public float ElapsedSeconds
    {
        set => SetParameter(
            "elapsedSeconds",
            value
        );
    }


    public string SceneName
    {
        set => SetParameter(
            "sceneName",
            value
        );
    }


    public string ActionName
    {
        set => SetParameter(
            "actionName",
            value
        );
    }


    public string BuildVersion
    {
        set => SetParameter(
            "buildVersion",
            value
        );
    }


    public string RuntimePlatform
    {
        set => SetParameter(
            "runtimePlatform",
            value
        );
    }
}