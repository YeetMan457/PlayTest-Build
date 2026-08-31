using UnityEngine;

public class UndoButton : MonoBehaviour
{
    public void OnClick()
    {

        if (GameManager.instance.objectHistory.TryPeek(out (MapObject, Zone) history))
        {
            history.Item2.Undo(history.Item1);
            GameManager.instance.objectHistory.Pop();
        }
    }
}
