using UnityEngine;

public class FoodItem : MonoBehaviour
{
    // 음식 아이콘 (플레이어 머리 위에 표시)
    public Sprite foodIcon;

    // 음식 종류를 enum으로 정의
    public enum FoodType
    {
        Fish,
        Cabbage,
        Meat,
        Egg,
        Bread
    }

    // Inspector에서 선택 가능
    public FoodType foodType;
}
