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
    public Stack<(MapObject, Zone)> objectHistory = new();
    public CurrentAction currentAction;
    
    public List<AudioClip> gameMusic;
    public HashSet<string> goalItems = new();
    public HashSet<string> completedGoalItems = new();
    public bool goalItemsFinished = false;
    public GoalItemUI goalItemUI;

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
        AudioManager.instance.ChangeMusic(SceneManager.GetActiveScene());
    }
    void Update()
    {
        if (inputTracker.TimeSinceLastInput > inputTracker.ResetTimer)
        {
            PlaytestAnalytics.Track(
                "System: Inactivity Reset"
            );

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

    internal void AddCompletedItem(string name)
    {
        if (!completedGoalItems.Contains(name))
        {
            completedGoalItems.Add(name);
            Destroy(goalItemUI.finalFormButtons[name].gameObject);
            if (completedGoalItems.Count == goalItems.Count)
            {
                goalItemsFinished = true;
                goalItemUI.CreateFinalItems();
            }
                
        }
    }

    public void RecycleItem(Material material, GameObject gameObject)
    {
        TextPopup popup = Popup.instance.ShowText(gameObject, $"+1 <sprite name=\"Waste edit\">");
        ChangeStoredMaterialAmount(1);     
    }
}
