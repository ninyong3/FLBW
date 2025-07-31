using UnityEngine;
using TMPro;
using System.Text;
using System.Collections.Generic;

public class RecipeUIManager : MonoBehaviour
{
    public TMP_Text recipeText;

    public void DisplayRecipe(List<RecipeManager.RecipeStep> recipeSteps)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("<size=120%><b>레시피</b></size>\n");

        foreach (var step in recipeSteps)
        {
            string line = step.foodType.ToString();

            if (step.cookType.HasValue)
            {
                line += " + " + step.cookType.Value.ToString();
            }

            sb.AppendLine("• " + line);
        }

        recipeText.text = sb.ToString();
    }

    public void ClearRecipe()
    {
        recipeText.text = "";
    }
}
