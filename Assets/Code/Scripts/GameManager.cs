using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    [HideInInspector]
    public Material CurrentMaterial;
    [HideInInspector]
    public Action CurrentAction;

    public int currentRecycledWaste;
    public StorageUI storageUi;
    public InputTracker inputTracker;
    public CurrentAction currentAction;
    
    public List<AudioClip> gameMusic;
    public bool menuOpen = false;
    public float wasteWeight = 50;

    [SerializeField] private InputAction mouseClick;
    [HideInInspector] public bool IsMoving = false;
    void Awake()
    {

        if (instance == null)

            instance = this;

        else if (instance != this)

            Destroy(gameObject);


    }
    
    void Start()
    {
    }

    void Update()
    {
        if (inputTracker.TimeSinceLastInput > inputTracker.ResetTimer)
        {
            ResetScene();
        }

        if (Input.GetMouseButtonDown(1))
        {
            ResetCurrentAction();
        }
        
    }
    
    public void SetCurrentMaterial(Material material)
    {
        ZoneManager.instance.UnHighlightObject();
        CurrentAction = null;
        CurrentMaterial = material;
        currentAction.image.enabled = true;
        currentAction.image.sprite = CurrentMaterial.image;
        
    }

    public void SetCurrentAction(Action action)
    {
        ZoneManager.instance.UnHighlightObject();
        CurrentMaterial = null;
        CurrentAction = action;
        currentAction.image.enabled = true;
        currentAction.image.sprite = CurrentAction.image;
        

    }

    public void ResetCurrentAction()
    {
        CurrentMaterial = null;
        CurrentAction = null;
        currentAction.image.sprite = null;
        currentAction.image.enabled = false;
        ZoneManager.instance.UnHighlightObject();
    }
    public void ResetScene()
    {
        TutorialPromptManager.Instance.ResetTutorialSession();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public bool HasRequiredMatierals(MapObject mapObject)
    {
        if (currentRecycledWaste >= mapObject.RequiredStoredMaterialAmount)
            return true;
        else
            return false;
    }

    public void ChangeStoredMaterialAmount(int amount)
    {
        currentRecycledWaste += amount;
        storageUi.ChangeStorageAmount();
    }


    public void RecycleItem(GameObject gameObject)
    {
        TextPopup popup = Popup.instance.ShowText(gameObject, $"+1 <sprite name=\"Waste edit\">");
        ChangeStoredMaterialAmount(1);     
    }
}
