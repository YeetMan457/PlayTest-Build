using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialPromptManager : MonoBehaviour
{
    public static TutorialPromptManager Instance { get; private set; }

    [System.Serializable]
    public class TutorialPrompt
    {
        public TutorialPromptId id;

        [TextArea(2, 4)]
        public string message;

        [Min(0.1f)]
        public float displayDuration = 5f;
    }

    [Header("Prompt UI")]
    [SerializeField]
    private CanvasGroup promptCanvasGroup;

    [SerializeField]
    private TMP_Text promptText;

    [Header("Animation")]
    [SerializeField]
    [Min(0f)]
    private float fadeDuration = 0.25f;

    [Header("Prompts")]
    [SerializeField]
    private List<TutorialPrompt> prompts = new();

    private readonly HashSet<TutorialPromptId> triggeredPrompts = new();

    private readonly Queue<TutorialPrompt> promptQueue = new();

    private bool isDisplayingPrompt;

    private void Awake()
    {
        // Keep one manager alive for the entire application session.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        // Start hidden.
        if (promptCanvasGroup != null)
        {
            promptCanvasGroup.alpha = 0f;
            promptCanvasGroup.interactable = false;
            promptCanvasGroup.blocksRaycasts = false;
        }

        //Display first tutorial prompt when the game loads
        TutorialPromptManager.ShowOnce(TutorialPromptId.GameLoaded);
    }


    // Attempts to show a tutorial prompt. Each TutorialPromptId can only be triggered once during the current application session.
    public static void ShowOnce(TutorialPromptId id)
    {
        if (Instance == null)
        {
            Debug.LogWarning(
                $"TutorialPromptManager does not exist. " +
                $"Could not show prompt: {id}"
            );

            return;
        }

        Instance.TryQueuePrompt(id);
    }

    private void TryQueuePrompt(TutorialPromptId id)
    {
        // Already triggered during this session.
        if (triggeredPrompts.Contains(id))
        {
            return;
        }

        TutorialPrompt prompt =
            prompts.Find(prompt => prompt.id == id);

        if (prompt == null)
        {
            Debug.LogWarning(
                $"No tutorial prompt has been configured for {id}."
            );

            return;
        }

        // Mark immediately so repeated actions cannot add duplicates while the prompt is still waiting in the queue.
        triggeredPrompts.Add(id);

        promptQueue.Enqueue(prompt);

        if (!isDisplayingPrompt)
        {
            StartCoroutine(DisplayPromptQueue());
        }
    }

    private IEnumerator DisplayPromptQueue()
    {
        isDisplayingPrompt = true;

        while (promptQueue.Count > 0)
        {
            TutorialPrompt prompt = promptQueue.Dequeue();

            promptText.text = prompt.message;

            yield return FadeCanvasGroup(0f, 1f);

            yield return new WaitForSecondsRealtime(
                prompt.displayDuration
            );

            yield return FadeCanvasGroup(1f, 0f);
        }

        isDisplayingPrompt = false;
    }

    private IEnumerator FadeCanvasGroup(
        float startAlpha,
        float endAlpha)
    {
        if (promptCanvasGroup == null)
        {
            yield break;
        }

        if (fadeDuration <= 0f)
        {
            promptCanvasGroup.alpha = endAlpha;
            yield break;
        }

        float elapsed = 0f;

        promptCanvasGroup.alpha = startAlpha;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t =
                Mathf.Clamp01(elapsed / fadeDuration);

            promptCanvasGroup.alpha =
                Mathf.Lerp(startAlpha, endAlpha, t);

            yield return null;
        }

        promptCanvasGroup.alpha = endAlpha;
    }

    // Clears all tutorial progress for the current session.
    [ContextMenu("Reset Tutorial Session")]
    public void ResetTutorialSession()
    {
        triggeredPrompts.Clear();
        promptQueue.Clear();

        StopAllCoroutines();

        isDisplayingPrompt = false;

        if (promptCanvasGroup != null)
        {
            promptCanvasGroup.alpha = 0f;
        }

        Debug.Log("[TUTORIAL] Tutorial session reset.");
    }
}