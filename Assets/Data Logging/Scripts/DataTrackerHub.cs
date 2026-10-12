using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class DataTrackerHub : MonoBehaviour
{
    [Header("Discord Settings")]
    [Tooltip("Paste your Discord Webhook URL here")]
    public string discordWebhookURL = "";

    private void Start()
    {
        // Resets timer and data when the scene loads
        DataLogging.Reset();
    }

    /// <summary>
    /// Call this method when a run ends, player quits, or victory screen opens.
    /// </summary>
    public void SendLogToDiscord()
    {
        StartCoroutine(PostToDiscord(DataLogging.currentLog.BuildLogString()));
    }

    private IEnumerator PostToDiscord(string message)
    {
        if (string.IsNullOrEmpty(discordWebhookURL))
        {
            Debug.LogWarning("Discord Webhook URL is empty!");
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("content", message);

        using (UnityWebRequest www = UnityWebRequest.Post(discordWebhookURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error sending Discord Webhook: " + www.error);
            }
            else
            {
                Debug.Log("Game Log sent to Discord successfully!");
            }
        }
    }

    // Automatically send the log if the player exits or closes the game app
    private void OnApplicationQuit()
    {
        SendLogToDiscord();
    }
}