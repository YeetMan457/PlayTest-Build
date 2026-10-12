using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class DiscordWebhooks : MonoBehaviour
{
    public static string WEBHOOK_URL = "https://discord.com/api/webhooks/1557398619303317575/nAfVAveqKu1fvhzXnxNpwsFw4L2n4_E7xA3s8LMQBaHDyaLHIdCNZH4mzbfCWJ7EneDi";

    public static async Task SendMessage(string message, string username = "", string avatar_url = "")
    {
        await SendMessageFromWebhook(WEBHOOK_URL, message, username, avatar_url);
    }

    public static async Task SendScreenshot(string optionalMessage = "", string username = "", string avatar_url = "")
    {
        await SendScreenshotFromWebhook(WEBHOOK_URL, optionalMessage, username, avatar_url);
    }

    public static async Task SendImage(Texture2D texture, string optionalMessage = "", string username = "", string avatar_url = "")
    {
        await SendImageFromWebhook(WEBHOOK_URL, texture, optionalMessage, username, avatar_url);
    }

    public static async Task SendImageFromURL(string imageURL, string optionalMessage = "", string username = "", string avatar_url = "")
    {
        await SendImageURLFromWebhook(WEBHOOK_URL, imageURL, optionalMessage, username, avatar_url);
    }

    public static async Task SendData(string filename, byte[] data, string optionalMessage = "", string username = "", string avatar_url = "")
    {
        await SendDataFromWebhook(WEBHOOK_URL, filename, data, optionalMessage, username, avatar_url);
    }

    public static async Task SendFile(string path, string optionalMessage = "", string username = "", string avatar_url = "")
    {
        await SendFileFromWebhook(WEBHOOK_URL, path, optionalMessage, username, avatar_url);
    }

    public static async Task SendMessageFromWebhook(string webhookURL, string message, string username = "", string avatar_url = "")
    {
        WWWForm form = new WWWForm();
        form.AddField("content", message);
        if (username.Length > 0) form.AddField("username", username);
        if (avatar_url.Length > 0) form.AddField("avatar_url", avatar_url);
        await UnityEngine.Networking.UnityWebRequest.Post(webhookURL, form).SendWebRequest();
    }

    public static async Task SendScreenshotFromWebhook(string webhookURL, string optionalMessage = "", string username = "", string avatar_url = "")
    {
        const string fileName = "Screenshot.png";
        await ScreenshotTools.TakeScreenshotAsync(fileName);
        await SendImageFromWebhook(webhookURL, fileName, optionalMessage, username, avatar_url);
    }

    public static async Task SendImageFromWebhook(string webhookURL, Texture2D texture, string optionalMessage = "", string username = "", string avatar_url = "")
    {
        byte[] bytes = texture.EncodeToPNG();
        WWWForm form = new WWWForm();
        form.headers["Content-Type"] = "multipart/form-data";
        form.AddBinaryData("file1", bytes, "Image.png");
        if (username.Length > 0) form.AddField("username", username);
        if (avatar_url.Length > 0) form.AddField("avatar_url", avatar_url);
        if (optionalMessage.Length > 0) form.AddField("content", optionalMessage);
        await UnityEngine.Networking.UnityWebRequest.Post(webhookURL, form).SendWebRequest();
    }

    public static async Task SendImageFromWebhook(string webhookURL, string imagePath, string optionalMessage = "", string username = "", string avatar_url = "")
    {
        byte[] bytes = File.ReadAllBytes(imagePath);
        WWWForm form = new WWWForm();
        form.headers["Content-Type"] = "multipart/form-data";
        form.AddBinaryData("file1", bytes, "Image.png");
        if (username.Length > 0) form.AddField("username", username);
        if (avatar_url.Length > 0) form.AddField("avatar_url", avatar_url);
        if (optionalMessage.Length > 0) form.AddField("content", optionalMessage);
        await UnityEngine.Networking.UnityWebRequest.Post(webhookURL, form).SendWebRequest();
    }

    public static async Task SendImageURLFromWebhook(string webhookURL, string imageURL, string optionalMessage = "", string username = "", string avatar_url = "")
    {
        WWWForm form = new WWWForm();
        string start = optionalMessage.Length > 0 ? optionalMessage + "\n" : "";
        if (username.Length > 0) form.AddField("username", username);
        if (avatar_url.Length > 0) form.AddField("avatar_url", avatar_url);
        form.AddField("content", optionalMessage + " " + imageURL);
        await UnityEngine.Networking.UnityWebRequest.Post(webhookURL, form).SendWebRequest();
    }

    public static async Task SendDataFromWebhook(string webhookURL, string filename, byte[] data, string optionalMessage = "", string username = "", string avatar_url = "")
    {
        WWWForm form = new WWWForm();
        form.headers["Content-Type"] = "multipart/form-data";
        form.AddBinaryData("file1", data, filename);
        if (username.Length > 0) form.AddField("username", username);
        if (avatar_url.Length > 0) form.AddField("avatar_url", avatar_url);
        if (optionalMessage.Length > 0) form.AddField("content", optionalMessage);
        await UnityEngine.Networking.UnityWebRequest.Post(webhookURL, form).SendWebRequest();
    }

    public static async Task SendFileFromWebhook(string webhookURL, string path, string optionalMessage = "", string username = "", string avatar_url = "")
    {
        await SendDataFromWebhook(webhookURL, Path.GetFileName(path), File.ReadAllBytes(path), optionalMessage, username, avatar_url);
    }
}
