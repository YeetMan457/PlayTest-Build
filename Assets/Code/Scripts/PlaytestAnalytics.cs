using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Core;
using Unity.Services.Analytics;
using UnityEngine.UnityConsent;

public class PlaytestAnalytics : MonoBehaviour
{
    public static PlaytestAnalytics Instance { get; private set; }

    [Header("Playtest")]
    [SerializeField]
    private string buildVersion = "PlayTest-1";

    [Header("Consent")]
    [Tooltip(
        "ONLY enable this if analytics consent has already been obtained " +
        "before the Unity application begins. For your own development test, " +
        "you can temporarily enable it."
    )]
    [SerializeField]
    private bool consentAlreadyObtained = false;

    private string playtestSession;
    private float sessionStartTime;
    private int actionIndex;

    private bool servicesInitialized;
    private bool consentGranted;

    private readonly Queue<PendingAction> pendingActions = new();

    private struct PendingAction
    {
        public int ActionIndex;
        public float ElapsedSeconds;
        public string SceneName;
        public string ActionName;
    }

    private async void Awake()
    {
        // Prevent duplicates when returning to MainMenu.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // A fresh ID every time the application is opened.
        playtestSession = Guid.NewGuid().ToString("N");

        // Our zero point for the playtest timer.
        sessionStartTime = Time.realtimeSinceStartup;
        actionIndex = 0;

        // Queue immediately so the timer starts before UGS finishes connecting.
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

    /// <summary>
    /// Call this after the participant has given the required
    /// consent for Analytics data collection.
    /// </summary>
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

        Debug.Log("[PLAYTEST ANALYTICS] Analytics consent granted.");

        TrySendPendingActions();
    }

    /// <summary>
    /// Records ANY meaningful player action or gameplay result.
    ///
    /// Examples:
    /// RecordAction("Wood Right");
    /// RecordAction("Cut Right");
    /// RecordAction("Mature Tree");
    /// RecordAction("Paper");
    /// RecordAction("Reset");
    /// </summary>
    public void RecordAction(string actionName)
    {
        if (string.IsNullOrWhiteSpace(actionName))
        {
            Debug.LogWarning(
                "[PLAYTEST ANALYTICS] Tried to record an empty action name."
            );

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

        // Queue it first.
        //
        // This means an early click isn't lost simply because Unity Services
        // was still initializing when the player performed it.
        pendingActions.Enqueue(action);

        TrySendPendingActions();
    }

    private void TrySendPendingActions()
    {
        if (!servicesInitialized || !consentGranted)
            return;

        while (pendingActions.Count > 0)
        {
            PendingAction action = pendingActions.Dequeue();

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
                RuntimePlatform = Application.platform.ToString()
            };

        AnalyticsService.Instance.RecordEvent(analyticsEvent);

        // Unity normally batches Analytics uploads.
        // For this small playtest, sending promptly is preferable so a
        // browser tab being closed is less likely to lose recent actions.
        AnalyticsService.Instance.Flush();

        Debug.Log(
            $"[PLAYTEST] " +
            $"{FormatElapsedTime(action.ElapsedSeconds)} | " +
            $"{action.SceneName} -> {action.ActionName} | " +
            $"#{action.ActionIndex} | " +
            $"Session: {playtestSession}"
        );
    }

    private string FormatElapsedTime(float elapsedSeconds)
    {
        int totalSeconds = Mathf.FloorToInt(elapsedSeconds);

        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        return $"{minutes:00}:{seconds:00}";
    }
}


// Matches the playtestAction schema that you created
// in Unity Analytics Event Manager.
public class PlaytestActionEvent : Unity.Services.Analytics.Event
{
    public PlaytestActionEvent()
        : base("playtestAction")
    {
    }

    public string PlaytestSession
    {
        set => SetParameter("playtestSession", value);
    }

    public int ActionIndex
    {
        set => SetParameter("actionIndex", value);
    }

    public float ElapsedSeconds
    {
        set => SetParameter("elapsedSeconds", value);
    }

    public string SceneName
    {
        set => SetParameter("sceneName", value);
    }

    public string ActionName
    {
        set => SetParameter("actionName", value);
    }

    public string BuildVersion
    {
        set => SetParameter("buildVersion", value);
    }

    public string RuntimePlatform
    {
        set => SetParameter("runtimePlatform", value);
    }
}