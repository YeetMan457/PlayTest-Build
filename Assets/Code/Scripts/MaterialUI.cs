using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaterialUI : MonoBehaviour
{
    public GameObject MaterialUi;
    public GameObject MaterialButtonPrefab;

    void Awake()
    {
        MaterialSO[] materialSO = Resources.LoadAll<MaterialSO>("Scriptable Objects/Materials");

        for (int i = 0; i < materialSO.Length; i++)
        {
            Material material = new Material(materialSO[i]);
            GameObject button = Instantiate(MaterialButtonPrefab, MaterialUi.transform);
            TextMeshProUGUI text = button.GetComponent<TextMeshProUGUI>();
            text.text = material.Name;
            MaterialButton materialButton = button.GetComponentInChildren<MaterialButton>();
            materialButton.material = material;
            Image image = button.GetComponentInChildren<Image>();
            image.sprite = material.image;
            Debug.Log(image);
        }                
    }
}
