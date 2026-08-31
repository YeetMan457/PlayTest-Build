using System;
using TMPro;
using UnityEngine;

public class ObjectPopup : MonoBehaviour
{
    public GameObject history;
    public GameObject recycle;
    public GameObject action;

    private System.Action onHistory;
    private System.Action onAction;
    private System.Action onRecycle;

    public void Initialize(
        string actionText,
        System.Action historyCallback,
        System.Action actionCallback,
        System.Action recycleCallback)
    {
        gameObject.SetActive(true);

        onHistory = historyCallback;
        onAction = actionCallback;
        onRecycle = recycleCallback;

        if (actionText != null)
        {
            action.SetActive(true);
            action.GetComponentInChildren<TextMeshProUGUI>().text = actionText;
        }

        else
        {
            action.SetActive(false);
        }
    }

    public void Disable()
    {
        action.GetComponentInChildren<TextMeshProUGUI>().text = null;
        action.SetActive(false);
        gameObject.SetActive(false);

        onHistory = null;
        onAction = null;
        onRecycle = null;
    }

    public void OnActionClick()
    {
        onAction?.Invoke();
        Disable();
    }

    public void OnHistoryClick()
    {
        onHistory?.Invoke();
        Disable();
    }

    public void OnRecycleClick()
    {
        onRecycle?.Invoke();
        Disable();
    }
}
