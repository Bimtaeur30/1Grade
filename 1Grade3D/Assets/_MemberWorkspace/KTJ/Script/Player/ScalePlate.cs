using _MemberWorkspace.JJW.Asset._02_Script.Item;
using UnityEngine;

public class ScalePlate : MonoBehaviour
{
    [field:SerializeField] public ScalePlateEnum ScalePlateEnum { get; private set; }
    [SerializeField] private ScaleItem ItemPrefab;
    [SerializeField] private Transform ItemPrefabParent;

    public void AddItem(ItemSO item)
    {
        if (item == null || ItemPrefab == null)
        {
            Debug.LogError("ScalePlate: Item 또는 ItemPrefab이 지정되지 않았습니다.", this);
            return;
        }

        ScaleItem scaleItem = Instantiate(ItemPrefab, ItemPrefabParent);
        scaleItem.Initialize(item);
        scaleItem.transform.localPosition = new Vector3(
            Random.Range(-0.1f, 0.1f),
            Random.Range(-0.1f, 0f),
            0f);
    }
}
