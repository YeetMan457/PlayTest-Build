using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class ApplicationSession
{
    public static event Func<Task> SessionEnd;

    private static bool quitting;
    private static bool allowQuit;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialise()
    {
        Application.wantsToQuit += WantsToQuit;

        var go = new GameObject("[ApplicationShutdown]");
        UnityEngine.Object.DontDestroyOnLoad(go);
        go.hideFlags = HideFlags.HideInHierarchy;

        go.AddComponent<ShutdownRunner>();
    }

    private static bool WantsToQuit()
    {
        Debug.Log($"WantsToQuit: quitting={quitting}, allowQuit={allowQuit}");

        if (allowQuit)
            return true;

        if (!quitting)
        {
            quitting = true;
            ShutdownRunner.Instance.BeginShutdown();
        }

        return false;
    }

    internal static async Task RunShutdown()
    {
        if (SessionEnd != null)
        {
            var handlers = SessionEnd
                .GetInvocationList()
                .Cast<Func<Task>>();

            foreach (var handler in handlers)
            {
                try
                {
                    await handler();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        allowQuit = true;
        await Task.Delay(100);
        Application.Quit();
    }
}
