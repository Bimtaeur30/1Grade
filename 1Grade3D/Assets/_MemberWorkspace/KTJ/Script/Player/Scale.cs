using System.Collections.Generic;
using GameLib.EventChannelSystem;
using _MemberWorkspace.JJW.Asset._02_Script.Events;
using _MemberWorkspace.JJW.Asset._02_Script.Item;
using UnityEngine;
using UnityEngine.UI;

public enum ScalePlateEnum
{
    Left, Right
}

public class Scale : MonoBehaviour
{
    [SerializeField] private EventChannelSO PlayerChannel;
    [SerializeField] private EventChannelSO TurnChannel;
    [SerializeField] private PlayerStat PlayerStat;
    [SerializeField] private Slider WeightSlider;
    [SerializeField] private ScalePlate LeftScalePlate;
    [SerializeField] private ScalePlate RightScalePlate;

    [Header("Scale Tilt")]
    [SerializeField] private Transform ScaleObject;
    [SerializeField, Range(0f, 45f)] private float MaxTiltAngle = 25f;
    [SerializeField, Min(0.01f)] private float TiltSmoothTime = 0.3f;

    public float LeftWeight { get; private set; }
    public float RightWeight { get; private set; }
    public IReadOnlyList<ItemSO> LoadedItems => loadedItems;

    private readonly List<ItemSO> loadedItems = new();
    private ItemSO currentItem;
    private bool isSliderInitialized;
    private bool hasReachedMaxWeight;
    private Vector3 initialScaleEulerAngles;
    private float currentTiltAngle;
    private float targetTiltAngle;
    private float tiltVelocity;

    private void Awake()
    {
        if (PlayerStat == null)
        {
            PlayerStat = GetComponentInChildren<PlayerStat>(true);
        }

        if (ScaleObject != null)
        {
            initialScaleEulerAngles = ScaleObject.localEulerAngles;
        }
    }

    private void Update()
    {
        if (ScaleObject == null)
        {
            return;
        }

        currentTiltAngle = Mathf.SmoothDampAngle(
            currentTiltAngle,
            targetTiltAngle,
            ref tiltVelocity,
            TiltSmoothTime);

        ScaleObject.localRotation = Quaternion.Euler(
            initialScaleEulerAngles.x,
            initialScaleEulerAngles.y,
            initialScaleEulerAngles.z + currentTiltAngle);
    }

    private void OnEnable()
    {
        PlayerChannel?.AddListener<ItemEquipEvent>(HandleItemEquipEvent);
        TurnChannel?.AddListener<TurnEndEvent>(HandleTurnEndEvent);

        if (PlayerStat != null)
        {
            PlayerStat.MaxWeightChanged += HandleMaxWeightChanged;
            HandleMaxWeightChanged(PlayerStat.MaxWeight);
        }
    }

    private void OnDisable()
    {
        PlayerChannel?.RemoveListener<ItemEquipEvent>(HandleItemEquipEvent);
        TurnChannel?.RemoveListener<TurnEndEvent>(HandleTurnEndEvent);

        if (PlayerStat != null)
        {
            PlayerStat.MaxWeightChanged -= HandleMaxWeightChanged;
        }
    }

    private void HandleItemEquipEvent(ItemEquipEvent @event)
    {
        if (@event.Item == null)
        {
            Debug.LogError("Scale: ItemEquipEvent에 Item이 없습니다.", this);
            return;
        }

        if (!CanAddItem(@event.Item))
        {
            Debug.Log(
                $"Scale: 최대 무게를 초과하여 {@event.Item.name} 아이템을 실을 수 없습니다.",
                this);
            return;
        }

        currentItem = @event.Item;

        ScalePlate targetPlate = @event.ScalePlateEnum switch
        {
            ScalePlateEnum.Left => LeftScalePlate,
            ScalePlateEnum.Right => RightScalePlate,
            _ => null
        };

        if (targetPlate == null)
        {
            Debug.LogError(
                $"Scale: {@event.ScalePlateEnum} ScalePlate가 지정되지 않았습니다.",
                this);
            return;
        }

        targetPlate.AddItem(currentItem);
        loadedItems.Add(currentItem);
        AddPlateWeight(@event.ScalePlateEnum, currentItem.Weight);
        AddItemWeight(currentItem.Weight);
        @event.Accept();
    }

    private void HandleTurnEndEvent(TurnEndEvent @event)
    {
        if (TurnChannel == null)
        {
            return;
        }

        TurnChannel.RaiseEvent(
            new SettlementEvent(new List<ItemSO>(loadedItems)));
        loadedItems.Clear();
    }

    private bool CanAddItem(ItemSO item)
    {
        float maxWeight;

        if (PlayerStat != null)
        {
            maxWeight = PlayerStat.MaxWeight;
        }
        else if (WeightSlider != null)
        {
            maxWeight = WeightSlider.maxValue;
        }
        else
        {
            Debug.LogError(
                "Scale: 최대 무게를 확인할 PlayerStat 또는 WeightSlider가 없습니다.",
                this);
            return false;
        }

        float currentWeight = LeftWeight + RightWeight;
        return maxWeight > 0f &&
               currentWeight + item.Weight <= maxWeight;
    }

    private void AddPlateWeight(ScalePlateEnum plate, float weight)
    {
        if (plate == ScalePlateEnum.Left)
        {
            LeftWeight += weight;
        }
        else
        {
            RightWeight += weight;
        }

        UpdateTargetTiltAngle();
    }

    private void UpdateTargetTiltAngle()
    {
        if (Mathf.Approximately(LeftWeight, RightWeight))
        {
            targetTiltAngle = 0f;
            return;
        }

        float heavierWeight = Mathf.Max(LeftWeight, RightWeight);
        float lighterWeight = Mathf.Min(LeftWeight, RightWeight);
        float tiltRatio;

        if (lighterWeight <= 0f)
        {
            tiltRatio = 1f;
        }
        else
        {
            float weightRatio = heavierWeight / lighterWeight;
            tiltRatio = Mathf.InverseLerp(1f, 2f, weightRatio);
        }

        float smoothTiltRatio = Mathf.SmoothStep(0f, 1f, tiltRatio);
        float direction = LeftWeight > RightWeight ? 1f : -1f;
        targetTiltAngle = direction * MaxTiltAngle * smoothTiltRatio;
    }

    private void HandleMaxWeightChanged(int maxWeight)
    {
        if (WeightSlider == null)
        {
            return;
        }

        WeightSlider.minValue = 0f;
        WeightSlider.maxValue = Mathf.Max(0f, maxWeight);

        if (!isSliderInitialized)
        {
            WeightSlider.value = 0f;
            isSliderInitialized = true;
        }

        hasReachedMaxWeight =
            WeightSlider.maxValue > 0f &&
            WeightSlider.value >= WeightSlider.maxValue;
    }

    private void AddItemWeight(float weight)
    {
        if (WeightSlider == null)
        {
            return;
        }

        bool wasBelowMaxWeight = WeightSlider.value < WeightSlider.maxValue;
        WeightSlider.value += weight;

        if (wasBelowMaxWeight &&
            WeightSlider.maxValue > 0f &&
            WeightSlider.value >= WeightSlider.maxValue &&
            !hasReachedMaxWeight)
        {
            hasReachedMaxWeight = true;
            HandleMaxWeightReached();
        }
    }

    public void HandleMaxWeightReached()
    {
        Debug.Log("Scale: 최대 무게에 도달했습니다.", this);
    }
}
