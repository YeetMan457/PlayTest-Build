using UnityEngine;

internal class ShutdownRunner : MonoBehaviour
{
    public static ShutdownRunner Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public async void BeginShutdown()
    {
        await ApplicationSession.RunShutdown();
    }
}