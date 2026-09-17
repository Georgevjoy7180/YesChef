using UnityEngine;
using System.Collections; 
using TMPro;
using System.Linq;

public class StoveSlot
{
    public Transform Point;
    public TMP_Text CounterText;
    public Vector3 LocalPos;
    public Quaternion LocalRot;
    public bool Occupied;
    public bool Cooked;
    public GameObject MeatObject;
}
public class StoveInteract : MonoBehaviour
{   
    [SerializeField] private Transform Parent;


    [SerializeField] private Transform[] stovePoints;      
    [SerializeField] private TMP_Text[] counterTexts;       

    private StoveSlot[] slots;
    [SerializeField] private GameObject CookedMeatPrefab;
    [SerializeField] private float CookTime = 6f;
 
    private int activeCookingCount=0;

    private ChefInventory playerInsideTrigger;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   slots = new StoveSlot[stovePoints.Length];
        for(int i=0;i<stovePoints.Length;i++){
            slots[i]=new StoveSlot{
                Point=stovePoints[i],
                CounterText = counterTexts[i],
                LocalPos = stovePoints[i].localPosition,
                LocalRot=  stovePoints[i].localRotation
            };
            slots[i].CounterText?.gameObject.SetActive(false);
        }
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
        playerInsideTrigger = inventory; //to remember player inside the stove trigger
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

        playerInsideTrigger = null;//player exited the trigger
    }

    private IEnumerator CookMeat(ChefInventory chefInventory){
        StoveSlot slot = slots.FirstOrDefault(s => !s.Occupied);
        if(slot==null)yield break;
        activeCookingCount++;
        slot.Occupied=true;
        slot.MeatObject = chefInventory.GetHeldItem();
        chefInventory.PlaceItemAtPos(slot.LocalPos,slot.LocalRot,Parent);
        

        float timeRemaining= CookTime;
        slot.CounterText?.gameObject.SetActive(true);
        while(timeRemaining>0){
            if(slot.CounterText!=null){
                slot.CounterText.text = timeRemaining.ToString("F1")+"s";
            }
            timeRemaining -=Time.deltaTime;
            yield return null;
        }
        slot.CounterText?.gameObject.SetActive(false);
        Destroy(slot.MeatObject);

        slot.MeatObject = Instantiate(CookedMeatPrefab, slot.Point.position, slot.Point.rotation, Parent);
        slot.Cooked = true;
        if(playerInsideTrigger!=null && !playerInsideTrigger.HasItem()){
            TryPickUpFromStove(playerInsideTrigger);
        }
    }
    private void TryPickUpFromStove(ChefInventory inventory){
        StoveSlot slot = slots.FirstOrDefault(s => s.Occupied && s.MeatObject != null && s.Cooked);
        if (slot == null) return;

        inventory.PickUpExistingItem(slot.MeatObject);
        slot.MeatObject = null;
        slot.Occupied = false;
        slot.Cooked = false;
        activeCookingCount--;
    }
}
