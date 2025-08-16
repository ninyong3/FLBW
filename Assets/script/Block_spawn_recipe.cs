using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Block_spawn_recipe",
                 menuName = "RushHour/Block Spawn Recipe", order = 0)]
public class Block_spawn_recipe : ScriptableObject
{
    [System.Serializable]
    public class Recipe
    {
        // 6줄, 각 줄 6글자 ('.' 빈칸, 'R' 목표 가로2, 그 외 같은 문자는 한 차량)
        public string[] rows = new string[6];
    }

    public List<Recipe> recipes = new List<Recipe>();

    // (grid, ok, index) 형태로 인덱스까지 반환
    public (char[,] grid, bool ok, int index) GetRandomGrid()
    {
        if (recipes == null || recipes.Count == 0)
            return (null, false, -1);

        int idx = Random.Range(0, recipes.Count);
        var r = recipes[idx];
        if (r.rows == null || r.rows.Length != 6)
            return (null, false, idx);

        var g = new char[6, 6];
        for (int i = 0; i < 6; i++)
        {
            var line = (r.rows[i] ?? string.Empty).PadRight(6, '.');
            for (int j = 0; j < 6; j++) g[i, j] = line[j];
        }
        return (g, true, idx);
    }
}
