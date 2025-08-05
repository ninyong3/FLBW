using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private string currentTag = null;
    private GameObject currentObject = null;

    public SpriteRenderer heldItemRenderer; // 머리 위 아이콘 표시
    private Sprite heldSprite = null;
    private FoodItem.FoodType? heldFoodType = null;
    private CookerItem.CookType? heldCookType = null;

    [SerializeField] private RecipeManager recipeManager;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && currentTag != null)
        {
            switch (currentTag)
            {
                case "Food":
                    FoodItem food = currentObject.GetComponentInParent<FoodItem>();
                    if (food != null)
                    {
                        heldSprite = food.foodIcon;
                        heldItemRenderer.sprite = heldSprite;
                        heldItemRenderer.enabled = true;
                        heldFoodType = food.foodType;
                        heldCookType = null;
                        Debug.Log("재료 획득: " + heldFoodType);
                        FindObjectOfType<TextDisplay>().ShowLog("재료 획득 : " + heldFoodType);
                    }
                    break;

                case "Cooker":
                    CookerItem cooker = currentObject.GetComponentInParent<CookerItem>();
                    if (cooker != null)
                    {
                        heldSprite = cooker.cookerIcon;
                        heldItemRenderer.sprite = heldSprite;
                        heldItemRenderer.enabled = true;
                        heldCookType = cooker.cookType;
                        heldFoodType = null;
                        Debug.Log("도구 획득: " + heldCookType);
                        FindObjectOfType<TextDisplay>().ShowLog("도구 획득 : " + heldCookType);
                    }
                    break;

                case "Table":
                    if (heldSprite != null)
                    {
                        bool accepted = recipeManager.TrySubmit(heldFoodType, heldCookType);
                        if (accepted)
                        {
                            Debug.Log("조리대에 올바르게 제출됨");
                            FindObjectOfType<TextDisplay>().ShowPriorityLog("성공!");
                        }
                        else
                        {
                            Debug.Log("틀린 재료");
                            FindObjectOfType<TextDisplay>().ShowPriorityLog("이게 아닌거 같은데?");
                        }

                        heldSprite = null;
                        heldFoodType = null;
                        heldCookType = null;
                        heldItemRenderer.sprite = null;
                        heldItemRenderer.enabled = false;
                    }
                    break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        currentTag = other.tag;
        currentObject = other.gameObject;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (currentObject == other.gameObject)
        {
            currentTag = null;
            currentObject = null;
        }
    }
}
