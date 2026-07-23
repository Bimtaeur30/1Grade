using GameLib.EventChannelSystem;
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
    [SerializeField] private PlayerStat PlayerStat;
    [SerializeField] private Slider WeightSlider;
    [SerializeField] private ScalePlate LeftScalePlate;
    [SerializeField] private ScalePlate RightScalePlate;

    private ItemSO currentItem;
    private bool isSliderInitialized;
    private bool hasReachedMaxWeight;

    private void Awake()
    {
        if (PlayerStat == null)
        {
            PlayerStat = GetComponentInChildren<PlayerStat>(true);
        }
    }

    private void OnEnable()
    {
        PlayerChannel?.AddListener<ItemEquipEvent>(HandleItemEquipEvent);

        if (PlayerStat != null)
        {
            PlayerStat.MaxWeightChanged += HandleMaxWeightChanged;
            HandleMaxWeightChanged(PlayerStat.MaxWeight);
        }
    }

    private void OnDisable()
    {
        PlayerChannel?.RemoveListener<ItemEquipEvent>(HandleItemEquipEvent);

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
        AddItemWeight(currentItem.Weight);
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
