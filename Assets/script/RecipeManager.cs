using UnityEngine;
using System.Collections.Generic;

public class RecipeManager : MonoBehaviour
{
    [System.Serializable]
    public class RecipeStep
    {
        public FoodItem.FoodType foodType;
        public CookerItem.CookType? cookType; // 조리방식이 필요한 경우만
    }

    public List<RecipeStep> currentRecipe;
    private int currentStep = 0;

    public bool TrySubmit(FoodItem.FoodType? foodType, CookerItem.CookType? cookType)
    {
        if (currentStep >= currentRecipe.Count) return false;

        var expected = currentRecipe[currentStep];

        if (foodType.HasValue && expected.foodType == foodType.Value)
        {
            if (expected.cookType == null)
            {
                currentStep++;
                CheckRecipeCompletion();
                return true;
            }
        }
        else if (cookType.HasValue && expected.cookType == cookType.Value)
        {
            if (expected.foodType == null)
            {
                currentStep++;
                CheckRecipeCompletion();
                return true;
            }
        }

        // 틀림 → 현재 단계 유지, 아이템만 소멸
        return false;
    }

    private void CheckRecipeCompletion()
    {
        if (currentStep >= currentRecipe.Count)
        {
            Debug.Log("요리 완성!");
            // TODO: 성공 처리 (점수, 다음 레시피 등)
        }
    }

    public void ResetRecipe()
    {
        currentStep = 0;
        // TODO: 새로운 레시피 생성 or 같은 레시피 반복
    }
}
