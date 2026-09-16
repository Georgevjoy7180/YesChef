using UnityEngine;
public enum IngredientType{
    Vegetable,
    CutVegetable,
    Cheese,
    Meat,
    CookedMeat
}

public class Ingredient : MonoBehaviour
{   
    [SerializeField] public IngredientType ingredientType;
    public IngredientType Type => ingredientType;
}
