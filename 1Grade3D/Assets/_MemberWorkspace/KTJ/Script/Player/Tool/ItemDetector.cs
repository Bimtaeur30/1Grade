using System.Collections;
using System.Collections.Generic;
using _MemberWorkspace.JJH._02_Scripts.Map;
using _MemberWorkspace.JJW.Asset._02_Script.Item;
using GameLib.EventChannelSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class ItemDetector : MonoBehaviour
{
    private readonly struct DigTarget
    {
        public DigTarget(ItemSO item, int groundIndex)
        {
            Item = item;
            GroundIndex = groundIndex;
        }

        public ItemSO Item { get; }
        public int GroundIndex { get; }
    }

    [Header("References")]
    [SerializeField] private PlayerStateMachine playerStateMachine;
    [SerializeField] private EventChannelSO playerEventChannel;

    [Header("Detection")]
    [SerializeField, Min(0f)] private float detectionRange = 5f;
    [SerializeField, Min(0.02f)] private float highlightRefreshInterval = 0.1f;

    public IReadOnlyList<GroundTile> DetectedItemTiles => detectedItemTiles;
    public bool IsDigging { get; private set; }

    private readonly List<GroundTile> detectedItemTiles = new();
    private readonly HashSet<GroundItem> highlightedItems = new();
    private float nextHighlightRefreshTime;

    private void Awake()
    {
        if (playerStateMachine == null)
        {
            playerStateMachine = GetComponentInParent<PlayerStateMachine>();
        }
    }

    private void Update()
    {
        if (Time.unscaledTime >= nextHighlightRefreshTime)
        {
            RefreshRangeHighlights();
            nextHighlightRefreshTime =
                Time.unscaledTime + highlightRefreshInterval;
        }

        if (IsDigging)
        {
            return;
        }

        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            TryStartDigging();
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        ClearRangeHighlights();
        detectedItemTiles.Clear();
        IsDigging = false;
    }

    private void RefreshRangeHighlights()
    {
        ClearRangeHighlights();

        GroundItem[] groundItems =
            FindObjectsByType<GroundItem>(FindObjectsSortMode.None);
        Vector3 detectorPosition = transform.position;
        float rangeSqr = detectionRange * detectionRange;

        foreach (GroundItem groundItem in groundItems)
        {
            if (groundItem == null || !groundItem.gameObject.activeInHierarchy)
            {
                continue;
            }

            Vector3 itemPosition = groundItem.transform.position;
            float xDistance = itemPosition.x - detectorPosition.x;
            float zDistance = itemPosition.z - detectorPosition.z;
            float planarSqrDistance =
                xDistance * xDistance + zDistance * zDistance;

            if (planarSqrDistance > rangeSqr)
            {
                continue;
            }

            groundItem.SetDigRangeHighlighted(true);
            highlightedItems.Add(groundItem);
        }
    }

    private void ClearRangeHighlights()
    {
        foreach (GroundItem groundItem in highlightedItems)
        {
            if (groundItem != null)
            {
                groundItem.SetDigRangeHighlighted(false);
            }
        }

        highlightedItems.Clear();
    }

    private void RefreshDetectedItems()
    {
        detectedItemTiles.Clear();

        GroundTile[] groundTiles =
            FindObjectsByType<GroundTile>(FindObjectsSortMode.None);
        Vector3 detectorPosition = transform.position;
        float rangeSqr = detectionRange * detectionRange;

        foreach (GroundTile tile in groundTiles)
        {
            if (tile == null ||
                !tile.gameObject.activeInHierarchy ||
                !tile.HasItem ||
                tile.Item == null)
            {
                continue;
            }

            Vector3 tilePosition = tile.transform.position;
            float xDistance = tilePosition.x - detectorPosition.x;
            float zDistance = tilePosition.z - detectorPosition.z;
            float planarSqrDistance =
                xDistance * xDistance + zDistance * zDistance;

            if (planarSqrDistance <= rangeSqr)
            {
                detectedItemTiles.Add(tile);
            }
        }
    }

    private void TryStartDigging()
    {
        if (playerStateMachine == null || playerEventChannel == null)
        {
            Debug.LogError(
                "ItemDetector: PlayerStateMachine 또는 PlayerEventChannel이 지정되지 않았습니다.",
                this);
            return;
        }

        RefreshDetectedItems();
        List<DigTarget> targets = CreateDigSnapshot();

        if (targets.Count == 0)
        {
            return;
        }

        StartCoroutine(DigRoutine(targets));
    }

    private List<DigTarget> CreateDigSnapshot()
    {
        List<DigTarget> targets =
            new List<DigTarget>(detectedItemTiles.Count);

        foreach (GroundTile tile in detectedItemTiles)
        {
            if (tile == null || !tile.HasItem || tile.Item == null)
            {
                continue;
            }

            targets.Add(new DigTarget(tile.Item, tile.GroundIndex));
        }

        return targets;
    }

    private IEnumerator DigRoutine(List<DigTarget> targets)
    {
        IsDigging = true;
        playerStateMachine.OnDigStarted();

        float totalDigDuration = 0f;

        foreach (DigTarget target in targets)
        {
            totalDigDuration += GetDigDuration(target.Item.Grade);
        }

        yield return new WaitForSeconds(totalDigDuration);

        foreach (DigTarget target in targets)
        {
            playerEventChannel.RaiseEvent(
                PlayerEvents.ItemDigEvent.Init(target.GroundIndex));
        }

        playerStateMachine.ChangeState(playerStateMachine.IdleState);
        IsDigging = false;
        detectedItemTiles.Clear();
    }

    private static float GetDigDuration(ItemGrade grade)
    {
        return grade switch
        {
            ItemGrade.Common => 3f,
            ItemGrade.Normal => 5f,
            ItemGrade.Legendary => 8f,
            _ => 3f
        };
    }

    private void OnDrawGizmosSelected()
    {
        const int segmentCount = 64;
        Vector3 center = transform.position;

        Gizmos.color = new Color(1f, 0.75f, 0.1f, 0.9f);

        Vector3 previousPoint = center +
            new Vector3(detectionRange, 0f, 0f);

        for (int i = 1; i <= segmentCount; i++)
        {
            float angle = i * Mathf.PI * 2f / segmentCount;
            Vector3 nextPoint = center + new Vector3(
                Mathf.Cos(angle) * detectionRange,
                0f,
                Mathf.Sin(angle) * detectionRange);

            Gizmos.DrawLine(previousPoint, nextPoint);
            previousPoint = nextPoint;
        }
    }
}
