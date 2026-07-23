using _MemberWorkspace.JJW.Asset._02_Script.Item;
using GGMLib.ObjectPool.Runtime;
using UnityEngine;

namespace _MemberWorkspace.JJH._02_Scripts.Map
{
    public class GroundTile : AbstractMonoPoolable
    {
        public ItemSO Item { get; private set; }
        public bool HasItem { get; private set; }
        public int GroundIndex { get; private set; }

        private MeshRenderer groundRenderer;
        private SpriteRenderer itemSprite;

        public override void ResetItem()
        {
            Item = null;
            HasItem = false;
        }

        private void Awake()
        {
            groundRenderer = GetComponent<MeshRenderer>();
            itemSprite = GetComponentInChildren<SpriteRenderer>();
        }

        public void Initialize(int groundIndex, bool hasItem, ItemSO item = null)
        {
            GroundIndex = groundIndex;
            HasItem = hasItem;
            itemSprite.gameObject.SetActive(HasItem);
            if (HasItem)
            {
                Item = item;
                itemSprite.sprite = item.Icon;
                FitSpriteToGround();
            }
        }

        private void FitSpriteToGround()
        {
            Vector3 groundSize = groundRenderer.bounds.size;
            Vector2 spriteSize = itemSprite.sprite.bounds.size;

            itemSprite.transform.localScale = new Vector3(groundSize.x / spriteSize.x,
                                                                                     groundSize.z / spriteSize.y, 1f);
        }
    }
}