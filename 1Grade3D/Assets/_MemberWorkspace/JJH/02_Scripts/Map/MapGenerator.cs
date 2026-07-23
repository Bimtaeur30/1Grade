using _MemberWorkspace.JJW.Asset._02_Script.Item;
using GGMLib.ObjectPool.Runtime;
using System.Collections.Generic;
using UnityEngine;

namespace _MemberWorkspace.JJH._02_Scripts.Map
{
    public class MapGenerator : MonoBehaviour
    {
        [Header("Map Size")]
        [SerializeField] private int width = 10;
        [SerializeField] private int height = 10;

        [Header("Item")]
        [SerializeField] private List<ItemSO> itemList;
        [SerializeField] private int minItemCount = 5;
        [SerializeField] private int maxItemCount = 15;

        [Header("Pooling")]
        [SerializeField] private PoolManagerSO poolManager;
        [SerializeField] private PoolItemSO groundTilePool;

        private GroundTile[,] tiles;

        private void Start()
        {
            GenerateMap();
            SpawnItems();
        }

        private void GenerateMap()
        {
            if (tiles != null)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < height; z++)
                    {
                        if (tiles[x, z] != null)
                            poolManager.Push(tiles[x, z]);
                    }
                }
            }

            tiles = new GroundTile[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    GroundTile tile = poolManager.Pop<GroundTile>(groundTilePool);
                    tile.transform.SetParent(transform);
                    tile.transform.SetPositionAndRotation(new Vector3(x, 0, z),
                                                                                  groundTilePool.prefab.transform.rotation);
                    tile.Initialize(z * width + x, false);
                    tiles[x, z] = tile;
                }
            }
        }

        private void SpawnItems()
        {
            int itemCount = Random.Range(minItemCount, maxItemCount + 1);
            int spawned = 0;
            while (spawned < itemCount)
            {
                int x = Random.Range(0, width);
                int z = Random.Range(0, height);
                if (tiles[x, z].HasItem)
                    continue;

                ItemSO item = itemList[Random.Range(0, itemList.Count)];
                tiles[x, z].Initialize(tiles[x, z].GroundIndex, true, item);
                spawned++;
            }
        }
    }
}
