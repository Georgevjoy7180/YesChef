using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class CustomerWindow : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private List<Image> IngredientSlots = new List<Image>();
    [SerializeField] private TMP_Text TimerText;
    [SerializeField] private TMP_Text ScoreText;

    [Header("Patience Settings")]
    [SerializeField] private float maxOrderTime = 30f;
    [SerializeField] private Color happyColor = new Color(0.18f, 0.8f, 0.44f);      // Green (100% - 60%)
    [SerializeField] private Color impatientColor = new Color(0.95f, 0.77f, 0.06f); // Yellow (60% - 25%)
    [SerializeField] private Color angryColor = new Color(0.91f, 0.3f, 0.24f);      // Red (25% - 0%)

    [Header("Icons")]
    [SerializeField] private Sprite vegetableIconPrefab;
    [SerializeField] private Sprite cheeseIconPrefab;
    [SerializeField] private Sprite meatIconPrefab;

    private List<OrderIngredient> requiredIngredients = new List<OrderIngredient>();
    private float orderTimer;
    private bool orderActive = false;
    private int orderBaseScore;

    public bool IsOrderActive => orderActive;

    private void Start()
    {
        ClearWindow();
    }

    public void ClearWindow()
    {
        orderActive = false;
        requiredIngredients.Clear();

        foreach (Image slot in IngredientSlots)
        {
            if (slot != null) slot.gameObject.SetActive(false);
        }

        if (TimerText != null) TimerText.gameObject.SetActive(false);
        if (ScoreText != null) ScoreText.gameObject.SetActive(false);
    }

    public void StartFirstOrder()
    {
        SpawnOrder();
    }

    public void SpawnOrder()
    {
        if (orderActive) return;
        GenerateOrder();
    }

    void Update()
    {
        if (!orderActive)
        {
            return;
        }

        orderTimer -= Time.deltaTime;

        if (orderTimer <= 0f)
        {
            orderTimer = 0f;
            UpdatePatienceUI();
            HandleWalkout();
            return;
        }

        UpdatePatienceUI();
    }

    private void UpdatePatienceUI()
    {
        float ratio = Mathf.Clamp01(orderTimer / maxOrderTime);

        if (TimerText != null)
        {
            int secondsLeft = Mathf.CeilToInt(orderTimer);
            TimerText.text = secondsLeft + "s";

            if (ratio >= 0.60f)
            {
                TimerText.color = happyColor;
            }
            else if (ratio >= 0.25f)
            {
                TimerText.color = impatientColor;
            }
            else
            {
                TimerText.color = angryColor;
            }
        }
    }

    private void GenerateOrder()
    {
        if (!GameManager.Instance.IsGameActive()) return;

        requiredIngredients.Clear();
        orderBaseScore = 0;

        int ingredientCount = Random.Range(2, 4);
        for (int i = 0; i < ingredientCount; i++)
        {
            OrderIngredient randomIngredient = (OrderIngredient)Random.Range(0, 3);
            requiredIngredients.Add(randomIngredient);
            switch (randomIngredient)
            {
                case OrderIngredient.Vegetable:
                    orderBaseScore += 20;
                    break;
                case OrderIngredient.Meat:
                    orderBaseScore += 30;
                    break;
                case OrderIngredient.Cheese:
                    orderBaseScore += 10;
                    break;
            }
        }

        orderTimer = maxOrderTime;
        orderActive = true;

        if (TimerText != null)
        {
            TimerText.gameObject.SetActive(true);
        }

        if (ScoreText != null)
        {
            ScoreText.gameObject.SetActive(false);
        }

        UpdatePatienceUI();
        DisplayOrder();
    }

    private void DisplayOrder()
    {
        foreach (Image slot in IngredientSlots)
        {
            if (slot != null) slot.gameObject.SetActive(false);
        }

        for (int i = 0; i < requiredIngredients.Count; i++)
        {
            if (i >= IngredientSlots.Count) break;

            if (IngredientSlots[i] != null)
            {
                IngredientSlots[i].gameObject.SetActive(true);
                switch (requiredIngredients[i])
                {
                    case OrderIngredient.Vegetable:
                        IngredientSlots[i].sprite = vegetableIconPrefab;
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!GameManager.Instance.IsGameActive()) return;
        if (!orderActive)
        {
            return;
        }

        ChefInventory inventory = other.GetComponent<ChefInventory>();
        if (inventory == null)
        {
            return;
        }

        TryDeliverIngredient(inventory);
    }

    private void TryDeliverIngredient(ChefInventory inventory)
    {
        Ingredient heldIngredient = inventory.GetHeldIngredient();
        if (heldIngredient == null)
        {
            return;
        }

        OrderIngredient requiredType;
        switch (heldIngredient.ingredientType)
        {
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
        if (ingredientIndex == -1)
        {
            return;
        }

        requiredIngredients.RemoveAt(ingredientIndex);
        inventory.RemoveHeldItem();

        // Supplying an item bumps up remaining time by 5s (capped at max 30s)
        orderTimer = Mathf.Min(orderTimer + 5f, maxOrderTime);
        UpdatePatienceUI();

        DisplayOrder();

        if (requiredIngredients.Count == 0)
        {
            CompleteOrder();
        }
    }

    private void CompleteOrder()
    {
        orderActive = false;

        float ratio = orderTimer / maxOrderTime;
        int finalScore;

        if (ratio >= 0.60f)
        {
            // Happy (100% - 60%): 100% of total order score
            finalScore = Mathf.RoundToInt(orderBaseScore * 1.0f);
        }
        else if (ratio >= 0.25f)
        {
            // Impatient (60% - 25%): 75% of total order score
            finalScore = Mathf.RoundToInt(orderBaseScore * 0.75f);
        }
        else
        {
            // Angry (25% - 0%): 50% of total order score
            finalScore = Mathf.RoundToInt(orderBaseScore * 0.50f);
        }

        GameManager.Instance.AddScore(finalScore);
        GameManager.Instance.IncrementStreak();

        foreach (Image slot in IngredientSlots)
        {
            if (slot != null) slot.gameObject.SetActive(false);
        }

        if (TimerText != null)
        {
            TimerText.gameObject.SetActive(false);
        }

        ScoreText.text = "+" + finalScore;
        ScoreText.gameObject.SetActive(true);

        StartCoroutine(ClearScorePopup());
    }

    private void HandleWalkout()
    {
        orderActive = false;

        // Walkout: 0 score awarded and streak reset
        GameManager.Instance.ResetStreak();

        foreach (Image slot in IngredientSlots)
        {
            if (slot != null) slot.gameObject.SetActive(false);
        }

        if (TimerText != null)
        {
            TimerText.gameObject.SetActive(false);
        }

        ScoreText.text = "+0";
        ScoreText.gameObject.SetActive(true);

        StartCoroutine(ClearScorePopup());
    }

    private IEnumerator ClearScorePopup()
    {
        yield return new WaitForSeconds(3f);
        if (ScoreText != null)
        {
            ScoreText.gameObject.SetActive(false);
        }
    }
}
