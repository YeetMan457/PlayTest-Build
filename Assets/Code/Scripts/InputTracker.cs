using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class InputTracker : MonoBehaviour
{
    public float LastInputTime { get; private set; }
    public float ResetTimer;

    void OnEnable()
    {
        InputSystem.onEvent += OnInputEvent;
    }

    void OnDisable()
    {
        InputSystem.onEvent -= OnInputEvent;
    }

    void OnInputEvent(InputEventPtr eventPtr, InputDevice device)
    {
        if (eventPtr.IsA<StateEvent>() || eventPtr.IsA<DeltaStateEvent>())
        {
            LastInputTime = Time.unscaledTime;
        }
    }



    public float TimeSinceLastInput =>
        Time.unscaledTime - LastInputTime;
}