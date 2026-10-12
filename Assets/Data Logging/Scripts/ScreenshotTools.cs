using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class ScreenshotTools
{
    private static Texture2D screen;

    public static async Task TakeScreenshotAsync(string fileName, int superSize = 1)
    {
        ScreenCapture.CaptureScreenshot(fileName, superSize);
        while (!File.Exists(fileName))
        {
            await Task.Yield();
        }
        await Task.Delay(200);
    }

    private static async Task<Texture2D> CaptureScreenshot()
    {
        screen = null;
        Camera.onPostRender += CaptureFrame;
        while (screen == null) await Task.Yield();
        return screen;
    }

    private static void CaptureFrame(Camera camera)
    {
        screen = new Texture2D(Screen.width, Screen.height);
        screen.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screen.Apply();
        Camera.onPostRender -= CaptureFrame;
    }
}
