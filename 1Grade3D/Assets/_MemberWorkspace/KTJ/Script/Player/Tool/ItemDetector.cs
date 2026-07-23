using System.Collections;
using System.Collections.Generic;
using _MemberWorkspace.JJH._02_Scripts.Map;
using _MemberWorkspace.JJW.Asset._02_Script.Item;
using GameLib.EventChannelSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class ItemDetector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerStateMachine playerStateMachine;
    [SerializeField] private EventChannelSO playerEventChannel;

    public GroundTile TargetGroundTile { get; private set; }
    public ItemSO TargetItem { get; private set; }
    public bool IsDigging { get; private set; }

    private readonly Dictionary<GroundTile, int> contactedTiles = new();

    private void Awake()
    {
        if (playerStateMachine == null)
        {
            playerStateMachine = GetComponentInParent<PlayerStateMachine>();
        }
    }

    private void Update()
    {
        UpdateNearestTarget();

        if (!IsDigging &&
            TargetItem != null &&
            Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            StartCoroutine(DigRoutine(
                TargetItem,
                TargetGroundTile.GroundIndex));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GroundTile groundTile = other.GetComponentInParent<GroundTile>();

        if (groundTile == null)
        {
            return;
        }

        contactedTiles.TryGetValue(groundTile, out int contactCount);
        contactedTiles[groundTile] = contactCount + 1;
        UpdateNearestTarget();
    }

    private void OnTriggerExit(Collider other)
    {
        GroundTile groundTile = other.GetComponentInParent<GroundTile>();

        if (groundTile == null || !contactedTiles.TryGetValue(groundTile, out int contactCount))
        {
            return;
        }

        if (contactCount <= 1)
        {
            contactedTiles.Remove(groundTile);
        }
        else
        {
            contactedTiles[groundTile] = contactCount - 1;
        }

        UpdateNearestTarget();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        contactedTiles.Clear();
        TargetGroundTile = null;
        TargetItem = null;
        IsDigging = false;
    }

    private void UpdateNearestTarget()
    {
        GroundTile nearestTile = null;
        float nearestSqrDistance = float.PositiveInfinity;
        List<GroundTile> invalidTiles = null;

        foreach (GroundTile tile in contactedTiles.Keys)
        {
            if (tile == null || !tile.gameObject.activeInHierarchy)
            {
                invalidTiles ??= new List<GroundTile>();
                invalidTiles.Add(tile);
                continue;
            }

            float sqrDistance = (tile.transform.position - transform.position).sqrMagnitude;

            if (sqrDistance >= nearestSqrDistance)
            {
                continue;
            }

            nearestSqrDistance = sqrDistance;
            nearestTile = tile;
        }

        if (invalidTiles != null)
        {
            foreach (GroundTile invalidTile in invalidTiles)
            {
                contactedTiles.Remove(invalidTile);
            }
        }

        TargetGroundTile = nearestTile;
        TargetItem = nearestTile != null && nearestTile.HasItem
            ? nearestTile.Item
            : null;
    }

    private IEnumerator DigRoutine(
        ItemSO diggingItem,
        int diggingGroundIndex)
    {
        if (playerStateMachine == null || playerEventChannel == null)
        {
            Debug.LogError(
                "ItemDetector: PlayerStateMachine 또는 PlayerEventChannel이 지정되지 않았습니다.",
                this);
            yield break;
        }

        IsDigging = true;
        playerStateMachine.OnDigStarted();

        yield return new WaitForSeconds(GetDigDuration(diggingItem.Grade));

        playerStateMachine.ChangeState(playerStateMachine.IdleState);
        IsDigging = false;
        playerEventChannel.RaiseEvent(
            PlayerEvents.ItemDigEvent.Init(diggingGroundIndex));
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
}
