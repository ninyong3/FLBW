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
            string line = "- " + step.foodType.ToString();

            if (step.cookType.HasValue)
            {
                line += " + " + step.cookType.Value.ToString();
            }

            recipeText.text += line + "\n";
        }
    }
}
