using System.Collections.Generic;
using UnityEngine;

namespace GGMLib.ObjectPool.Runtime
{
    [CreateAssetMenu(fileName = "Pool manager", menuName = "Lib/ObjectPool/Manager", order = 10)]
    public class PoolManagerSO : ScriptableObject
    {
        public List<PoolItemSO> itemList = new List<PoolItemSO>();

        private Dictionary<PoolItemSO, Pool> _pools;
        private Transform _rootTrm;

        public void InitializePool(Transform root)
        {
            _rootTrm = root;
            _pools = new Dictionary<PoolItemSO, Pool>();

            foreach (PoolItemSO poolItem in itemList)
            {
                IPoolable poolable = poolItem.prefab.GetComponent<IPoolable>();
                Debug.Assert(poolable != null, $"PoolItem이 IPoolable을 구현하지 않았습니다. {poolItem.prefab}");

                Pool pool = new Pool(poolable, _rootTrm, poolItem.initCount); //초기 값으로 풀 생성
                _pools.Add(poolItem, pool); //풀 딕셔너리에 넣는다.
            }
        }

        public T Pop<T>(PoolItemSO item) where T : IPoolable
        {
            Debug.Assert(_rootTrm != null, "풀SO는 사용하기전에 반드시 초기화가 되어 있어야 합니다.");
            
            if(_pools.TryGetValue(item, out Pool pool))
                return(T)pool.Pop();
            return default;
        }

        public void Push(IPoolable poolable)
        {
            Debug.Assert(_rootTrm != null, "풀SO는 사용하기전에 반드시 초기화가 되어 있어야 합니다.");
            if(_pools.TryGetValue(poolable.PoolItem, out Pool pool))
                pool.Push(poolable);
        }
    }
}