using UnityEngine;

public class ActionButton : MonoBehaviour
{
    public Action action;

    public void SetActiveAction()
    {
        GameManager.instance.SetCurrentAction(action);
        ZoneManager.instance.UnHighlightObject();
        ZoneManager.instance.HighlightObject(action: action);
    }
}
