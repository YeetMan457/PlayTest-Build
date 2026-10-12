using System.Collections;
using TMPro;
using UnityEngine;

public class GallerySaveFeedback : MonoBehaviour
{
    [SerializeField]
    private TMP_Text feedbackText;

    [SerializeField]
    private float displayDuration = 2f;

    private Coroutine hideCoroutine;


    private void Awake()
    {
        if (feedbackText != null)
        {
            feedbackText.gameObject.SetActive(false);
        }
    }


    public void ShowSavedMessage()
    {
        if (feedbackText == null)
            return;

        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        feedbackText.text = "Saved to Gallery!";
        feedbackText.gameObject.SetActive(true);

        hideCoroutine =
            StartCoroutine(HideAfterDelay());
    }


    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSecondsRealtime(
            displayDuration
        );

        feedbackText.gameObject.SetActive(false);

        hideCoroutine = null;
    }
}