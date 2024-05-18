using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class RandomTilemapIcons : MonoBehaviour
{
    public Tilemap terrainTilemap; // 地形瓦片地图
    public Tilemap monsterTilemap; // 怪物瓦片地图
    public Tile[] monsterTiles; // 怪物瓦片图标
    public int[] tileCounts; // 每种怪物瓦片的生成数量
    public Transform playerTransform; // 玩家Transform

    private List<Vector3Int> terrainTilePositions = new List<Vector3Int>(); // 用于存储地形瓦片的坐标值

    private void Start()
    {
        if (terrainTilemap != null && monsterTilemap != null && monsterTiles != null && monsterTiles.Length > 0 && tileCounts != null && tileCounts.Length == monsterTiles.Length && playerTransform != null)
        {
            // 获取玩家所在瓦片的位置
            Vector3Int playerTilePosition = terrainTilemap.WorldToCell(playerTransform.position);

            // 获取地形瓦片地图上所有有瓦片的坐标值
            BoundsInt terrainBounds = terrainTilemap.cellBounds;
            for (int x = terrainBounds.xMin; x < terrainBounds.xMax; x++)
            {
                for (int y = terrainBounds.yMin; y < terrainBounds.yMax; y++)
                {
                    Vector3Int position = new Vector3Int(x, y, 0);

                    // 检查当前位置是否为玩家所在位置，如果是则跳过
                    if (position == playerTilePosition)
                    {
                        continue;
                    }

                    if (terrainTilemap.HasTile(position))
                    {
                        terrainTilePositions.Add(position);
                    }
                }
            }

            Debug.Log("Total Terrain Tiles: " + terrainTilePositions.Count);

            // 随机打乱地形瓦片坐标列表顺序
            ShuffleTerrainTilePositions();

            // 根据地形瓦片的数量执行多次生成怪物瓦片的操作
            foreach (Vector3Int position in terrainTilePositions)
            {
                GenerateMonsterTile(position);
            }
        }
        else
        {
            Debug.LogWarning("Terrain tilemap, monster tiles, tile counts, or player transform are not properly assigned!");
        }
    }


    // 随机打乱地形瓦片坐标列表顺序
    private void ShuffleTerrainTilePositions()
    {
        for (int i = 0; i < terrainTilePositions.Count; i++)
        {
            Vector3Int temp = terrainTilePositions[i];
            int randomIndex = Random.Range(i, terrainTilePositions.Count);
            terrainTilePositions[i] = terrainTilePositions[randomIndex];
            terrainTilePositions[randomIndex] = temp;
        }
    }

    


    // 在指定位置生成怪物瓦片
    private void GenerateMonsterTile(Vector3Int position)
    {
        // 存储剩余数量大于零的怪物瓦片的索引
        List<int> availableIndexes = new List<int>();

        // 遍历所有怪物瓦片
        for (int i = 0; i < monsterTiles.Length; i++)
        {
            // 检查该怪物瓦片类型的生成数量是否大于零
            if (tileCounts[i] > 0)
            {
                availableIndexes.Add(i); // 将索引添加到可用索引列表中
            }
        }

        // 如果没有剩余数量大于零的怪物瓦片，退出函数
        if (availableIndexes.Count == 0)
        {
            return;
        }

        // 从剩余数量大于零的怪物瓦片中随机选择一个
        int randomIndex = Random.Range(0, availableIndexes.Count);
        int selectedTileIndex = availableIndexes[randomIndex];

        // 获取选定的怪物瓦片
        Tile monsterTile = monsterTiles[selectedTileIndex];

        // 将怪物瓦片添加到怪物瓦片地图上
        monsterTilemap.SetTile(position, monsterTile);

        // 减少该怪物瓦片图标的生成数量
        tileCounts[selectedTileIndex]--;
    }
}
