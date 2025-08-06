using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RecipeUIManager : MonoBehaviour
{
    public TextMeshProUGUI recipeText;

    public void DisplayRecipe(List<RecipeManager.RecipeStep> recipe)
    {
        if (recipeText == null)
        {
            Debug.LogWarning("recipeText is not assigned!");
            return;
        }

        recipeText.text = "레시피\n";

        foreach (var step in recipe)
        {
            string foodText = step.foodType.ToString();
            string cookText = step.cookType.HasValue ? step.cookType.Value.ToString() : null;

            // ✅ 음식 제출 여부에 따라 취소선
            if (step.foodDelivered)
                foodText = $"<s>{foodText}</s>";

            // ✅ 도구 제출 여부에 따라 취소선
            if (step.cookType.HasValue)
            {
                if (step.cookDelivered)
                    cookText = $"<s>{cookText}</s>";

                recipeText.text += $"- {foodText} + {cookText}\n";
            }
            else
            {
                recipeText.text += $"- {foodText}\n";
            }
        }
    }


}
