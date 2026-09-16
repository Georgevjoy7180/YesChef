using UnityEngine;
using UnityEngine.UI; 
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class CustomerWindow : MonoBehaviour
{   [Header("UI")]
    [SerializeField] private List<Image> IngredientSlots = new List<Image>();
    [SerializeField] private TMP_Text TimerText;
    [SerializeField] private TMP_Text ScoreText;

    [SerializeField] private Sprite vegetableIconPrefab;
    [SerializeField] private Sprite cheeseIconPrefab;
    [SerializeField] private Sprite meatIconPrefab;

    private  List<OrderIngredient> requiredIngredients = new List<OrderIngredient>();
    private float orderTimer;
    private bool orderActive;
    private int orderBaseScore;
    
    public void StartFirstOrder()
    {
        GenerateOrder();
    }
    // Update is called once per frame
    void Update()
    {   if(!orderActive){
            return;
        }
        orderTimer+=Time.deltaTime;
        TimerText.text = Mathf.FloorToInt(orderTimer) + "s";
        
    }
    private void GenerateOrder(){
        if (!GameManager.Instance.IsGameActive())return;
        requiredIngredients.Clear();
        orderBaseScore=0;
        int ingredientCount = Random.Range(2,4);
        Debug.Log("Ingredient Count+"+ ingredientCount);
        for(int i=0;i<ingredientCount;i++){
            OrderIngredient randomIngredient = (OrderIngredient)Random.Range(0,3);
            requiredIngredients.Add(randomIngredient);
            switch(randomIngredient){
                case OrderIngredient.Vegetable:
                    orderBaseScore+=20;
                    break;
                case OrderIngredient.Meat:
                    orderBaseScore+=30;
                    break;
                case OrderIngredient.Cheese:
                    orderBaseScore+=10;
                    break;
            }
        }
        orderTimer =0f;
        orderActive= true;
        DisplayOrder();
    }
    private void DisplayOrder(){
        foreach(Image slot in IngredientSlots){
            slot.gameObject.SetActive(false);
        }
        for(int i=0;i<requiredIngredients.Count;i++){
            if(i>=requiredIngredients.Count)break;
            
            IngredientSlots[i].gameObject.SetActive(true);
            switch (requiredIngredients[i])
            {   case OrderIngredient.Vegetable:
                    IngredientSlots[i].sprite=vegetableIconPrefab;
                    break;
                case OrderIngredient.Meat:
                    IngredientSlots[i].sprite = meatIconPrefab;
                    break;
                case OrderIngredient.Cheese:
                    IngredientSlots[i].sprite = cheeseIconPrefab;
                    break;
                
                
            }

        }
        
    }
    private void OnTriggerEnter(Collider other){
        if (!GameManager.Instance.IsGameActive())return;
        if(!orderActive){
            return;
        }
        ChefInventory inventory = other.GetComponent<ChefInventory>();
        if(inventory==null){
            return;
        }
        TryDeliverIngredient(inventory);
    }
    private void TryDeliverIngredient(ChefInventory inventory){
        Ingredient heldIngredient=inventory.GetHeldIngredient();
        Debug.Log(heldIngredient==null);
        if(heldIngredient==null){
            return;
        }
        OrderIngredient requiredType;
        Debug.Log(heldIngredient.ingredientType);
        switch(heldIngredient.ingredientType){
            case IngredientType.CutVegetable:
                requiredType = OrderIngredient.Vegetable;
                break;
            case IngredientType.CookedMeat:
                requiredType = OrderIngredient.Meat;
                break;
            case IngredientType.Cheese:
                requiredType = OrderIngredient.Cheese;
                break;
            default:
                return;
        }
        int ingredientIndex = requiredIngredients.IndexOf(requiredType);
        if(ingredientIndex==-1){
            return;
        }
        requiredIngredients.RemoveAt(ingredientIndex);
        inventory.RemoveHeldItem();
        DisplayOrder();
        if(requiredIngredients.Count == 0){
            CompleteOrder();
        }
    }
    private void CompleteOrder(){
        orderActive=false;
        int timePenalty = Mathf.FloorToInt(orderTimer);
        int finalScore = orderBaseScore -  timePenalty;
        GameManager.Instance.AddScore(finalScore);
        ScoreText.text = (finalScore>0?"+":"")+finalScore;
        ScoreText.gameObject.SetActive(true);
        TimerText.gameObject.SetActive(false);
        StartCoroutine(RespawnOrder());
    }
    private IEnumerator RespawnOrder()
    {
        yield return new WaitForSeconds(5f);
        if (!GameManager.Instance.IsGameActive())yield break;

        ScoreText.gameObject.SetActive(false);

        TimerText.gameObject.SetActive(true);

        GenerateOrder();
    }
}
