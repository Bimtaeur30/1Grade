using _MemberWorkspace.JJW.Asset._02_Script.Item;
using GGMLib.ObjectPool.Runtime;
using UnityEngine;

namespace _MemberWorkspace.JJH._02_Scripts.Map
{
    public class GroundTile : AbstractMonoPoolable
    {
        [SerializeField] private PoolManagerSO poolManager;
        [SerializeField] private PoolItemSO groundPoolItem;

        public ItemSO Item { get; private set; }
        public bool HasItem { get; private set; }
        public int GroundIndex { get; private set; }

        private MeshRenderer groundRenderer;

        public override void ResetItem()
        {
            Item = null;
            HasItem = false;
        }

        private void Awake()
        {
            groundRenderer = GetComponent<MeshRenderer>();
        }

        public void Initialize(int groundIndex, bool hasItem, ItemSO item = null)
        {
            GroundIndex = groundIndex;
            HasItem = hasItem;
            if (HasItem)
            {
                Item = item;

                GroundItem poolItem = poolManager.Pop<GroundItem>(groundPoolItem);
                poolItem.InitItem(item);
                poolItem.FitSpriteToGround(groundRenderer.bounds.size);
                poolItem.transform.position = transform.position;
            }
        }
    }
}