using UnityEngine;

public class FridgeInteraction : MonoBehaviour
{   
    [Header("UI")]
    [SerializeField] private GameObject ingredientPanel;

    [Header("Ingredients")]
    [SerializeField] private GameObject meatPrefab;
    [SerializeField] private GameObject vegetablePrefab;
    [SerializeField] private GameObject cheesePrefab;
    private ChefInventory chefInventory;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ingredientPanel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {   
        ChefInventory inventory = other.GetComponent<ChefInventory>();
        
        if(inventory == null){
            return;
        }
        chefInventory = inventory;

        if(!chefInventory.HasItem())
        {
            ingredientPanel.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        ChefInventory inventory = other.GetComponent<ChefInventory>();
        if(inventory== null){
            return;
        }
        ingredientPanel.SetActive(false);
        chefInventory=null;

    }
    public void PickMeat(){
        PickIngredient(meatPrefab);
    }
    public void PickVegetable(){
        PickIngredient(vegetablePrefab);
    }
    public void PickCheese(){
        PickIngredient(cheesePrefab);
    }
    private void PickIngredient(GameObject ingredientPrefab)
    {
        if (chefInventory==null){
            return;
        }
        if (chefInventory.HasItem()){
            return;
        }
        chefInventory.PickUpItem(ingredientPrefab);
        ingredientPanel.SetActive(false);
    }
}
