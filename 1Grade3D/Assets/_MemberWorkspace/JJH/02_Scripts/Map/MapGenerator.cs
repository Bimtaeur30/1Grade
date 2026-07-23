using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Size")]
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;

    [Header("Tile")]
    [SerializeField] private GameObject floorPrefab;

    [Header("Item")]
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private int minItemCount = 5;
    [SerializeField] private int maxItemCount = 15;

    private List<Vector2Int> emptyPositions = new List<Vector2Int>();

    private void Start()
    {
        GenerateMap();
        SpawnItems();
    }

    private void GenerateMap()
    {
        emptyPositions.Clear();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Instantiate(floorPrefab, new Vector3(x, 0, y), floorPrefab.transform.rotation, transform);

                emptyPositions.Add(new Vector2Int(x, y));
            }
        }
    }

    private void SpawnItems()
    {
        int itemCount = Random.Range(minItemCount, maxItemCount + 1);

        itemCount = Mathf.Min(itemCount, emptyPositions.Count);

        for (int i = 0; i < itemCount; i++)
        {
            int index = Random.Range(0, emptyPositions.Count);

            Vector2Int pos = emptyPositions[index];

            Instantiate(itemPrefab, new Vector3(pos.x, 0, pos.y), itemPrefab.transform.rotation, transform);

            emptyPositions.RemoveAt(index);
        }
    }
}
