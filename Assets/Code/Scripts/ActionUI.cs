using NUnit.Framework;
using TMPro;
using UnityEngine;

public class ActionUI : MonoBehaviour
{
    public GameObject ActionUi;
    public GameObject ActionButtonPrefab;


    void Awake()
    {
        ActionSO[] actionSO = Resources.LoadAll<ActionSO>("Scriptable Objects/Actions");

        for (int i = 0; i < actionSO.Length; i++)
        {
            Action action = new Action(actionSO[i]);
            GameObject button = Instantiate(ActionButtonPrefab, ActionUi.transform);
            TextMeshProUGUI text = button.GetComponent<TextMeshProUGUI>();
            text.text = action.Name;
            ActionButton actionButton = button.GetComponentInChildren<ActionButton>();
            actionButton.action = action;
        }
    }
}
