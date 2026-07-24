using _MemberWorkspace.JJW.Asset._02_Script.Item;
using GameLib.EventChannelSystem;
using GGMLib.ObjectPool.Runtime;
using UnityEngine;

namespace _MemberWorkspace.JJH._02_Scripts.Map
{
    public class GroundTile : AbstractMonoPoolable
    {
        [SerializeField] private PoolManagerSO poolManager;
        [SerializeField] private PoolItemSO groundPoolItem;
        [SerializeField] private EventChannelSO playerChannel;

        public ItemSO Item { get; private set; }
        public bool HasItem { get; private set; }
        public int GroundIndex { get; private set; }

        private MeshRenderer _groundRenderer;
        private GroundItem _groundItem;

        public override void ResetItem()
        {
            Item = null;
            HasItem = false;
        }

        private void Awake()
        {
            _groundRenderer = GetComponent<MeshRenderer>();
            playerChannel.AddListener<ItemDigEvent>(ItemDig);
        }

        private void OnDestroy()
        {
            playerChannel.RemoveListener<ItemDigEvent>(ItemDig);
        }

        public void Initialize(int groundIndex, bool hasItem, ItemSO item = null)
        {
            GroundIndex = groundIndex;
            HasItem = hasItem;
            Item = item;
        }

        public GroundItem SpawnGroundItem()
        {

            if (!HasItem || Item == null)
                return null;

            GroundItem poolItem = poolManager.Pop<GroundItem>(groundPoolItem);

            _groundItem = poolItem;

            poolItem.transform.position = transform.position + Vector3.up * 0.5f;
            poolItem.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-45, 45));

            poolItem.InitItem(Item);

            return poolItem;
        }

        private void ItemDig(ItemDigEvent evt)
        {
            if (evt.GroundCeilNumber != GroundIndex)
                return;

            if (HasItem && _groundItem != null)
            {
                _groundItem.PopUp();

                Item = null;
                HasItem = false;
                _groundItem = null;
            }
        }
    }
}