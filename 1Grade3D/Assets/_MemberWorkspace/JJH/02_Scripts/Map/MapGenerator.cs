using _MemberWorkspace.JJW.Asset._02_Script.Events;
using _MemberWorkspace.JJW.Asset._02_Script.Item;
using GameLib.EventChannelSystem;
using GGMLib.ObjectPool.Runtime;
using System.Collections.Generic;
using UnityEngine;

namespace _MemberWorkspace.JJH._02_Scripts.Map
{
    public class MapGenerator : MonoBehaviour
    {
        [Header("Spawn")]
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private MapIntroCutscene cutscene;
        [field: SerializeField] public EventChannelSO FlowChannel { get; private set; }

        [Header("Map Size")]
        [SerializeField] private int width = 10;
        [SerializeField] private int height = 10;

        [Header("Pooling")]
        [SerializeField] private PoolManagerSO poolManager;
        [SerializeField] private PoolItemSO groundTilePool;

        public IReadOnlyList<GroundTile> ItemTiles => _itemTiles;
        private readonly List<GroundTile> _itemTiles = new();

        private List<ItemSO> _itemList;
        private GroundTile[,] _tiles;

        private void Awake()
        {
            FlowChannel.AddListener<StormStartEvent>(StartGenerate);
        }

        private void OnDestroy()
        {
            FlowChannel.RemoveListener<StormStartEvent>(StartGenerate);
        }

        private void StartGenerate(StormStartEvent evt)
        {
            _itemList = evt.ItemList;

            GenerateMap();
            SpawnItems();

            cutscene.Play();
        }

        private void GenerateMap()
        {
            if (_tiles != null)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < height; z++)
                    {
                        if (_tiles[x, z] != null)
                            poolManager.Push(_tiles[x, z]);
                    }
                }
            }

            _tiles = new GroundTile[width, height];

            float offsetX = (width - 1) * 0.5f;
            float offsetZ = (height - 1) * 0.5f;

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    GroundTile tile = poolManager.Pop<GroundTile>(groundTilePool);
                    tile.transform.SetParent(transform);

                    Vector3 position = spawnPoint.position + new Vector3(x - offsetX, 0f, z - offsetZ);

                    tile.transform.SetPositionAndRotation(position, groundTilePool.prefab.transform.rotation);

                    tile.Initialize(z * width + x, false);
                    _tiles[x, z] = tile;
                }
            }
        }

        private void SpawnItems()
        {
            _itemTiles.Clear();

            List<ItemSO> randomItems = new(_itemList);

            for (int i = randomItems.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);

                ItemSO temp = randomItems[i];
                randomItems[i] = randomItems[randomIndex];
                randomItems[randomIndex] = temp;
            }

            foreach (ItemSO item in randomItems)
            {
                int x;
                int z;

                do
                {
                    x = Random.Range(0, width);
                    z = Random.Range(0, height);
                }
                while (_tiles[x, z].HasItem);

                _tiles[x, z].Initialize(_tiles[x, z].GroundIndex, true, item);

                _itemTiles.Add(_tiles[x, z]);
            }
        }
    }
}
