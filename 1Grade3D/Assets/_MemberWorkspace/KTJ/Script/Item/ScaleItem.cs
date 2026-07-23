using _MemberWorkspace.JJW.Asset._02_Script.Item;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public sealed class ScaleItem : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Vector3 initialScale;
    private float itemScale = 1f;
    private bool isInitialized;

    private void Awake()
    {
        CacheComponents();
    }

    public void Initialize(ItemSO item)
    {
        if (item == null)
        {
            Debug.LogError("ScaleItem: ItemSO가 지정되지 않았습니다.", this);
            return;
        }

        CacheComponents();

        itemScale = item.Scale;
        transform.localScale = initialScale * itemScale;
        spriteRenderer.sprite = item.Icon;
        isInitialized = true;
    }

    private void CacheComponents()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (!isInitialized)
        {
            initialScale = transform.localScale;
        }
    }
}
