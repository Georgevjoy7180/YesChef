using UnityEngine;
using System.Collections; 
using TMPro;

public class TableInteract : MonoBehaviour
{   [Header("UI")]
    [SerializeField] private TMP_Text TableCounter;

    [SerializeField] private GameObject CurrentKnife;
    [SerializeField] private Transform KitchenPoint;
    [SerializeField] private GameObject CutVegetablePrefab;
    [SerializeField] private Transform KnifePoint;
    [SerializeField] private Transform Parent;
    private bool isChopping=false;

    private Vector3 originalLocalPos;
    private Quaternion originalLocalRotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TableCounter.gameObject.SetActive(false);
        if(CurrentKnife!=null){
            originalLocalPos = CurrentKnife.transform.localPosition;
            originalLocalRotation = CurrentKnife.transform.localRotation;
            Debug.Log($"Knife Original Pos: {originalLocalPos} | Rotation: {originalLocalRotation}");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isChopping) return; 
        if(!other.CompareTag("Player"))
        {
            return;
        }
        ChefInventory inventory = other.GetComponent<ChefInventory>();
        
        
        if(inventory==null){
            return;
        }
        Ingredient Ingredient = inventory.GetHeldIngredient();
        Debug.Log("Is Inventory Null? " + (inventory == null));
        Debug.Log("Is Ingredient Null? " + (Ingredient == null));
        if(Ingredient==null){
            return;
        }
        Debug.Log("Type is "+ Ingredient.ingredientType);
        if(Ingredient.ingredientType != IngredientType.Vegetable){
            return;
        }
        StartCoroutine(ChopVegetable(inventory));

    }
    private IEnumerator ChopVegetable(ChefInventory chefInventory){
        isChopping=true;
        chefInventory.RemoveHeldItem();
        GameObject tableVegetable = Instantiate(
            CutVegetablePrefab,
            KitchenPoint.position,
            KitchenPoint.rotation,
            KitchenPoint
        );
        tableVegetable.transform.localPosition =  Vector3.zero;

        chefInventory.PickUpExistingItem(CurrentKnife);
        
        float timeRemaining=2f;
        TableCounter.gameObject.SetActive(true);
        while(timeRemaining>0)
        {
            TableCounter.text = timeRemaining.ToString("F1")+"s";
            timeRemaining -= Time.deltaTime;
            yield return null;
        }

        TableCounter.gameObject.SetActive(false);
        chefInventory.PlaceItemAtPos(originalLocalPos,originalLocalRotation,Parent);

        chefInventory.PickUpExistingItem(tableVegetable);
        isChopping=false;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
