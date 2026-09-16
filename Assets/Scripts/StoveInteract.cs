using UnityEngine;
using System.Collections; 
using TMPro;

public class StoveInteract : MonoBehaviour
{   [Header("UI")]
    [SerializeField] private TMP_Text Stove1CounterText;
    [SerializeField] private TMP_Text Stove2CounterText;


    [SerializeField] private  Transform StovePoint1;
    [SerializeField] private Transform StovePoint2;
    [SerializeField] private Transform Parent;

    [SerializeField] private GameObject CookedMeatPrefab;
    [SerializeField] private float CookTime = 6f;
 
    private int activeCookingCount=0;
    private bool Stove1Occupied = false;
    private bool Stove2Occupied= false;

    private Vector3 StoveOnePos;
    private Quaternion StoveOneRotation;
    private Vector3 StoveTwoPos;
    private Quaternion StoveTwoRotation;

    private GameObject meatOnStove1;
    private GameObject meatOnStove2;

    private bool Stove1Cooked = false;
    private bool Stove2Cooked = false;
    private ChefInventory playerInsideTrigger;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   if(Stove1CounterText!=null){
            Stove1CounterText.gameObject.SetActive(false);
        }
        if(Stove2CounterText!=null){
            Stove2CounterText.gameObject.SetActive(false);
        }
        StoveOnePos=StovePoint1.transform.localPosition;
        StoveOneRotation = StovePoint1.transform.localRotation;
        StoveTwoPos=StovePoint2.transform.localPosition;
        StoveTwoRotation = StovePoint2.transform.localRotation;
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other){
        if(!other.CompareTag("Player")){
            return;
        }

        ChefInventory inventory = other.GetComponent<ChefInventory>();
        if(inventory==null){
            return;
        }
        playerInsideTrigger = inventory;
        if(!inventory.HasItem()){
            TryPickUpFromStove(inventory);
            return;
        }
        Ingredient ingredient = inventory.GetHeldIngredient();
        if(ingredient==null || ingredient.ingredientType != IngredientType.Meat){
            return;
        }
        if(activeCookingCount>=2)return;
        StartCoroutine(CookMeat(inventory));
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerInsideTrigger = null;
    }

    private IEnumerator CookMeat(ChefInventory chefInventory){
        activeCookingCount++;
        int assignedSlot=0;
        GameObject rawMeatInstance=chefInventory.GetHeldItem();
        
        if (!Stove1Occupied)
        {
            assignedSlot = 1;
            Stove1Occupied = true;
            meatOnStove1 = rawMeatInstance;
            chefInventory.PlaceItemAtPos(StoveOnePos, StoveOneRotation, Parent);
        }
        else if (!Stove2Occupied)
        {
            assignedSlot =2;
            Stove2Occupied=true;
            meatOnStove2 = rawMeatInstance;
            chefInventory.PlaceItemAtPos(StoveTwoPos,StoveTwoRotation,Parent);
        }

        float timeRemaining= CookTime;
        TMP_Text currentText = (assignedSlot==1)?Stove1CounterText:Stove2CounterText;
        currentText.gameObject.SetActive(true);
        while(timeRemaining>0){
            if(currentText!=null){
                currentText.text = timeRemaining.ToString("F1")+"s";
            }
            timeRemaining -=Time.deltaTime;
            yield return null;
        }
        if(currentText!=null)currentText.gameObject.SetActive(false);
        Transform activeSlotTransform = (assignedSlot==1)? StovePoint1:StovePoint2;
        if(assignedSlot==1){
            Destroy(meatOnStove1);
        }
        else{
            Destroy(meatOnStove2);
        }
        GameObject CookedMeatInstance = Instantiate(
            CookedMeatPrefab,
            activeSlotTransform.position,
            activeSlotTransform.rotation,
            Parent
        );
        if(assignedSlot==1){
            meatOnStove1  = CookedMeatInstance;
            Stove1Cooked=true;
        }
        else{
            meatOnStove2 = CookedMeatInstance;
            Stove2Cooked=true;
        }
        if(playerInsideTrigger!=null && !playerInsideTrigger.HasItem()){
            TryPickUpFromStove(playerInsideTrigger);
        }
    }
    private void TryPickUpFromStove(ChefInventory inventory){
        if(Stove1Occupied && meatOnStove1 !=null && Stove1Cooked){
            inventory.PickUpExistingItem(meatOnStove1);
            meatOnStove1=null;
            Stove1Occupied = false;
            activeCookingCount--;
            

        }
         else if (Stove2Occupied && meatOnStove2 != null && Stove2Cooked)
        {
            inventory.PickUpExistingItem(meatOnStove2);
            meatOnStove2 = null;
            Stove2Occupied = false;
            activeCookingCount--;
            
        }
    }
}
