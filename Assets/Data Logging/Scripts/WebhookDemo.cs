using System.IO;
using UnityEngine;

/// <summary>
/// This class shows some example implementations for various Discord Webhook functionality.
/// 
/// For these functions to work, you first need to assign a valid webhook URL in the inspector
/// so that these functions know where to upload the content.
/// </summary>
public class WebhookDemo : MonoBehaviour
{
    /// <summary>
    /// Example 1: Write some text (max 2000 characters).
    /// </summary>
    public void Example1()
    {
        _ = DiscordWebhooks.SendMessage($"Hello! This is a some basic text.");
    }

    /// <summary>
    /// Example 2: Write some text, but put it in a codeblock.
    /// </summary>
    public void Example2()
    {
        _ = DiscordWebhooks.SendMessage("```Hello! This is some text in a code block!```");
    }

    /// <summary>
    /// Example 3: Write some text, but give the bot a custom username.
    /// </summary>
    public void Example3()
    {
        const string username = "Botty McBottson";
        _ = DiscordWebhooks.SendMessage("Hello! This is a some basic text with a custom bot name and avatar.", username);
    }

    /// <summary>
    /// Example 4: Write some text, but give the bot a custom username and avatar URL.
    /// </summary>
    public void Example4()
    {
        const string username = "Botty McBottson";
        const string avatarURL = "https://preview.redd.it/8n6x4gk2pnr71.png?auto=webp&s=c2e0b2084c4046ce9091c585e0f85752f767c2ed";
        _ = DiscordWebhooks.SendMessage("Hello! This is a some basic text with a custom bot name and avatar.", username, avatarURL);
    }

    /// <summary>
    /// Example 5: Write some text with some custom markdown.
    /// </summary>
    public void Example5()
    {
        _ = DiscordWebhooks.SendMessage($"This _is_ **a** ***custom*** __message__ __*containing*__ __**custom**__ __***markdown***__ ~~text~~.");
    }

    /// <summary>
    /// Example 6: Upload a file attachment.
    /// </summary>
    public void Example6()
    {
        _ = DiscordWebhooks.SendFile("Assets/Data Logging/Data/Example.xlsx");
    }

    /// <summary>
    /// Example 7: Write some text and embed an image via URL.
    /// </summary>
    public void Example7()
    {
        _ = DiscordWebhooks.SendImageFromURL("https://preview.redd.it/8n6x4gk2pnr71.png?auto=webp&s=c2e0b2084c4046ce9091c585e0f85752f767c2ed", "This is the Unity logo.");
    }

    /// <summary>
    /// Example 8: Upload an image from file.
    /// </summary>
    public void Example8()
    {
        _ = DiscordWebhooks.SendFile("Assets/Data Logging/Images/Example.jpg");
    }

    /// <summary>
    /// Example 9: Take screenshot of game and upload it.
    /// </summary>
    public void Example9()
    {
        _ = DiscordWebhooks.SendScreenshot();
    }

    /// <summary>
    /// Example 10: Upload a screenshot of the game with included text.
    /// </summary>
    public void Example10()
    {
        _ = DiscordWebhooks.SendScreenshot("This is a screenshot from the game.");
    }
}
