using UnityEngine;

namespace GGMLib.ObjectPool.Runtime
{
    public abstract class AbstractMonoPoolable : MonoBehaviour, IPoolable
    {
        [field: SerializeField] public PoolItemSO PoolItem { get; set; }
        public GameObject GameObject => this != null ? gameObject : null;
        
        //리셋 아이템이 있는 녀석만 구현하도록.
        public virtual void ResetItem()
        { }
    }
}