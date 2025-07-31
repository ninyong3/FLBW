using UnityEngine;
using System.Collections.Generic;

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
    public List<List<RecipeStep>> allRecipes = new();

    void Start()
    {
        InitializeRecipes();
        PickNewRecipe();
    }

    void InitializeRecipes()
    {
        allRecipes.Clear();

        allRecipes.Add(new List<RecipeStep> {
            new RecipeStep { foodType = FoodItem.FoodType.Bread },
            new RecipeStep { foodType = FoodItem.FoodType.Meat, cookType = CookerItem.CookType.Grill },
            new RecipeStep { foodType = FoodItem.FoodType.Cabbage, cookType = CookerItem.CookType.Boil },
            new RecipeStep { foodType = FoodItem.FoodType.Bread }
        });

        allRecipes.Add(new List<RecipeStep> {
            new RecipeStep { foodType = FoodItem.FoodType.Bread },
            new RecipeStep { foodType = FoodItem.FoodType.Egg, cookType = CookerItem.CookType.Grill },
            new RecipeStep { foodType = FoodItem.FoodType.Egg, cookType = CookerItem.CookType.Grill },
            new RecipeStep { foodType = FoodItem.FoodType.Bread }
        });

        allRecipes.Add(new List<RecipeStep> {
            new RecipeStep { foodType = FoodItem.FoodType.Bread, cookType = CookerItem.CookType.Grill },
            new RecipeStep { foodType = FoodItem.FoodType.Meat, cookType = CookerItem.CookType.Boil },
            new RecipeStep { foodType = FoodItem.FoodType.Cabbage, cookType = CookerItem.CookType.Cut },
            new RecipeStep { foodType = FoodItem.FoodType.Bread, cookType = CookerItem.CookType.Grill }
        });

        allRecipes.Add(new List<RecipeStep> {
            new RecipeStep { foodType = FoodItem.FoodType.Bread },
            new RecipeStep { foodType = FoodItem.FoodType.Fish, cookType = CookerItem.CookType.Cut },
            new RecipeStep { foodType = FoodItem.FoodType.Fish, cookType = CookerItem.CookType.Cut },
            new RecipeStep { foodType = FoodItem.FoodType.Bread }
        });

        allRecipes.Add(new List<RecipeStep> {
            new RecipeStep { foodType = FoodItem.FoodType.Bread },
            new RecipeStep { foodType = FoodItem.FoodType.Egg, cookType = CookerItem.CookType.Grill },
            new RecipeStep { foodType = FoodItem.FoodType.Cabbage, cookType = CookerItem.CookType.Boil },
            new RecipeStep { foodType = FoodItem.FoodType.Bread }
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
            new RecipeStep { foodType = FoodItem.FoodType.Bread },
            new RecipeStep { foodType = FoodItem.FoodType.Meat, cookType = CookerItem.CookType.Grill },
            new RecipeStep { foodType = FoodItem.FoodType.Egg, cookType = CookerItem.CookType.Grill },
            new RecipeStep { foodType = FoodItem.FoodType.Bread }
        });
    }

    void PickNewRecipe()
    {
        if (allRecipes.Count == 0)
        {
            Debug.Log("모든 레시피 완료!");
            FindObjectOfType<TextDisplay>()?.ShowLog("모든 레시피 완료!");
            return;
        }

        int index = Random.Range(0, allRecipes.Count);
        currentRecipe = allRecipes[index];
        allRecipes.RemoveAt(index); // 중복 방지

        submissionHistory.Clear();

        foreach (var step in currentRecipe)
        {
            step.foodDelivered = false;
            step.cookDelivered = false;
        }

        Debug.Log("새 레시피 시작!");
        FindObjectOfType<TextDisplay>()?.ShowLog("새 레시피 시작!");
        FindObjectOfType<RecipeUIManager>()?.DisplayRecipe(currentRecipe);
    }

    public bool TrySubmit(FoodItem.FoodType? foodType, CookerItem.CookType? cookType)
    {
        submissionHistory.Add((foodType, cookType));

        foreach (var step in currentRecipe)
        {
            if (step.foodDelivered && step.cookDelivered) continue;

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

            break; // 순서 강제
        }

        return false;
    }

    private void CheckRecipeCompletion()
    {
        foreach (var step in currentRecipe)
        {
            if (!step.foodDelivered || (step.cookType != null && !step.cookDelivered))
                return;
        }

        Debug.Log("요리 완성!");
        FindObjectOfType<TextDisplay>()?.ShowLog("요리 완성!");

        completedRecipeCount++;
        if (completedRecipeCount >= 3)
        {
            Debug.Log("게임 종료!");
            FindObjectOfType<TextDisplay>()?.ShowLog("게임 종료!");
            // 게임 종료 처리 로직
        }
        else
        {
            PickNewRecipe();
        }
    }

    public void ResetRecipe()
    {
        PickNewRecipe();
    }
}
