using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeManager : MonoBehaviour
{
    [System.Serializable]
    public class RecipeStep
    {
        public FoodItem.FoodType foodType;
        public CookerItem.CookType? cookType;

        [HideInInspector] public bool foodDelivered = false;
        [HideInInspector] public bool cookDelivered = false;
    }

    public List<RecipeStep> currentRecipe;
    public List<(FoodItem.FoodType?, CookerItem.CookType?)> submissionHistory = new();
    private int completedRecipeCount = 0;

    private List<List<RecipeStep>> allRecipes = new();
    private List<List<RecipeStep>> remainingRecipes = new();

    void Start()
    {
        InitializeRecipes();
        ResetRemainingRecipes();
        PickNewRecipe();
    }

    void InitializeRecipes()
    {
        allRecipes.Clear();

        allRecipes.Add(new List<RecipeStep> {
            new RecipeStep { foodType = FoodItem.FoodType.Bread, cookType = CookerItem.CookType.Cut },
            new RecipeStep { foodType = FoodItem.FoodType.Meat, cookType = CookerItem.CookType.Grill },
            new RecipeStep { foodType = FoodItem.FoodType.Meat, cookType = CookerItem.CookType.Boil },
            new RecipeStep { foodType = FoodItem.FoodType.Bread, cookType = CookerItem.CookType.Cut  }
        });

        allRecipes.Add(new List<RecipeStep> {
            new RecipeStep { foodType = FoodItem.FoodType.Bread, cookType = CookerItem.CookType.Cut },
            new RecipeStep { foodType = FoodItem.FoodType.Egg, cookType = CookerItem.CookType.Grill },
            new RecipeStep { foodType = FoodItem.FoodType.Egg, cookType = CookerItem.CookType.Grill },
            new RecipeStep { foodType = FoodItem.FoodType.Bread, cookType = CookerItem.CookType.Cut}
        });

        allRecipes.Add(new List<RecipeStep> {
            new RecipeStep { foodType = FoodItem.FoodType.Bread, cookType = CookerItem.CookType.Grill },
            new RecipeStep { foodType = FoodItem.FoodType.Meat, cookType = CookerItem.CookType.Boil },
            new RecipeStep { foodType = FoodItem.FoodType.Cabbage, cookType = CookerItem.CookType.Cut },
            new RecipeStep { foodType = FoodItem.FoodType.Bread, cookType = CookerItem.CookType.Grill }
        });

        allRecipes.Add(new List<RecipeStep> {
            new RecipeStep { foodType = FoodItem.FoodType.Bread, cookType = CookerItem.CookType.Grill },
            new RecipeStep { foodType = FoodItem.FoodType.Fish, cookType = CookerItem.CookType.Cut },
            new RecipeStep { foodType = FoodItem.FoodType.Fish, cookType = CookerItem.CookType.Cut },
            new RecipeStep { foodType = FoodItem.FoodType.Bread, cookType = CookerItem.CookType.Grill }
        });

        allRecipes.Add(new List<RecipeStep> {
            new RecipeStep { foodType = FoodItem.FoodType.Bread, cookType = CookerItem.CookType.Cut },
            new RecipeStep { foodType = FoodItem.FoodType.Egg, cookType = CookerItem.CookType.Grill },
            new RecipeStep { foodType = FoodItem.FoodType.Cabbage, cookType = CookerItem.CookType.Boil },
            new RecipeStep { foodType = FoodItem.FoodType.Bread, cookType = CookerItem.CookType.Cut}
        });

        allRecipes.Add(new List<RecipeStep> {
            new RecipeStep { foodType = FoodItem.FoodType.Bread, cookType = CookerItem.CookType.Grill },
            new RecipeStep { foodType = FoodItem.FoodType.Meat, cookType = CookerItem.CookType.Grill },
            new RecipeStep { foodType = FoodItem.FoodType.Cabbage, cookType = CookerItem.CookType.Boil },
            new RecipeStep { foodType = FoodItem.FoodType.Bread, cookType = CookerItem.CookType.Grill }
        });

        allRecipes.Add(new List<RecipeStep> {
            new RecipeStep { foodType = FoodItem.FoodType.Bread, cookType = CookerItem.CookType.Grill },
            new RecipeStep { foodType = FoodItem.FoodType.Fish, cookType = CookerItem.CookType.Grill },
            new RecipeStep { foodType = FoodItem.FoodType.Meat, cookType = CookerItem.CookType.Grill },
            new RecipeStep { foodType = FoodItem.FoodType.Bread, cookType = CookerItem.CookType.Grill }
        });

        allRecipes.Add(new List<RecipeStep> {
            new RecipeStep { foodType = FoodItem.FoodType.Bread, cookType = CookerItem.CookType.Cut },
            new RecipeStep { foodType = FoodItem.FoodType.Meat, cookType = CookerItem.CookType.Grill },
            new RecipeStep { foodType = FoodItem.FoodType.Egg, cookType = CookerItem.CookType.Grill },
            new RecipeStep { foodType = FoodItem.FoodType.Bread , cookType = CookerItem.CookType.Cut}
        });
    }

    void ResetRemainingRecipes()
    {
        remainingRecipes = new List<List<RecipeStep>>(allRecipes);
    }

    void PickNewRecipe()
    {
        StartCoroutine(TimeDelay(3.0f));
        ShowRecipeUI();
        if (remainingRecipes.Count == 0)
        {
            ResetRemainingRecipes();
        }

        int index = Random.Range(0, remainingRecipes.Count);
        currentRecipe = remainingRecipes[index];
        remainingRecipes.RemoveAt(index);

        submissionHistory.Clear();

        foreach (var step in currentRecipe)
        {
            step.foodDelivered = false;
            step.cookDelivered = false;
        }

        FindObjectOfType<TextDisplay>()?.ShowPriorityLog("새 레시피 시작!");
        FindObjectOfType<RecipeUIManager>()?.DisplayRecipe(currentRecipe);
    }

    public bool TrySubmit(FoodItem.FoodType? foodType, CookerItem.CookType? cookType)
    {
        submissionHistory.Add((foodType, cookType));

        for (int i = 0; i < currentRecipe.Count; i++)
        {
            var step = currentRecipe[i];

            if (step.foodDelivered && (step.cookType == null || step.cookDelivered))
                continue;

            if (foodType.HasValue && !cookType.HasValue && !step.foodDelivered)
            {
                if (step.foodType == foodType.Value)
                {
                    step.foodDelivered = true;
                    return true;
                }
            }

            if (!foodType.HasValue && cookType.HasValue && step.foodDelivered && !step.cookDelivered)
            {
                if (step.cookType != null && step.cookType == cookType.Value)
                {
                    step.cookDelivered = true;
                    CheckRecipeCompletion();
                    return true;
                }
            }

            break;
        }

        return false;
    }

    void CheckRecipeCompletion()
    {
        foreach (var step in currentRecipe)
        {
            if (!step.foodDelivered || (step.cookType != null && !step.cookDelivered))
                return;
        }
        StartCoroutine(TimeDelay(0.01f));
        Debug.Log("요리 완성!");
        FindObjectOfType<TextDisplay>()?.ShowLog("요리 완성!");
        RemoverecipeUI();

        completedRecipeCount++;
        if (completedRecipeCount >= 3)
        {
            StartCoroutine(TimeDelay(0.1f));
            Debug.Log("게임 종료");
            FindObjectOfType<TextDisplay>()?.ShowLog("게임 종료!");

            RemoverecipeUI();
            StartCoroutine(TimeDelay(10.0f));
            // 게임 종료 처리
            UnityEditor.EditorApplication.isPlaying = false;
            // 씬 이동으로 추후 변경
        }
        else
        {
            PickNewRecipe();
        }

    }
    IEnumerator TimeDelay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
    }
    void RemoverecipeUI()
    {
        RecipeUIManager recipeUI = FindObjectOfType<RecipeUIManager>();
        if (recipeUI != null && recipeUI.recipeText != null)
        {
            recipeUI.recipeText.enabled = false;
        }
    }
    public void ShowRecipeUI()
    {
        RecipeUIManager recipeUI = FindObjectOfType<RecipeUIManager>();
        if (recipeUI != null && recipeUI.recipeText != null)
        {
            recipeUI.recipeText.enabled = true;
        }
    }
    public void ResetRecipe()
    {
        ResetRemainingRecipes();
        PickNewRecipe();
    }
}